using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.PostgreSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update11 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterTable(name: "Tracks",
                                    comment: "Tracks table containing track information from the game.");

        migrationBuilder.AlterTable(name: "Teams",
                                    comment: "Entity of all teams, depends on the game version");

        migrationBuilder.AlterTable(name: "Sessions",
                                    comment: "Session table containing information about game sessions, including track, game version, and session details.");

        migrationBuilder.AlterTable(name: "SessionAttributes",
                                    comment: "Attributes of a session, like weather, assists and so on.");

        migrationBuilder.AlterTable(name: "Participants",
                                    comment: "Table with participant data, depends on the game version");

        migrationBuilder.AlterTable(name: "Nationalities",
                                    comment: "Entity of all nationalities");

        migrationBuilder.AlterTable(name: "Laps",
                                    comment: "Table with all laps of participants in the session, depends on the game version");

        migrationBuilder.AlterTable(name: "GameVersions",
                                    comment: "Game versions table. Contains information about game versions used in sessions.");

        migrationBuilder.AlterTable(name: "FinalClassifications",
                                    comment: "Entity of final classification of session, contains results of participants after the session ends.");

        migrationBuilder.AlterTable(name: "Drivers",
                                    comment: "Entity of all drivers, depends on the game version.");

        migrationBuilder.AlterTable(name: "ChampionshipTracks",
                                    comment: "ChampionshipTracks table containing information about tracks in a championship season, including associated sessions.");

        migrationBuilder.AlterTable(name: "Championships",
                                    comment: "Championships table containing information about championship seasons, including associated tracks and points.");

        migrationBuilder.AlterTable(name: "ChampionshipPoints",
                                    comment: "Entity of points in a championship season.");

        migrationBuilder.AlterTable(name: "CarTelemetries",
                                    comment: "Car telemetry data for each lap in a session.");

        migrationBuilder.CreateIndex(name: "IX_Tracks_TrackNumber",
                                     table: "Tracks",
                                     column: "TrackNumber");

        migrationBuilder.CreateIndex(name: "IX_Teams_TeamGameId",
                                     table: "Teams",
                                     column: "TeamGameId");

        migrationBuilder.CreateIndex(name: "IX_Nationalities_NationalityGameId",
                                     table: "Nationalities",
                                     column: "NationalityGameId");

        migrationBuilder.CreateIndex(name: "IX_Laps_LapNumber",
                                     table: "Laps",
                                     column: "LapNumber");

        migrationBuilder.CreateIndex(name: "IX_Laps_SessionId",
                                     table: "Laps",
                                     column: "SessionId");

        migrationBuilder.CreateIndex(name: "IX_GameVersions_Version",
                                     table: "GameVersions",
                                     column: "Version");

        migrationBuilder.CreateIndex(name: "IX_Drivers_DriverGameId",
                                     table: "Drivers",
                                     column: "DriverGameId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_Tracks_TrackNumber",
                                   table: "Tracks");

        migrationBuilder.DropIndex(name: "IX_Teams_TeamGameId",
                                   table: "Teams");

        migrationBuilder.DropIndex(name: "IX_Nationalities_NationalityGameId",
                                   table: "Nationalities");

        migrationBuilder.DropIndex(name: "IX_Laps_LapNumber",
                                   table: "Laps");

        migrationBuilder.DropIndex(name: "IX_Laps_SessionId",
                                   table: "Laps");

        migrationBuilder.DropIndex(name: "IX_GameVersions_Version",
                                   table: "GameVersions");

        migrationBuilder.DropIndex(name: "IX_Drivers_DriverGameId",
                                   table: "Drivers");

        migrationBuilder.AlterTable(name: "Tracks",
                                    oldComment: "Tracks table containing track information from the game.");

        migrationBuilder.AlterTable(name: "Teams",
                                    oldComment: "Entity of all teams, depends on the game version");

        migrationBuilder.AlterTable(name: "Sessions",
                                    oldComment: "Session table containing information about game sessions, including track, game version, and session details.");

        migrationBuilder.AlterTable(name: "SessionAttributes",
                                    oldComment: "Attributes of a session, like weather, assists and so on.");

        migrationBuilder.AlterTable(name: "Participants",
                                    oldComment: "Table with participant data, depends on the game version");

        migrationBuilder.AlterTable(name: "Nationalities",
                                    oldComment: "Entity of all nationalities");

        migrationBuilder.AlterTable(name: "Laps",
                                    oldComment: "Table with all laps of participants in the session, depends on the game version");

        migrationBuilder.AlterTable(name: "GameVersions",
                                    oldComment: "Game versions table. Contains information about game versions used in sessions.");

        migrationBuilder.AlterTable(name: "FinalClassifications",
                                    oldComment: "Entity of final classification of session, contains results of participants after the session ends.");

        migrationBuilder.AlterTable(name: "Drivers",
                                    oldComment: "Entity of all drivers, depends on the game version.");

        migrationBuilder.AlterTable(name: "ChampionshipTracks",
                                    oldComment: "ChampionshipTracks table containing information about tracks in a championship season, including associated sessions.");

        migrationBuilder.AlterTable(name: "Championships",
                                    oldComment: "Championships table containing information about championship seasons, including associated tracks and points.");

        migrationBuilder.AlterTable(name: "ChampionshipPoints",
                                    oldComment: "Entity of points in a championship season.");

        migrationBuilder.AlterTable(name: "CarTelemetries",
                                    oldComment: "Car telemetry data for each lap in a session.");
    }

    #endregion // Migration
}