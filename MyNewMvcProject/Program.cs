using Microsoft.EntityFrameworkCore;
using MyLibrary.DbModel.Context;
using MyLibrary.JsonRepositoryModel.Context;
using MyLibrary.JsonRepositoryModel.Repositories;
using MyLibrary.JsonRepositoryModel.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("MySqlDbContext");
builder.Services.AddDbContext<MySqlDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register JsonDbContext
builder.Services.AddScoped<JsonDbContext>(sp =>
{
    var dataFolderPath = builder.Configuration["DataFolderPath"];
    return new JsonDbContext(dataFolderPath);
});

// Register Repositories for JsonDbContext
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookAuthorRepository, BookAuthorRepository>();
builder.Services.AddScoped<IBookCopyRepository, BookCopyRepository>();
builder.Services.AddScoped<IBookThemeRepository, BookThemeRepository>();
builder.Services.AddScoped<IBorrowRepository, BorrowRepository>();
builder.Services.AddScoped<IThemeRepository, ThemeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Initial}/{action=Index}/{id?}");

app.Run();
