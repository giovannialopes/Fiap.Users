using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Users.Domain.Entity;

namespace Users.Infrastructure.Data;

public class DbUser : DbContext
{
    public DbUser(DbContextOptions<DbUser> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PerfilEnt>().ToTable("USERS");
        modelBuilder.Entity<ILoggerEnt>().ToTable("LOGS");
        modelBuilder.Entity<CarteiraEnt>().ToTable("CARTEIRA");

        modelBuilder.Entity<CarteiraEnt>()
            .Property(c => c.Saldo)
            .HasPrecision(18, 2);
    }

    public DbSet<PerfilEnt> USERS { get; set; }
    public DbSet<ILoggerEnt> LOGS { get; set; }
    public DbSet<CarteiraEnt> CARTEIRA { get; set; }
}
