using AppGateway.Models;
using AppGateway.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Persistence;
using Persistence.Entities;
using IEmailSender = Microsoft.AspNetCore.Identity.UI.Services.IEmailSender;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<JwtHandler>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<User>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
        options.SignIn.RequireConfirmedPhoneNumber = false;
        options.SignIn.RequireConfirmedAccount = true;
        options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;
        options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 0;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(1);
});
// Configure HttpClient to add the API key header to all requests
builder.Services.AddHttpClient("GestionStockClient",
    client => { client.DefaultRequestHeaders.Add("X-API-KEY", "Aziz-GestionStock-2025"); });

builder.Services.AddHttpClient("GestionCommandesClient",
    client => { client.DefaultRequestHeaders.Add("X-API-KEY", "Khalil-GestionCommande-2025"); });

builder.Services.AddHttpClient("GestionClientsClient",
    client => { client.DefaultRequestHeaders.Add("X-API-KEY", "Oussama-GestionClients-2025"); });

builder.Services.AddHttpClient("GestionFacturesClient",
    client => { client.DefaultRequestHeaders.Add("X-API-KEY", "Rayen-Facturation-2025"); });

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
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new List<Roles>
        { Roles.ADMIN, Roles.GESTIONNAIRE_STOCK, Roles.GESTIONNAIRE_CLIENTS, Roles.GESTIONNAIRE_COMMANDES, Roles.COMPTABLE };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role.ToString()))
        {
            await roleManager.CreateAsync(new IdentityRole(role.ToString()));
        }
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var user = await userManager.FindByEmailAsync("admin@gmail.com");
    if (user == null)
    {
        user = new User { Email = "admin@gmail.com", UserName = "admin@gmail.com", EmailConfirmed = true };
        await userManager.CreateAsync(user, "Admin@123");
        await userManager.AddToRoleAsync(user, Roles.ADMIN.ToString());
    }
}

/*app.MapIdentityApi<User>();*/
app.Run();