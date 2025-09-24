using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HeYuanERP.Infrastructure.Migrations;

/// <summary>
/// 添加发票与发票明细表（含电票字段）。
/// 说明：实际迁移应通过 EF CLI 生成，本文件为占位草案，便于代码审阅与结构对齐。
/// </summary>
public partial class AddInvoices : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Invoices",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Number = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CustomerName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                SourceType = table.Column<int>(type: "int", nullable: false),
                SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                SourceNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                DefaultTaxRate = table.Column<decimal>(type: "decimal(9,6)", nullable: true),
                SubtotalExcludingTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TotalTaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                IssuedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                IsElectronic = table.Column<bool>(type: "bit", nullable: false),
                Remark = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),

                // Owned: 电子发票字段
                EInvoice_InvoiceCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                EInvoice_InvoiceNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                EInvoice_CheckCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                EInvoice_PdfUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                EInvoice_ViewUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                EInvoice_QrCodeUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                EInvoice_ElectronicIssuedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                EInvoice_BuyerTaxId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                EInvoice_SellerTaxId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Invoices", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "InvoiceItems",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProductCode = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                ProductName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Specification = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                Unit = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                Quantity = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                AmountExcludingTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TaxRate = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                AmountIncludingTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                SortOrder = table.Column<int>(type: "int", nullable: false),
                Remark = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InvoiceItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_InvoiceItems_Invoices_InvoiceId",
                    column: x => x.InvoiceId,
                    principalTable: "Invoices",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Invoices_Number",
            table: "Invoices",
            column: "Number",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Invoices_CustomerId",
            table: "Invoices",
            column: "CustomerId");

        migrationBuilder.CreateIndex(
            name: "IX_Invoices_SourceType_SourceId",
            table: "Invoices",
            columns: new[] { "SourceType", "SourceId" });

        migrationBuilder.CreateIndex(
            name: "IX_Invoices_Status",
            table: "Invoices",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_InvoiceItems_InvoiceId",
            table: "InvoiceItems",
            column: "InvoiceId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "InvoiceItems");

        migrationBuilder.DropTable(
            name: "Invoices");
    }
}

