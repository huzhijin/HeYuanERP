using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HeYuanERP.Api.Data.Migrations;

// P3 迁移：新增主数据/业务/库存/财务等实体表
public partial class P3_DomainEntities : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Accounts
        migrationBuilder.CreateTable(
            name: "Accounts",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                OwnerId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                TaxNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                Active = table.Column<bool>(type: "bit", nullable: false),
                LastEventDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Accounts", x => x.Id);
            });
        migrationBuilder.CreateIndex(
            name: "IX_Accounts_Code",
            table: "Accounts",
            column: "Code",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_Accounts_Name",
            table: "Accounts",
            column: "Name");

        // Contacts
        migrationBuilder.CreateTable(
            name: "Contacts",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                AccountId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                Mobile = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Contacts", x => x.Id);
                table.ForeignKey(
                    name: "FK_Contacts_Accounts_AccountId",
                    column: x => x.AccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });
        migrationBuilder.CreateIndex(
            name: "IX_Contacts_AccountId",
            table: "Contacts",
            column: "AccountId");
        migrationBuilder.CreateIndex(
            name: "IX_Contacts_AccountId_IsPrimary",
            table: "Contacts",
            columns: new[] { "AccountId", "IsPrimary" });

        // Products
        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Spec = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                DefaultPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                Active = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
            });
        migrationBuilder.CreateIndex(
            name: "IX_Products_Code",
            table: "Products",
            column: "Code",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_Products_Name",
            table: "Products",
            column: "Name");

        // Attachments
        migrationBuilder.CreateTable(
            name: "Attachments",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                RefType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                RefId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Size = table.Column<long>(type: "bigint", nullable: false),
                StorageUri = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Sha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UploadedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Attachments", x => x.Id);
            });
        migrationBuilder.CreateIndex(
            name: "IX_Attachments_RefType_RefId",
            table: "Attachments",
            columns: new[] { "RefType", "RefId" });

        // ReportSnapshots
        migrationBuilder.CreateTable(
            name: "ReportSnapshots",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                ParamsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                FileUri = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ReportSnapshots", x => x.Id);
            });
        migrationBuilder.CreateIndex(
            name: "IX_ReportSnapshots_Name_CreatedAt",
            table: "ReportSnapshots",
            columns: new[] { "Name", "CreatedAt" });

        // SalesOrders
        migrationBuilder.CreateTable(
            name: "SalesOrders",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                OrderNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                AccountId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SalesOrders", x => x.Id);
                table.ForeignKey(
                    name: "FK_SalesOrders_Accounts_AccountId",
                    column: x => x.AccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_SalesOrders_OrderNo",
            table: "SalesOrders",
            column: "OrderNo",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_SalesOrders_AccountId_OrderDate",
            table: "SalesOrders",
            columns: new[] { "AccountId", "OrderDate" });

        // SalesOrderLines
        migrationBuilder.CreateTable(
            name: "SalesOrderLines",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                OrderId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                ProductId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Qty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Discount = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                TaxRate = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SalesOrderLines", x => x.Id);
                table.ForeignKey(
                    name: "FK_SalesOrderLines_SalesOrders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "SalesOrders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_SalesOrderLines_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_SalesOrderLines_OrderId",
            table: "SalesOrderLines",
            column: "OrderId");
        migrationBuilder.CreateIndex(
            name: "IX_SalesOrderLines_ProductId",
            table: "SalesOrderLines",
            column: "ProductId");

        // Deliveries
        migrationBuilder.CreateTable(
            name: "Deliveries",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                DeliveryNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                OrderId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Deliveries", x => x.Id);
                table.ForeignKey(
                    name: "FK_Deliveries_SalesOrders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "SalesOrders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_Deliveries_DeliveryNo",
            table: "Deliveries",
            column: "DeliveryNo",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_Deliveries_OrderId_DeliveryDate",
            table: "Deliveries",
            columns: new[] { "OrderId", "DeliveryDate" });

        // DeliveryLines
        migrationBuilder.CreateTable(
            name: "DeliveryLines",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                DeliveryId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                OrderLineId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                ProductId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Qty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DeliveryLines", x => x.Id);
                table.ForeignKey(
                    name: "FK_DeliveryLines_Deliveries_DeliveryId",
                    column: x => x.DeliveryId,
                    principalTable: "Deliveries",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_DeliveryLines_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_DeliveryLines_SalesOrderLines_OrderLineId",
                    column: x => x.OrderLineId,
                    principalTable: "SalesOrderLines",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_DeliveryLines_DeliveryId",
            table: "DeliveryLines",
            column: "DeliveryId");
        migrationBuilder.CreateIndex(
            name: "IX_DeliveryLines_ProductId",
            table: "DeliveryLines",
            column: "ProductId");
        migrationBuilder.CreateIndex(
            name: "IX_DeliveryLines_OrderLineId",
            table: "DeliveryLines",
            column: "OrderLineId");

        // Returns
        migrationBuilder.CreateTable(
            name: "Returns",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                ReturnNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                OrderId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                SourceDeliveryId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Returns", x => x.Id);
                table.ForeignKey(
                    name: "FK_Returns_Deliveries_SourceDeliveryId",
                    column: x => x.SourceDeliveryId,
                    principalTable: "Deliveries",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Returns_SalesOrders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "SalesOrders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_Returns_ReturnNo",
            table: "Returns",
            column: "ReturnNo",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_Returns_OrderId_ReturnDate",
            table: "Returns",
            columns: new[] { "OrderId", "ReturnDate" });

        // ReturnLines
        migrationBuilder.CreateTable(
            name: "ReturnLines",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                ReturnId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                ProductId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Qty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ReturnLines", x => x.Id);
                table.ForeignKey(
                    name: "FK_ReturnLines_Returns_ReturnId",
                    column: x => x.ReturnId,
                    principalTable: "Returns",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ReturnLines_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_ReturnLines_ReturnId",
            table: "ReturnLines",
            column: "ReturnId");
        migrationBuilder.CreateIndex(
            name: "IX_ReturnLines_ProductId",
            table: "ReturnLines",
            column: "ProductId");

        // PurchaseOrders
        migrationBuilder.CreateTable(
            name: "PurchaseOrders",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                PoNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                VendorId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                PoDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                table.ForeignKey(
                    name: "FK_PurchaseOrders_Accounts_VendorId",
                    column: x => x.VendorId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_PurchaseOrders_PoNo",
            table: "PurchaseOrders",
            column: "PoNo",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_PurchaseOrders_VendorId_PoDate",
            table: "PurchaseOrders",
            columns: new[] { "VendorId", "PoDate" });

        // POLines
        migrationBuilder.CreateTable(
            name: "POLines",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                PoId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                ProductId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Qty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_POLines", x => x.Id);
                table.ForeignKey(
                    name: "FK_POLines_PurchaseOrders_PoId",
                    column: x => x.PoId,
                    principalTable: "PurchaseOrders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_POLines_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_POLines_PoId",
            table: "POLines",
            column: "PoId");
        migrationBuilder.CreateIndex(
            name: "IX_POLines_ProductId",
            table: "POLines",
            column: "ProductId");

        // POReceives
        migrationBuilder.CreateTable(
            name: "POReceives",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                PoId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                ReceiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_POReceives", x => x.Id);
                table.ForeignKey(
                    name: "FK_POReceives_PurchaseOrders_PoId",
                    column: x => x.PoId,
                    principalTable: "PurchaseOrders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_POReceives_PoId_ReceiveDate",
            table: "POReceives",
            columns: new[] { "PoId", "ReceiveDate" });

        // POReceiveLines
        migrationBuilder.CreateTable(
            name: "POReceiveLines",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                ReceiveId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                ProductId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Qty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                Whse = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                Loc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_POReceiveLines", x => x.Id);
                table.ForeignKey(
                    name: "FK_POReceiveLines_POReceives_ReceiveId",
                    column: x => x.ReceiveId,
                    principalTable: "POReceives",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_POReceiveLines_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_POReceiveLines_ReceiveId",
            table: "POReceiveLines",
            column: "ReceiveId");
        migrationBuilder.CreateIndex(
            name: "IX_POReceiveLines_ProductId",
            table: "POReceiveLines",
            column: "ProductId");

        // InventoryBalances
        migrationBuilder.CreateTable(
            name: "InventoryBalances",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                ProductId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Whse = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Loc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                OnHand = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                Reserved = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                Available = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InventoryBalances", x => x.Id);
                table.ForeignKey(
                    name: "FK_InventoryBalances_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_InventoryBalances_ProductId_Whse_Loc",
            table: "InventoryBalances",
            columns: new[] { "ProductId", "Whse", "Loc" },
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_InventoryBalances_ProductId",
            table: "InventoryBalances",
            column: "ProductId");

        // InventoryTxns
        migrationBuilder.CreateTable(
            name: "InventoryTxns",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                TxnCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                ProductId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Qty = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                Whse = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Loc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                TxnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                RefType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                RefId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_InventoryTxns", x => x.Id);
                table.ForeignKey(
                    name: "FK_InventoryTxns_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_InventoryTxns_ProductId_TxnDate",
            table: "InventoryTxns",
            columns: new[] { "ProductId", "TxnDate" });
        migrationBuilder.CreateIndex(
            name: "IX_InventoryTxns_TxnCode_TxnDate",
            table: "InventoryTxns",
            columns: new[] { "TxnCode", "TxnDate" });

        // Invoices
        migrationBuilder.CreateTable(
            name: "Invoices",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                InvoiceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                AccountId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                OrderId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                DeliveryId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TaxRate = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                AmountWithTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Invoices", x => x.Id);
                table.ForeignKey(
                    name: "FK_Invoices_Accounts_AccountId",
                    column: x => x.AccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Invoices_SalesOrders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "SalesOrders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Invoices_Deliveries_DeliveryId",
                    column: x => x.DeliveryId,
                    principalTable: "Deliveries",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_Invoices_InvoiceNo",
            table: "Invoices",
            column: "InvoiceNo",
            unique: true);
        migrationBuilder.CreateIndex(
            name: "IX_Invoices_AccountId_InvoiceDate",
            table: "Invoices",
            columns: new[] { "AccountId", "InvoiceDate" });
        migrationBuilder.CreateIndex(
            name: "IX_Invoices_OrderId",
            table: "Invoices",
            column: "OrderId");
        migrationBuilder.CreateIndex(
            name: "IX_Invoices_DeliveryId",
            table: "Invoices",
            column: "DeliveryId");

        // Payments
        migrationBuilder.CreateTable(
            name: "Payments",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                AccountId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Method = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                RefNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                VoucherNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Payments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Payments_Accounts_AccountId",
                    column: x => x.AccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_Payments_AccountId_Date",
            table: "Payments",
            columns: new[] { "AccountId", "Date" });

        // 索引的附加创建
        migrationBuilder.CreateIndex(
            name: "IX_Deliveries_OrderId",
            table: "Deliveries",
            column: "OrderId");

        // 外键依赖已在各表创建中定义
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Payments");
        migrationBuilder.DropTable(name: "Invoices");
        migrationBuilder.DropTable(name: "InventoryTxns");
        migrationBuilder.DropTable(name: "InventoryBalances");
        migrationBuilder.DropTable(name: "POReceiveLines");
        migrationBuilder.DropTable(name: "POReceives");
        migrationBuilder.DropTable(name: "POLines");
        migrationBuilder.DropTable(name: "PurchaseOrders");
        migrationBuilder.DropTable(name: "ReturnLines");
        migrationBuilder.DropTable(name: "Returns");
        migrationBuilder.DropTable(name: "DeliveryLines");
        migrationBuilder.DropTable(name: "Deliveries");
        migrationBuilder.DropTable(name: "SalesOrderLines");
        migrationBuilder.DropTable(name: "SalesOrders");
        migrationBuilder.DropTable(name: "ReportSnapshots");
        migrationBuilder.DropTable(name: "Attachments");
        migrationBuilder.DropTable(name: "Contacts");
        migrationBuilder.DropTable(name: "Products");
        migrationBuilder.DropTable(name: "Accounts");
    }
}
