using Elasticsearch.Net;
using Nest;

using System.Runtime.CompilerServices;

namespace Elasticsearch.API.Extensions
{
    public static class ElasticsearchExt
    {
        public static void AddElastic(this IServiceCollection services,IConfiguration configuration)
        {
            var elasticSearchConf = configuration.GetSection("Elastic");
            var pool = new SingleNodeConnectionPool(new Uri(elasticSearchConf["Url"]!));
            var settings = new ConnectionSettings(pool);
            //settings.BasicAuthentication(elasticSearchConf["Username"], elasticSearchConf["Password"]);
            var client = new ElasticClient(settings);
            services.AddSingleton(client);
        }
    }
}
