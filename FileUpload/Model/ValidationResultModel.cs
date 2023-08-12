using FileUpload.Enum;

namespace FileUpload.Model
{
    public class ValidationResultModel
    {
        public string? StatusDesc { get; set; } = null;
        public ValidationStatus Status { get; set; }
        public string? ErrorMessage { get; set; } = null;
    }
}
