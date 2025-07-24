using Microsoft.EntityFrameworkCore;
using SurveyApplication.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Survey> Surveys { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Response> Responses { get; set; }
    public DbSet<ResponseAnswer> ResponseAnswers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Survey has many Questions
        modelBuilder.Entity<Survey>()
            .HasMany(s => s.Questions)
            .WithOne(q => q.Survey)
            .HasForeignKey(q => q.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Question has many Options
        modelBuilder.Entity<Question>()
            .HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Question has many ResponseAnswers
        modelBuilder.Entity<ResponseAnswer>()
            .HasOne(ra => ra.Question)
            .WithMany()
            .HasForeignKey(ra => ra.QuestionId)
            .OnDelete(DeleteBehavior.Restrict); // ✅ prevent multiple cascade

        // Option is optional in ResponseAnswer
        modelBuilder.Entity<ResponseAnswer>()
            .HasOne(ra => ra.SelectedOption)
            .WithMany()
            .HasForeignKey(ra => ra.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict); // ✅ no cascade here

        // Response has many ResponseAnswers
        modelBuilder.Entity<Response>()
            .HasMany(r => r.Answers)
            .WithOne(ra => ra.Response)
            .HasForeignKey(ra => ra.ResponseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Response belongs to Survey
        modelBuilder.Entity<Response>()
            .HasOne(r => r.Survey)
            .WithMany()
            .HasForeignKey(r => r.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}