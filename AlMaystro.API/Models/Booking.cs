namespace AlMaystro.API.Models;

public class Booking
{
    public int Id { get; set; }
    public string? CusName { get; set; }
    public string? CusPhone { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsRead { get; set; } = false; // ← جديد
    public int? ServiceId { get; set; }
    public Service? Service { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public BookingSource Source { get; set; } = BookingSource.Web;
    public string? Notes { get; set; }
}