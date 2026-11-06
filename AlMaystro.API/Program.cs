using AlMaystro.API.Data;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Connection String ثابت
var connectionString = "Server=db49554.public.databaseasp.net;" +
                       "Database=db49554;" +
                       "User Id=db49554;" +
                       "Password=0103371240;" +
                       "TrustServerCertificate=True;" +
                       "Encrypt=False;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
// في Program.cs أو أول ما يشتغل الـ API — مرة واحدة بس
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(
        Path.Combine(AppContext.BaseDirectory, "almaystroapp-firebase-adminsdk-fbsvc-7688c29b24.json"))
});
var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

app.Run();