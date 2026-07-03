using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace F1Server.Db.PostgreSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update4 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<decimal>(name: "SessionId",
                                              table: "Sessions",
                                              type: "numeric(20,0)",
                                              nullable: false,
                                              oldClrType: typeof(decimal),
                                              oldType: "numeric(20)");

        migrationBuilder.CreateTable(name: "SessionAttributes",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                                                           SessionId = table.Column<long>(type: "bigint", nullable: false),
                                                           FlashbacksUsed = table.Column<int>(type: "integer", nullable: false),
                                                           IsExtendedSession = table.Column<int>(type: "integer", nullable: false),
                                                           SteeringAssistFirst = table.Column<int>(type: "integer", nullable: false),
                                                           SteeringAssistLast = table.Column<int>(type: "integer", nullable: false),
                                                           SteeringAssistChanged = table.Column<int>(type: "integer", nullable: false),
                                                           BrakingAssistFirst = table.Column<int>(type: "integer", nullable: false),
                                                           BrakingAssistLast = table.Column<int>(type: "integer", nullable: false),
                                                           BrakingAssistChanged = table.Column<int>(type: "integer", nullable: false),
                                                           GearBoxAssistFirst = table.Column<int>(type: "integer", nullable: false),
                                                           GearBoxAssistLast = table.Column<int>(type: "integer", nullable: false),
                                                           GearBoxAssistChanged = table.Column<int>(type: "integer", nullable: false),
                                                           GameMode = table.Column<int>(type: "integer", nullable: false),
                                                           RuleSet = table.Column<int>(type: "integer", nullable: false),
                                                           WeatherStart = table.Column<int>(type: "integer", nullable: false),
                                                           WeatherEnd = table.Column<int>(type: "integer", nullable: false),
                                                           VirtualSafetyCarStages = table.Column<long>(type: "bigint", nullable: false),
                                                           SafetyCarStages = table.Column<long>(type: "bigint", nullable: false),
                                                           RedFlags = table.Column<long>(type: "bigint", nullable: false)
                                                       },
                                     constraints: table =>
                                                  {
                                                      table.PrimaryKey("PK_SessionAttributes", x => x.Id);
                                                      table.ForeignKey(name: "FK_SessionAttributes_Sessions_SessionId",
                                                                       column: x => x.SessionId,
                                                                       principalTable: "Sessions",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Cascade);
                                                  });

        migrationBuilder.CreateIndex(name: "IX_SessionAttributes_SessionId",
                                     table: "SessionAttributes",
                                     column: "SessionId");

        // Move data from session table to sessionattributes table
        migrationBuilder.Sql("INSERT INTO \"SessionAttributes\" (\"SessionId\", \"FlashbacksUsed\", \"IsExtendedSession\", \"SteeringAssistFirst\", \"SteeringAssistLast\", \"SteeringAssistChanged\", \"BrakingAssistFirst\", \"BrakingAssistLast\", \"BrakingAssistChanged\", \"GearBoxAssistFirst\", \"GearBoxAssistLast\", \"GearBoxAssistChanged\", \"GameMode\", \"RuleSet\", \"WeatherStart\", \"WeatherEnd\", \"VirtualSafetyCarStages\", \"SafetyCarStages\", \"RedFlags\")\r\n(SELECT \"Id\", \"FlashbacksUsed\", \"IsExtendedSession\", \"SteeringAssistFirst\", \"SteeringAssistLast\", \"SteeringAssistChanged\", \"BrakingAssistFirst\", \"BrakingAssistLast\", \"BrakingAssistChanged\", \"GearBoxAssistFirst\", \"GearBoxAssistLast\", \"GearBoxAssistChanged\", \"GameMode\", \"RuleSet\", \"WeatherStart\", \"WeatherEnd\", 0, 0, 0 FROM \"Sessions\");");

        migrationBuilder.DropColumn(name: "BrakingAssistChanged",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "BrakingAssistFirst",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "BrakingAssistLast",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "FlashbacksUsed",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "GameMode",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "GearBoxAssistChanged",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "GearBoxAssistFirst",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "GearBoxAssistLast",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "IsExtendedSession",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "RuleSet",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "SteeringAssistChanged",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "SteeringAssistFirst",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "SteeringAssistLast",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "WeatherEnd",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "WeatherStart",
                                    table: "Sessions");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "SessionAttributes");

        migrationBuilder.AlterColumn<decimal>(name: "SessionId",
                                              table: "Sessions",
                                              type: "numeric(20)",
                                              nullable: false,
                                              oldClrType: typeof(decimal),
                                              oldType: "numeric(20,0)");

        migrationBuilder.AddColumn<int>(name: "BrakingAssistChanged",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "BrakingAssistFirst",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "BrakingAssistLast",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "FlashbacksUsed",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "GameMode",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "GearBoxAssistChanged",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "GearBoxAssistFirst",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "GearBoxAssistLast",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "IsExtendedSession",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "RuleSet",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "SteeringAssistChanged",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "SteeringAssistFirst",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "SteeringAssistLast",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "WeatherEnd",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "WeatherStart",
                                        table: "Sessions",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);
    }

    #endregion // Migration
}