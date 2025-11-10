using Elasticsearch.API.DTOs;
using Elasticsearch.API.Models;

using Nest;

namespace Elasticsearch.API.Repository
{
    public class ProductRepository
    {
        private readonly ElasticClient _client;
        private const string indexName="products";
        public ProductRepository(ElasticClient client)
        {
            _client=client;
        }
        //id değerini client tarafında set edip dbye yollamak performans açısından daha iyi olacaktır.
        public async Task<Product?> SaveAsync(Product newProduct)
        {
            newProduct.Created=DateTime.Now;
            //index metodu ile veriyi ekliyoruz Elastic tarafında indexlemek kaydetmek demektir.
            var response = await _client.IndexAsync(newProduct,x=>x.Index(indexName).Id(Guid.NewGuid().ToString()));
            
            //fast fail yöntemi mümkün olduğunca else'lerden kurtul.
            if(!response.IsValid) return null;

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
            if (!response.IsValid) { return null;}
            response.Source.Id = response.Id;
            return response.Source;
        }

        public async Task<bool> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var response = await _client.UpdateAsync<Product, ProductUpdateDto>(updateProduct.Id, u => u.Index(indexName).Doc(updateProduct));
            return response.IsValid;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var response = await _client.DeleteAsync<Product>(id, x => x.Index(indexName));
            return response.IsValid;
        }
    }
}
