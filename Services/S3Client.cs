using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace SimpleDriveProject.Services
{
    public class S3Client
    {
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _bucket;
        private readonly string _endpoint;
        private readonly string _region;

        public S3Client(string accessKey, string secretKey, string bucket, string endpoint, string region)
        {
            _accessKey = accessKey;
            _secretKey = secretKey;
            _bucket = bucket;
            _endpoint = endpoint;
            _region = region;
        }

        private string CreateSignature(string method, string uri, string body = "")
        {
            // Request headers && signature as per AWS Signature v4
            var date = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
            string host = new Uri(_endpoint).Host;
            string requestPath = uri;

            string canonicalRequest = $"{method}\n{requestPath}\n\nhost:{host}\n\nhost\n{body}";
            string stringToSign = $"AWS4-HMAC-SHA256\n{date}\n{_region}/s3/aws4_request\n{HashString(canonicalRequest)}";

            var signingKey = GetSignatureKey(_secretKey, date.Substring(0, 8), _region, "s3");
            var signature = ComputeHmacSHA256(stringToSign, signingKey);

            return signature;
        }

        private string HashString(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private byte[] GetSignatureKey(string key, string dateStamp, string regionName, string serviceName)
        {
            byte[] kSecret = Encoding.UTF8.GetBytes("AWS4" + key);
            byte[] kDate = HmacSHA256(dateStamp, kSecret);
            byte[] kRegion = HmacSHA256(regionName, kDate);
            byte[] kService = HmacSHA256(serviceName, kRegion);
            byte[] kSigning = HmacSHA256("aws4_request", kService);
            return kSigning;
        }

        private byte[] HmacSHA256(string data, byte[] key)
        {
            using (var hmacsha256 = new HMACSHA256(key))
            {
                return hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            }
        }

        private string ComputeHmacSHA256(string data, byte[] key)
        {
            byte[] hmac = HmacSHA256(data, key);
            return BitConverter.ToString(hmac).Replace("-", "").ToLower();
        }

        public async Task UploadFileAsync(string filePath)
        {
            using (HttpClient client = new HttpClient())
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{_endpoint}/{_bucket}/{Path.GetFileName(filePath)}")
                {
                    Content = new ByteArrayContent(fileBytes)
                };

                request.Headers.Add("x-amz-date", DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ"));
                request.Headers.Add("Authorization", $"AWS4-HMAC-SHA256 Credential={_accessKey}/{DateTime.UtcNow:yyyyMMdd}/{_region}/s3/aws4_request, SignedHeaders=host, Signature={CreateSignature("PUT", $"/{_bucket}/{Path.GetFileName(filePath)}")}");
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("File uploaded successfully.");
                }
                else
                {
                    Console.WriteLine($"Error uploading file: {response.ReasonPhrase}");
                }
            }
        }

        public async Task RetrieveFileAsync(string fileName)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_endpoint}/{_bucket}/{fileName}");
                request.Headers.Add("x-amz-date", DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ"));
                request.Headers.Add("Authorization", $"AWS4-HMAC-SHA256 Credential={_accessKey}/{DateTime.UtcNow:yyyyMMdd}/{_region}/s3/aws4_request, SignedHeaders=host, Signature={CreateSignature("GET", $"/{_bucket}/{fileName}")}");

                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    byte[] fileContent = await response.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes($"downloaded_{fileName}", fileContent);
                    Console.WriteLine("File downloaded successfully.");
                }
                else
                {
                    Console.WriteLine($"Error retrieving file: {response.ReasonPhrase}");
                }
            }
        }

        public async Task DeleteFileAsync(string fileName)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_endpoint}/{_bucket}/{fileName}");
                request.Headers.Add("x-amz-date", DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ"));
                request.Headers.Add("Authorization", $"AWS4-HMAC-SHA256 Credential={_accessKey}/{DateTime.UtcNow:yyyyMMdd}/{_region}/s3/aws4_request, SignedHeaders=host, Signature={CreateSignature("DELETE", $"/{_bucket}/{fileName}")}");

                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("File deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Error deleting file: {response.ReasonPhrase}");
                }
            }
        }
    }
}
