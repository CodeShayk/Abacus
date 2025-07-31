using Abacus.API.Model;
using Microsoft.EntityFrameworkCore;

namespace Abacus.API.Data
{
    public class WorkflowDbContext : DbContext
    {
        public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
        {
        }

        public DbSet<WorkflowTemplate> WorkflowTemplates { get; set; }
        public DbSet<WorkflowTask> WorkflowTasks { get; set; }
        public DbSet<WorkflowTransition> WorkflowTransitions { get; set; }
        public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
        public DbSet<TaskInstance> TaskInstances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // WorkflowTemplate configuration
            modelBuilder.Entity<WorkflowTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasIndex(e => e.Name);
            });

            // WorkflowTask configuration
            modelBuilder.Entity<WorkflowTask>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Type).HasConversion<string>();

                entity.HasOne(e => e.WorkflowTemplate)
                    .WithMany(e => e.Tasks)
                    .HasForeignKey(e => e.WorkflowTemplateId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // WorkflowTransition configuration
            modelBuilder.Entity<WorkflowTransition>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TriggerType).HasConversion<string>();

                entity.HasOne(e => e.WorkflowTemplate)
                    .WithMany(e => e.Transitions)
                    .HasForeignKey(e => e.WorkflowTemplateId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.FromTask)
                    .WithMany(e => e.FromTransitions)
                    .HasForeignKey(e => e.FromTaskId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ToTask)
                    .WithMany(e => e.ToTransitions)
                    .HasForeignKey(e => e.ToTaskId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // WorkflowInstance configuration
            modelBuilder.Entity<WorkflowInstance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.EntityId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);

                entity.HasOne(e => e.WorkflowTemplate)
                    .WithMany(e => e.Instances)
                    .HasForeignKey(e => e.WorkflowTemplateId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.EntityId, e.EntityType });
            });

            // TaskInstance configuration
            modelBuilder.Entity<TaskInstance>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasConversion<string>();

                entity.HasOne(e => e.WorkflowInstance)
                    .WithMany(e => e.TaskInstances)
                    .HasForeignKey(e => e.WorkflowInstanceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.WorkflowTask)
                    .WithMany(e => e.TaskInstances)
                    .HasForeignKey(e => e.WorkflowTaskId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}