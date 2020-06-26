using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Task5_AWSAccount.Models;

namespace Task5_AWSAccount.Services
{
    public interface IS3Service
    {
        Task<S3Response> CreateBucketAsync(string bucketName);
        Task UploadFileAsync(string path);
    }
}