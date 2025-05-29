﻿using Domain.Entities;
using Infrastructure.DataContext.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RatingsConfiguration).Assembly);
    }
}
