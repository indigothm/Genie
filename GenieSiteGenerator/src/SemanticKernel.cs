using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace GenieSiteGenerator.src
{
    public static class SemanticKernel

    {
        public static WebApplicationBuilder AddSemanticKernelServices(this WebApplicationBuilder builder) {

            var azureOpenAIConfig = builder.Configuration.GetSection("AzureOpenAI");
            string key = azureOpenAIConfig["Key"]!;
            string endpoint = azureOpenAIConfig["Endpoint"]!;
            string deploymentName = azureOpenAIConfig["DeploymentName"]!;

            var kernelBuilder = Kernel.CreateBuilder()
                .AddAzureOpenAIChatCompletion(
                    deploymentName: deploymentName,
                    endpoint: endpoint,
                    apiKey: key
                );

            var kernel = kernelBuilder.Build();

            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
            };


            builder.Services.AddSingleton<ChatMemoryService>((sp) => new ChatMemoryService());            

            builder.Services.AddSingleton<IChatCompletionService>(chatCompletionService);


            return builder;
        }
    }
}
