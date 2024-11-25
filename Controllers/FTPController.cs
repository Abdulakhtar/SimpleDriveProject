using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleDriveProject.Models;
using System.IO;
using System.Net;
using System.Text;

namespace SimpleDriveProject.Controllers
{
    [ApiController]
    [Route("v1/[controller]/")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class FTPController : ControllerBase
    {
        private static string _host;
        private static string _username;
        private static string _password;
        private static string _remotePath;

        // POST method to save FTP configuration
        [HttpPost("configure")]
        public IActionResult ConfigureFTP([FromBody] FTPConfig ftpConfig)
        {
            _host = ftpConfig.Host;
            _username = ftpConfig.Username;
            _password = ftpConfig.Password;
            _remotePath = ftpConfig.RemotePath;
            return Ok(new { Message = "FTP configuration saved successfully." });
        }

        // POST method to upload a file to the configured FTP server
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public IActionResult UploadToFtp(IFormFile file)
        {
            if (string.IsNullOrEmpty(_host) || string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password) || string.IsNullOrEmpty(_remotePath))
            {
                return BadRequest(new { Message = "FTP configuration is incomplete. Please configure FTP first." });
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "No file uploaded or file is empty." });
            }

            try
            {
                var safeRemotePath = _remotePath.TrimEnd('/').TrimStart('/');
                var uri = new Uri($"ftp://{_host}/{safeRemotePath}/{file.FileName}");

                var ftpRequest = (FtpWebRequest)WebRequest.Create(uri);
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Credentials = new NetworkCredential(_username, _password);
                ftpRequest.UseBinary = true;
                ftpRequest.KeepAlive = false;
                ftpRequest.UsePassive = true;

                try
                {
                    var dirRequest = (FtpWebRequest)WebRequest.Create(new Uri($"ftp://{_host}/{safeRemotePath}"));
                    dirRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                    dirRequest.Credentials = ftpRequest.Credentials;

                    using (var response = (FtpWebResponse)dirRequest.GetResponse())
                    {
                        // Directory exists, do nothing
                    }
                }
                catch (WebException)
                {
                    // Directory does not exist, create it
                    var createDirRequest = (FtpWebRequest)WebRequest.Create(new Uri($"ftp://{_host}/{safeRemotePath}"));
                    createDirRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                    createDirRequest.Credentials = ftpRequest.Credentials;
                    using (var response = (FtpWebResponse)createDirRequest.GetResponse())
                    {
                        // Directory created
                    }
                }

                using (var fileStream = file.OpenReadStream())
                {
                    using (var ftpStream = ftpRequest.GetRequestStream())
                    {
                        fileStream.CopyTo(ftpStream);
                    }
                }

                using (var response = (FtpWebResponse)ftpRequest.GetResponse())
                {
                    if (response.StatusCode == FtpStatusCode.ClosingData)
                    {
                        return Ok(new { Message = "File uploaded successfully." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error uploading file to FTP server.",
                    Details = ex.Message
                });
            }

            return Ok(new { Message = "File uploaded successfully." });

        }
    }
}
