using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using System.IO;
using System.Reflection;

namespace GenieSiteGenerator.src
{ 

    public static class GeneratorExtension {
        public static WebApplicationBuilder AddWebsiteGeneratorService(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<Generator>();
            return builder;
        }

    }

    public class Generator
    {

        private readonly IChatCompletionService chatCompletionService;
        private readonly ChatMemoryService chatMemoryService;
        private readonly Kernel kernel;
        private readonly OpenAIPromptExecutionSettings openAIPromptExecutionSettings;

        public Generator(
            IChatCompletionService chatCompletionService, 
            ChatMemoryService chatMemoryService,
            Kernel kernel,
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings
            )
        {
            this.chatCompletionService = chatCompletionService;
            this.chatMemoryService = chatMemoryService;
            this.kernel = kernel;
            this.openAIPromptExecutionSettings = openAIPromptExecutionSettings;

        }
        public async Task<string> GenerateHtmlWithAI(PageContent content)
        {
            string templatePath = Path.Combine(GetResourcePath(), "Templates", "template.html");

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template file not found at {templatePath}");
            }

            string templateContent = await File.ReadAllTextAsync(templatePath);

            string systemPrompt = 
                
                @"You are an AI assistant specialized in generating HTML content. 
                Your task is to create a simple, elegant HTML page based on the provided description, image URLs, and template example. 
                Use modern HTML5 and inline CSS for styling. Ensure the page is responsive and looks good on both desktop and mobile devices.
                Return raw HTML without any additional descriptions and without ```html around the output";

            string imageUrlList = string.Join("\n", content.ImageUrls?.Select((url, index) => $"Image {index + 1}: {url}") ?? Enumerable.Empty<string>());

            string userPrompt = $@"Generate an HTML page with the following content:
                Title: {content.Description}
                Number of images: {(content.ImageUrls ?? []).Count}
        
                Image URLs:
                {imageUrlList}
        
                Please include these image URLs directly in img tags within the HTML. 
                Ensure proper responsive design for the images.
        
                Use the following template as a starting point and adapt it to the content:R

                {templateContent}";

            // Example - https://learn.microsoft.com/en-us/semantic-kernel/get-started/quick-start-guide?pivots=programming-language-csharp

            var currentChatHistory = chatMemoryService.GetHistory(content.SiteId.ToString());

            currentChatHistory.AddSystemMessage(systemPrompt);
            currentChatHistory.AddUserMessage(userPrompt);

            var result = await chatCompletionService.GetChatMessageContentAsync(
              currentChatHistory,
              executionSettings: openAIPromptExecutionSettings,
              kernel: kernel);


            var generatedHtml = result.Content ?? string.Empty;

            // Log the generated HTML to the console for debugging
            Console.WriteLine("Generated HTML:");
            Console.WriteLine(generatedHtml);

            currentChatHistory.AddMessage(result.Role, result.Content ?? string.Empty);

            return generatedHtml;
        }

        public static string GenerateUniqueId()
        {
            return $"page-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }

        private static string GetResourcePath()
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyLocation) ?? throw new InvalidOperationException("Unable to determine the directory of the executing assembly.");
            return Path.Combine(assemblyDirectory, "Resources");
        }

    }
}