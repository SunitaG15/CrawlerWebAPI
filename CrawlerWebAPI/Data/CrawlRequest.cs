namespace CrawlerWebAPI.Data
{
    public class CrawlRequest
    {
        public List<string> Urls { get; set; }
        public int MaxDepth { get; set; }

    }
}
