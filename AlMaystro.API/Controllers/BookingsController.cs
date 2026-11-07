using AlMaystro.API.Data;
using AlMaystro.API.Dtos;
using AlMaystro.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace AlMaystro.API.Controllers;

[ApiController, Route("api/bookings")]
public class BookingsController(AppDbContext db) : ControllerBase
{
    static readonly TimeZoneInfo Cairo = FindCairo();
    static TimeZoneInfo FindCairo()
    {
        foreach (var id in new[] { "Egypt Standard Time", "Africa/Cairo" })
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); } catch { }
        return TimeZoneInfo.Local;
    }
    // The database stores shop-local time. Web/desktop send ISO (UTC) -> convert to Cairo time.
    static DateTime ToShopTime(DateTime d) => d.Kind switch
    {
        DateTimeKind.Utc => TimeZoneInfo.ConvertTimeFromUtc(d, Cairo),
        DateTimeKind.Local => TimeZoneInfo.ConvertTime(d.ToUniversalTime(), Cairo),
        _ => d
    };
    static DateTime ShopNow => TimeZoneInfo.ConvertTime(DateTime.UtcNow, Cairo);

    // Owner: /api/bookings?from=2026-11-06&to=2026-11-07&status=Pending
    [Authorize(Roles = "Owner"), HttpGet]
    public async Task<List<BookingDto>> Get(DateTime? from, DateTime? to, BookingStatus? status)
    {
        var q = db.Bookings.AsNoTracking().Include(b => b.Service).AsQueryable();
        if (from is not null) q = q.Where(b => b.BookingDate >= from.Value.Date);
        if (to is not null) q = q.Where(b => b.BookingDate < to.Value.Date.AddDays(1));
        if (status is not null) q = q.Where(b => b.Status == status);
        return await q.OrderBy(b => b.BookingDate).Select(b => new BookingDto(
            b.Id, b.CusName ?? "", b.CusPhone ?? "", b.ServiceId, b.Service != null ? b.Service.Name : null,
            b.BookingDate, b.Status, b.Source, b.Notes)).ToListAsync();
    }

    // Public: website + desktop. Desktop sends Source=Desktop.
    [HttpPost, EnableRateLimiting("booking")]
    public async Task<IActionResult> Create(BookingCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CustomerName) || string.IsNullOrWhiteSpace(dto.Phone))
            return BadRequest("الاسم ورقم الموبايل مطلوبين");
        var when = ToShopTime(dto.Date);
        if (dto.Source == BookingSource.Web && when < ShopNow.AddMinutes(-5))
            return BadRequest("لا يمكن الحجز في وقت سابق");

        var b = new Booking
        {
            CusName = dto.CustomerName.Trim(),
            CusPhone = dto.Phone.Trim(),
            BookingDate = when,
            CreatedAt = ShopNow,
            ServiceId = dto.ServiceId,
            Source = dto.Source,
            Notes = dto.Notes
        };
        db.Bookings.Add(b);
        await db.SaveChangesAsync();
        return Created($"api/bookings/{b.Id}", new { b.Id });
    }

    [Authorize(Roles = "Owner"), HttpPut("{id:int}/status")]
    public async Task<IActionResult> SetStatus(int id, BookingStatusDto dto)
    {
        var b = await db.Bookings.FindAsync(id);
        if (b is null) return NotFound();
        b.Status = dto.Status;
        await db.SaveChangesAsync();
        return NoContent();
    }
}
