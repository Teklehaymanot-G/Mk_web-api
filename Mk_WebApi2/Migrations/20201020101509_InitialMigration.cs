using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mk_WebApi2.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "maekel",
                columns: table => new
                {
                    maekel_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    location = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(nullable: false),
                    created_by = table.Column<int>(nullable: false),
                    created_on = table.Column<DateTime>(nullable: false),
                    modified_by = table.Column<int>(nullable: false),
                    modified_on = table.Column<DateTime>(nullable: false),
                    trash = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maekel", x => x.maekel_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Maekel");
        }
    }
}
