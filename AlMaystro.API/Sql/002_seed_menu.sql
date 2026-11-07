-- يملأ جداول Categories / Services بالمنيو الحالي بتاع الموقع. شغّله مرة واحدة بس.
-- (لو الجداول فيها بيانات فعلاً مش هيعمل حاجة)
-- لو الإيموجي ظهرت ????? عدّل أسماء الأقسام من التطبيق.
IF OBJECT_ID('dbo.Categories') IS NULL OR OBJECT_ID('dbo.Services') IS NULL
    RAISERROR(N'شغّل 001_menu_and_booking_columns.sql الأول', 16, 1);
ELSE IF NOT EXISTS (SELECT 1 FROM dbo.Categories)
BEGIN
    DECLARE @c INT;
    INSERT INTO dbo.Categories (Name, SortOrder) VALUES (N'💇‍♂️ قسم الشعر', 1); SET @c = SCOPE_IDENTITY();
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'قص شعر عادي', 100, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'تدريج شعر عادي', 100, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'تدريج شعر زيرو او تنعيم', 150, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'قص شعر كلاسيك بالمقص', 150, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'صبغة شعر اسود ايطالي', 150, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'صبغة شعر حنة', 80, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'صبغة شعر ألوان', 300, 700, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'حلاقة شعر عادي اطفال', 50, 100, @c);
    INSERT INTO dbo.Categories (Name, SortOrder) VALUES (N'🧔 قسم الذقن', 2); SET @c = SCOPE_IDENTITY();
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'ذقن عادي موس او تنعيم', 100, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'ذقن عادي بالمكنة', 50, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'ذقن تحديد + تدريج', 100, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'تحديد ذقن بالفتلة', 100, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'فتلة عادي', 50, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'صبغة ذقن ايطالي', 50, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'صبغة ذقن حنة', 50, NULL, @c);
    INSERT INTO dbo.Categories (Name, SortOrder) VALUES (N'🔌 قسم الأجهزة', 3); SET @c = SCOPE_IDENTITY();
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'فرشة صنفرة', 150, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'باندويل + تنظيف وش كامل', 200, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'ليزر تنظيف وش كامل', 200, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'شمع كامل', 150, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'شفط دهون او ابرة تنظيف', 150, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'مساج كامل', 300, NULL, @c);
    INSERT INTO dbo.Categories (Name, SortOrder) VALUES (N'🧴 قسم الكريمات', 4); SET @c = SCOPE_IDENTITY();
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'حمام كريم', 50, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'فرد شعر عادي', 50, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'فرد بروتين ( 1 جرام )', 25, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'تسريح عادي جل او كريم', 50, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'تسريح ويفي', 100, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'تسريح سفنجية كيرلى', 70, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'لفة سسته يدوى', 300, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'لفة سسته مكوه', 300, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'استشوار عادي', 80, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'استشوار + تخصيل مشط', 200, NULL, @c);
    INSERT INTO dbo.Categories (Name, SortOrder) VALUES (N'🎭 قسم الماسكات', 5); SET @c = SCOPE_IDENTITY();
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'ماسك عادي وش', 50, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'ماسك صنفرة وش', 100, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'ماسك رقبة', 50, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'ماسك صنفرة رقبة', 100, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'ماسك مستورد وش', 70, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'ماسك مستورد صنفرة وش', 120, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'ماسك مستورد رقبة', 70, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'ماسك مستورد صنفرة رقبة', 120, NULL, @c);
    INSERT INTO dbo.Categories (Name, SortOrder) VALUES (N'🤵 باكيدج العريس', 6); SET @c = SCOPE_IDENTITY();
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'باكيدج عادي', 1500, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'باكيدج مميز', 2500, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'باكيدج VIP', 5000, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'✨ يوجد مساج كابينة سونة ✨', 0, NULL, @c);  -- سعر 0 = ملاحظة
    INSERT INTO dbo.Categories (Name, SortOrder) VALUES (N'🪒 قسم الاستعمال الشخصي', 7); SET @c = SCOPE_IDENTITY();
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'شفرة خاصة', 20, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'فوطة تنشيف خاصة', 20, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'فوطة قص خاصة', 20, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'فوطة سخنة', 50, NULL, @c);
    INSERT INTO dbo.Services (Name, PriceFrom, PriceTo, CategoryId) VALUES (N'فوطة نعناع ساقعة', 50, NULL, @c);
END
