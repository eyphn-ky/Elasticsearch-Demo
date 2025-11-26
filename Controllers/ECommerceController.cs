using Elasticsearch.API.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Elasticsearch.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ECommerceController : ControllerBase
    {
        private readonly ECommerceRepository _repository;
        public ECommerceController(ECommerceRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> TermQuery(string customerFirstName)
        {
            return Ok(await _repository.TermQueryAsync(customerFirstName));
        }

        [HttpPost]
        public async Task<IActionResult> TermsQuery(List<string> customerFirstNameList)
        {
            return Ok(await _repository.TermsQueryAsync(customerFirstNameList));
        }

        [HttpGet]
        public async Task<IActionResult> PrefixQuery(string customerFullName)
        {
            return Ok(await _repository.PrefixQueryAsync(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> RangeQuery(double fromPrice, double toPrice)
        {
            return Ok(await _repository.RangeQueryAsync(fromPrice, toPrice));
        }

        [HttpGet]
        public async Task<IActionResult> MatchAllQuery()
        {
            return Ok(await _repository.MatchAllQueryAsync());
        }

        [HttpGet]
        public async Task<IActionResult> PaginationQuery(int page = 1, int pageSize = 3)
        {
            return Ok(await _repository.PaginationQueryAsync(page, pageSize));
        }

        [HttpGet]
        public async Task<IActionResult> WildCardQuery(string customerFullName)
        {
            return Ok(await _repository.WildCardQueryAsync(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> FuzzyQuery(string customerFirstName)
        {
            return Ok(await _repository.FuzzyQueryAsync(customerFirstName));
        }

        [HttpGet]
        public async Task<IActionResult> MatchAllFullTextQuery(string category)
        {
            return Ok(await _repository.MatchAllFullTextQueryAsync(category));
        }

        [HttpGet]
        public async Task<IActionResult> MatchBoolPrefixQuery(string customerFullName)
        {
            return Ok(await _repository.MatchBoolPrefixFullTextQueryAsync(customerFullName));
        }

        [HttpGet]
        public async Task<IActionResult> MatchPhraseQuery(string customerFullName)
        {
            return Ok(await _repository.MatchPhraseFullTextQueryAsync(customerFullName));
        }


        [HttpGet]
        public async Task<IActionResult> CompoundQueryExampleOne(string cityName, double taxfulTotalPrice, string categoryName, string manufacturer)
        {
            return Ok(await _repository.CompoundQueryExampleOneAsync(cityName, taxfulTotalPrice, categoryName, manufacturer));
        }
    }
}
