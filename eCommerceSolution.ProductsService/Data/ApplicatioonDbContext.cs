using eCommerceSolution.ProductsService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace eCommerceSolution.ProductsService.Data;

public class ApplicatioonDbContext : DbContext
{
    public ApplicatioonDbContext(DbContextOptions<ApplicatioonDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
}
