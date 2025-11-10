using Elasticsearch.API.Models;

namespace Elasticsearch.API.DTOs
{
    //record c# 9 ile geldi
    //record immutable'dır. Oluşturulduktan sonra değeri değiştirilmez. Değiştirilmek istenen yerde hata verecektir.
    public record ProductCreateDto(string Name, decimal Price, int Stock, ProductFeatureDto Feature)
    {
        //clean code yaklaşımına göre ilgili işi kim yapıyorsa ona yakın yap diyebiliriz ve Mapleme işini burada yapabiliriz.
        public Product CreateProduct()
        {
            return new Product
            {
                Name = Name,
                Price = Price,
                Stock = Stock,
                Feature = new ProductFeature()
                {
                    Width = Feature.Width,
                    Height = Feature.Height,
                    Color = (EColor)int.Parse(Feature.Color)
                }
            };
        }
    }
}
