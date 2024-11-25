using Microsoft.EntityFrameworkCore;
using SimpleDriveProject.Data;
using SimpleDriveProject.Models;
using System;
using System.Threading.Tasks;

namespace SimpleDriveProject.Services
{
    public class BlobService : IBlobService
    {
        private readonly ApplicationDbContext _context;

        public BlobService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Blob> StoreBlobAsync(string id, string data)
        {
            byte[] decodedData = Convert.FromBase64String(data);

            var blobStorage = new BlobStorage
            {
                Id = id,
                BlobData = decodedData,
                CreatedAt = DateTime.UtcNow
            };

            _context.BlobStorages.Add(blobStorage);
            await _context.SaveChangesAsync();

            return new Blob
            {
                Id = blobStorage.Id,
                Data = data,
                Size = decodedData.Length,
                CreatedAt = blobStorage.CreatedAt
            };
        }

        public async Task<Blob> RetrieveBlobAsync(string id)
        {
            var blobStorage = await _context.BlobStorages.FindAsync(id);
            if (blobStorage == null)
            {
                return null;
            }

            return new Blob
            {
                Id = blobStorage.Id,
                Data = Convert.ToBase64String(blobStorage.BlobData),
                Size = blobStorage.BlobData.Length,
                CreatedAt = blobStorage.CreatedAt
            };
        }
    }
}
