using FileUpload.BusinessLogic;
using FileUpload.Enum;
using FileUpload.Interface.BusinessLogic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IUploadService uploadService;

        public FileUploadController(IUploadService uploadService)
        {
            this.uploadService = uploadService;
        }

        [HttpPost("upload")]
        //I should have added Authorization Filter
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");
                else
                {
                    var validationResult = await uploadService.Upload(file);
                    return Ok(validationResult);
                }
               
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
