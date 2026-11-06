using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlMaystro.API.Migrations
{
    public partial class AddAttendanceNotificationAppointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ===== Attendances =====
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Attendances' AND xtype='U')
                CREATE TABLE [Attendances] (
                    [AttendanceId] int NOT NULL IDENTITY,
                    [EmployeeId] int NULL,
                    [NameEmployee] nvarchar(max) NULL,
                    [AttendDate] datetime2 NULL,
                    [CheckIn] time NULL,
                    [CheckOut] time NULL,
                    [Status] nvarchar(max) NULL,
                    [WorkedHours] decimal(18,2) NULL,
                    [OvertimeHours] decimal(18,2) NULL,
                    CONSTRAINT [PK_Attendances] PRIMARY KEY ([AttendanceId])
                );
            ");

            // ===== Notfications =====
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Notfications' AND xtype='U')
                CREATE TABLE [Notfications] (
                    [id] int NOT NULL IDENTITY,
                    [notfication] nvarchar(max) NULL,
                    [CreatedAt] datetime2 NULL,
                    [IsRead] bit NOT NULL DEFAULT 0,
                    CONSTRAINT [PK_Notfications] PRIMARY KEY ([id])
                );
            ");

            // ===== tb_appointment =====
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tb_appointment' AND xtype='U')
                CREATE TABLE [tb_appointment] (
                    [id] int NOT NULL IDENTITY,
                    [sell_cus] nvarchar(max) NULL,
                    [sell_tprice] float NULL,
                    [sell_date] datetime2 NULL,
                    [pay] float NULL,
                    [stay] float NULL,
                    [total_price] float NULL,
                    [sell_add] float NULL,
                    [cut] float NULL,
                    [invocie_id] int NULL,
                    [total_earnings] float NULL,
                    [safe] nvarchar(max) NULL,
                    [namecasher] nvarchar(max) NULL,
                    [barber_name] nvarchar(max) NULL,
                    [app_status] nvarchar(max) NULL,
                    [cus_name] nvarchar(max) NULL,
                    CONSTRAINT [PK_tb_appointment] PRIMARY KEY ([id])
                );
            ");

            // ===== Bookings - لو مش موجودة =====
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Bookings' AND xtype='U')
                CREATE TABLE [Bookings] (
                    [id] int NOT NULL IDENTITY,
                    [cus_name] nvarchar(100) NULL,
                    [cus_phone] nvarchar(20) NULL,
                    [booking_date] datetime2 NOT NULL DEFAULT '2000-01-01',
                    [created_at] datetime2 NOT NULL DEFAULT GETDATE(),
                    [IsRead] bit NOT NULL DEFAULT 0,
                    CONSTRAINT [PK_Bookings] PRIMARY KEY ([id])
                );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS [Attendances]");
            migrationBuilder.Sql("DROP TABLE IF EXISTS [Notfications]");
            migrationBuilder.Sql("DROP TABLE IF EXISTS [tb_appointment]");
        }
    }
}