using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.MsSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update2 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(name: "WeatherEnd",
                                        table: "Sessions",
                                        type: "int",
                                        nullable: false,
                                        defaultValue: 0);

        migrationBuilder.AddColumn<int>(name: "WeatherStart",
                                        table: "Sessions",
                                        type: "int",
                                        nullable: false,
                                        defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "WeatherEnd",
                                    table: "Sessions");

        migrationBuilder.DropColumn(name: "WeatherStart",
                                    table: "Sessions");
    }

    #endregion // Migration
}