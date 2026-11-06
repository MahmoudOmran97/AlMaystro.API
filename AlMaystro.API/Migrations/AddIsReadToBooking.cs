using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlMaystro.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIsReadToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Bookings' AND xtype='U')
                BEGIN
                    CREATE TABLE [Bookings] (
                        [id] int NOT NULL IDENTITY,
                        [cus_name] nvarchar(100) NULL,
                        [cus_phone] nvarchar(20) NULL,
                        [booking_date] datetime2 NOT NULL DEFAULT '2000-01-01',
                        [created_at] datetime2 NOT NULL DEFAULT GETDATE(),
                        [IsRead] bit NOT NULL DEFAULT 0,
                        CONSTRAINT [PK_Bookings] PRIMARY KEY ([id])
                    )
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS [Bookings]");
        }
    }
}