
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace Elasticsearch.API.Extensions
{
    public static class ElasticsearchExt
    {
        public static void AddElastic(this IServiceCollection services,IConfiguration configuration)
        { 
            var elasticSearchConf = configuration.GetSection("Elastic");
            string username = elasticSearchConf["Username"]!.ToString();
            var password = elasticSearchConf["Password"]!.ToString();
            var settings = new ElasticsearchClientSettings(new Uri(elasticSearchConf["Url"]!)).Authentication(new BasicAuthentication(username, password));
            var client = new ElasticsearchClient(settings); 
            services.AddSingleton(client);
        }
    }
}
