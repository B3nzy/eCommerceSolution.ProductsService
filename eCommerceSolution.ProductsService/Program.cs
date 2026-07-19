using eCommerceSolution.ProductsService.Data;
using eCommerceSolution.ProductsService.HttpClients;
using eCommerceSolution.ProductsService.Middlewares;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using OpenAI;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using System.ClientModel;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("microservices-baseurl.json", optional: false, reloadOnChange: true);


// Load LM Studio configuration from appsettings.json
var endpoint = new Uri(builder.Configuration["LMStudio:Endpoint"]);
var modelId = builder.Configuration["LMStudio:ModelId"];
var embeddingModelId = builder.Configuration["LMStudio:EmbeddingModelId"];
var apiKey = builder.Configuration["LMStudio:ApiKey"];
var qdrantHost = builder.Configuration["Qdrant:QdrantHost"];

// Create a custom HttpClient pointing to LM Studio
// Note: Ensure your appsettings.json endpoint ends with a slash (e.g., "http://localhost:1234/v1/")
var options = new OpenAIClientOptions
{
    Endpoint = endpoint
};

// Create the client (ApiKeyCredential requires a non-null string, even if dummy)
var openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey), options);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the LM Studio kernel with the specified configuration
var kernelBuilder = builder.Services.AddKernel();
// 3. Pass the client directly into both services
kernelBuilder.AddOpenAIChatCompletion(modelId, openAIClient);

// Suppress the experimental warning specifically for this line in csproj
kernelBuilder.AddOpenAIEmbeddingGenerator(embeddingModelId, openAIClient);


// Register Qdrant client as a singleton service
builder.Services.AddSingleton(new QdrantClient(qdrantHost));

// Register DbContext with SQL Server provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DockerMSSQLConnection")));

// Register MediatR services
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

// Register the InventoryMicroserviceHttpClient with the base address from configuration
string? userServiceUrl = builder.Configuration["ServiceUrls:InventoryService"];
builder.Services.AddHttpClient<InventoryMicroserviceHttpClient>(client =>
{
    client.BaseAddress = new Uri(userServiceUrl ?? throw new InvalidOperationException("Inventory Service URL is missing."));
});

// Register MassTransit with RabbitMQ
builder.Services.AddMassTransit(cfg =>
{
    cfg.SetKebabCaseEndpointNameFormatter();

    cfg.UsingRabbitMq((context, cfg) =>
    {
        var host = builder.Configuration["MessageBroker:Host"];
        var username = builder.Configuration["MessageBroker:Username"];
        var password = builder.Configuration["MessageBroker:Password"];

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });
    });
});

var app = builder.Build();


// Before you can insert data, you must create a "Collection" in Qdrant and define the vector size.
var qdrantClient = app.Services.GetRequiredService<QdrantClient>();
var collections = await qdrantClient.ListCollectionsAsync();
if (!collections.Contains("products"))
{
    await qdrantClient.CreateCollectionAsync(
        collectionName: "products",
        vectorsConfig: new VectorParams { Size = 768, Distance = Distance.Cosine }
    );
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandlingMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
