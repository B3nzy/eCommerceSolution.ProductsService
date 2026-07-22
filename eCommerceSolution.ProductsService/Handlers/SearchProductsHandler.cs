using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.Models.DTOs.SearchProducts;
using eCommerceSolution.ProductsService.Models.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace eCommerceSolution.ProductsService.Handlers;

public class SearchProductsHandler : IRequestHandler<SearchProductsRequest, SearchProductsResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly QdrantClient _qdrantClient;

    public SearchProductsHandler(ApplicationDbContext dbContext, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, QdrantClient qdrantClient)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));
        _qdrantClient = qdrantClient ?? throw new ArgumentNullException(nameof(qdrantClient));
    }

    public async Task<SearchProductsResponse> Handle(SearchProductsRequest request, CancellationToken cancellationToken)
    {
        SearchProductsResponse searchProductsResponse = new SearchProductsResponse
        {
            products = new List<SearchResultDto>()
        };

        // 1. Convert the natural language query into a vector
        // E.g., userQuery = "I need a fast laptop for video editing"
        var embedQuery = $"search_query: {request.SearchString}";
        var queryEmbedding = await _embeddingGenerator.GenerateAsync(new[] { embedQuery }, cancellationToken: cancellationToken);
        var queryVector = queryEmbedding[0].Vector.ToArray();

        // 2. Perform the similarity search in Qdrant
        var searchResults = await _qdrantClient.SearchAsync(
            collectionName: "products",
            vector: queryVector,
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
