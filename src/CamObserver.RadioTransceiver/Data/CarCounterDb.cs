﻿using Microsoft.EntityFrameworkCore;
using CamObserver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CamObserver.RadioTransceiver.Data;

namespace CamObserver.RadioTransceiver.Data
{
    public class CamObserverDB : DbContext
    {

        public CamObserverDB()
        {
        }

        public CamObserverDB(DbContextOptions<CamObserverDB> options)
            : base(options)
        {
        }
        public DbSet<CCTV> CCTVs { get; set; }
        public DbSet<Gateway> Gateways { get; set; }
        public DbSet<DataCounter> DataCounters { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<WeatherData> WeatherDatas { get; set; }
      
        protected override void OnModelCreating(ModelBuilder builder)
        {
            /*
            builder.Entity<DataEventRecord>().HasKey(m => m.DataEventRecordId);
            builder.Entity<SourceInfo>().HasKey(m => m.SourceInfoId);

            // shadow properties
            builder.Entity<DataEventRecord>().Property<DateTime>("UpdatedTimestamp");
            builder.Entity<SourceInfo>().Property<DateTime>("UpdatedTimestamp");
            */
            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            /*
            ChangeTracker.DetectChanges();

            updateUpdatedProperty<SourceInfo>();
            updateUpdatedProperty<DataEventRecord>();
            */
            return base.SaveChanges();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(AppConstants.SQLConn, ServerVersion.AutoDetect(AppConstants.SQLConn));
            }
        }
        private void updateUpdatedProperty<T>() where T : class
        {
            /*
            var modifiedSourceInfo =
                ChangeTracker.Entries<T>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in modifiedSourceInfo)
            {
                entry.Property("UpdatedTimestamp").CurrentValue = DateTime.UtcNow;
            }
            */
        }

    }
}
