using AlMaystro.API.Data;
using AlMaystro.API.Dtos;
using AlMaystro.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlMaystro.API.Controllers;

[ApiController, Route("api/categories")]
public class CategoriesController(AppDbContext db) : ControllerBase
{
    [HttpGet] // public: website + mobile read the menu from here
    public async Task<List<CategoryDto>> GetAll() =>
        await db.Categories.AsNoTracking().OrderBy(c => c.SortOrder).Select(c =>
            new CategoryDto(c.Id, c.Name, c.SortOrder,
                c.Services.OrderBy(s => s.Name)
                    .Select(s => new ServiceDto(s.Id, s.Name, s.PriceFrom, s.PriceTo, s.CategoryId)).ToList()))
            .ToListAsync();

    [Authorize(Roles = "Owner"), HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CategorySaveDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("الاسم مطلوب");
        var c = new Category { Name = dto.Name.Trim(), SortOrder = dto.SortOrder };
        db.Categories.Add(c);
        await db.SaveChangesAsync();
        return new CategoryDto(c.Id, c.Name, c.SortOrder, new());
    }

    [Authorize(Roles = "Owner"), HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategorySaveDto dto)
    {
        var c = await db.Categories.FindAsync(id);
        if (c is null) return NotFound();
        c.Name = dto.Name.Trim(); c.SortOrder = dto.SortOrder;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "Owner"), HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await db.Categories.FindAsync(id);
        if (c is null) return NotFound();
        db.Categories.Remove(c); // cascades to its services
        await db.SaveChangesAsync();
        return NoContent();
    }
}
