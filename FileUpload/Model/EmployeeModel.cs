using System.ComponentModel.DataAnnotations;

namespace FileUpload.Model
{
    public class EmployeeModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(5)]
        public string EmployeeNumber { get; set; }

        [Required]
        public string FirstName { get; set; }
    }
}
