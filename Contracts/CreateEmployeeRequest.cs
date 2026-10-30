using System.ComponentModel.DataAnnotations;

namespace EmployeeApi.Contracts;

public class CreateEmployeeRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Position { get; set; } = string.Empty;

    [Required]
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal? Salary { get; set; }
}
