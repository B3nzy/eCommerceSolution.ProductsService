using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.Models.DTOs.SearchProducts;
using eCommerceSolution.ProductsService.Models.Entities;
using MassTransit;
using MassTransit.Testing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.ChatCompletion;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace eCommerceSolution.ProductsService.Handlers;

public class SearchProductsHandler : IRequestHandler<SearchProductsRequest, SearchProductsResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly QdrantClient _qdrantClient;
    private readonly IChatCompletionService _chatCompletionService;

    public SearchProductsHandler(ApplicationDbContext dbContext, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, QdrantClient qdrantClient, IChatCompletionService chatCompletionService)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));
        _qdrantClient = qdrantClient ?? throw new ArgumentNullException(nameof(qdrantClient));
        _chatCompletionService = chatCompletionService ?? throw new ArgumentNullException(nameof(chatCompletionService));
    }

    public async Task<SearchProductsResponse> Handle(SearchProductsRequest request, CancellationToken cancellationToken)
    {
        SearchProductsResponse searchProductsResponse = new SearchProductsResponse
        {
            products = new List<SearchResultDto>()
        };

        ChatHistory chats = new ChatHistory();
        chats.AddSystemMessage("Your job is to parse natual language into structured json output. " +
            "User will type natual language search query on eCommerce website, " +
            "you need to parse the natural language into simple text and price low or price high if it exists otherwise return null for price. " +
            "Expected output json " +
            "{searchText: string, priceLow: decimal, priceHigh: decimal}");

        chats.AddUserMessage(request.SearchString);

        var response = await _chatCompletionService.GetChatMessageContentAsync(chats);

        SearchIntent searchIntent;

        try
        {
            string rawContent = response.Content ?? string.Empty;

            // Local LLMs love wrapping JSON in ```json ... ``` blocks. Clean them out first:
            string cleanedJson = Regex.Replace(rawContent, @"```(json)?", string.Empty, RegexOptions.IgnoreCase).Trim('`', '\r', '\n', ' ');

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Use the null-coalescing operator (??) to prevent searchIntent from being null if deserialization returns null
            searchIntent = JsonSerializer.Deserialize<SearchIntent>(cleanedJson, options)
                           ?? new SearchIntent { SearchText = request.SearchString };
        }
        catch (Exception)
        {
            // Fallback gracefully to using raw input as search text if parsing fails
            searchIntent = new SearchIntent
            {
                SearchText = request.SearchString,
                PriceLow = null,
                PriceHigh = null
            };
        }

        List<Guid>? productIds = null;

        // Only apply SQL filtering if the user specified at least one price boundary
        if (searchIntent.PriceLow.HasValue || searchIntent.PriceHigh.HasValue)
        {
            var query = _dbContext.Products.AsNoTracking();

            if (searchIntent.PriceLow.HasValue)
            {
                query = query.Where(p => p.Price >= searchIntent.PriceLow.Value);
            }

            if (searchIntent.PriceHigh.HasValue)
            {
                query = query.Where(p => p.Price <= searchIntent.PriceHigh.Value);
            }

            // Materialize IDs
            productIds = await query
                .Select(p => p.ProductId)
                .ToListAsync(cancellationToken);
        }

        // 1. Convert the natural language query into a vector
        // E.g., userQuery = "I need a fast laptop for video editing"
        var embedQuery = $"search_query: {request.SearchString}";
        var queryEmbedding = await _embeddingGenerator.GenerateAsync(new[] { embedQuery }, cancellationToken: cancellationToken);
        var queryVector = queryEmbedding[0].Vector.ToArray();

        // Convert your SQL Guids into Qdrant PointIds
        //var pointIds = productIds.Select(id => new PointId { Uuid = id.ToString() }).ToList();

        Filter? qdrantFilter = null;

        if (productIds != null)
        {
            qdrantFilter = new Filter
            {
                Must = { Conditions.HasId(productIds) }
            };
        }
        // Create the filter restricting Qdrant to ONLY these IDs
        

        // 2. Perform the similarity search in Qdrant
        var searchResults = await _qdrantClient.SearchAsync(
            collectionName: "products",
            vector: queryVector,
            filter: qdrantFilter,
            //scoreThreshold: 0.5f, // <-- ADD THIS LINE (Values usually range between 0.0 and 1.0 for Cosine)
            limit: 5, // How many top results to return
            cancellationToken: cancellationToken
        );

        foreach (var result in searchResults)
        {
            Guid qdrantId = Guid.Parse(result.Id.Uuid);
            // 3. Retrieve the product details from the database using the IDs returned by Qdrant
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == qdrantId, cancellationToken);
            if (product != null)
            {
                // 4. Map the product entity to a DTO and add it to the response
                var productDto = new SearchResultDto
                {
                    ProductId = product.ProductId,
                    ProductDescription = product.ProductDescription,
                    Category = product.Category,
                    Price = product.Price,
                    ProductName = product.ProductName,
                    SimilarityScore = result.Score
                };
                searchProductsResponse.products.Add(productDto);
            }
        }

        return searchProductsResponse;

    }
}
