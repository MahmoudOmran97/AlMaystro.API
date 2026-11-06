using AlMaystro.API.Data;
using AlMaystro.API.Models;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FcmNotification = FirebaseAdmin.Messaging.Notification;
using DbNotification = AlMaystro.API.Models.Notification;
namespace AlMaystro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly AppDbContext _db;
    public NotificationController(AppDbContext db) => _db = db;

    // POST: api/notification  ← الديسكتوب يبعت هنا
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NotificationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Message))
            return BadRequest(new { success = false, message = "النص فارغ" });

        // ✅ تحقق مش موجود نفس الرسالة في آخر دقيقة عشان تمنع التكرار
        var oneMinuteAgo = DateTime.Now.AddMinutes(-1);
        bool alreadyExists = await _db.Notifications
            .AnyAsync(x => x.Notfication1 == dto.Message &&
                           x.CreatedAt >= oneMinuteAgo);

        if (alreadyExists)
            return Ok(new { success = true, skipped = true });

        _db.Notifications.Add(new DbNotification          // ✅ كان: new Notification
        {
            Notfication1 = dto.Message,
            CreatedAt = DateTime.Now,
            IsRead = false
        });

        await _db.SaveChangesAsync();
        await SendFcmAsync(dto.Message ?? "إشعار جديد");
        return Ok(new { success = true });
    }
    // في NotificationController.cs — احذف الكود القديم بالكامل
    private async Task SendFcmAsync(string message)
    {
        var fcmMessage = new Message
        {
            Topic = "maystro_all",
            Notification = new FcmNotification
            {
                Title = "المايسترو ✂️",
                Body = message,
            },
            Android = new AndroidConfig
            {
                Priority = Priority.High,
                Notification = new AndroidNotification
                {
                    ChannelId = "maystro_channel",
                    Sound = "default"
                }
            }
        };

        string response = await FirebaseMessaging.DefaultInstance
            .SendAsync(fcmMessage);

        System.Diagnostics.Debug.WriteLine($"✅ FCM sent: {response}");
    }
    // GET: api/notification/unread  ← الماوي يجيب منه
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnread()
    {
        var items = await _db.Notifications
            .Where(x => !x.IsRead)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        foreach (var n in items)
            n.IsRead = true;

        await _db.SaveChangesAsync();
        return Ok(items);
    }

    // GET: api/notification/all
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Notifications
            .OrderByDescending(x => x.CreatedAt)
            .Take(50)
            .ToListAsync();

        return Ok(items);
    }
}

public class NotificationDto
{
    public string? Message { get; set; }
}