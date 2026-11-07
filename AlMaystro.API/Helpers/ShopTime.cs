namespace AlMaystro.API.Helpers;

/// <summary>
/// وقت المحل (القاهرة) بدل وقت السيرفر. الاستضافة غالباً على UTC،
/// فـ DateTime.Today كان بيرجع "تاريخ تاني" بعد منتصف الليل بتوقيت مصر
/// وإندبوينتس "today" ترجع فاضية.
/// </summary>
public static class ShopTime
{
    static readonly TimeZoneInfo Cairo = Find();

    static TimeZoneInfo Find()
    {
        foreach (var id in new[] { "Egypt Standard Time", "Africa/Cairo" })
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); } catch { }
        return TimeZoneInfo.Local;
    }

    public static DateTime Now => TimeZoneInfo.ConvertTime(DateTime.UtcNow, Cairo);
    public static DateTime Today => Now.Date;
}
