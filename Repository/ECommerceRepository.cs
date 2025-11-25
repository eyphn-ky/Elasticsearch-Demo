using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.API.Models.ECommerceModels;
using System.Collections.Immutable;

namespace Elasticsearch.API.Repository
{
    public class ECommerceRepository
    {
        private readonly ElasticsearchClient _client;
        private const string IndexName = "kibana_sample_data_ecommerce";
        public ECommerceRepository(ElasticsearchClient client)
        {
            _client = client;
        }
        public async Task<IImmutableList<ECommerce>> TermQueryAsync(string customerFirstName)
        {
            //1.yol
            //var result = await _client.SearchAsync<ECommerce>(s => s
            //    .Index(IndexName)
            //    .Query(q => q
            //        .Term(t => t
            //            .Field("customer_first_name.keyword")
            //            .Value(customerFirstName)
            //        )
            //    )
            //);

            //2.yol tip güvenli
            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Query(q => q.Term(t => t.CustomerFirstName.Suffix("keyword"), customerFirstName)));

            //3.yol
            var termQuery = new TermQuery("customer_first_name.keyword")
            {
                Value = customerFirstName,
                CaseInsensitive = true
            };
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Query(termQuery));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> TermsQueryAsync(List<string> customerFirstNamesList)
        {
            List<FieldValue> fieldValues = customerFirstNamesList.ConvertAll(c => (FieldValue)c);

            //var termsQuery = new TermsQuery()
            //{
            //    Field = "customer_first_name.keyword",
            //    Terms = new TermsQueryField(fieldValues.AsReadOnly())
            //};

            //var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Query(termsQuery));
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
            .Query(q => q.Terms(t => t.Field(f => f.CustomerFirstName.Suffix("keyword")).Terms(new TermsQueryField(fieldValues.AsReadOnly())))));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> PrefixQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
            .Query(q => q
                .Prefix(p => p
                    .Field(f => f.CustomerFullName
                        .Suffix("keyword"))
                            .Value(customerFullName))));
            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> RangeQueryAsync(double fromPrice, double toPrice)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
            .Query(q => q
                .Range(r => r.NumberRange(nr => nr
                    .Field(f => f.TaxfulTotalPrice)
                        .Gte(fromPrice).Lte(toPrice)))));
            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> MatchAllQueryAsync()
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Query(q => q.MatchAll()));
            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> PaginationQueryAsync(int page, int pageSize)
        {
            var pageFrom = (page - 1) * pageSize;
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Size(pageSize).From(pageFrom).Query(q => q.MatchAll()));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> WildCardQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Query(q => q.Wildcard(w=>w.Field(f=>f.CustomerFullName.Suffix("keyword")).Wildcard(customerFullName))));

            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }
            return result.Documents.ToImmutableList();
        }
    }
}
