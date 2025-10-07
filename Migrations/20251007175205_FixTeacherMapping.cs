using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTL_WebNC.Migrations
{
    /// <inheritdoc />
    public partial class FixTeacherMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubject_Teacher_TeacherModelId",
                table: "TeacherSubject");

            migrationBuilder.DropIndex(
                name: "IX_TeacherSubject_TeacherModelId",
                table: "TeacherSubject");

            migrationBuilder.DropColumn(
                name: "TeacherModelId",
                table: "TeacherSubject");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeacherModelId",
                table: "TeacherSubject",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubject_TeacherModelId",
                table: "TeacherSubject",
                column: "TeacherModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubject_Teacher_TeacherModelId",
                table: "TeacherSubject",
                column: "TeacherModelId",
                principalTable: "Teacher",
                principalColumn: "Id");
        }
    }
}
