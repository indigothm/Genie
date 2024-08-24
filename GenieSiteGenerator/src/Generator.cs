using OpenAI.Chat;

namespace GenieSiteGenerator.src
{
    public class Generator
    {

        public static async Task<string> GenerateHtmlWithAI(PageContent content, ChatClient chatClient)
        {
            string systemPrompt = @"You are an AI assistant specialized in generating HTML content. 
        Your task is to create a simple, elegant HTML page based on the provided description and image URLs. 
        Use modern HTML5 and inline CSS for styling. Ensure the page is responsive and looks good on both desktop and mobile devices.
        Return raw HTML without any additional descriptions and without ```html around the output";

            string userPrompt = $@"Generate an HTML page with the following content:
        Title: {content.Description}
        Number of images: {content.ImageUrls.Count}
        
        Please include the image URLs in img tags within the HTML. 
        Ensure proper responsive design for the images.";

            ChatCompletion completion = await chatClient.CompleteChatAsync([
                new SystemChatMessage(systemPrompt),
        new UserChatMessage(userPrompt)
            ]);

            string htmlTemplate = completion.Content[0].Text ?? "";

            // Replace image placeholders with actual image URLs
            for (int i = 0; i < content.ImageUrls.Count; i++)
            {
                htmlTemplate = htmlTemplate.Replace($"{{image_url_{i}}}", content.ImageUrls[i]);
            }

            // Log the generated HTML to the console for debugging
            Console.WriteLine("Generated HTML:");
            Console.WriteLine(htmlTemplate);

            return htmlTemplate;
        }

        public static string GenerateUniqueId()
        {
            return $"page-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }
    }
}
