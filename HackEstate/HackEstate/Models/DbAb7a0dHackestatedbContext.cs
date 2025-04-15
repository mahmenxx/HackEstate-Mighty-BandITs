using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HackEstate.Models;

public partial class DbAb7a0dHackestatedbContext : DbContext
{
    public DbAb7a0dHackestatedbContext()
    {
    }

    public DbAb7a0dHackestatedbContext(DbContextOptions<DbAb7a0dHackestatedbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AgentProperty> AgentProperties { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<PropertyImage> PropertyImages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserQuizAnswer> UserQuizAnswers { get; set; }

    public virtual DbSet<UserSeminarCertification> UserSeminarCertifications { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseLazyLoadingProxies().UseSqlServer("Data Source=sql1003.site4now.net;Initial Catalog=db_ab7a0d_hackestatedb;User Id=db_ab7a0d_hackestatedb_admin;Password=jules0019");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AgentProperty>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AgentId).HasColumnName("agent_id");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");

            entity.HasOne(d => d.Agent).WithMany(p => p.AgentProperties)
                .HasForeignKey(d => d.AgentId)
                .HasConstraintName("FK_AgentProperties_User");

            entity.HasOne(d => d.Property).WithMany(p => p.AgentProperties)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AgentProperties_Property");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.ToTable("ChatMessage");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FromUserId).HasColumnName("from_user_id");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");
            entity.Property(e => e.Timestamp)
                .HasColumnType("datetime")
                .HasColumnName("timestamp");
            entity.Property(e => e.ToUserId).HasColumnName("to_user_id");

            entity.HasOne(d => d.FromUser).WithMany(p => p.ChatMessageFromUsers)
                .HasForeignKey(d => d.FromUserId)
                .HasConstraintName("FK_ChatMessage_User");

            entity.HasOne(d => d.Property).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("FK_ChatMessage_Property");

            entity.HasOne(d => d.ToUser).WithMany(p => p.ChatMessageToUsers)
                .HasForeignKey(d => d.ToUserId)
                .HasConstraintName("FK_ChatMessage_User1");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Event");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Events)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Event_User");
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.ToTable("Property");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amenities).HasColumnName("amenities");
            entity.Property(e => e.BathroomQty).HasColumnName("bathroomQty");
            entity.Property(e => e.BedroomQty).HasColumnName("bedroomQty");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.LotSize).HasColumnName("lotSize");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.PropertyType)
                .HasMaxLength(50)
                .HasColumnName("propertyType");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Properties)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Property_User");
        });

        modelBuilder.Entity<PropertyImage>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ImageUrl).HasColumnName("imageUrl");
            entity.Property(e => e.PropertyId).HasColumnName("property_id");

            entity.HasOne(d => d.Property).WithMany(p => p.PropertyImages)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PropertyImages_Property");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Contact)
                .HasMaxLength(50)
                .HasColumnName("contact");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");
            entity.Property(e => e.IdentificationCardUrl).HasColumnName("identificationCardURL");
            entity.Property(e => e.IsVerified)
                .HasMaxLength(50)
                .HasDefaultValue("FALSE")
                .HasColumnName("isVerified");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("lastName");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_User_Role");
        });

        modelBuilder.Entity<UserQuizAnswer>(entity =>
        {
            entity.ToTable("UserQuizAnswer");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BudgetMax).HasColumnName("budgetMax");
            entity.Property(e => e.BudgetMin).HasColumnName("budgetMin");
            entity.Property(e => e.MostImportantInAgent)
                .HasMaxLength(50)
                .HasColumnName("mostImportantInAgent");
            entity.Property(e => e.PreferCommunication)
                .HasMaxLength(50)
                .HasColumnName("preferCommunication");
            entity.Property(e => e.PreferredLocation).HasColumnName("preferredLocation");
            entity.Property(e => e.TypeOfProperty)
                .HasMaxLength(50)
                .HasColumnName("typeOfProperty");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.WhenToBuy)
                .HasMaxLength(50)
                .HasColumnName("whenToBuy");

            entity.HasOne(d => d.User).WithMany(p => p.UserQuizAnswers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserQuizAnswer_User");
        });

        modelBuilder.Entity<UserSeminarCertification>(entity =>
        {
            entity.ToTable("UserSeminarCertification");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ImageUrl).HasColumnName("imageURL");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserSeminarCertifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserSeminarCertification_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
