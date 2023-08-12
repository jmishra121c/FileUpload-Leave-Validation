using FileUpload.Model;

namespace FileUpload.Interface.BusinessLogic
{
    public interface IUploadService
    {
        Task<ValidationResultModel> Upload(IFormFile file);
    }
}