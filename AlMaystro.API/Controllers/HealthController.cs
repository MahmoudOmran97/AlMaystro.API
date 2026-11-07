using AlMaystro.API.Data;
using AlMaystro.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AlMaystro.API.Controllers;

/// <summary>
/// GET /api/health  (من المتصفح) => لكل جدول: "ok" أو كود الخطأ.
/// أمثلة: SqlException #18456 = باسورد/يوزر غلط، #53 أو #40 = السيرفر مش متوصل،
/// #208 = جدول مش موجود، #207 = عمود مش موجود.
/// الرد مفيهوش أسماء سيرفرات ولا باسوردات، والتفاصيل الكاملة في اللوج.
/// </summary>
[ApiController, Route("api/health")]
public class HealthController(AppDbContext db, ILogger<HealthController> log) : ControllerBase
{
    static string Describe(Exception ex)
    {
        var e = ex;
        while (e is not SqlException && e.InnerException != null) e = e.InnerException;
        return e is SqlException s ? $"SqlException #{s.Number}" : e.GetType().Name;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var checks = new Dictionary<string, string>();

        async Task Check(string name, Func<Task> probe)
        {
            try { await probe(); checks[name] = "ok"; }
            catch (Exception ex)
            {
                checks[name] = Describe(ex);
                log.LogError(ex, "Health check failed: {Name}", name);
            }
        }

        await Check("connection", async () =>
        {
            await db.Database.OpenConnectionAsync();
            await db.Database.CloseConnectionAsync();
        });
        await Check("appointments",  () => db.Appointments.AsNoTracking().Take(1).ToListAsync());
        await Check("attendances",   () => db.Attendances.AsNoTracking().Take(1).ToListAsync());
        await Check("notifications", () => db.Notifications.AsNoTracking().Take(1).ToListAsync());
        await Check("bookings",      () => db.Bookings.AsNoTracking().Include(b => b.Service).Take(1).ToListAsync());
        await Check("categories",    () => db.Categories.AsNoTracking().Take(1).ToListAsync());
        await Check("services",      () => db.Services.AsNoTracking().Take(1).ToListAsync());

        var ok = checks.Values.All(v => v == "ok");
        return StatusCode(ok ? 200 : 503, new
        {
            ok,
            shopTime = ShopTime.Now.ToString("yyyy-MM-dd HH:mm"),
            serverUtc = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"),
            checks
        });
    }
}
