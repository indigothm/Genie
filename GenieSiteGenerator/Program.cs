using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.AI.OpenAI;
using Azure;
using OpenAI.Chat;
using GenieSiteGenerator.src;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(x =>
    new BlobServiceClient(builder.Configuration["AzureBlobStorage:ConnectionString"]));

builder.AddSemanticKernelServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    var blobServiceClient = app.Services.GetRequiredService<BlobServiceClient>();
    var containerClient = blobServiceClient.GetBlobContainerClient("$web");
    await containerClient.CreateIfNotExistsAsync();
    await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);
});

app.MapRoutes();

app.Run();



