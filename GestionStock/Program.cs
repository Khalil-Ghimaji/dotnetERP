using GestionStock.DTO.Mapping;
using GestionStock.Services;
using Persistence;
using Persistence.entities.Stock;
using Persistence.Repository;
using Persistence.Repository.StockRepositories.Contracts;
using Persistence.Repository.StockRepositories.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IArticleStockRepo, ArticleStockRepo>();
builder.Services.AddScoped<IProduitRepo, ProduitRepo>();
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IStockService, StockService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<APIKeyMiddleware>();
app.Run();