using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlMaystro.API.Models;

[Table("Attendances")]
public class Attendance
{
    [Key]
    public int AttendanceId { get; set; }
    public int? EmployeeId { get; set; }
    public string? NameEmployee { get; set; }
    public DateTime? AttendDate { get; set; }
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public string? Status { get; set; }
    public decimal? WorkedHours { get; set; }
    public decimal? OvertimeHours { get; set; }
}