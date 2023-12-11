using CrawlerWebAPI.Data;
using HtmlAgilityPack;

namespace CrawlerWebAPI.Services
{
    public class CrawlService:ICrawlService
    {
        private readonly HttpClient _httpClient;
        public CrawlService(IHttpClientFactory httpClientFactory)
        {
            _httpClient=httpClientFactory.CreateClient();
        }
        public async Task<CrawlResult> CrawlAsync(List<string> urls, int maxDepth)
        {
            var pageTitles = new Dictionary<string, string>();
            HtmlWeb web = new HtmlWeb(); ;
            await Task.Delay(100);
            foreach (var url in urls)
            {
                var doc = web.Load(url);
                var title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText ?? "No Title";
                pageTitles.Add(url, title);
            }
            return new CrawlResult
            {
                VisitedUrls = urls,
                PageTitles = pageTitles,
                MaxDepthReached = maxDepth
            };

        }
        public async Task<List<PageRelation>> GetPageRelationsAsync(string url, int maxDepth, int currentDepth = 1)
        {
            if (currentDepth > maxDepth)
            {
                return new List<PageRelation>();
            }

            var pageContent = await _httpClient.GetStringAsync(url);
            var pageLinks = GetPageLinks(pageContent);

            var childPageRelations = new List<PageRelation>();
            foreach (var link in pageLinks)
            {
                var childPageInfo = await GetPageRelationsAsync(link, maxDepth, currentDepth + 1);
                childPageRelations.AddRange(childPageInfo);
            }

            var pageInfo = new PageRelation { Url = url, Title = GetPageTitle(url) };
            childPageRelations.Insert(0, pageInfo); // Add the current page at the beginning

            return childPageRelations;
        }

        private List<string> GetPageLinks(string pageContent)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(pageContent);

            var links = htmlDocument.DocumentNode.SelectNodes("//a[@href]")
                ?.Select(link => link.GetAttributeValue("href", ""))
                .ToList() ?? new List<string>();

            return links;
        }

        private string GetPageTitle(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var title = doc.DocumentNode.SelectSingleNode("//title")?.InnerText ?? "No Title";
            return title;
        }

        
    }
}
