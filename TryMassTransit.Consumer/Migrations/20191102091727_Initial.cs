using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TryMassTransit.Consumer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sagaReportSagaState",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(nullable: false),
                    CurrentState = table.Column<string>(maxLength: 200, nullable: true),
                    ReportId = table.Column<Guid>(nullable: false),
                    RequestTime = table.Column<DateTime>(nullable: false),
                    EMail = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sagaReportSagaState", x => x.CorrelationId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sagaReportSagaState");
        }
    }
}
