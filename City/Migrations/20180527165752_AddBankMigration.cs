using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CyberCity.Migrations
{
    public partial class AddBankMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.CreateTable(
                name: "Credits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Client = table.Column<int>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    Summa = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MoneyContributions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Client = table.Column<int>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    Summa = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyContributions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Moneytransfers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Recipient = table.Column<int>(nullable: false),
                    Sender = table.Column<int>(nullable: false),
                    Summa = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moneytransfers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NeedOprerations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Money = table.Column<double>(nullable: false),
                    Recipient = table.Column<int>(nullable: false),
                    Sender = table.Column<int>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NeedOprerations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentUtilits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Client = table.Column<int>(nullable: false),
                    Summa = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentUtilits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Residents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Debt = table.Column<double>(nullable: false),
                    Home = table.Column<int>(nullable: false),
                    Login = table.Column<string>(nullable: false),
                    Money = table.Column<double>(nullable: false),
                    MoneyInCourse = table.Column<double>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Patronymic = table.Column<string>(nullable: false),
                    Surname = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Salarys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Client = table.Column<int>(nullable: false),
                    Summa = table.Column<double>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salarys", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Credits");

            migrationBuilder.DropTable(
                name: "MoneyContributions");

            migrationBuilder.DropTable(
                name: "Moneytransfers");

            migrationBuilder.DropTable(
                name: "NeedOprerations");

            migrationBuilder.DropTable(
                name: "PaymentUtilits");

            migrationBuilder.DropTable(
                name: "Residents");

            migrationBuilder.DropTable(
                name: "Salarys");

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    State = table.Column<string>(nullable: true),
                    Subject = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                });
        }
    }
}
