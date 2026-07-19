using eCommerce.Microservices.Events.Product;
using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.Models.DTOs.CreateProduct;
using eCommerceSolution.ProductsService.Models.Entities;
using MassTransit;
using MediatR;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel.Embeddings;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace eCommerceSolution.ProductsService.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductRequest, CreateProductResponse>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly QdrantClient _qdrantClient;

    public CreateProductHandler(ApplicationDbContext dbContext, IPublishEndpoint publishEndpoint, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, QdrantClient qdrantClient)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _embeddingGenerator = embeddingGenerator ?? throw new ArgumentNullException(nameof(embeddingGenerator));
        _qdrantClient = qdrantClient ?? throw new ArgumentNullException(nameof(qdrantClient));
    }

    public async Task<CreateProductResponse> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        Product product = new Product()
        {
            ProductId = Guid.NewGuid(),
            ProductName = request.ProductName,
            ProductDescription = request.ProductDescription,
            Category = request.Category,
            Price = request.Price
        };

        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        // Generate embeddings from actual product data
        var textToEmbed = $"ProductName: {request.ProductName}, ProductDescription: {request.ProductDescription}, Category: {request.Category}";

        // Pass the cancellation token to the AI service
        var generatedEmbeddings = await _embeddingGenerator.GenerateAsync(new[] { textToEmbed }, cancellationToken: cancellationToken);

        // Extract the actual float array (ReadOnlyMemory<float>)
        var embeddingVector = generatedEmbeddings[0].Vector;

        ProductStock productStock = new ProductStock()
        {
            ProductId = product.ProductId,
            QuantityInStock = request.QuantityInStock
        };

        var point = new PointStruct
        {
            Id = product.ProductId, // Qdrant accepts Guids automatically mapped to UUIDs
            Vectors = embeddingVector.ToArray()
        };

        // Upsert the point into Qdrant
        await _qdrantClient.UpsertAsync(
            collectionName: "products",
            points: new[] { point },
            cancellationToken: cancellationToken
        );
        
        await _publishEndpoint.Publish(productStock, cancellationToken);

        return new CreateProductResponse()
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            Category = product.Category,
            Price = product.Price,
            QuantityInStock = productStock.QuantityInStock
        };
    }
}
