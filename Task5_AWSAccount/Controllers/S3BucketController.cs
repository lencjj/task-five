using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task5_AWSAccount.Services;

namespace Task5_AWSAccount.Controllers
{
    [Produces("application/json")]
    [Route("api/S3Bucket")]
    public class S3BucketController : ControllerBase
    {
        private readonly IS3Service _service;
        private IHostingEnvironment _env;

        public S3BucketController(IS3Service service, IHostingEnvironment env)
        {
            _service = service;
            _env = env;
        }

        [HttpPost("{bucketName}")]
        public async Task<IActionResult> CreateBucket([FromRoute] string bucketName)
        {
            var response = await _service.CreateBucketAsync(bucketName);

            return Ok(response);
        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var dir = _env.ContentRootPath;
            string path = Path.Combine(dir, file.FileName);
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }

            
            await _service.UploadFileAsync(path);

            return Redirect("/Home/Talent");
        }

        [HttpPost]
        [Route("ShortenURL")]
        public async Task<string> ShortenURL(string url)
        {
            Console.WriteLine(url);
            string _bitlyToken = "4dc7840ba918cba5ecf20e416f1edaa19faa936a";
            HttpClient client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,
                "https://api-ssl.bitly.com/v4/shorten")
            {
                Content = new StringContent($"{{\"long_url\":\"{url}\"}}",
                                                Encoding.UTF8,
                                                "application/json")
            };

            try
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bitlyToken);

                var response = await client.SendAsync(request).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    return "";

                string responsestr = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responsestr);
                return responsestr;

            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}