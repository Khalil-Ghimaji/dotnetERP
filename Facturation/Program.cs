using Facturation.Repository;
using Facturation.Services;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IFactureRepo,FactureRepo>();
builder.Services.AddScoped<IPaiementRepo,PaiementRepo>();
builder.Services.AddScoped<IFactureService,FactureService>();

// Add AutoMapper to the DI container
builder.Services.AddAutoMapper(typeof(Facturation.Mapping.FacturationMappingProfile));


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