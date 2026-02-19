using Microsoft.EntityFrameworkCore;
using shopping.Data;
using shopping.Repository;
using shopping.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== DBCONTEXT =====
builder.Services.AddDbContext<ToDoContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ===== REPOSITORY =====
builder.Services.AddScoped<IToDoRepository, ToDoRepository>();

// ===== SERVICE =====
builder.Services.AddScoped<IToDoService, ToDoService>();

// ===== CONTROLLERS =====
builder.Services.AddControllers();

// ===== SWAGGER =====
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===== MIDDLEWARE =====
if (app.Environment.IsDevelopment())
{
//    app.UseSwagger();
//    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();