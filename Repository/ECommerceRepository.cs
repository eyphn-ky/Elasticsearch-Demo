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

            SetResultSourceIdField(result);
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

            SetResultSourceIdField(result);
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
            SetResultSourceIdField(result);
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> RangeQueryAsync(double fromPrice, double toPrice)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName)
            .Query(q => q
                .Range(r => r.NumberRange(nr => nr
                    .Field(f => f.TaxfulTotalPrice)
                        .Gte(fromPrice).Lte(toPrice)))));
            SetResultSourceIdField(result);
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> MatchAllQueryAsync()
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Query(q => q.MatchAll()));
            SetResultSourceIdField(result);
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> PaginationQueryAsync(int page, int pageSize)
        {
            var pageFrom = (page - 1) * pageSize;
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Size(pageSize).From(pageFrom).Query(q => q.MatchAll()));

            SetResultSourceIdField(result);
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> WildCardQueryAsync(string customerFullName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s.Index(IndexName).Query(q => q.Wildcard(w => w.Field(f => f.CustomerFullName.Suffix("keyword")).Wildcard(customerFullName))));

            SetResultSourceIdField(result);
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> FuzzyQueryAsync(string customerFirstName)
        {
            var result = await _client.SearchAsync<ECommerce>(s => s
                .Index(IndexName)
                    .Query(q => q
                        .Fuzzy(f => f
                            .Field(fi => fi.CustomerFirstName
                            .Suffix("keyword"))
                            .Value(customerFirstName)
                            .Fuzziness(new Fuzziness(2))))
                    .Sort(p => p.Field(f => f.OrderDate, new FieldSort() { Order = SortOrder.Desc })));

            SetResultSourceIdField(result);
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> MatchAllFullTextQueryAsync(string categoryName)
        {

            var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(IndexName)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Category)
                        .Query(categoryName).Operator(Operator.And)))); //=> Operator.And kelimeleri bir bütün gibi düşün her kelimenin ayrı ayrı geçtiği verileri getirme sadece tammaen eşleşenleri getir.

            SetResultSourceIdField(result);
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> MatchBoolPrefixFullTextQueryAsync(string customerFullName)
        {

            var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(IndexName)
                .Query(q => q
                    .MatchBoolPrefix(m => m
                        .Field(f => f.CustomerFullName)
                        .Query(customerFullName).Operator(Operator.Or))));

            SetResultSourceIdField(result);
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> MatchPhraseFullTextQueryAsync(string customerFullName)
        {

            var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(IndexName)
                .Query(q => q
                    .MatchPhrase(m => m
                        .Field(f => f.CustomerFullName)
                        .Query(customerFullName))));

            SetResultSourceIdField(result);
            return result.Documents.ToImmutableList();
        }

        public async Task<IImmutableList<ECommerce>> CompoundQueryExampleOneAsync(string cityName, double taxfulTotalPrice, string categoryName, string manufacturer)
        {

            var result = await _client.SearchAsync<ECommerce>(s => s
            .Index(IndexName)
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Term(t=>t
                                .Field("geoip.city_name")
                                    .Value(cityName))) //must içinde de arttırmak istediklerini diğerleri içinde uç uca ekleyebilirsin
                        .MustNot(mn => mn
                            .Range(r=>r
                                .NumberRange(nr=>nr
                                    .Field(fi=>fi.TaxfulTotalPrice).Lte(taxfulTotalPrice))))
                        .Should(s=>s
                            .Term(t=>t
                                .Field(f=>f.Category.Suffix("keyword"))
                                    .Value(categoryName)))
                        .Filter(f=>f
                            .Term(t=>t
                                .Field("manufacturer.keyword")
                                    .Value(manufacturer))))));

            SetResultSourceIdField(result);
            return result.Documents.ToImmutableList();
        }

        private SearchResponse<ECommerce> SetResultSourceIdField(SearchResponse<ECommerce> result)
        {
            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }
            return result;
        }
    }
}
