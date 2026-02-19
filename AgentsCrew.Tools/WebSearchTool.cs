using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace AgentsCrew.Tools
{
    public interface IWebSearchService
    {
        Task<string> SearchAsync(string query);
    }

    public class WebSearchTool
    {
        private readonly IWebSearchService _searchService;

        public WebSearchTool(IWebSearchService searchService)
        {
            _searchService = searchService;
        }

        [KernelFunction]
        [Description("Searches the web for a given query.")]
        public async Task<string> SearchAsync(
            [Description("The query to search for.")] string query
        )
        {
            return await _searchService.SearchAsync(query);
        }
    }
}
