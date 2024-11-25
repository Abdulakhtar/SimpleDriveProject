using SimpleDriveProject.Models;
using System.Threading.Tasks;

namespace SimpleDriveProject.Services
{
    public interface IBlobService
    {
        Task<Blob> StoreBlobAsync(string id, string data);
        Task<Blob> RetrieveBlobAsync(string id);
    }
}
