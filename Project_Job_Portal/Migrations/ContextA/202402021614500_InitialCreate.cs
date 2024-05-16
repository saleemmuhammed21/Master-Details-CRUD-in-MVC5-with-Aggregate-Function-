namespace Project_Job_Portal.Migrations.ContextA
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Candidates",
                c => new
                    {
                        CandidateId = c.Int(nullable: false, identity: true),
                        CandidateName = c.String(nullable: false, maxLength: 50),
                        BirthDate = c.DateTime(nullable: false, storeType: "date"),
                        AppliedFor = c.Int(nullable: false),
                        ExpectedSalary = c.Decimal(nullable: false, storeType: "money"),
                        Conditions = c.Boolean(nullable: false),
                        Picture = c.String(nullable: false, maxLength: 30),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CandidateId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        DepartmentName = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DepartmentId);
            
            CreateTable(
                "dbo.Qualifications",
                c => new
                    {
                        QualificationId = c.Int(nullable: false, identity: true),
                        Degree = c.String(nullable: false, maxLength: 50),
                        PassingYear = c.Int(nullable: false),
                        Institute = c.String(nullable: false, maxLength: 50),
                        Result = c.String(nullable: false, maxLength: 20),
                        CandidateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QualificationId)
                .ForeignKey("dbo.Candidates", t => t.CandidateId, cascadeDelete: true)
                .Index(t => t.CandidateId);


            CreateStoredProcedure("InsertCandidate", c => new {

                CandidateName = c.String(maxLength: 50),
                BirthDate = c.DateTime(),
                AppliedFor = c.Int(),
                ExpectedSalary = c.Decimal(),
                Conditions = c.Boolean(),
                Picture = c.String(maxLength: 30),
                DepartmentId = c.Int()

            }, @"INSERT INTO Candidates ([CandidateName], BirthDate,AppliedFor,ExpectedSalary, Conditions, Picture,DepartmentId)
	                    VALUES (@CandidateName, @BirthDate, @AppliedFor,@ExpectedSalary,@Conditions, @Picture,@DepartmentId)
	                    SELECT SCOPE_IDENTITY() AS CandidateId
                    RETURN ");
            CreateStoredProcedure("UpdateCandidate", c => new {
                CandidateId = c.Int(),
                CandidateName = c.String(maxLength: 50),
                BirthDate = c.DateTime(),
                AppliedFor = c.Int(),
                ExpectedSalary = c.Decimal(),
                Conditions = c.Boolean(),
                Picture = c.String(maxLength: 30),
                DepartmentId = c.Int()

            }, @"Update Candidates SET [CandidateName] = @CandidateName, BirthDate=@BirthDate,AppliedFor=@AppliedFor,ExpectedSalary=@ExpectedSalary, Conditions=@Conditions, Picture=@Picture,DepartmentId=@DepartmentId
                    WHERE CandidateId = @CandidateId
                RETURN");
            CreateStoredProcedure("DeleteCandidate", c => new
            {
                CandidateId = c.Int()

            }, @"DELETE FROM Candidates
                WHERE CandidateId = @CandidateId
            RETURN");
            CreateStoredProcedure("DeleteQualificationByCandidateId", c => new
            {
                CandidateId = c.Int()

            }, @"DELETE FROM Qualifications
                WHERE CandidateId = @CandidateId
            RETURN");
            CreateStoredProcedure("InsertQualification", c => new
            {
                Degree = c.String(maxLength: 50),
                PassingYear = c.Int(),
                Institute = c.String(maxLength: 50),
                Result = c.String(maxLength: 20),
                CandidateId = c.Int()

            }, @"INSERT INTO Qualifications (Degree, PassingYear, Institute,Result, CandidateId)
	            VALUES (@Degree, @PassingYear, @Institute, @Result,@CandidateId)
	            SELECT SCOPE_IDENTITY() as QualificationId
            RETURN");

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Qualifications", "CandidateId", "dbo.Candidates");
            DropForeignKey("dbo.Candidates", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.Qualifications", new[] { "CandidateId" });
            DropIndex("dbo.Candidates", new[] { "DepartmentId" });
            DropTable("dbo.Qualifications");
            DropTable("dbo.Departments");
            DropTable("dbo.Candidates");

            DropStoredProcedure("InsertCandidate");
            DropStoredProcedure("UpdateCandidate");
            DropStoredProcedure("DeleteCandidate");
            DropStoredProcedure("DeleteQualificationByCandidateId");
            DropStoredProcedure("InsertQualification");
        }
    }
}
