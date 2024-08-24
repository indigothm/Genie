using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace GenieSiteGenerator.src
{
    public static class WebApplicationExtensions
    {

        public static WebApplication MapRoutes(this WebApplication app)
        {

            app.MapPost("/api/generate", async (
            [FromBody] PageContent content,
            [FromServices] BlobServiceClient blobServiceClient,
            [FromServices] ChatClient chatClient) =>
            {
                string htmlContent = await Generator.GenerateHtmlWithAI(content, chatClient);
                string uniqueId = Generator.GenerateUniqueId();
                var containerClient = blobServiceClient.GetBlobContainerClient("$web");
                var blobClient = containerClient.GetBlobClient($"{uniqueId}.html");
                await blobClient.UploadAsync(BinaryData.FromString(htmlContent), new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = "text/html",
                        CacheControl = "max-age=3600"
                    }
                });
                string cdnEndpoint = "https://geniecdn.azureedge.net";
                string cdnUrl = $"{cdnEndpoint}/$web/{uniqueId}.html";
                return Results.Ok(new { Url = cdnUrl, Id = uniqueId });
            })
            .WithName("GenerateStaticPage")
            .WithOpenApi();

            app.MapGet("/api/site/{url}", async (
                string id,
                [FromServices] BlobServiceClient blobServiceClient) =>
            {
                if (!id.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                {
                    id += ".html";
                }
                var containerClient = blobServiceClient.GetBlobContainerClient("$web");
                var blobClient = containerClient.GetBlobClient(id);
                if (await blobClient.ExistsAsync())
                {
                    // TODO: Replace with appsetting values
                    string cdnEndpoint = "https://geniecdn.azureedge.net";
                    string cdnUrl = $"{cdnEndpoint}/$web/{id}";
                    return Results.Redirect(cdnUrl);
                }
                else
                {
                    return Results.NotFound();
                }
            })
            .WithName("GetStaticPage")
            .WithOpenApi();

            return app;
        }

    }
}
