using Elastic.Clients.Elasticsearch;
using Elasticsearch.API.DTOs;
using Elasticsearch.API.Models;


namespace Elasticsearch.API.Repository
{
    public class ProductRepository
    {
        private readonly ElasticsearchClient _client;
        private const string indexName="products";
        public ProductRepository(ElasticsearchClient client)
        {
            _client=client;
        }
        //id değerini client tarafında set edip dbye yollamak performans açısından daha iyi olacaktır.
        public async Task<Product?> SaveAsync(Product newProduct)
        {
            newProduct.Created=DateTime.Now;
            //index metodu ile veriyi ekliyoruz Elastic tarafında indexlemek kaydetmek demektir.
            var response = await _client.IndexAsync(newProduct,x=>x.Index(indexName));
            
            //fast fail yöntemi mümkün olduğunca else'lerden kurtul.
            if(!response.IsSuccess()) return null;

            newProduct.Id=response.Id;
            return newProduct;
        }

        public async Task<IReadOnlyCollection<Product>> GetAllAsync()
        {
            var result = await _client.SearchAsync<Product>(s=>s.Index(indexName).Query(q=>q.MatchAll()));
            //ürün id'si source dışında hit içinde geliyor burada source'a setledik.
            foreach (var hit in result.Hits)
            {
                hit.Source.Id = hit.Id;
            }
            return result.Documents;
        }

        public async Task<Product?> GetByIdAsync(string Id)
        {
            var response = await _client.GetAsync<Product>(Id, x => x.Index(indexName));
            if (!response.IsSuccess()) { return null;}
            response.Source.Id = response.Id;
            return response.Source;
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var response = await _client.UpdateAsync<Product, ProductUpdateDto>(indexName, updateProduct.Id, x=>x.Doc(updateProduct));
            return response.IsSuccess();
        }

        public async Task<DeleteResponse> DeleteAsync(string id)
        {
            var response = await _client.DeleteAsync<Product>(id, x => x.Index(indexName));
            return response;
        }
    }
}
