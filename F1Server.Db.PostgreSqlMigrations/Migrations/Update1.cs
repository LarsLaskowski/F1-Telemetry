using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.PostgreSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update1 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_CarTelemetries_Participants_ParticipantId",
                                        table: "CarTelemetries");

        migrationBuilder.DropForeignKey(name: "FK_Laps_Participants_ParticipantId",
                                        table: "Laps");

        migrationBuilder.DropIndex(name: "IX_CarTelemetries_ParticipantId",
                                   table: "CarTelemetries");

        migrationBuilder.DropColumn(name: "ParticipantId",
                                    table: "CarTelemetries");

        migrationBuilder.AddForeignKey(name: "FK_Laps_Participants_ParticipantId",
                                       table: "Laps",
                                       column: "ParticipantId",
                                       principalTable: "Participants",
                                       principalColumn: "Id",
                                       onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_Laps_Participants_ParticipantId",
                                        table: "Laps");

        migrationBuilder.AddColumn<long>(name: "ParticipantId",
                                         table: "CarTelemetries",
                                         type: "bigint",
                                         nullable: false,
                                         defaultValue: 0L);

        migrationBuilder.CreateIndex(name: "IX_CarTelemetries_ParticipantId",
                                     table: "CarTelemetries",
                                     column: "ParticipantId");

        migrationBuilder.AddForeignKey(name: "FK_CarTelemetries_Participants_ParticipantId",
                                       table: "CarTelemetries",
                                       column: "ParticipantId",
                                       principalTable: "Participants",
                                       principalColumn: "Id",
                                       onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(name: "FK_Laps_Participants_ParticipantId",
                                       table: "Laps",
                                       column: "ParticipantId",
                                       principalTable: "Participants",
                                       principalColumn: "Id",
                                       onDelete: ReferentialAction.Cascade);
    }

    #endregion // Migration
}