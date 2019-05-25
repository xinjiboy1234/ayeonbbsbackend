using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ayeonbbsbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public FileUploadController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost("uploadpostimg")]
        public ActionResult PostImgUpload(IFormCollection files)
        {
            try
            {
                if (files == null)
                    return null;
                var fileDir = _configuration["imguploadpath"];
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }
                var projectFileName = Guid.NewGuid() + ".png";
                //上传的文件的路径
                var filePath = fileDir + @"/" + projectFileName;
                using (var fs = System.IO.File.Create(filePath))
                {
                    files.Files[0].CopyTo(fs);
                    fs.Flush();
                }
                return Ok(new
                {
                    uploaded = "1",
                    fileName = projectFileName,
                    url = "http://" + Request.Host + "/uploadimg/" + projectFileName
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
    }
}