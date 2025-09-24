// 版权所有(c) HeYuanERP
// 说明：新增报表任务与快照的迁移（中文注释）。

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HeYuanERP.Infrastructure.Data.Migrations;

/// <summary>
/// 创建 ReportJobs 与 ReportSnapshots 表。
/// </summary>
public partial class _20250910_AddReports : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ReportJobs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Format = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                ParametersJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FileUri = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                ErrorMessage = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                StartedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                CompletedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ReportJobs", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ReportSnapshots",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                ParametersJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                FileUri = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                ParamHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                ClientIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                UserAgent = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                CorrelationId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ReportSnapshots", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ReportJobs_Status",
            table: "ReportJobs",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_ReportJobs_CreatedAtUtc",
            table: "ReportJobs",
            column: "CreatedAtUtc");

        migrationBuilder.CreateIndex(
            name: "IX_ReportSnapshots_Type_ParamHash",
            table: "ReportSnapshots",
            columns: new[] { "Type", "ParamHash" });

        migrationBuilder.CreateIndex(
            name: "IX_ReportSnapshots_CreatedAtUtc",
            table: "ReportSnapshots",
            column: "CreatedAtUtc");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ReportJobs");
        migrationBuilder.DropTable(name: "ReportSnapshots");
    }
}

