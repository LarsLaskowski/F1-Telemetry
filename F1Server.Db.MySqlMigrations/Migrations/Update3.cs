using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.MySqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update3 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name: "CarTelemetries",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                                           PacketNumber = table.Column<int>(type: "int", nullable: false),
                                                           ParticipantId = table.Column<long>(type: "bigint", nullable: false),
                                                           LapNumberId = table.Column<long>(type: "bigint", nullable: false),
                                                           Throttle = table.Column<float>(type: "float", nullable: false),
                                                           Brake = table.Column<float>(type: "float", nullable: false),
                                                           Clutch = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           Steer = table.Column<float>(type: "float", nullable: false),
                                                           EngineRPM = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           EngineTemperature = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           Gear = table.Column<short>(type: "smallint", nullable: false),
                                                           Speed = table.Column<int>(type: "int", nullable: false),
                                                           IsDRS = table.Column<int>(type: "int", nullable: false),
                                                           RevLightsIndicator = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           BrakesTempFrontLeft = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           BrakesTempFrontRight = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           BrakesTempRearLeft = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           BrakesTempRearRight = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           TyresSurfaceTempFrontLeft = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           TyresSurfaceTempFrontRight = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           TyresSurfaceTempRearLeft = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           TyresSurfaceTempRearRight = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           TyresInnerTempFrontLeft = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           TyresInnerTempFrontRight = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           TyresInnerTempRearLeft = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           TyresInnerTempRearRight = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           TyresPressureFrontLeft = table.Column<float>(type: "float", nullable: false),
                                                           TyresPressureFrontRight = table.Column<float>(type: "float", nullable: false),
                                                           TyresPressureRearLeft = table.Column<float>(type: "float", nullable: false),
                                                           TyresPressureRearRight = table.Column<float>(type: "float", nullable: false)
                                                       },
                                     constraints: table =>
                                                  {
                                                      table.PrimaryKey("PK_CarTelemetries", x => x.Id);
                                                      table.ForeignKey(name: "FK_CarTelemetries_Laps_LapNumberId",
                                                                       column: x => x.LapNumberId,
                                                                       principalTable: "Laps",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Cascade);
                                                      table.ForeignKey(name: "FK_CarTelemetries_Participants_ParticipantId",
                                                                       column: x => x.ParticipantId,
                                                                       principalTable: "Participants",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Cascade);
                                                  })
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(name: "IX_CarTelemetries_LapNumberId",
                                     table: "CarTelemetries",
                                     column: "LapNumberId");

        migrationBuilder.CreateIndex(name: "IX_CarTelemetries_PacketNumber",
                                     table: "CarTelemetries",
                                     column: "PacketNumber");

        migrationBuilder.CreateIndex(name: "IX_CarTelemetries_ParticipantId",
                                     table: "CarTelemetries",
                                     column: "ParticipantId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CarTelemetries");
    }

    #endregion // Migration
}