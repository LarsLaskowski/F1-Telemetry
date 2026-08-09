using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.MySqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update23 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(name: "DriverName",
                                             table: "Participants",
                                             type: "varchar(100)",
                                             maxLength: 100,
                                             nullable: false,
                                             oldClrType: typeof(string),
                                             oldType: "longtext")
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "GameVersions",
                                             type: "varchar(100)",
                                             maxLength: 100,
                                             nullable: false,
                                             oldClrType: typeof(string),
                                             oldType: "longtext")
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .OldAnnotation("MySql:CharSet", "utf8mb4");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(name: "DriverName",
                                             table: "Participants",
                                             type: "longtext",
                                             nullable: false,
                                             oldClrType: typeof(string),
                                             oldType: "varchar(100)",
                                             oldMaxLength: 100)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .OldAnnotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "GameVersions",
                                             type: "longtext",
                                             nullable: false,
                                             oldClrType: typeof(string),
                                             oldType: "varchar(100)",
                                             oldMaxLength: 100)
                        .Annotation("MySql:CharSet", "utf8mb4")
                        .OldAnnotation("MySql:CharSet", "utf8mb4");
    }

    #endregion // Migration
}