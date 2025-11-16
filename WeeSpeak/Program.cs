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

// CORS – zezwól na wywołania z frontu Vite (http://localhost:5173)
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

// Swagger tylko w dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Wymuszamy HTTPS (dobrze i bezpiecznie)
app.UseHttpsRedirection();

// CORS przed MapControllers
app.UseCors(CorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();