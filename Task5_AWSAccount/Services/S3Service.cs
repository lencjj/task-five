using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task5_AWSAccount.Models;

namespace Task5_AWSAccount.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _client;

        public S3Service(IAmazonS3 client)
        {
            _client = client;
        }

        public async Task<S3Response> CreateBucketAsync(String bucketName)
        {
            try
            {
                if (await AmazonS3Util.DoesS3BucketExistV2Async(_client, bucketName) == false)
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    var response = await _client.PutBucketAsync(putBucketRequest);

                    return new S3Response
                    {
                        Message = response.ResponseMetadata.RequestId,
                        Status = response.HttpStatusCode
                    };
                }
            } catch(AmazonS3Exception e)
            {
                return new S3Response
                {
                    Message = e.Message,
                    Status = e.StatusCode
                };
            } catch(Exception ex)
            {
                return new S3Response
                {
                    Message = ex.Message,
                    Status = System.Net.HttpStatusCode.InternalServerError
                };
            }
            return new S3Response
            {
                Message = "Something went wrong!",
                Status = System.Net.HttpStatusCode.InternalServerError
            };
        }

        private const String FilePath = "C:\\Users\\Lenc\\Desktop\\IMG-20190630-WA0066.jpg";

        public async Task UploadFileAsync(string path)
        {
            try
            {
                var fileTransferUtitlity = new TransferUtility(_client);

                await fileTransferUtitlity.UploadAsync(path, "task5talentphotos");
               
            }

            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }

        
    }
}
