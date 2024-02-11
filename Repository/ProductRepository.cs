using Elasticsearch.API.Models;

using Nest;

namespace Elasticsearch.API.Repository
{
    public class ProductRepository
    {
        private readonly ElasticClient _client;
        public ProductRepository(ElasticClient client)
        {
            _client=client;
        }
        //id değerini client tarafında set edip dbye yollamak performans açısından daha iyi olacaktır.
        public async Task<Product?> SaveAsync(Product newProduct)
        {
            newProduct.Created=DateTime.Now;
            var response = await _client.IndexAsync(newProduct,x=>x.Index("products").Id(Guid.NewGuid().ToString()));
            
            //fast fail yöntemi mümkün olduğunca else'lerden kurtul.
            if(!response.IsValid) return null;

            newProduct.Id=response.Id;
            return newProduct;
        }
    }
}
