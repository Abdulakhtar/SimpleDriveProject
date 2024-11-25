namespace SimpleDriveProject.Models
{
    public class BlobStorage
    {
        public string Id { get; set; }
        public byte[] BlobData { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}
