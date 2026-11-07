using AlMaystro.API.Data;
using AlMaystro.API.Dtos;
using AlMaystro.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlMaystro.API.Controllers;

[ApiController, Route("api/services"), Authorize(Roles = "Owner")]
public class ServicesController(AppDbContext db) : ControllerBase
{
    static string? Validate(ServiceSaveDto d)
    {
        if (string.IsNullOrWhiteSpace(d.Name)) return "الاسم مطلوب";
        if (d.PriceFrom < 0) return "السعر غير صحيح";
        if (d.PriceTo is not null && d.PriceTo < d.PriceFrom) return "سعر (إلى) لازم يكون أكبر من (من)";
        return null;
    }

    [HttpPost]
    public async Task<ActionResult<ServiceDto>> Create(ServiceSaveDto dto)
    {
        if (Validate(dto) is { } err) return BadRequest(err);
        if (await db.Categories.FindAsync(dto.CategoryId) is null) return BadRequest("القسم غير موجود");
        var s = new Service { Name = dto.Name.Trim(), PriceFrom = dto.PriceFrom, PriceTo = dto.PriceTo, CategoryId = dto.CategoryId };
        db.Services.Add(s);
        await db.SaveChangesAsync();
        return new ServiceDto(s.Id, s.Name, s.PriceFrom, s.PriceTo, s.CategoryId);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ServiceSaveDto dto)
    {
        if (Validate(dto) is { } err) return BadRequest(err);
        var s = await db.Services.FindAsync(id);
        if (s is null) return NotFound();
        s.Name = dto.Name.Trim(); s.PriceFrom = dto.PriceFrom; s.PriceTo = dto.PriceTo; s.CategoryId = dto.CategoryId;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var s = await db.Services.FindAsync(id);
        if (s is null) return NotFound();
        db.Services.Remove(s);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
