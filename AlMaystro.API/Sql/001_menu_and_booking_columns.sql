-- Run once on the existing database. Safe to re-run. Does NOT touch Attendances / Notfications / tb_appointment.
IF OBJECT_ID('dbo.Categories') IS NULL
    CREATE TABLE dbo.Categories (
        Id        INT IDENTITY(1,1) PRIMARY KEY,
        Name      NVARCHAR(100) NOT NULL,
        SortOrder INT NOT NULL CONSTRAINT DF_Categories_SortOrder DEFAULT 0
    );

IF OBJECT_ID('dbo.Services') IS NULL
    CREATE TABLE dbo.Services (
        Id         INT IDENTITY(1,1) PRIMARY KEY,
        Name       NVARCHAR(150) NOT NULL,
        PriceFrom  DECIMAL(10,2) NOT NULL,
        PriceTo    DECIMAL(10,2) NULL,
        CategoryId INT NOT NULL
            CONSTRAINT FK_Services_Categories REFERENCES dbo.Categories(Id) ON DELETE CASCADE
    );

IF COL_LENGTH('dbo.Bookings', 'service_id') IS NULL
    ALTER TABLE dbo.Bookings ADD service_id INT NULL
        CONSTRAINT FK_Bookings_Services REFERENCES dbo.Services(Id) ON DELETE SET NULL;

IF COL_LENGTH('dbo.Bookings', 'status') IS NULL
    ALTER TABLE dbo.Bookings ADD status INT NOT NULL CONSTRAINT DF_Bookings_status DEFAULT 0;   -- 0 = Pending

IF COL_LENGTH('dbo.Bookings', 'source') IS NULL
    ALTER TABLE dbo.Bookings ADD source INT NOT NULL CONSTRAINT DF_Bookings_source DEFAULT 0;   -- 0 = Web

IF COL_LENGTH('dbo.Bookings', 'notes') IS NULL
    ALTER TABLE dbo.Bookings ADD notes NVARCHAR(500) NULL;

-- Optional: mark OLD bookings so they don't all show as "Pending" in the mobile app
-- UPDATE dbo.Bookings SET status = 3 WHERE booking_date < CAST(GETDATE() AS date);   -- 3 = Completed
