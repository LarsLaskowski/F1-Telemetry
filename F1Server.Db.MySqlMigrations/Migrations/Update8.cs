using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace F1Server.Db.MySqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update8 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 258L,
                                    column: "Name",
                                    value: "Mercedes '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 259L,
                                    column: "Name",
                                    value: "Ferrari '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 260L,
                                    column: "Name",
                                    value: "Red Bull Racing '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 261L,
                                    column: "Name",
                                    value: "Williams '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 262L,
                                    column: "Name",
                                    value: "Aston Martin '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 263L,
                                    column: "Name",
                                    value: "Alpine '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 264L,
                                    column: "Name",
                                    value: "Alpha Tauri '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 265L,
                                    column: "Name",
                                    value: "Haas '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 266L,
                                    column: "Name",
                                    value: "McLaren '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 267L,
                                    column: "Name",
                                    value: "Alfa Romeo '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 268L,
                                    column: "Name",
                                    value: "Konnersport '22");

        migrationBuilder.InsertData(table: "Teams",
                                    columns: new[] { "Id", "Name", "TeamGameId" },
                                    values: new object[,]
                                            {
                                                { 269L, "Konnersport", 2023129 },
                                                { 270L, "Prema '22", 2023130 },
                                                { 271L, "Virtuosi '22", 2023131 },
                                                { 272L, "Carlin '22", 2023132 },
                                                { 273L, "MP Motorsport '22", 2023133 },
                                                { 274L, "Charouz '22", 2023134 },
                                                { 275L, "Dams '22", 2023135 },
                                                { 276L, "Campos '22", 2023136 },
                                                { 277L, "Van Amersfoort Racing '22", 2023137 },
                                                { 278L, "Trident '22", 2023138 },
                                                { 279L, "Hitech '22", 2023139 },
                                                { 280L, "Art GP '22", 2023140 }
                                            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 269L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 270L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 271L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 272L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 273L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 274L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 275L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 276L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 277L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 278L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 279L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 280L);

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 258L,
                                    column: "Name",
                                    value: "Prema '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 259L,
                                    column: "Name",
                                    value: "Virtuosi '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 260L,
                                    column: "Name",
                                    value: "Carlin '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 261L,
                                    column: "Name",
                                    value: "Hitech '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 262L,
                                    column: "Name",
                                    value: "Art GP '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 263L,
                                    column: "Name",
                                    value: "MP Motorsport '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 264L,
                                    column: "Name",
                                    value: "Charouz '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 265L,
                                    column: "Name",
                                    value: "Dams '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 266L,
                                    column: "Name",
                                    value: "Campos '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 267L,
                                    column: "Name",
                                    value: "Van Amersfoort Racing '22");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 268L,
                                    column: "Name",
                                    value: "Trident '22");
    }

    #endregion // Migration
}