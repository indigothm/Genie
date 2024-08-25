using OpenAI.Chat;
using System.IO;
using System.Reflection;

namespace GenieSiteGenerator.src
{
    public class Generator
    {
        public static async Task<string> GenerateHtmlWithAI(PageContent content, ChatClient chatClient)
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

                    string imageUrlList = string.Join("\n", content.ImageUrls.Select((url, index) => $"Image {index + 1}: {url}"));

                    string userPrompt = $@"Generate an HTML page with the following content:
                Title: {content.Description}
                Number of images: {content.ImageUrls.Count}
        
                Image URLs:
                {imageUrlList}
        
                Please include these image URLs directly in img tags within the HTML. 
                Ensure proper responsive design for the images.
        
                Use the following template as a starting point and adapt it to the content:R

                {templateContent}";

            ChatCompletion completion = await chatClient.CompleteChatAsync([
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userPrompt)
            ]);

            string generatedHtml = completion.Content[0].Text ?? "";

            // Log the generated HTML to the console for debugging
            Console.WriteLine("Generated HTML:");
            Console.WriteLine(generatedHtml);

            return generatedHtml;
        }

        public static string GenerateUniqueId()
        {
            return $"page-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }

        private static string GetResourcePath()
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
            return Path.Combine(assemblyDirectory, "Resources");
        }
    }
}