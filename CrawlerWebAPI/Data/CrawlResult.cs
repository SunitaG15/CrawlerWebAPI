namespace CrawlerWebAPI.Data
{
    public class CrawlResult
    {
        public List<string> VisitedUrls { get; set; }
        public Dictionary<string, string> PageTitles { get; set; }
        public int MaxDepthReached { get; set; }    

    }
}
