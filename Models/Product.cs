using Elasticsearch.API.DTOs;

using Nest;

namespace Elasticsearch.API.Models
{
    public class Product
    {
        //elasticsearchte id değerleri metadatada tutulur. Dolayısıyla idnin metadataki id değeri olmasını sağlama için veririz.
        //vermezsek normal bir property olarak anlaşılır.
        [PropertyName("_id")]
        public string Id { get; set; } = null!; //not null
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public ProductFeature? Feature { get; set; }

        public ProductDto CreateDto() {
            if (Feature == null)
                return new ProductDto(Id, Name, Price, Stock,null);
            return new ProductDto(Id, Name, Price, Stock, new ProductFeatureDto(Feature.Width, Feature.Height, Feature.Color.ToString()));
        }
    }
}
