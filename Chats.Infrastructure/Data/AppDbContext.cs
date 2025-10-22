using Chats.Core.Enums;
using Chats.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;

namespace Chats.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMember> ChatMembers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<FileEntity> Files { get; set; }
        public DbSet<Reaction> Reactions { get; set; }
        public DbSet<ReadMessage> ReadMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>().ToTable("chats");
            modelBuilder.Entity<ChatMember>().ToTable("chat_members");
            modelBuilder.Entity<Message>().ToTable("messages");
            modelBuilder.Entity<FileEntity>().ToTable("files");
            modelBuilder.Entity<Reaction>().ToTable("reactions");
            modelBuilder.Entity<ReadMessage>().ToTable("read_messages");

            modelBuilder.Entity<ChatMember>().HasKey(cm => new { cm.ChatId, cm.UserId });
            modelBuilder.Entity<ReadMessage>().HasKey(rm => new { rm.MessageId, rm.UserId });

            var chatTypeConverter = new EnumToStringConverter<ChatType>();
            modelBuilder.Entity<Chat>()
                .Property(c => c.Type)
                .HasConversion(chatTypeConverter)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<Chat>();
             

            modelBuilder.Entity<ChatMember>()
                .HasOne(cm => cm.Chat)
                .WithMany(c => c.Members)
                .HasForeignKey(cm => cm.ChatId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId);

            modelBuilder.Entity<FileEntity>()
                .HasOne(f => f.Message)
                .WithMany(m => m.Files)
                .HasForeignKey(f => f.MessageId);

            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Message)
                .WithMany(m => m.Reactions)
                .HasForeignKey(r => r.MessageId);

            modelBuilder.Entity<ReadMessage>()
                .HasOne(rm => rm.Message)
                .WithMany(m => m.ReadByUsers)
                .HasForeignKey(rm => rm.MessageId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
