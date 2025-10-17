using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExamResultAttrbuiteUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_AnswerOptions_SelectedOptionId",
                table: "StudentAnswers");

            migrationBuilder.RenameColumn(
                name: "SelectedOptionId",
                table: "StudentAnswers",
                newName: "SelectedAnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswers_SelectedOptionId",
                table: "StudentAnswers",
                newName: "IX_StudentAnswers_SelectedAnswerId");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AnsweredAt",
                table: "StudentAnswers",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "StudentAnswers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SubmittedAt",
                table: "ExamResults",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_AnswerOptions_SelectedAnswerId",
                table: "StudentAnswers",
                column: "SelectedAnswerId",
                principalTable: "AnswerOptions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_AnswerOptions_SelectedAnswerId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "AnsweredAt",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "ExamResults");

            migrationBuilder.RenameColumn(
                name: "SelectedAnswerId",
                table: "StudentAnswers",
                newName: "SelectedOptionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAnswers_SelectedAnswerId",
                table: "StudentAnswers",
                newName: "IX_StudentAnswers_SelectedOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_AnswerOptions_SelectedOptionId",
                table: "StudentAnswers",
                column: "SelectedOptionId",
                principalTable: "AnswerOptions",
                principalColumn: "Id");
        }
    }
}
