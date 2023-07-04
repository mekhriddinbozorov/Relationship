using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductDetail> ProductDetails { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //  public string Name { get; set; }
        // public decimal Price { get; set; }
        // public DateTime CreatedAt { get; set; }
        // public DateTime ModifiedAt { get; set; }
        // public EProductStatus Status { get; set; }


        modelBuilder.Entity<Product>()
            .HasKey(b => b.Id);

        modelBuilder.Entity<Product>()
            .Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(125);

        modelBuilder.Entity<Product>()
            .Property(b => b.Price)
            .IsRequired();

        modelBuilder.Entity<Product>()
            .Property(b => b.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Product>()
            .Property(b => b.ModifiedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ProductDetail>()
           .HasKey(b => b.Id);

        modelBuilder.Entity<ProductDetail>()
            .Property(b => b.Description)
            .HasMaxLength(125);

        modelBuilder.Entity<ProductDetail>()
            .Property(b => b.Color)
            .HasMaxLength(125);

        modelBuilder.Entity<ProductDetail>()
            .Property(b => b.Material)
            .HasMaxLength(125);

        modelBuilder.Entity<ProductDetail>()
            .Property(b => b.Weight)
            .IsRequired();

        modelBuilder.Entity<ProductDetail>()
            .Property(b => b.Weight)
            .IsRequired();

        modelBuilder.Entity<ProductDetail>()
            .Property(b => b.ManufactureDate)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ProductDetail>()
            .Property(b => b.ExpiryDate)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ProductDetail>()
            .Property(b => b.Size)
            .HasMaxLength(125);

        modelBuilder.Entity<ProductDetail>()
            .Property(b => b.Manufacturer)
            .HasMaxLength(125);

        modelBuilder.Entity<ProductDetail>()
            .Property(b => b.CountryOfOrigin)
            .HasMaxLength(125);

        modelBuilder.Entity<Product>()
            .HasOne(d => d.ProductDetail)
            .WithOne(u => u.Product)
            .HasForeignKey<ProductDetail>(u => u.Id)
            .HasPrincipalKey<Product>(u => u.Id);

        base.OnModelCreating(modelBuilder);
    }
}