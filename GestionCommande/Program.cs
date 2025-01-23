using GestionCommande.DTOs.Mapper;
using GestionCommande.Services;
using Persistence;
using Persistence.Repository.ClientRepositories;
using Persistence.Repository.CommandeRepositories;
using Persistence.Repository.StockRepositories.Contracts;
using Persistence.Repository.StockRepositories.Implementations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<ICommandeRepo,CommandeRepo>();
builder.Services.AddScoped<IProduitRepo,ProduitRepo>();
builder.Services.AddScoped<IArticleStockRepo,ArticleStockRepo>();
builder.Services.AddScoped<IClientRepo,ClientRepo>();
builder.Services.AddScoped<ICommandeService, CommandeService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
// Add services to the container.

builder.Services.AddControllers();
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

app.Run();