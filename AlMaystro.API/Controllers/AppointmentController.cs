using AlMaystro.API.Helpers;
using AlMaystro.API.Data;
using AlMaystro.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlMaystro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly AppDbContext _db;
    public AppointmentController(AppDbContext db) => _db = db;

    // POST: api/appointment/sync  ← الديسكتوب يبعت الفواتير
    [HttpPost("sync")]
    public async Task<IActionResult> Sync([FromBody] List<AppointmentDto> records)
    {
        if (records == null || !records.Any())
            return BadRequest(new { success = false, message = "لا توجد بيانات" });

        int added = 0, updated = 0;

        foreach (var rec in records)
        {
            var existing = await _db.Appointments
                .FirstOrDefaultAsync(x => x.InvoiceId == rec.InvoiceId);

            if (existing == null)
            {
                _db.Appointments.Add(new Appointment
                {
                    SellCus = rec.SellCus,
                    SellTprice = rec.SellTprice,
                    SellDate = rec.SellDate,
                    Pay = rec.Pay,
                    Stay = rec.Stay,
                    TotalPrice = rec.TotalPrice,
                    SellAdd = rec.SellAdd,
                    Cut = rec.Cut,
                    InvoiceId = rec.InvoiceId,
                    TotalEarnings = rec.TotalEarnings,
                    Safe = rec.Safe,
                    NameCasher = rec.NameCasher,
                    BarberName = rec.BarberName,
                    AppStatus = rec.AppStatus,
                    CusName = rec.CusName,
                   
                });
                added++;
            }
            else
            {
                existing.SellCus = rec.SellCus;
                existing.SellTprice = rec.SellTprice;
                existing.SellDate = rec.SellDate;
                existing.Pay = rec.Pay;
                existing.Stay = rec.Stay;
                existing.TotalPrice = rec.TotalPrice;
                existing.SellAdd = rec.SellAdd;
                existing.Cut = rec.Cut;
                existing.TotalEarnings = rec.TotalEarnings;
                existing.Safe = rec.Safe;
                existing.NameCasher = rec.NameCasher;
                existing.BarberName = rec.BarberName;
                existing.AppStatus = rec.AppStatus;
                existing.CusName = rec.CusName;
             
                updated++;
            }
        }

        await _db.SaveChangesAsync();
        return Ok(new { success = true, added, updated });
    }
    // POST: api/appointment/sync-delete
    // الديسكتوب بيبعت IDs الموجودة عنده — السيرفر يحذف اللي مش فيها
    [HttpPost("sync-delete")]
    public async Task<IActionResult> SyncDelete([FromBody] List<int> localIds)
    {
        if (localIds == null) return BadRequest();

        // ✅ كل الفواتير على السيرفر — مش بس اليوم
        var allServerIds = await _db.Appointments
            .Where(x => x.InvoiceId.HasValue)
            .Select(x => x.InvoiceId.Value)
            .ToListAsync();

        var deletedIds = allServerIds
            .Where(sid => !localIds.Contains(sid))
            .ToList();

        if (deletedIds.Any())
        {
            var toDelete = await _db.Appointments
                .Where(x => x.InvoiceId.HasValue && deletedIds.Contains(x.InvoiceId.Value))
                .ToListAsync();

            _db.Appointments.RemoveRange(toDelete);
            await _db.SaveChangesAsync();
        }

        return Ok(new { success = true, deleted = deletedIds.Count });
    }
    // GET: api/appointment/today  ← الماوي يجيب فواتير اليوم
    [HttpGet("today")]
    public async Task<IActionResult> GetToday()
    {
        var today = ShopTime.Today;
        var data = await _db.Appointments
            .Where(x => x.SellDate.HasValue && x.SellDate.Value.Date == today)
            .OrderByDescending(x => x.SellDate)
            .Select(x => new {
                x.Id,
                x.InvoiceId,
                x.SellCus,      // ✅ موجود
                x.CusName,
                x.Pay,
                x.TotalPrice,
                x.SellTprice,
                x.Stay,
                x.Safe,
                x.BarberName,
                x.NameCasher,
                AppStatus = x.AppStatus ?? "–",  // ✅ مش null
                SellDate = x.SellDate.HasValue
                    ? x.SellDate.Value.ToString("yyyy-MM-dd HH:mm") : null
            })
            .ToListAsync();
        return Ok(data);
    }
    // GET: api/appointment/summary/today  ← ملخص مالي لليوم
    [HttpGet("summary/today")]
    public async Task<IActionResult> SummaryToday()
    {
        var today = ShopTime.Today;
        var rows = await _db.Appointments
            .Where(x => x.SellDate.HasValue && x.SellDate.Value.Date == today)
            .Select(x => new { x.Pay, x.SellTprice, x.TotalEarnings })
            .ToListAsync();

        return Ok(new
        {
            TotalInvoices = rows.Count,
            TotalPaid = rows.Sum(x => x.Pay ?? 0),
            TotalUnpaid = rows.Where(x => x.Pay == 0).Sum(x => x.SellTprice ?? 0),
            TotalRevenue = rows.Sum(x => x.TotalEarnings ?? 0)
        });
    }
    // أضف الـ endpoints دي في AppointmentController

    // GET: api/appointment/range?from=2025-01-01&to=2025-01-31
    [HttpGet("range")]
    public async Task<IActionResult> GetByRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var data = await _db.Appointments
            .Where(x => x.SellDate.HasValue &&
                        x.SellDate.Value.Date >= from.Date &&
                        x.SellDate.Value.Date <= to.Date)
            .OrderByDescending(x => x.SellDate)
            .Select(x => new {
                x.Id,
                x.InvoiceId,
                x.SellCus,      // ✅ أضف
                x.CusName,
                x.Pay,
                x.TotalPrice,
                x.SellTprice,
                x.Stay,
                x.Safe,
                x.BarberName,
                x.NameCasher,
                AppStatus = x.AppStatus ?? "–",  // ✅ مش null
                SellDate = x.SellDate.HasValue
                    ? x.SellDate.Value.ToString("yyyy-MM-dd HH:mm") : null
            })
            .ToListAsync();
        return Ok(data);
    }

    // DELETE: api/appointment/cleanup  ← يمسح أكتر من 30 يوم
    [HttpDelete("cleanup")]
    public async Task<IActionResult> Cleanup()
    {
        var cutoff = ShopTime.Today.AddDays(-30);
        var old = await _db.Appointments
            .Where(x => x.SellDate.HasValue && x.SellDate.Value.Date < cutoff)
            .ToListAsync();
        _db.Appointments.RemoveRange(old);
        await _db.SaveChangesAsync();
        return Ok(new { success = true, deleted = old.Count });
    }
}

public class AppointmentDto
{
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