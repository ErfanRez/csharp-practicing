using ETL.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETL.Infrastructure.Data;

public class WorkflowDbContext : DbContext
{
    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options)
        : base(options)
    {
    }

    public DbSet<Pipeline> Pipelines { get; set; }
    public DbSet<Plugin> Plugins { get; set; }
    public DbSet<DataSetMetadata> DataSources { get; set; } // Add this


    public WorkflowDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map DataSetMetadata to existing table and columns
        modelBuilder.Entity<DataSetMetadata>(entity =>
        {
            entity.ToTable("data_sets", t => t.ExcludeFromMigrations()); // don’t let EF create/alter this table
            entity.HasKey(d => d.Id);

            // Map properties to actual column names
            entity.Property(d => d.Id).HasColumnName("id");
            entity.Property(d => d.TableName).HasColumnName("table_name");
            entity.Property(d => d.UploadedByUserId).HasColumnName("uploaded_by_user_id");
            entity.Property(d => d.CreatedAt).HasColumnName("uploaded_at");
        });

        // Configure Pipeline
        modelBuilder.Entity<Pipeline>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(255);
            entity.Property(p => p.CreatedByUserId).IsRequired();


            // Pipeline → Plugin (1:N)
            entity.HasMany(p => p.Plugins)
                .WithOne(s => s.Pipeline)
                .HasForeignKey(s => s.PipelineId)
                .OnDelete(DeleteBehavior.Cascade);

            // Pipeline → DataSetMetadata (N:1)
            entity.HasOne(p => p.DataSource)
                .WithMany() // could be .WithMany(d => d.Pipelines) if you add collection
                .HasForeignKey(p => p.DataSourceId)
                .OnDelete(DeleteBehavior.Restrict); // don’t delete DataSet when Pipeline is deleted
        });

        // Configure Plugin
        modelBuilder.Entity<Plugin>(entity =>
        {
            entity.HasKey(s => s.Id);

            // Ensure unique step order within a pipeline
            entity.HasIndex(s => new { s.PipelineId, s.StepOrder }).IsUnique();
        });
    }


}