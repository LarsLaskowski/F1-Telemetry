using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.MsSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update18 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(name: "DriverName",
                                             table: "Participants",
                                             type: "nvarchar(100)",
                                             maxLength: 100,
                                             nullable: false,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "GameVersions",
                                             type: "nvarchar(100)",
                                             maxLength: 100,
                                             nullable: false,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(max)");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(name: "DriverName",
                                             table: "Participants",
                                             type: "nvarchar(max)",
                                             nullable: false,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(100)",
                                             oldMaxLength: 100);

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "GameVersions",
                                             type: "nvarchar(max)",
                                             nullable: false,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(100)",
                                             oldMaxLength: 100);
    }

    #endregion // Migration
}