using AppGateway.Models;
using AppGateway.Services;
using Microsoft.AspNetCore.Identity;
using Persistence;
using Persistence.Entities;
using Persistence.entities.Client;
using IEmailSender = Microsoft.AspNetCore.Identity.UI.Services.IEmailSender;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddScoped<JwtHandler>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();

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

builder.Services.AddHttpClient("GestionStockClient",
    client => { client.DefaultRequestHeaders.Add("X-API-KEY", "Aziz-GestionStock-2025"); });

builder.Services.AddHttpClient("GestionCommandesClient",
    client => { client.DefaultRequestHeaders.Add("X-API-KEY", "Khalil-GestionCommande-2025"); });

builder.Services.AddHttpClient("GestionClientsClient",
    client => { client.DefaultRequestHeaders.Add("X-API-KEY", "Oussama-GestionClients-2025"); });

builder.Services.AddHttpClient("GestionFacturesClient",
    client => { client.DefaultRequestHeaders.Add("X-API-KEY", "Rayen-Facturation-2025"); });

var app = builder.Build();


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
    {
        Roles.ADMIN, Roles.GESTIONNAIRE_STOCK, Roles.GESTIONNAIRE_CLIENTS, Roles.GESTIONNAIRE_COMMANDES, Roles.COMPTABLE
    };
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


    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var clientKhalil = context.Clients.FirstOrDefault(c => c.nom == "khalil");
    if (clientKhalil == null)
    {
        var client = new Client()
        {
            nom = "khalil", address = "Tunis", email = "khalilghimaji@gmail.com", estRestreint = false, nbNotes = 1,
            note = 10, sumNotes = 10, telephone = 12345678
        };
        context.Clients.Add(client);
    }
    else
    {
        clientKhalil.nbNotes = 1;
        clientKhalil.note = 10;
        clientKhalil.sumNotes = 10;
        clientKhalil.telephone = 12345678;
        clientKhalil.email = "khalilghimaji@gmail.com";
        clientKhalil.address = "Tunis";
        clientKhalil.estRestreint = false;
        context.Clients.Update(clientKhalil);
    }

    var clientOussema = context.Clients.FirstOrDefault(c => c.nom == "oussema");
    if (clientOussema == null)
    {
        var client = new Client()
        {
            nom = "oussema", address = "Menzah", email = "oussema.guerami@insat.ucar.tn", estRestreint = true,
            sumNotes = 0, nbNotes = 0, note = 0, telephone = 51844856
        };
        context.Clients.Add(client);
    }
    else
    {
        clientOussema.estRestreint = true;
        clientOussema.sumNotes = 0;
        clientOussema.nbNotes = 0;
        clientOussema.note = 0;
        clientOussema.telephone = 51844856;
        clientOussema.email = "oussema.guerami@insat.ucar.tn";
        clientOussema.address = "Menzah";
        context.Clients.Update(clientOussema);
    }

    context.SaveChanges();
}

/*app.MapIdentityApi<User>();*/
app.Run();