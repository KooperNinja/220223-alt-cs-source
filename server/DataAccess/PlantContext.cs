using _220223KCore.Models;
using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace _220223KCore.DataAccess
{
    public class PlantContext : DbContext
    {
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ServerVersion sv = MariaDbServerVersion.AutoDetect(Constants.ConnectionString);

            optionsBuilder.UseMySql(Constants.ConnectionString, sv);
        }

        public DbSet<Plant> Plants { get; set; }
        public DbSet<PlantType> PlantTypes { get; set; }
        public DbSet<PlantTypeModel> PlantTypeModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Plant>(entity =>
            {
                entity.Property(b => b.Position)
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<Position>(v)
                        );
                entity.Property(b => b.Rotation)
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<Vector3>(v)
                    );
            });
                
        }

    }
}
