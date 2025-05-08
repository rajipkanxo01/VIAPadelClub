using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VIAPadelClub.Infrastructure.EfcDmPersistence.Migrations
{
    /// <inheritdoc />
    public partial class assignmentEight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailySchedules",
                columns: table => new
                {
                    ScheduleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    availableFrom = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    availableUntil = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    isDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    scheduleDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailySchedules", x => x.ScheduleId);
                });

            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    isBlackListed = table.Column<bool>(type: "INTEGER", nullable: false),
                    isQuarantined = table.Column<bool>(type: "INTEGER", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    ProfileUrl = table.Column<string>(type: "TEXT", nullable: false),
                    QuarantineStartDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    QuarantineEndDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    VIPStartDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    VIPEndDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    IsVip = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.email);
                });

            migrationBuilder.CreateTable(
                name: "Courts",
                columns: table => new
                {
                    CourtName = table.Column<string>(type: "TEXT", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courts", x => new { x.CourtName, x.ScheduleId });
                    table.ForeignKey(
                        name: "FK_Courts_DailySchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "DailySchedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VipTimeRanges",
                columns: table => new
                {
                    VipStart = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    VipEnd = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    DailyScheduleId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VipTimeRanges", x => new { x.DailyScheduleId, x.VipStart, x.VipEnd });
                    table.ForeignKey(
                        name: "FK_VipTimeRanges_DailySchedules_DailyScheduleId",
                        column: x => x.DailyScheduleId,
                        principalTable: "DailySchedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BookedBy = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Duration = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    BookedDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    BookingStatus = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Courts_Name_ScheduleId",
                        columns: x => new { x.Name, x.ScheduleId },
                        principalTable: "Courts",
                        principalColumns: new[] { "CourtName", "ScheduleId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_DailySchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "DailySchedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Player_BookedBy",
                        column: x => x.BookedBy,
                        principalTable: "Player",
                        principalColumn: "email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookedBy",
                table: "Bookings",
                column: "BookedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Name_ScheduleId",
                table: "Bookings",
                columns: new[] { "Name", "ScheduleId" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ScheduleId",
                table: "Bookings",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Courts_ScheduleId",
                table: "Courts",
                column: "ScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "VipTimeRanges");

            migrationBuilder.DropTable(
                name: "Courts");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "DailySchedules");
        }
    }
}
