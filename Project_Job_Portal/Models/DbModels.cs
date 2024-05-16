using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Project_Job_Portal.Models
{
    public enum AppliedFor { Manager = 1,Officer, OfficeAssistant, Others }
    public enum DepartmentName { CSE=1, BBA,EEE}
    public class Department
    {
        public int DepartmentId { get; set; }
        [Required, EnumDataType(typeof(DepartmentName))]
        public DepartmentName DepartmentName { get; set; }
        public virtual ICollection<Candidate> Candidate { get; set; }= new List<Candidate>();
       
    }
    public class Candidate
    {
        public int CandidateId { get; set; }
        [Required, StringLength(50), Display(Name = "Candidate name")]
        public string CandidateName { get; set; }
        [Required, Column(TypeName = "date"), Display(Name = "First introduced"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime BirthDate { get; set; }
        [Required, EnumDataType(typeof(AppliedFor))]
        public AppliedFor AppliedFor { get; set; }
        [Required, Column(TypeName = "money"), DataType(DataType.Currency)]
        public decimal ExpectedSalary { get; set; }
        [Display(Name = "Ready to work at night")]
        public bool Conditions { get; set; }
        [Required, StringLength(30)]
        public string Picture { get; set; }
        [Required, ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public virtual ICollection<Qualification> Qualifications { get; set; } = new List<Qualification>();
    }
    
    public class Qualification
    {
        public int QualificationId { get; set; }
        [Required, StringLength(50)]
        public string Degree { get; set; }
        [Required]
        public int PassingYear { get; set; }
        [Required, StringLength(50)]
        public string Institute { get; set; }
        [Required, StringLength(20)]
        public string Result { get; set; }
        [Required, ForeignKey("Candidate")]
        public int CandidateId { get; set; }
        public virtual Candidate Candidate { get; set; }
    }
    
    public class CandidateDbContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
    }
}