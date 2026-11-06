using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlMaystro.API.Models;

[Table("Notfications")]
public class Notification
{
    [Key]
    public int Id { get; set; }
    public string? Notfication1 { get; set; }
    public DateTime? CreatedAt { get; set; }
    public bool IsRead { get; set; } = false;
}