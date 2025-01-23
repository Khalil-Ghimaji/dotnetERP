using Facturation.Services;
using Persistence;
using Persistence.Repository.FacturationRepositories;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IFactureRepo,FactureRepo>();
builder.Services.AddScoped<IPaiementRepo,PaiementRepo>();
builder.Services.AddScoped<IFactureService,FactureService>();
builder.Services.AddScoped<IPDFService,PDFService>();
builder.Services.AddScoped<IMailService,MailService>();


// Add AutoMapper to the DI container
builder.Services.AddAutoMapper(typeof(Facturation.Mapping.FacturationMappingProfile));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
QuestPDF.Settings.License = LicenseType.Community;

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