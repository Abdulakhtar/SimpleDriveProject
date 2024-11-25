using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleDriveProject.Services;

namespace SimpleDriveProject.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class S3HttpClientController : Controller
    {
        private readonly S3Client _s3Client;

        public S3HttpClientController(S3Client s3Client)
        {
            _s3Client = s3Client;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            await _s3Client.UploadFileAsync(filePath);
            return Ok("File uploaded successfully.");
        }

        [HttpGet("retrieve/{fileName}")]
        public async Task<IActionResult> RetrieveFile(string fileName)
        {
            await _s3Client.RetrieveFileAsync(fileName);
            return Ok("File retrieved successfully.");
        }

        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            await _s3Client.DeleteFileAsync(fileName);
            return Ok("File deleted successfully.");
        }
    }
}
