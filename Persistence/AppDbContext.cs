using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;
using Persistence.entities.Client;
using Persistence.entities.Commande;
using Persistence.entities.Facturation;
using Persistence.entities.Stock;

namespace Persistence
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=../Persistence/app.db", x => x.MigrationsAssembly("AppGateway"));
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<ArticleCommande> ArticleCommandes { get; set; }
        public DbSet<Facture> Factures { get; set; }
        public DbSet<Echeance> Echeances { get; set; }
        public DbSet<ArticleStock> AricleStocks { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Produit> Produits { get; set; }
    }
}