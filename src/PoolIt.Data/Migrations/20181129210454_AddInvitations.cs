using Microsoft.EntityFrameworkCore.Migrations;

namespace PoolIt.Data.Migrations
{
    public partial class AddInvitations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RideId = table.Column<string>(nullable: false),
                    Key = table.Column<string>(maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                    table.UniqueConstraint("AK_Invitations_Key", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Invitations_Rides_RideId",
                        column: x => x.RideId,
                        principalTable: "Rides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_RideId",
                table: "Invitations",
                column: "RideId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invitations");
        }
    }
}
