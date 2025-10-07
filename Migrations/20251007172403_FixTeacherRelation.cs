using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTL_WebNC.Migrations
{
    /// <inheritdoc />
    public partial class FixTeacherRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubject_Account_TeacherId",
                table: "TeacherSubject");

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
                name: "FK_TeacherSubject_Teacher_TeacherId",
                table: "TeacherSubject",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubject_Teacher_TeacherModelId",
                table: "TeacherSubject",
                column: "TeacherModelId",
                principalTable: "Teacher",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubject_Teacher_TeacherId",
                table: "TeacherSubject");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubject_Teacher_TeacherModelId",
                table: "TeacherSubject");

            migrationBuilder.DropIndex(
                name: "IX_TeacherSubject_TeacherModelId",
                table: "TeacherSubject");

            migrationBuilder.DropColumn(
                name: "TeacherModelId",
                table: "TeacherSubject");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubject_Account_TeacherId",
                table: "TeacherSubject",
                column: "TeacherId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
