using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.MySqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update1 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(name: "CreationTimestamp",
                                               table: "Sessions",
                                               type: "datetime(6)",
                                               nullable: false,
                                               oldClrType: typeof(DateTime),
                                               oldType: "datetime");

        migrationBuilder.AlterColumn<DateTime>(name: "LastUsed",
                                               table: "GameVersions",
                                               type: "datetime(6)",
                                               nullable: true,
                                               oldClrType: typeof(DateTime),
                                               oldType: "datetime",
                                               oldNullable: true);

        migrationBuilder.CreateTable(name: "FinalClassifications",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                                           SessionId = table.Column<long>(type: "bigint", nullable: false),
                                                           ParticipantId = table.Column<long>(type: "bigint", nullable: false),
                                                           GridPosition = table.Column<int>(type: "int", nullable: false),
                                                           FinishPosition = table.Column<int>(type: "int", nullable: false),
                                                           LapsDriven = table.Column<int>(type: "int", nullable: false),
                                                           PitStops = table.Column<int>(type: "int", nullable: false),
                                                           ResultStatus = table.Column<int>(type: "int", nullable: false),
                                                           FastestLapTime = table.Column<uint>(type: "int unsigned", nullable: false),
                                                           TotalRaceTime = table.Column<double>(type: "double", nullable: false),
                                                           PenaltiesTime = table.Column<uint>(type: "int unsigned", nullable: false),
                                                           NumberOfPenalties = table.Column<uint>(type: "int unsigned", nullable: false)
                                                       },
                                     constraints: table =>
                                                  {
                                                      table.PrimaryKey("PK_FinalClassifications", x => x.Id);
                                                      table.ForeignKey(name: "FK_FinalClassifications_Participants_ParticipantId",
                                                                       column: x => x.ParticipantId,
                                                                       principalTable: "Participants",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Cascade);
                                                      table.ForeignKey(name: "FK_FinalClassifications_Sessions_SessionId",
                                                                       column: x => x.SessionId,
                                                                       principalTable: "Sessions",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Cascade);
                                                  })
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(name: "IX_FinalClassifications_ParticipantId",
                                     table: "FinalClassifications",
                                     column: "ParticipantId");

        migrationBuilder.CreateIndex(name: "IX_FinalClassifications_SessionId",
                                     table: "FinalClassifications",
                                     column: "SessionId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "FinalClassifications");

        migrationBuilder.AlterColumn<DateTime>(name: "CreationTimestamp",
                                               table: "Sessions",
                                               type: "datetime",
                                               nullable: false,
                                               oldClrType: typeof(DateTime),
                                               oldType: "datetime(6)");

        migrationBuilder.AlterColumn<DateTime>(name: "LastUsed",
                                               table: "GameVersions",
                                               type: "datetime",
                                               nullable: true,
                                               oldClrType: typeof(DateTime),
                                               oldType: "datetime(6)",
                                               oldNullable: true);
    }

    #endregion // Migration
}