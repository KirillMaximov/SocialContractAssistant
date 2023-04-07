using Microsoft.EntityFrameworkCore;
using SocialContractAssistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialContractAssistant.DataAccess
{
    internal class DataBaseContext : DbContext
    {
        public DbSet<QuestionModel> Questions { get; set; }
        public DbSet<OptionModel> Options { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<AnswerModel> Answers { get; set; }
        public DbSet<MessageModel> Messages { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<DocumentModel> Documents { get; set; }
        public DbSet<LinkDocumentModel> LinkDocuments { get; set; }
        public DbSet<DataDocumentsModel> DataDocuments { get; set; }

        public DbSet<SettingsModel> Settings { get; set; }
        public DbSet<LogModel> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;database=social_contract;user=root;password=44f91C29ec");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<QuestionModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Text).IsRequired();
                entity.Property(e => e.ApplicationId).IsRequired();
            });

            modelBuilder.Entity<OptionModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QuestionId).IsRequired();
                entity.Property(e => e.Text).IsRequired();
                entity.Property(e => e.NextQuestionId).IsRequired(false);
                entity.Property(e => e.LinkDocumentId).IsRequired(false);
            });

            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.HasKey(e => e.ChatId);
                entity.Property(e => e.Created).IsRequired();
                entity.Property(e => e.Username).IsRequired(false);
                entity.Property(e => e.FirstName).IsRequired(false);
                entity.Property(e => e.SecondName).IsRequired(false);
            });

            modelBuilder.Entity<AnswerModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ChatId).IsRequired();
                entity.Property(e => e.QuestionId).IsRequired();
                entity.Property(e => e.OptionId).IsRequired();
            });

            modelBuilder.Entity<MessageModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ChatId).IsRequired();
                entity.Property(e => e.MessageId).IsRequired();
                entity.Property(e => e.Type).IsRequired(false);
                entity.Property(e => e.Created).IsRequired();
            });

            modelBuilder.Entity<PaymentModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ChatId).IsRequired();
                entity.Property(e => e.PaymentId).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.TypeId).IsRequired();
                entity.Property(e => e.Created).IsRequired();
                entity.Property(e => e.Updated).IsRequired(false);
                entity.Property(e => e.IsActive).IsRequired();
            });

            modelBuilder.Entity<DocumentModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Description).IsRequired(false);
            });

            modelBuilder.Entity<LinkDocumentModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LinkId).IsRequired();
                entity.Property(e => e.DocumentId).IsRequired();
            });

            modelBuilder.Entity<DataDocumentsModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired(false);
                entity.Property(e => e.Text).IsRequired(false);
            });

            modelBuilder.Entity<SettingsModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ApplicationId).IsRequired();
                entity.Property(e => e.TelegramToken).IsRequired();
                entity.Property(e => e.PaymentTokenTest).IsRequired();
                entity.Property(e => e.PaymentTokenLife).IsRequired();
                entity.Property(e => e.PriceAmount).IsRequired();
                entity.Property(e => e.PriceLable).IsRequired();
                entity.Property(e => e.PriceTitle).IsRequired();
                entity.Property(e => e.PriceDescription).IsRequired();
            });

            modelBuilder.Entity<LogModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreateDate).IsRequired();
                entity.Property(e => e.Class).IsRequired();
                entity.Property(e => e.Method).IsRequired();
                entity.Property(e => e.Message).IsRequired();
                entity.Property(e => e.ChatId).IsRequired();
            });
        }
    }
}
