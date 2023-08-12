using System.ComponentModel.DataAnnotations;

namespace FileUpload.Model
{
    public class LeaveApplicationModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Reason { get; set; }

        public string Comments { get; set; }

        public EmployeeModel Employee { get; set; }
    }
}
