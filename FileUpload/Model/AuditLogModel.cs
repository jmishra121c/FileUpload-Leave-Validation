using FileUpload.Enum;
using Microsoft.VisualBasic.FileIO;
using System.ComponentModel.DataAnnotations;

namespace FileUpload.Model
{
    public class AuditLogModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
        public FileType? FileUploadType { get; set; }

        public ValidationStatus? Status { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
