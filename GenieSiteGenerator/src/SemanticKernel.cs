using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.Extensions.DependencyInjection;

namespace GenieSiteGenerator.src
{
    public static class SemanticKernelExtensions
    {
        public static WebApplicationBuilder AddSemanticKernelServices(this WebApplicationBuilder builder)
        {
            var azureOpenAIConfig = builder.Configuration.GetSection("AzureOpenAI");
            string key = azureOpenAIConfig["Key"]!;
            string endpoint = azureOpenAIConfig["Endpoint"]!;
            string deploymentName = azureOpenAIConfig["DeploymentName"]!;

            builder.Services.AddSingleton<Kernel>(sp =>
            {
                var kernelBuilder = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(
                        deploymentName: deploymentName,
                        endpoint: endpoint,
                        apiKey: key
                    );
                return kernelBuilder.Build();
            });

            builder.Services.AddSingleton<IChatCompletionService>(sp =>
            {
                var kernel = sp.GetRequiredService<Kernel>();
                return kernel.Services.GetRequiredService<IChatCompletionService>();
            });

            builder.Services.AddSingleton(new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            });

            builder.Services.AddSingleton<ChatMemoryService>();

            return builder;
        }
    }
}