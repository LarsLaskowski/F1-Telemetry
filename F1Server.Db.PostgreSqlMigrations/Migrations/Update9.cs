using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace F1Server.Db.PostgreSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update9 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name: "ChampionshipPoints",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                                                           ChampionshipId = table.Column<long>(type: "bigint", nullable: false),
                                                           TrackId = table.Column<long>(type: "bigint", nullable: false),
                                                           DriverId = table.Column<long>(type: "bigint", nullable: false),
                                                           RacePoints = table.Column<int>(type: "integer", nullable: false),
                                                           SprintRacePoints = table.Column<int>(type: "integer", nullable: false),
                                                           AdditionalPoints = table.Column<int>(type: "integer", nullable: false)
                                                       },
                                     constraints: table =>
                                                  {
                                                      table.PrimaryKey("PK_ChampionshipPoints", x => x.Id);
                                                      table.ForeignKey(name: "FK_ChampionshipPoints_Championships_ChampionshipId",
                                                                       column: x => x.ChampionshipId,
                                                                       principalTable: "Championships",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Restrict);
                                                      table.ForeignKey(name: "FK_ChampionshipPoints_Drivers_DriverId",
                                                                       column: x => x.DriverId,
                                                                       principalTable: "Drivers",
                                                                       principalColumn: "Id");
                                                      table.ForeignKey(name: "FK_ChampionshipPoints_Tracks_TrackId",
                                                                       column: x => x.TrackId,
                                                                       principalTable: "Tracks",
                                                                       principalColumn: "Id");
                                                  });

        migrationBuilder.CreateIndex(name: "IX_ChampionshipPoints_ChampionshipId",
                                     table: "ChampionshipPoints",
                                     column: "ChampionshipId");

        migrationBuilder.CreateIndex(name: "IX_ChampionshipPoints_DriverId",
                                     table: "ChampionshipPoints",
                                     column: "DriverId");

        migrationBuilder.CreateIndex(name: "IX_ChampionshipPoints_TrackId",
                                     table: "ChampionshipPoints",
                                     column: "TrackId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ChampionshipPoints");
    }

    #endregion // Migration
}