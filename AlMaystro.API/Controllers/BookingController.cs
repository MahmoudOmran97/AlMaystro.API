using AlMaystro.API.Helpers;
using AlMaystro.API.Data;
using AlMaystro.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace AlMaystro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly AppDbContext _db;

    public BookingController(AppDbContext db)
    {
        _db = db;
    }

    // POST: api/booking  → عميل يحجز
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BookingRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CusName) || string.IsNullOrWhiteSpace(request.CusPhone))
            return BadRequest(new { success = false, message = "الاسم ورقم الهاتف مطلوبين" });


        var booking = new Booking
        {
            CusName = request.CusName.Trim(),
            CusPhone = request.CusPhone.Trim(),
            BookingDate = request.BookingDate,
            CreatedAt = DateTime.Now
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return Ok(new { success = true, message = "تم الحجز بنجاح ✅", bookingId = booking.Id });
    }

    // GET: api/booking  → عرض كل الحجوزات (للإدارة)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _db.Bookings
            .OrderByDescending(b => b.BookingDate)
            .Select(b => new
            {
                b.Id,
                b.CusName,
                b.CusPhone,
                BookingDate = b.BookingDate.ToString("yyyy-MM-dd HH:mm"),
                CreatedAt = b.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                Status = b.Status.ToString(),
                Source = b.Source.ToString(),
                ServiceName = b.Service != null ? b.Service.Name : null,
                b.Notes
            })
            .ToListAsync();

        return Ok(bookings);
    }

    // DELETE: api/booking/5  → حذف حجز
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking == null) return NotFound(new { success = false, message = "الحجز مش موجود" });

        _db.Bookings.Remove(booking);
        await _db.SaveChangesAsync();
        return Ok(new { success = true, message = "تم حذف الحجز" });
    }
    // GET: api/booking/unread → جلب الحجوزات الجديدة فقط
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnread()
    {
        var bookings = await _db.Bookings
            .Where(b => !b.IsRead)
            .OrderBy(b => b.BookingDate)
            .ToListAsync();

        // ← علّم عليهم كـ مقروء فورًا
        foreach (var b in bookings)
            b.IsRead = true;

        await _db.SaveChangesAsync();

        return Ok(bookings);
    }
    // أضف في BookingController

    // GET: api/booking/range?from=2025-01-01&to=2025-01-31
    [HttpGet("range")]
    public async Task<IActionResult> GetByRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var bookings = await _db.Bookings
            .Where(b => b.BookingDate.Date >= from.Date && b.BookingDate.Date <= to.Date)
            .OrderByDescending(b => b.BookingDate)
            .Select(b => new {
                b.Id,
                b.CusName,
                b.CusPhone,
                BookingDate = b.BookingDate.ToString("yyyy-MM-dd HH:mm"),
                CreatedAt = b.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                Status = b.Status.ToString(),
                Source = b.Source.ToString(),
                ServiceName = b.Service != null ? b.Service.Name : null,
                b.Notes
            })
            .ToListAsync();
        return Ok(bookings);
    }

    // DELETE: api/booking/cleanup
    [HttpDelete("cleanup")]
    public async Task<IActionResult> Cleanup()
    {
        var cutoff = ShopTime.Today.AddDays(-30);
        var old = await _db.Bookings
            .Where(b => b.BookingDate.Date < cutoff)
            .ToListAsync();
        _db.Bookings.RemoveRange(old);
        await _db.SaveChangesAsync();
        return Ok(new { success = true, deleted = old.Count });
    }
}

// DTO بسيط للطلب
public class BookingRequest
{
    public string? CusName { get; set; }
    public string? CusPhone { get; set; }
    public DateTime BookingDate { get; set; }
}
