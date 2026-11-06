using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlMaystro.API.Models;

[Table("tb_appointment")]
public class Appointment
{
    [Key]
    public int Id { get; set; }
    public string? SellCus { get; set; }
    public double? SellTprice { get; set; }
    public DateTime? SellDate { get; set; }
    public double? Pay { get; set; }
    public double? Stay { get; set; }
    public double? TotalPrice { get; set; }
    public double? SellAdd { get; set; }
    public double? Cut { get; set; }
    public int? InvoiceId { get; set; }
    public double? TotalEarnings { get; set; }
    public string? Safe { get; set; }
    public string? NameCasher { get; set; }
    public string? BarberName { get; set; }
    public string? AppStatus { get; set; }
    public string? CusName { get; set; }

  
}