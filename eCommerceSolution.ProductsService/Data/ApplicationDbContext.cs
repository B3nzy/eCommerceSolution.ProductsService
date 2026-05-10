using eCommerceSolution.ProductsService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace eCommerceSolution.ProductsService.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
}
