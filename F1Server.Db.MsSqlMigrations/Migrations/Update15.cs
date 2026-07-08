using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.MsSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update15 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Re-point telemetry rows of duplicate laps to the kept lap (the lap with telemetry data or the lowest id)
        migrationBuilder.Sql(@"UPDATE t
SET t.LapNumberId = k.KeepId
FROM CarTelemetries t
INNER JOIN Laps l ON l.Id = t.LapNumberId
INNER JOIN
(
    SELECT l2.SessionId, l2.ParticipantId, l2.LapNumber, COALESCE(MIN(CASE WHEN tc.TelemetryCount > 0 THEN l2.Id END), MIN(l2.Id)) AS KeepId
    FROM Laps l2
    LEFT JOIN (SELECT LapNumberId, COUNT(*) AS TelemetryCount FROM CarTelemetries GROUP BY LapNumberId) tc ON tc.LapNumberId = l2.Id
    GROUP BY l2.SessionId, l2.ParticipantId, l2.LapNumber
    HAVING COUNT(*) > 1
) k ON k.SessionId = l.SessionId AND k.ParticipantId = l.ParticipantId AND k.LapNumber = l.LapNumber
WHERE l.Id <> k.KeepId;");

        // Remove the duplicate laps that lost their telemetry rows in the previous statement
        migrationBuilder.Sql(@"DELETE l
FROM Laps l
INNER JOIN
(
    SELECT l2.SessionId, l2.ParticipantId, l2.LapNumber, COALESCE(MIN(CASE WHEN tc.TelemetryCount > 0 THEN l2.Id END), MIN(l2.Id)) AS KeepId
    FROM Laps l2
    LEFT JOIN (SELECT LapNumberId, COUNT(*) AS TelemetryCount FROM CarTelemetries GROUP BY LapNumberId) tc ON tc.LapNumberId = l2.Id
    GROUP BY l2.SessionId, l2.ParticipantId, l2.LapNumber
    HAVING COUNT(*) > 1
) k ON k.SessionId = l.SessionId AND k.ParticipantId = l.ParticipantId AND k.LapNumber = l.LapNumber
WHERE l.Id <> k.KeepId;");

        migrationBuilder.CreateIndex(name: "IX_Laps_SessionId_ParticipantId_LapNumber",
                                     table: "Laps",
                                     columns: new[] { "SessionId", "ParticipantId", "LapNumber" },
                                     unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_Laps_SessionId_ParticipantId_LapNumber",
                                   table: "Laps");
    }

    #endregion // Migration
}