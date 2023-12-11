using CrawlerWebAPI.Data;

namespace CrawlerWebAPI.Services
{
    public interface ICrawlService
    {
        Task<CrawlResult> CrawlAsync(List<string> urls, int maxDepth);
        Task<List<PageRelation>> GetPageRelationsAsync(string url, int maxDepth, int currentDepth = 1);
    }
}
