using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.PostgreSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update6 : Migration
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

        migrationBuilder.AddColumn<int>(name: "IsInvalidLapTime",
                                        table: "Laps",
                                        type: "integer",
                                        nullable: false,
                                        defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "IsInvalidLapTime",
                                    table: "Laps");

        migrationBuilder.AlterColumn<decimal>(name: "SessionId",
                                              table: "Sessions",
                                              type: "numeric(20)",
                                              nullable: false,
                                              oldClrType: typeof(decimal),
                                              oldType: "numeric(20,0)");
    }

    #endregion // Migration
}