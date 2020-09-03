using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace SSTUScheduleBot.Models
{
    public partial class DbDataContext : DbContext
    {
        private readonly string connectionString;

        public DbDataContext(Config config)
        {
            connectionString = config.ConnectionString;
        }

        public DbDataContext()
        {
        }

        public DbDataContext(DbContextOptions<DbDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ConnectionType> ConnectionTypes { get; set; }
        public virtual DbSet<Group>          Groups          { get; set; }
        public virtual DbSet<Request>        Requests        { get; set; }
        public virtual DbSet<Schedule>       Schedules       { get; set; }
        public virtual DbSet<SubGroupLesson> SubGroupLessons { get; set; }
        public virtual DbSet<User>           Users           { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConnectionType>(entity =>
            {
                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("name")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Formattedname)
                    .HasMaxLength(15)
                    .HasColumnName("formattedname")
                    .IsFixedLength(true);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(15)
                    .HasColumnName("name")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Datetime)
                    .HasColumnType("datetime")
                    .HasColumnName("datetime")
                    .HasAnnotation("Relational:ColumnType", "datetime");

                entity.Property(e => e.Request1).HasColumnName("request");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(x => x.UserId)
                    .HasConstraintName("FK_Requests_Users");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");

                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Classroom)
                    .HasMaxLength(50)
                    .HasColumnName("classroom");

                entity.Property(e => e.Day).HasColumnName("day");

                entity.Property(e => e.GroupId).HasColumnName("group_id");

                entity.Property(e => e.IsEvenWeek).HasColumnName("is_even_week");

                entity.Property(e => e.IsSubgroup).HasColumnName("is_subgroup");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Order).HasColumnName("order");

                entity.Property(e => e.Teacher)
                    .HasMaxLength(100)
                    .HasColumnName("teacher");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(x => x.GroupId)
                    .HasConstraintName("FK_Schedule_Groups");
            });

            modelBuilder.Entity<SubGroupLesson>(entity =>
            {
                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Classroom)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("classroom");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Teacher)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("teacher");

                entity.Property(e => e.Type)
                    .HasMaxLength(50)
                    .HasColumnName("type");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.SubGroupLessons)
                    .HasForeignKey(x => x.CourseId)
                    .HasConstraintName("FK_SubGroupLessons_Schedule");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasAnnotation("Relational:IsTableExcludedFromMigrations", false);

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConnectionType).HasColumnName("connection_type");

                entity.Property(e => e.GroupId).HasColumnName("group_id");

                entity.Property(e => e.InnerId).HasColumnName("inner_id");

                entity.Property(e => e.State).HasColumnName("state");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasColumnName("username")
                    .IsFixedLength(true);

                entity.HasOne(d => d.ConnectionTypeNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(x => x.ConnectionType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_ConnectionTypes");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.Users)
                    .HasForeignKey(x => x.GroupId)
                    .HasConstraintName("FK_Users_Groups");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}