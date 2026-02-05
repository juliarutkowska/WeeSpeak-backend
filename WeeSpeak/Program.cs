using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using WeeSpeak.Data;

const string CorsPolicy = "AllowFrontend";

var builder = WebApplication.CreateBuilder(args);

// Controllers (API)
builder.Services.AddControllers();

// DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS – na start najprościej (żeby działało z frontem na innym adresie)
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Forwarded headers (ważne na Azure App Service)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// ✅ Swagger WŁĄCZONY TAKŻE NA PRODUCTION
app.UseSwagger();
app.UseSwaggerUI();

// HTTPS redirect
app.UseHttpsRedirection();

// CORS przed MapControllers
app.UseCors(CorsPolicy);

app.UseAuthorization();
app.MapControllers();

// (Opcjonalnie, ale polecam) migracje przy starcie
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();