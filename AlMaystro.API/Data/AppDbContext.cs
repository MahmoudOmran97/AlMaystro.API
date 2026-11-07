using AlMaystro.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AlMaystro.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // ----- existing (unchanged) -----
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    // ----- new -----
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Service> Services => Set<Service>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Bookings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.CusName).HasColumnName("cus_name").HasMaxLength(100);
            entity.Property(e => e.CusPhone).HasColumnName("cus_phone").HasMaxLength(20);
            entity.Property(e => e.BookingDate).HasColumnName("booking_date");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            // new columns (added by Sql/001_menu_and_booking_columns.sql)
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<int>();
            entity.Property(e => e.Source).HasColumnName("source").HasConversion<int>();
            entity.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(500);
            entity.HasOne(e => e.Service).WithMany().HasForeignKey(e => e.ServiceId)
                  .OnDelete(DeleteBehavior.SetNull);   // deleting a service keeps old bookings
            entity.HasIndex(e => e.BookingDate);
        });

        modelBuilder.Entity<Attendance>(e => {
            e.ToTable("Attendances");
            e.Property(x => x.AttendanceId).HasColumnName("AttendanceId");
            e.Property(x => x.EmployeeId).HasColumnName("EmployeeId");
            e.Property(x => x.NameEmployee).HasColumnName("NameEmployee");
            e.Property(x => x.AttendDate).HasColumnName("AttendDate");
            e.Property(x => x.CheckIn).HasColumnName("CheckIn");
            e.Property(x => x.CheckOut).HasColumnName("CheckOut");
            e.Property(x => x.Status).HasColumnName("Status");
            e.Property(x => x.WorkedHours).HasColumnName("WorkedHours");
            e.Property(x => x.OvertimeHours).HasColumnName("OvertimeHours");
        });

        // Notifications
        modelBuilder.Entity<Notification>(e => {
            e.ToTable("Notfications");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Notfication1).HasColumnName("notfication");
            e.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
        });

        // Appointments (invoices)
        modelBuilder.Entity<Appointment>(e => {
            e.ToTable("tb_appointment");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.SellCus).HasColumnName("sell_cus");
            e.Property(x => x.SellTprice).HasColumnName("sell_tprice");
            e.Property(x => x.SellDate).HasColumnName("sell_date");
            e.Property(x => x.Pay).HasColumnName("pay");
            e.Property(x => x.Stay).HasColumnName("stay");
            e.Property(x => x.TotalPrice).HasColumnName("total_price");
            e.Property(x => x.SellAdd).HasColumnName("sell_add");
            e.Property(x => x.Cut).HasColumnName("cut");
            e.Property(x => x.InvoiceId).HasColumnName("invocie_id");
            e.Property(x => x.TotalEarnings).HasColumnName("total_earnings");
            e.Property(x => x.Safe).HasColumnName("safe");
            e.Property(x => x.NameCasher).HasColumnName("namecasher");
            e.Property(x => x.BarberName).HasColumnName("barber_name");
            e.Property(x => x.AppStatus).HasColumnName("app_status");
            e.Property(x => x.CusName).HasColumnName("cus_name");
        });

        // ----- new menu tables -----
        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("Categories");
            e.Property(x => x.Name).HasMaxLength(100);
        });
        modelBuilder.Entity<Service>(e =>
        {
            e.ToTable("Services");
            e.Property(x => x.Name).HasMaxLength(150);
            e.Property(x => x.PriceFrom).HasPrecision(10, 2);
            e.Property(x => x.PriceTo).HasPrecision(10, 2);
            e.HasOne(x => x.Category).WithMany(c => c.Services)
             .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
