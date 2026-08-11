using Microsoft.EntityFrameworkCore;
using Pharmaceutical.Models;

namespace Pharmaceutical.Data;

public class PharmaceuticalDbContext :DbContext
{
    public PharmaceuticalDbContext(DbContextOptions<PharmaceuticalDbContext> options): base(options){}
    
    protected PharmaceuticalDbContext(){}
    
    public DbSet<UserAccount> UserAccounts { get; set; } 
    public DbSet<CandidateProfile> CandidateProfiles { get; set; } 
    public DbSet<EducationRecord> EducationRecords { get; set; } 
    public DbSet<WorkExperience> WorkExperiences { get; set; } 
    public DbSet<ResumeFile> ResumeFiles { get; set; } 
    public DbSet<InterviewInvitation> InterviewInvitations { get; set; } 
    public DbSet<ProductCategory> ProductCategories { get; set; } 
    public DbSet<Product> Products { get; set; } 
    public DbSet<CapsuleSpecification> CapsuleSpecifications { get; set; } 
    public DbSet<TabletSpecification> TabletSpecifications { get; set; } 
    public DbSet<LiquidFillingSpecification> LiquidFillingSpecifications { get; set; } 
    public DbSet<QuoteRequest> QuoteRequests { get; set; } 
    public DbSet<ContentPage> ContentPages { get; set; } 
}