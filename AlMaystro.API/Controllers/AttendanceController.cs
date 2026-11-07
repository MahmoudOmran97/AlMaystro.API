using AlMaystro.API.Helpers;
using AlMaystro.API.Data;
using AlMaystro.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AlMaystro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly AppDbContext _db;
    public AttendanceController(AppDbContext db) => _db = db;

    // POST: api/attendance
    [HttpPost]
    public async Task<IActionResult> Sync([FromBody] List<AttendanceDto> records)
    {
        if (records == null || !records.Any())
            return BadRequest(new { success = false, message = "لا توجد بيانات" });

        int added = 0, updated = 0;

        foreach (var rec in records)
        {
            var existing = await _db.Attendances.FirstOrDefaultAsync(x =>
                x.EmployeeId == rec.EmployeeId &&
                x.AttendDate.HasValue &&
                x.AttendDate.Value.Date == rec.AttendDate.Date);

            if (existing == null)
            {
                _db.Attendances.Add(new Attendance
                {
                    EmployeeId = rec.EmployeeId,
                    NameEmployee = rec.NameEmployee,
                    AttendDate = rec.AttendDate,
                    CheckIn = rec.CheckIn,
                    CheckOut = rec.CheckOut,
                    Status = rec.Status,
                    WorkedHours = rec.WorkedHours,
                    OvertimeHours = rec.OvertimeHours
                });
                added++;
            }
            else
            {
                if (rec.CheckOut.HasValue)
                {
                    existing.CheckOut = rec.CheckOut;
                    existing.WorkedHours = rec.WorkedHours;
                    existing.OvertimeHours = rec.OvertimeHours;
                    existing.Status = rec.Status;
                    updated++;
                }
            }
        }

        await _db.SaveChangesAsync();
        return Ok(new { success = true, added, updated });
    }

    // GET: api/attendance/today
    [HttpGet("today")]
    public async Task<IActionResult> GetToday()
    {
        var today = ShopTime.Today;
        var data = await _db.Attendances
            .Where(x => x.AttendDate.HasValue && x.AttendDate.Value.Date == today)
            .OrderBy(x => x.CheckIn)
            .Select(x => new {
                x.AttendanceId,
                x.EmployeeId,
                x.NameEmployee,
                AttendDate = x.AttendDate.HasValue ? x.AttendDate.Value.ToString("yyyy-MM-dd") : null,
                CheckIn = x.CheckIn.HasValue ? x.CheckIn.Value.ToString(@"hh\:mm") : null,
                CheckOut = x.CheckOut.HasValue ? x.CheckOut.Value.ToString(@"hh\:mm") : null,
                x.Status,
                x.WorkedHours,
                x.OvertimeHours
            })
            .ToListAsync();

        return Ok(data);
    }

    // GET: api/attendance/range?from=2025-01-01&to=2025-01-31
    [HttpGet("range")]
    public async Task<IActionResult> GetByRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var data = await _db.Attendances
            .Where(x => x.AttendDate.HasValue &&
                        x.AttendDate.Value.Date >= from.Date &&
                        x.AttendDate.Value.Date <= to.Date)
            .OrderByDescending(x => x.AttendDate)
            .Select(x => new {
                x.AttendanceId,
                x.EmployeeId,
                x.NameEmployee,
                AttendDate = x.AttendDate.HasValue ? x.AttendDate.Value.ToString("yyyy-MM-dd") : null,
                CheckIn = x.CheckIn.HasValue ? x.CheckIn.Value.ToString(@"hh\:mm") : null,
                CheckOut = x.CheckOut.HasValue ? x.CheckOut.Value.ToString(@"hh\:mm") : null,
                x.Status,
                x.WorkedHours,
                x.OvertimeHours
            })
            .ToListAsync();

        return Ok(data);
    }
    // POST: api/attendance/sync-delete
    [HttpPost("sync-delete")]
    public async Task<IActionResult> SyncDelete([FromBody] List<int> localEmployeeIds)
    {
        if (localEmployeeIds == null) return BadRequest();

        // ✅ كل الحضور على السيرفر — مش بس اليوم
        var allServerRecords = await _db.Attendances.ToListAsync();

        var toDelete = allServerRecords
            .Where(x => x.EmployeeId.HasValue &&
                         !localEmployeeIds.Contains(x.EmployeeId.Value))
            .ToList();

        if (toDelete.Any())
        {
            _db.Attendances.RemoveRange(toDelete);
            await _db.SaveChangesAsync();
        }

        return Ok(new { success = true, deleted = toDelete.Count });
    }
    // DELETE: api/attendance/cleanup  ← ✅ الجديد
    [HttpDelete("cleanup")]
    public async Task<IActionResult> Cleanup()
    {
        var cutoff = ShopTime.Today.AddDays(-30);
        var old = await _db.Attendances
            .Where(x => x.AttendDate.HasValue && x.AttendDate.Value.Date < cutoff)
            .ToListAsync();

        _db.Attendances.RemoveRange(old);
        await _db.SaveChangesAsync();

        return Ok(new { success = true, deleted = old.Count });
    }
}

// DTO
public class AttendanceDto
{
    public int EmployeeId { get; set; }
    public string? NameEmployee { get; set; }
    public DateTime AttendDate { get; set; }
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public string? Status { get; set; }
    public decimal WorkedHours { get; set; }
    public decimal OvertimeHours { get; set; }
}