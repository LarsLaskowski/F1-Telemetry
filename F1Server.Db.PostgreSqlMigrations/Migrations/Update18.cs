using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.PostgreSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update18 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(name: "DriverName",
                                             table: "Participants",
                                             type: "character varying(100)",
                                             maxLength: 100,
                                             nullable: false,
                                             oldClrType: typeof(string),
                                             oldType: "text");

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "GameVersions",
                                             type: "character varying(100)",
                                             maxLength: 100,
                                             nullable: false,
                                             oldClrType: typeof(string),
                                             oldType: "text");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(name: "DriverName",
                                             table: "Participants",
                                             type: "text",
                                             nullable: false,
                                             oldClrType: typeof(string),
                                             oldType: "character varying(100)",
                                             oldMaxLength: 100);

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "GameVersions",
                                             type: "text",
                                             nullable: false,
                                             oldClrType: typeof(string),
                                             oldType: "character varying(100)",
                                             oldMaxLength: 100);
    }

    #endregion // Migration
}