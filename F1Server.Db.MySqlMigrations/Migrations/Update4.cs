using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.MySqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update4 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<float>(name: "LapDistance",
                                          table: "CarTelemetries",
                                          type: "float",
                                          nullable: false,
                                          defaultValue: 0f);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "LapDistance",
                                    table: "CarTelemetries");
    }

    #endregion // Migration
}