using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.PostgreSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update17 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 75L,
                                    column: "Name",
                                    value: "Guanyu Zhou");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 202L,
                                    column: "Name",
                                    value: "Guanyu Zhou");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 289L,
                                    column: "Name",
                                    value: "Guanyu Zhou");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 75L,
                                    column: "Name",
                                    value: "Guanya Zhou");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 202L,
                                    column: "Name",
                                    value: "Guanya Zhou");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 289L,
                                    column: "Name",
                                    value: "Guanya Zhou");
    }

    #endregion // Migration
}