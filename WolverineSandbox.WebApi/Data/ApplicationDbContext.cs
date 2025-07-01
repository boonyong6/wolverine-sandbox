using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Wolverine.EntityFrameworkCore;
using WolverineSandbox.WebApi.Entities;

namespace WolverineSandbox.WebApi.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.LogTo(Console.WriteLine, [RelationalEventId.CommandExecuting])
            .EnableSensitiveDataLogging() // Include SQL parameters.
            .EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // This enables your DbContext to map the incoming and
        // outgoing messages as part of the outbox
        modelBuilder.MapWolverineEnvelopeStorage();

        SetBogusRandomizerSeed();
        List<Customer> seedCustomers = new();
        List<Order> seedOrders = new();

        modelBuilder.Entity<Customer>(builder =>
        {
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(c => c.Email)
                .HasMaxLength(256);

            int i = 0;
            string[] customerIds = [
                "676FF728-5853-F011-9A57-4074E0D1F019",
                "686FF728-5853-F011-9A57-4074E0D1F019",
                "696FF728-5853-F011-9A57-4074E0D1F019",
                "6A6FF728-5853-F011-9A57-4074E0D1F019",
                "6B6FF728-5853-F011-9A57-4074E0D1F019",
                ];
            seedCustomers = new Faker<Customer>()
                .RuleFor(c => c.Id, f => Guid.Parse(customerIds[i++]))
                .RuleFor(c => c.Email, f => f.Internet.Email())
                .Generate(3);
            builder.HasData(seedCustomers);
        });

        modelBuilder.Entity<Order>(builder =>
        {
            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWSEQUENTIALID()");
            builder.Property(o => o.TotalAmount)
                .HasPrecision(18, 2);
            builder.HasOne(typeof(Customer))
                .WithMany()
                .HasForeignKey(nameof(Order.CustomerId));

            int i = 0;
            string[] orderIds = [
                "7BCA9135-5853-F011-9A57-4074E0D1F019",
                "7CCA9135-5853-F011-9A57-4074E0D1F019",
                "7DCA9135-5853-F011-9A57-4074E0D1F019",
                "7ECA9135-5853-F011-9A57-4074E0D1F019",
                "7FCA9135-5853-F011-9A57-4074E0D1F019",
                "BA1F2742-5853-F011-9A57-4074E0D1F019",
                "BB1F2742-5853-F011-9A57-4074E0D1F019",
                "BC1F2742-5853-F011-9A57-4074E0D1F019",
                "BD1F2742-5853-F011-9A57-4074E0D1F019",
                "BE1F2742-5853-F011-9A57-4074E0D1F019",
                ];
            seedOrders = new Faker<Order>()
                .UseDateTimeReference(new DateTime(2025, 6, 27, 17, 57, 6, DateTimeKind.Utc))
                .RuleFor(o => o.Id, f => Guid.Parse(orderIds[i++]))
                .RuleFor(o => o.CustomerId, f => f.PickRandom(seedCustomers).Id)
                .RuleFor(o => o.OrderDate, f => f.Date.PastOffset())
                .RuleFor(o => o.TotalAmount, f => f.Finance.Amount(10, 1000))
                .Generate(9);
            builder.HasData(seedOrders);
        });
    }

    private static void SetBogusRandomizerSeed()
    {
        Randomizer.Seed = new Random(654321);
    }
}
