using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HeYuanERP.Api.Data.Migrations;

// P4 迁移：新增 Accounts 的扩展实体（共享与拜访）
public partial class P4_AccountExtras : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // AccountShares
        migrationBuilder.CreateTable(
            name: "AccountShares",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                AccountId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                TargetUserId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                Permission = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                ExpireAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AccountShares", x => x.Id);
                table.ForeignKey(
                    name: "FK_AccountShares_Accounts_AccountId",
                    column: x => x.AccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AccountShares_Users_TargetUserId",
                    column: x => x.TargetUserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });
        migrationBuilder.CreateIndex(
            name: "IX_AccountShares_AccountId_TargetUserId",
            table: "AccountShares",
            columns: new[] { "AccountId", "TargetUserId" },
            unique: true);

        // AccountVisits
        migrationBuilder.CreateTable(
            name: "AccountVisits",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                AccountId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                ContactId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                VisitorId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                Result = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                NextActionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AccountVisits", x => x.Id);
                table.ForeignKey(
                    name: "FK_AccountVisits_Accounts_AccountId",
                    column: x => x.AccountId,
                    principalTable: "Accounts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AccountVisits_Contacts_ContactId",
                    column: x => x.ContactId,
                    principalTable: "Contacts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_AccountVisits_Users_VisitorId",
                    column: x => x.VisitorId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });
        migrationBuilder.CreateIndex(
            name: "IX_AccountVisits_AccountId_VisitDate",
            table: "AccountVisits",
            columns: new[] { "AccountId", "VisitDate" });
        migrationBuilder.CreateIndex(
            name: "IX_AccountVisits_VisitorId_VisitDate",
            table: "AccountVisits",
            columns: new[] { "VisitorId", "VisitDate" });
        migrationBuilder.CreateIndex(
            name: "IX_AccountVisits_AccountId",
            table: "AccountVisits",
            column: "AccountId");
        migrationBuilder.CreateIndex(
            name: "IX_AccountVisits_ContactId",
            table: "AccountVisits",
            column: "ContactId");
        migrationBuilder.CreateIndex(
            name: "IX_AccountVisits_VisitorId",
            table: "AccountVisits",
            column: "VisitorId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "AccountShares");
        migrationBuilder.DropTable(name: "AccountVisits");
    }
}

