using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeBankingMindHub.Migrations
{
    public partial class addFixedCardEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CardHlder",
                table: "Cards",
                newName: "CardHolder");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CardHolder",
                table: "Cards",
                newName: "CardHlder");
        }
    }
}
