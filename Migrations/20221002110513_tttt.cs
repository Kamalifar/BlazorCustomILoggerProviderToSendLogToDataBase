using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SendBlazorLoggerToDataBase.Migrations
{
    public partial class tttt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExceptionMessage",
                table: "DbLogs",
                newName: "Message");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "DbLogs",
                newName: "ExceptionMessage");
        }
    }
}
