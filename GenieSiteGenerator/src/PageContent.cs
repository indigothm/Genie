namespace GenieSiteGenerator.src
{
    public class PageContent
    {
        public Guid SiteId { get; set; }
        public string Description { get; set; } = null!;
        public List<string>? ImageUrls { get; set; }
    
    }
}
