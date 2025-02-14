using ChatALot.Data.Models.Domains;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatALot.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User-Message (One-to-Many)
            modelBuilder.Entity<Message>()
                .HasOne(message => message.Sender)
                .WithMany(user => user.Messages)
                .HasForeignKey(message => message.SenderId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
