using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.PostgreSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update12 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(name: "IX_Sessions_SessionId",
                                     table: "Sessions",
                                     column: "SessionId");

        migrationBuilder.CreateIndex(name: "IX_Sessions_TrackId_GameVersionId",
                                     table: "Sessions",
                                     columns: new[] { "TrackId", "GameVersionId" });

        migrationBuilder.CreateIndex(name: "IX_Participants_SessionId_DriverId",
                                     table: "Participants",
                                     columns: new[] { "SessionId", "DriverId" });

        migrationBuilder.CreateIndex(name: "IX_Participants_SessionId_TeamId",
                                     table: "Participants",
                                     columns: new[] { "SessionId", "TeamId" });

        migrationBuilder.CreateIndex(name: "IX_Laps_ParticipantId_LapNumber",
                                     table: "Laps",
                                     columns: new[] { "ParticipantId", "LapNumber" });

        migrationBuilder.CreateIndex(name: "IX_Laps_ParticipantId_SessionId",
                                     table: "Laps",
                                     columns: new[] { "ParticipantId", "SessionId" });

        migrationBuilder.CreateIndex(name: "IX_Laps_SessionId_IsInvalidLapTime",
                                     table: "Laps",
                                     columns: new[] { "SessionId", "IsInvalidLapTime" });

        migrationBuilder.CreateIndex(name: "IX_Laps_SessionId_LapNumber",
                                     table: "Laps",
                                     columns: new[] { "SessionId", "LapNumber" });

        migrationBuilder.CreateIndex(name: "IX_FinalClassifications_SessionId_ParticipantId",
                                     table: "FinalClassifications",
                                     columns: new[] { "SessionId", "ParticipantId" });

        migrationBuilder.CreateIndex(name: "IX_ChampionshipTracks_ChampionshipId_TrackId",
                                     table: "ChampionshipTracks",
                                     columns: new[] { "ChampionshipId", "TrackId" });

        migrationBuilder.CreateIndex(name: "IX_Championships_GameVersionId_IsFinished",
                                     table: "Championships",
                                     columns: new[] { "GameVersionId", "IsFinished" });

        migrationBuilder.CreateIndex(name: "IX_Championships_IsFinished",
                                     table: "Championships",
                                     column: "IsFinished");

        migrationBuilder.CreateIndex(name: "IX_Championships_Mode",
                                     table: "Championships",
                                     column: "Mode");

        migrationBuilder.CreateIndex(name: "IX_Championships_Number",
                                     table: "Championships",
                                     column: "Number");

        migrationBuilder.CreateIndex(name: "IX_ChampionshipPoints_ChampionshipId_DriverId",
                                     table: "ChampionshipPoints",
                                     columns: new[] { "ChampionshipId", "DriverId" });

        migrationBuilder.CreateIndex(name: "IX_ChampionshipPoints_ChampionshipId_TrackId",
                                     table: "ChampionshipPoints",
                                     columns: new[] { "ChampionshipId", "TrackId" });

        migrationBuilder.CreateIndex(name: "IX_ChampionshipPoints_ChampionshipId_TrackId_DriverId",
                                     table: "ChampionshipPoints",
                                     columns: new[] { "ChampionshipId", "TrackId", "DriverId" });

        migrationBuilder.CreateIndex(name: "IX_ChampionshipPoints_TrackId_DriverId",
                                     table: "ChampionshipPoints",
                                     columns: new[] { "TrackId", "DriverId" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_Sessions_SessionId",
                                   table: "Sessions");

        migrationBuilder.DropIndex(name: "IX_Sessions_TrackId_GameVersionId",
                                   table: "Sessions");

        migrationBuilder.DropIndex(name: "IX_Participants_SessionId_DriverId",
                                   table: "Participants");

        migrationBuilder.DropIndex(name: "IX_Participants_SessionId_TeamId",
                                   table: "Participants");

        migrationBuilder.DropIndex(name: "IX_Laps_ParticipantId_LapNumber",
                                   table: "Laps");

        migrationBuilder.DropIndex(name: "IX_Laps_ParticipantId_SessionId",
                                   table: "Laps");

        migrationBuilder.DropIndex(name: "IX_Laps_SessionId_IsInvalidLapTime",
                                   table: "Laps");

        migrationBuilder.DropIndex(name: "IX_Laps_SessionId_LapNumber",
                                   table: "Laps");

        migrationBuilder.DropIndex(name: "IX_FinalClassifications_SessionId_ParticipantId",
                                   table: "FinalClassifications");

        migrationBuilder.DropIndex(name: "IX_ChampionshipTracks_ChampionshipId_TrackId",
                                   table: "ChampionshipTracks");

        migrationBuilder.DropIndex(name: "IX_Championships_GameVersionId_IsFinished",
                                   table: "Championships");

        migrationBuilder.DropIndex(name: "IX_Championships_IsFinished",
                                   table: "Championships");

        migrationBuilder.DropIndex(name: "IX_Championships_Mode",
                                   table: "Championships");

        migrationBuilder.DropIndex(name: "IX_Championships_Number",
                                   table: "Championships");

        migrationBuilder.DropIndex(name: "IX_ChampionshipPoints_ChampionshipId_DriverId",
                                   table: "ChampionshipPoints");

        migrationBuilder.DropIndex(name: "IX_ChampionshipPoints_ChampionshipId_TrackId",
                                   table: "ChampionshipPoints");

        migrationBuilder.DropIndex(name: "IX_ChampionshipPoints_ChampionshipId_TrackId_DriverId",
                                   table: "ChampionshipPoints");

        migrationBuilder.DropIndex(name: "IX_ChampionshipPoints_TrackId_DriverId",
                                   table: "ChampionshipPoints");
    }

    #endregion // Migration
}