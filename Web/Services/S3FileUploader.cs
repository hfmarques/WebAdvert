using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace Web.Services;

public class S3FileUploader : IFileUploader
{
    private readonly IConfiguration configuration;

    public S3FileUploader(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<bool> UploadFileAsync(string fileName, Stream storageStream)
    {
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("File name must be specified");

        var bucket = configuration["ImageBucket"];

        using var client = new AmazonS3Client();

        if (storageStream.Length > 0)
        {
            if (storageStream.CanSeek)
            {
                storageStream.Seek(0, SeekOrigin.Begin);
            }
        }

        var request = new PutObjectRequest
        {
            AutoCloseStream = true,
            BucketName = bucket,
            InputStream = storageStream,
            Key = fileName
        };

        var response = await client.PutObjectAsync(request).ConfigureAwait(false);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}