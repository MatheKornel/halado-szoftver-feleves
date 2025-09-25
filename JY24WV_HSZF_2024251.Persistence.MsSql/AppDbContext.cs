using JY24WV_HSZF_2024251.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JY24WV_HSZF_2024251.Persistence.MsSql
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<MeasurementPoint> Points { get; set; }
        public DbSet<MeasurementData> Data { get; set; }
        public AppDbContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=measurementdb;Integrated Security=True;MultipleActiveResultSets=true";
            optionsBuilder.UseSqlServer(connStr);
            base.OnConfiguring(optionsBuilder);
        }

        //Javítás
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MeasurementData>()
            .HasOne(md => md.MeasurementPoint)
            .WithMany(mp => mp.Measurements)
            .HasForeignKey(md => md.PointNumber)
            .HasPrincipalKey(mp => mp.PointNumber);

            modelBuilder.Entity<MeasurementPoint>().HasData(new MeasurementPoint[]
            {
                new MeasurementPoint("789","Balassagyarmat - Ipoly"),
                new MeasurementPoint("100","Atlantis")
            });

            modelBuilder.Entity<MeasurementData>().HasData(new MeasurementData[]
            {
                new MeasurementData(DateTime.Parse("2024-08-15T00:00:00Z"),23,0.5,200)
                {
                    PointNumber="789"
                },
                new MeasurementData(DateTime.Parse("2024-02-15T00:00:00Z"),22, 0.1, 412)
                {
                    PointNumber="789"
                },
                new MeasurementData(DateTime.Parse("2024-03-15T00:00:00Z"), 12, 0.8, 544)
                {
                    PointNumber="789"
                },
                new MeasurementData(DateTime.Parse("1000-10-10T00:00:00Z"), 100, 5, 1500)
                {
                    PointNumber="100"
                }
            });
        }
    }
}
