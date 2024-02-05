using Elasticsearch.API.DTOs;
using Elasticsearch.API.Repository;

using System.Net;

namespace Elasticsearch.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productrepository;
        public ProductService(ProductRepository productrepository)
        {
            _productrepository = productrepository;
        }

        public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
        {
            var responseProduct = await _productrepository.SaveAsync(request.CreateProduct());
            if(responseProduct  == null) {
                return ResponseDto<ProductDto>.Fail(
                    new List<string> 
                    { 
                        "Kayıt esnasında bir hata meydana geldi." 
                    }, 
                    HttpStatusCode.InternalServerError 
                );
            }
            else
            {
                return ResponseDto<ProductDto>.Success(responseProduct.CreateDto(), HttpStatusCode.Created);
            }
        }
    }
}
