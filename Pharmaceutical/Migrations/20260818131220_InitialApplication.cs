using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharmaceutical.Migrations
{
    /// <inheritdoc />
    public partial class InitialApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CandidateProfile",
                columns: table => new
                {
                    candidate_id = table.Column<int>(type: "int", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    profile_image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateProfile", x => x.candidate_id);
                });

            migrationBuilder.CreateTable(
                name: "CapsuleSpecification",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false),
                    output = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    capsule_size_mm = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    machine_dimension = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    shipping_weight_kg = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapsuleSpecification", x => x.product_id);
                });

            migrationBuilder.CreateTable(
                name: "ContentPage",
                columns: table => new
                {
                    page_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    banner_image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    updated_by = table.Column<int>(type: "int", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPage", x => x.page_id);
                });

            migrationBuilder.CreateTable(
                name: "EducationRecord",
                columns: table => new
                {
                    education_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    candidate_id = table.Column<int>(type: "int", nullable: true),
                    institution = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    qualification = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    field = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationRecord", x => x.education_id);
                });

            migrationBuilder.CreateTable(
                name: "InterviewInvitation",
                columns: table => new
                {
                    invitation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    candidate_id = table.Column<int>(type: "int", nullable: true),
                    sent_by = table.Column<int>(type: "int", nullable: true),
                    subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    sent_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewInvitation", x => x.invitation_id);
                });

            migrationBuilder.CreateTable(
                name: "LiquidFillingSpecification",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false),
                    air_pressure = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    air_volume = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    filling_speed = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    filling_range_ml = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiquidFillingSpecification", x => x.product_id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    PositionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SalaryRange = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.PositionId);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    model_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    summary = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    output_label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    is_published = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.product_id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategory",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategory", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "QuoteRequest",
                columns: table => new
                {
                    quote_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    company_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    state = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    postal_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    submitted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    handled_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteRequest", x => x.quote_id);
                });

            migrationBuilder.CreateTable(
                name: "ResumeFile",
                columns: table => new
                {
                    resume_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    candidate_id = table.Column<int>(type: "int", nullable: true),
                    storage_key = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    original_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    mime_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    size = table.Column<int>(type: "int", nullable: true),
                    uploaded_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_current = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeFile", x => x.resume_id);
                });

            migrationBuilder.CreateTable(
                name: "TabletSpecification",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false),
                    model_number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    dies = table.Column<int>(type: "int", nullable: true),
                    max_pressure = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    max_diameter_mm = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    max_depth_fill_mm = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    production_capacity = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    machine_size = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    net_weight_kg = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabletSpecification", x => x.product_id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccount",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    last_login_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccount", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "WorkExperience",
                columns: table => new
                {
                    experience_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    candidate_id = table.Column<int>(type: "int", nullable: true),
                    employer = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkExperience", x => x.experience_id);
                });

            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    application_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    candidate_id = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    AppliedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.application_id);
                    table.ForeignKey(
                        name: "FK_Application_CandidateProfile_candidate_id",
                        column: x => x.candidate_id,
                        principalTable: "CandidateProfile",
                        principalColumn: "candidate_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Application_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageProduct",
                columns: table => new
                {
                    image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: true),
                    url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    display_order = table.Column<int>(type: "int", nullable: true),
                    is_thumbnail = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageProduct", x => x.image_id);
                    table.ForeignKey(
                        name: "FK_ImageProduct_Product_product_id",
                        column: x => x.product_id,
                        principalTable: "Product",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application_candidate_id",
                table: "Application",
                column: "candidate_id");

            migrationBuilder.CreateIndex(
                name: "IX_Application_PositionId",
                table: "Application",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageProduct_product_id",
                table: "ImageProduct",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "CapsuleSpecification");

            migrationBuilder.DropTable(
                name: "ContentPage");

            migrationBuilder.DropTable(
                name: "EducationRecord");

            migrationBuilder.DropTable(
                name: "ImageProduct");

            migrationBuilder.DropTable(
                name: "InterviewInvitation");

            migrationBuilder.DropTable(
                name: "LiquidFillingSpecification");

            migrationBuilder.DropTable(
                name: "ProductCategory");

            migrationBuilder.DropTable(
                name: "QuoteRequest");

            migrationBuilder.DropTable(
                name: "ResumeFile");

            migrationBuilder.DropTable(
                name: "TabletSpecification");

            migrationBuilder.DropTable(
                name: "UserAccount");

            migrationBuilder.DropTable(
                name: "WorkExperience");

            migrationBuilder.DropTable(
                name: "CandidateProfile");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}
