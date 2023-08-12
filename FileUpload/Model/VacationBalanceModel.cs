using System.ComponentModel.DataAnnotations;

namespace FileUpload.Model
{
    public class VacationBalanceModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(7)]
        public string MonthYear { get; set; } 

        [Required]
        public int Balance { get; set; }

        public EmployeeModel Employee { get; set; }
    }
}
