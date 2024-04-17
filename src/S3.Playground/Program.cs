using Amazon.S3;
using Amazon.S3.Model;
using System.Net;
using System.Text;

var fileName = "test.txt";
var contentType = "text/plain";

await UploadFileAsync(fileName, contentType);

await DownloadFileAsync(fileName, true);
static async Task DownloadFileAsync(string fileName, bool printContents = false)
{
    var s3Client = new AmazonS3Client();    
    var getObjectRequest = new GetObjectRequest
    {
        BucketName = "dome-course",
        Key = $"files/{fileName}"
    };

    var response = await s3Client.GetObjectAsync(getObjectRequest);
    if (response is null || response.HttpStatusCode != HttpStatusCode.OK)
    {
        Console.WriteLine("Failed to download file");
        return;
    }

    using var memoryStream = new MemoryStream();
    await response.ResponseStream.CopyToAsync(memoryStream);

    if (!printContents)
    {
        return;
    }

    var text = Encoding.Default.GetString(memoryStream.ToArray());
    Console.WriteLine(text);
}

static async Task UploadFileAsync(string fileName, string contentType = "application/octet-stream")
{
    var s3Client = new AmazonS3Client();

    await using var inputStream = new FileStream($"./{fileName}", FileMode.Open, FileAccess.Read);
    var putObjectRequest = new PutObjectRequest
    {
        BucketName = "dome-course",
        Key = $"files/{fileName}",
        ContentType = contentType,
        InputStream = inputStream
    };


    await s3Client.PutObjectAsync(putObjectRequest);
}