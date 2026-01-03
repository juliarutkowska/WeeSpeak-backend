using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using WeeSpeak.Data;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger (tylko w Development)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB (connection string z konfiguracji / env)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS: lokalnie + domeny z konfiguracji
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Forwarded headers (ważne na App Service za proxy)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Swagger tylko w Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirect: na App Service jest OK (będzie działać przez proxy)
app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();
app.MapControllers();

// 🔽🔽🔽 DODAJ TO TUTAJ 🔽🔽🔽
app.MapGet("/", () => "WeeSpeak API is running");
app.MapGet("/health", () => Results.Ok("OK"));

// Auto-migracje
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();



// using Microsoft.EntityFrameworkCore;
// using WeeSpeak.Data;
//
// const string CorsPolicy = "AllowFrontend";
//
// var builder = WebApplication.CreateBuilder(args);
//
// // Controllers (API)
// builder.Services.AddControllers();
//
// // DB
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
//
// // Swagger
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
//
// // CORS – zezwól na wywołania z frontu Vite (http://localhost:5173)
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(CorsPolicy, policy =>
//         policy.WithOrigins("http://localhost:5173")
//             .AllowAnyHeader()
//             .AllowAnyMethod());
// });
//
// var app = builder.Build();
//
// // Swagger tylko w dev
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
//
// // Wymuszamy HTTPS (dobrze i bezpiecznie)
// app.UseHttpsRedirection();
//
// // CORS przed MapControllers
// app.UseCors(CorsPolicy);
//
// app.UseAuthorization();
//
// app.MapControllers();
//
// app.Run();