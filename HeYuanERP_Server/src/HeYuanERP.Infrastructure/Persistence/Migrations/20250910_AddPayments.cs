using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HeYuanERP.Infrastructure.Persistence.Migrations;

/// <summary>
/// 迁移：新增收款与附件表。
/// 注意：此文件为手写迁移示例，实际项目中建议使用 EF Core 命令生成，确保与 DbContext 配置一致。
/// </summary>
public partial class AddPayments : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "payments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                PaymentDate = table.Column<DateTime>(type: "date", nullable: false),
                Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                OrgId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_payments", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "payment_attachments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Size = table.Column<long>(type: "bigint", nullable: false),
                StorageKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                UploadedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_payment_attachments", x => x.Id);
                table.ForeignKey(
                    name: "FK_payment_attachments_payments_payment_id",
                    column: x => x.PaymentId,
                    principalTable: "payments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // 索引
        migrationBuilder.CreateIndex(
            name: "IX_payments_payment_date",
            table: "payments",
            column: "PaymentDate");

        migrationBuilder.CreateIndex(
            name: "IX_payments_method",
            table: "payments",
            column: "Method");

        migrationBuilder.CreateIndex(
            name: "IX_payments_amount",
            table: "payments",
            column: "Amount");

        migrationBuilder.CreateIndex(
            name: "IX_payment_attachments_payment_id",
            table: "payment_attachments",
            column: "PaymentId");

        migrationBuilder.CreateIndex(
            name: "IX_payment_attachments_storage_key",
            table: "payment_attachments",
            column: "StorageKey");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "payment_attachments");
        migrationBuilder.DropTable(name: "payments");
    }
}

