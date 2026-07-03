using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.MySqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update13 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name: "Championships",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                                           GameVersionId = table.Column<long>(type: "bigint", nullable: false),
                                                           Number = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           IsFinished = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           Mode = table.Column<int>(type: "int", nullable: false)
                                                       },
                                     constraints: table =>
                                                  {
                                                      table.PrimaryKey("PK_Championships", x => x.Id);
                                                      table.ForeignKey(name: "FK_Championships_GameVersions_GameVersionId",
                                                                       column: x => x.GameVersionId,
                                                                       principalTable: "GameVersions",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Cascade);
                                                  })
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(name: "ChampionshipTracks",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                                           ChampionshipId = table.Column<long>(type: "bigint", nullable: false),
                                                           TrackId = table.Column<long>(type: "bigint", nullable: false),
                                                           QualifyingSessionId = table.Column<long>(type: "bigint", nullable: true),
                                                           SprintQualifyingSessionId = table.Column<long>(type: "bigint", nullable: true),
                                                           SprintSessionId = table.Column<long>(type: "bigint", nullable: true),
                                                           RaceSessionId = table.Column<long>(type: "bigint", nullable: true)
                                                       },
                                     constraints: table =>
                                                  {
                                                      table.PrimaryKey("PK_ChampionshipTracks", x => x.Id);
                                                      table.ForeignKey(name: "FK_ChampionshipTracks_Championships_ChampionshipId",
                                                                       column: x => x.ChampionshipId,
                                                                       principalTable: "Championships",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Restrict);
                                                      table.ForeignKey(name: "FK_ChampionshipTracks_Sessions_QualifyingSessionId",
                                                                       column: x => x.QualifyingSessionId,
                                                                       principalTable: "Sessions",
                                                                       principalColumn: "Id");
                                                      table.ForeignKey(name: "FK_ChampionshipTracks_Sessions_RaceSessionId",
                                                                       column: x => x.RaceSessionId,
                                                                       principalTable: "Sessions",
                                                                       principalColumn: "Id");
                                                      table.ForeignKey(name: "FK_ChampionshipTracks_Sessions_SprintQualifyingSessionId",
                                                                       column: x => x.SprintQualifyingSessionId,
                                                                       principalTable: "Sessions",
                                                                       principalColumn: "Id");
                                                      table.ForeignKey(name: "FK_ChampionshipTracks_Sessions_SprintSessionId",
                                                                       column: x => x.SprintSessionId,
                                                                       principalTable: "Sessions",
                                                                       principalColumn: "Id");
                                                      table.ForeignKey(name: "FK_ChampionshipTracks_Tracks_TrackId",
                                                                       column: x => x.TrackId,
                                                                       principalTable: "Tracks",
                                                                       principalColumn: "Id");
                                                  })
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(name: "IX_Championships_GameVersionId",
                                     table: "Championships",
                                     column: "GameVersionId");

        migrationBuilder.CreateIndex(name: "IX_ChampionshipTracks_ChampionshipId",
                                     table: "ChampionshipTracks",
                                     column: "ChampionshipId");

        migrationBuilder.CreateIndex(name: "IX_ChampionshipTracks_QualifyingSessionId",
                                     table: "ChampionshipTracks",
                                     column: "QualifyingSessionId");

        migrationBuilder.CreateIndex(name: "IX_ChampionshipTracks_RaceSessionId",
                                     table: "ChampionshipTracks",
                                     column: "RaceSessionId");

        migrationBuilder.CreateIndex(name: "IX_ChampionshipTracks_SprintQualifyingSessionId",
                                     table: "ChampionshipTracks",
                                     column: "SprintQualifyingSessionId");

        migrationBuilder.CreateIndex(name: "IX_ChampionshipTracks_SprintSessionId",
                                     table: "ChampionshipTracks",
                                     column: "SprintSessionId");

        migrationBuilder.CreateIndex(name: "IX_ChampionshipTracks_TrackId",
                                     table: "ChampionshipTracks",
                                     column: "TrackId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ChampionshipTracks");

        migrationBuilder.DropTable(name: "Championships");
    }

    #endregion // Migration
}