using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechJobs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Search_SP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID('dbo.sp_SearchJobs', 'P') IS NOT NULL
  DROP PROCEDURE dbo.sp_SearchJobs;
GO
CREATE PROCEDURE dbo.sp_SearchJobs
  @TechStack NVARCHAR(100) = NULL,
  @Location  NVARCHAR(100) = NULL,
  @MinExperienceYears INT = NULL,
  @Role NVARCHAR(100) = NULL
AS
BEGIN
  SET NOCOUNT ON;

  SELECT DISTINCT j.*
  FROM Jobs j
  LEFT JOIN JobTechStacks jts ON j.Id = jts.JobId
  LEFT JOIN TechStacks ts ON ts.Id = jts.TechStackId
  WHERE
    (@Location IS NULL OR @Location = '' OR j.Location LIKE '%' + @Location + '%')
    AND (@MinExperienceYears IS NULL OR j.MinExperienceYears IS NULL OR j.MinExperienceYears <= @MinExperienceYears)
    AND (@Role IS NULL OR @Role = '' OR j.Role LIKE '%' + @Role + '%')
    AND (@TechStack IS NULL OR @TechStack = '' OR ts.Name LIKE '%' + @TechStack + '%')
    AND j.IsApproved = 1;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID('dbo.sp_SearchJobs', 'P') IS NOT NULL
  DROP PROCEDURE dbo.sp_SearchJobs;
");
        }
    }
}
