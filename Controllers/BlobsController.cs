using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleDriveProject.Models;
using SimpleDriveProject.Services;
using System.Threading.Tasks;

namespace SimpleDriveProject.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class BlobController : ControllerBase
    {
        private readonly IBlobService _blobService;

        public BlobController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        [HttpPost]
        public async Task<IActionResult> StoreBlob([FromBody] Blob blob)
        {
            if (string.IsNullOrEmpty(blob.Id) || string.IsNullOrEmpty(blob.Data))
            {
                return BadRequest("Id and Data are required.");
            }

            try
            {
                var storedBlob = await _blobService.StoreBlobAsync(blob.Id, blob.Data);
                return Ok(storedBlob);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> RetrieveBlob(string id)
        {
            try
            {
                var blob = await _blobService.RetrieveBlobAsync(id);

                if (blob == null)
                {
                    return NotFound($"Blob with Id {id} not found.");
                }

                return Ok(blob);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
