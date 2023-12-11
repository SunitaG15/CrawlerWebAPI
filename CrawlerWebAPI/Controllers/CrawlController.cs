using CrawlerWebAPI.Data;
using CrawlerWebAPI.Services;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace CrawlerWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrawlController : ControllerBase
    {
        private readonly ILogger<CrawlController> _logger;
        private static readonly Dictionary<int, CrawlResult> CrawlResults = new Dictionary<int, CrawlResult>();
        private static int CrawlIdCounter = 1;
        private readonly ICrawlService _crawlService;

        public CrawlController(ILogger<CrawlController> logger,ICrawlService crawlService)
        {
            _logger = logger;
            _crawlService = crawlService;
        }

        [HttpPost("crawl")]
        public async Task<ActionResult<int>> CrawlAsync([FromBody] CrawlRequest request)
        {
            try
            {
                // Validate request
                if (request == null || request.Urls == null || request.Urls.Count == 0 || request.MaxDepth <= 0)
                {
                    return BadRequest("Invalid request format");
                }

                // Simulate async crawling
                int crawlId = CrawlIdCounter++;
                var crawlResult = await _crawlService.CrawlAsync(request.Urls, request.MaxDepth);

                // Store result in memory
                CrawlResults[crawlId] = crawlResult;

                _logger.LogInformation($"Crawl completed for ID:{crawlId}");

                return Ok(crawlId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing crawl request");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("crawl/{id}")]
        public ActionResult<CrawlResult> GetCrawlResult(int id)
        {
            try
            {
                if (CrawlResults.TryGetValue(id, out var result))
                {
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning($"Crawl result not found for id :{id}");
                    return NotFound("Crawl result not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting crawl result for id {id}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("pagerelations")]
        public async Task<IActionResult> GetPageRelations([FromQuery] string url, [FromQuery] int maxDepth = 1)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                { return BadRequest("URL cannot be empty"); }

                _logger.LogInformation($"Fetching page relations for URL: {url}, Max Depth: {maxDepth}");

                var pageRelations = await _crawlService.GetPageRelationsAsync(url, maxDepth);

                return Ok(pageRelations);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching page relations: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        
    }


}
