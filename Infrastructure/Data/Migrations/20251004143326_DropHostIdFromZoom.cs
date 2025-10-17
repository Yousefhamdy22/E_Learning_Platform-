using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropHostIdFromZoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ZoomMeetings_Instructors_HostId",
                table: "ZoomMeetings");

            migrationBuilder.DropIndex(
                name: "IX_ZoomMeetings_HostId",
                table: "ZoomMeetings");

            migrationBuilder.DropColumn(
                name: "HostId",
                table: "ZoomMeetings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HostId",
                table: "ZoomMeetings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ZoomMeetings_HostId",
                table: "ZoomMeetings",
                column: "HostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ZoomMeetings_Instructors_HostId",
                table: "ZoomMeetings",
                column: "HostId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
