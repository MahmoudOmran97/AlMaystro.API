using AlMaystro.API.Models;
namespace AlMaystro.API.Dtos;

public record LoginDto(string Username, string Password);
public record LoginResult(string Token, DateTime ExpiresAt);

public record ServiceDto(int Id, string Name, decimal PriceFrom, decimal? PriceTo, int CategoryId);
public record ServiceSaveDto(string Name, decimal PriceFrom, decimal? PriceTo, int CategoryId);

public record CategoryDto(int Id, string Name, int SortOrder, List<ServiceDto> Services);
public record CategorySaveDto(string Name, int SortOrder);

public record BookingDto(int Id, string CustomerName, string Phone, int? ServiceId, string? ServiceName,
    DateTime Date, BookingStatus Status, BookingSource Source, string? Notes);
public record BookingCreateDto(string CustomerName, string Phone, int? ServiceId, DateTime Date,
    BookingSource Source, string? Notes);
public record BookingStatusDto(BookingStatus Status);
