using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AitukServer.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkSheldure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShopContacts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Contact = table.Column<string>(type: "text", nullable: false),
                    ContactTypeId = table.Column<int>(type: "integer", nullable: false),
                    ShopId = table.Column<long>(type: "bigint", nullable: false),
                    AShopId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopContacts_Shops_AShopId",
                        column: x => x.AShopId,
                        principalTable: "Shops",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkSheldures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShopId = table.Column<long>(type: "bigint", nullable: false),
                    Monday_StartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Monday_EndTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Monday_IsWorkingDay = table.Column<bool>(type: "boolean", nullable: true),
                    Tuesday_StartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Tuesday_EndTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Tuesday_IsWorkingDay = table.Column<bool>(type: "boolean", nullable: true),
                    Wednesday_StartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Wednesday_EndTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Wednesday_IsWorkingDay = table.Column<bool>(type: "boolean", nullable: true),
                    Thursday_StartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Thursday_EndTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Thursday_IsWorkingDay = table.Column<bool>(type: "boolean", nullable: true),
                    Friday_StartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Friday_EndTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Friday_IsWorkingDay = table.Column<bool>(type: "boolean", nullable: true),
                    Saturday_StartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Saturday_EndTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Saturday_IsWorkingDay = table.Column<bool>(type: "boolean", nullable: true),
                    Sunday_StartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Sunday_EndTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Sunday_IsWorkingDay = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSheldures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkSheldures_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopContacts_AShopId",
                table: "ShopContacts",
                column: "AShopId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSheldures_ShopId",
                table: "WorkSheldures",
                column: "ShopId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactTypes");

            migrationBuilder.DropTable(
                name: "ShopContacts");

            migrationBuilder.DropTable(
                name: "WorkSheldures");
        }
    }
}
