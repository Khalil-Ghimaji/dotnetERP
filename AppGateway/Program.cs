using Microsoft.Build.Execution;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();

// Configure HttpClient to add the API key header to all requests
builder.Services.AddHttpClient("GestionStockClient", client =>
{
    client.DefaultRequestHeaders.Add("X-API-KEY", "Aziz-GestionStock-2025");
});

builder.Services.AddHttpClient("GestionCommandesClient", client =>
{
    client.DefaultRequestHeaders.Add("X-API-KEY", "Khalil-GestionCommande-2025");
});

builder.Services.AddHttpClient("GestionClientsClient", client =>
{
    client.DefaultRequestHeaders.Add("X-API-KEY", "Oussama-GestionClients-2025");
});

builder.Services.AddHttpClient("GestionFacturesClient", client =>
{
    client.DefaultRequestHeaders.Add("X-API-KEY", "Rayen-Facturation-2025");
});

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