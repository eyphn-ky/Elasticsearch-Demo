using Elasticsearch.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Elasticsearch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        [NonAction] //http metodu olarak kullanılamasın
        public IActionResult CreateActionResult<T>(ResponseDto<T> response)
        {
            if(response.Status == HttpStatusCode.NoContent)
                return new ObjectResult(null){ StatusCode = response.Status.GetHashCode()}; //veri yoksa uygun kodu dön

            return new ObjectResult(response) { StatusCode=response.Status.GetHashCode()};
        }
    }
}
