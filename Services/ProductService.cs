using Elastic.Clients.Elasticsearch;
using Elasticsearch.API.DTOs;
using Elasticsearch.API.Repository;
using System.Collections.Immutable;
using System.Net;

namespace Elasticsearch.API.Services
{
    public class ProductService
    {
        private readonly ProductRepository _productrepository;
        private readonly ILogger<ProductService> _logger;
        public ProductService(ProductRepository productrepository, ILogger<ProductService> logger)
        {
            _productrepository = productrepository;
            _logger = logger;
        }

        public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
        {
            var responseProduct = await _productrepository.SaveAsync(request.CreateProduct());
            if (responseProduct == null)
            {
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

        public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
        {
            var products = await _productrepository.GetAllAsync();
            var productListDto = new List<ProductDto>();
            //var productListDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock, new ProductFeatureDto(p.Feature.Width, p.Feature.Height, p.Feature.Color))).ToList();

            foreach (var x in products)
            {
                if (x.Feature is null)
                {
                    productListDto.Add(new ProductDto(x.Id, x.Name, x.Price, x.Stock, null));
                }
                else
                {
                    productListDto.Add(new ProductDto(x.Id, x.Name, x.Price, x.Stock, new ProductFeatureDto(x.Feature.Width, x.Feature.Height, x.Feature.Color.ToString())));
                }
            }
            return ResponseDto<List<ProductDto>>.Success(productListDto, HttpStatusCode.OK);
        }

        public async Task<ResponseDto<ProductDto>> GetByIdAsync(string id)
        {
            var hasProduct = await _productrepository.GetByIdAsync(id);
            if (hasProduct == null)
            {
                return ResponseDto<ProductDto>.Fail("Ürün bulunamadı.", HttpStatusCode.NotFound);
            }
            return ResponseDto<ProductDto>.Success(hasProduct.CreateDto(), HttpStatusCode.OK);
        }
        
        public async Task<ResponseDto<bool>> UpdateAsync(ProductUpdateDto updateProduct)
        {
            var isSuccess = await _productrepository.UpdateAsync(updateProduct);
            if (!isSuccess)
            {
                return ResponseDto<bool>.Fail(new List<string> { "Update sırasında bir hata meydana geldi"}, HttpStatusCode.InternalServerError);
            }
            return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
        }

        public async Task<ResponseDto<bool>> DeleteAsync(string id)
        {
            var deleteResponse = await _productrepository.DeleteAsync(id);
            if(!deleteResponse.IsSuccess() && deleteResponse.Result == Result.NotFound)
            {
                return ResponseDto<bool>.Fail(new List<string> { "Silinecek ürün bulunamadı" }, HttpStatusCode.NotFound);
            }
            if (!deleteResponse.IsSuccess())
            {
                deleteResponse.TryGetOriginalException(out Exception? exception);
                _logger.LogError(exception, deleteResponse.ElasticsearchServerError.Error.ToString());

                return ResponseDto<bool>.Fail(new List<string> { "Delete işlemi sırasında bir hata oluştu" }, HttpStatusCode.InternalServerError);
            }

            return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
        }
    }
}
