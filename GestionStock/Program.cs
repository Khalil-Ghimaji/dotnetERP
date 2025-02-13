using GestionStock.DTO.Mapping;
using GestionStock.Services;
using Persistence;
using Persistence.entities.Stock;
using Persistence.Repository;
using Persistence.Repository.CommandeRepositories;
using Persistence.Repository.StockRepositories.Contracts;
using Persistence.Repository.StockRepositories.Implementations;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IArticleStockRepo, ArticleStockRepo>();
builder.Services.AddScoped<ICommandeRepo, CommandeRepo>();
builder.Services.AddScoped<IProduitRepo, ProduitRepo>();
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IStockService, StockService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


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