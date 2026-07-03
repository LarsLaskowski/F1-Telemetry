using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace F1Server.Db.MySqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update5 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(table: "Drivers",
                                    columns: new[] { "Id", "IsHumanDriver", "DriverGameId", "Name" },
                                    values: new object[,]
                                            {
                                                { 128L, 0, 139, "Callie Mayer" },
                                                { 129L, 0, 140, "Noah Bell" },
                                                { 130L, 0, 141, "Jake Hughes" },
                                                { 131L, 0, 142, "Frederik Vesti" },
                                                { 132L, 0, 143, "Olli Caldwell" },
                                                { 133L, 0, 144, "Logan Sargeant" },
                                                { 134L, 0, 145, "Cem Bolukbasi" },
                                                { 135L, 0, 146, "Ayumu Iwasa" },
                                                { 136L, 0, 147, "Clement Novalak" },
                                                { 137L, 0, 148, "Jack Doohan" },
                                                { 138L, 0, 149, "Amaury Cordeel" },
                                                { 139L, 0, 150, "Dennis Hauger" },
                                                { 140L, 0, 151, "Calan Williams" },
                                                { 141L, 0, 152, "Jamie Chadwick" },
                                                { 142L, 0, 153, "Kamui Kobayashi" },
                                                { 143L, 0, 154, "Pastor Maldonado" },
                                                { 144L, 0, 155, "Mika Hakkinen" },
                                                { 145L, 0, 156, "Nigel Mansell" }
                                            });

        migrationBuilder.InsertData(table: "Teams",
                                    columns: new[] { "Id", "Name", "TeamGameId" },
                                    values: new object[,]
                                            {
                                                { 216L, "Mercedes", 20230 },
                                                { 217L, "Ferrari", 20231 },
                                                { 218L, "Red Bull Racing", 20232 },
                                                { 219L, "Williams", 20233 },
                                                { 220L, "Aston Martin", 20234 },
                                                { 221L, "Alpine", 20235 },
                                                { 222L, "Alpha Tauri", 20236 },
                                                { 223L, "Haas", 20237 },
                                                { 224L, "McLaren", 20238 },
                                                { 225L, "Alfa Romeo", 20239 },
                                                { 226L, "Mercedes 2020", 202385 },
                                                { 227L, "Ferrari 2020", 202386 },
                                                { 228L, "Red Bull 2020", 202387 },
                                                { 229L, "Williams 2020", 202388 },
                                                { 230L, "Racing Point 2020", 202389 },
                                                { 231L, "Renault 2020", 202390 },
                                                { 232L, "Alpha Tauri 2020", 202391 },
                                                { 233L, "Haas 2020", 202392 },
                                                { 234L, "McLaren 2020", 202393 },
                                                { 235L, "Alfa Romeo 2020", 202394 },
                                                { 236L, "Aston Martin DB11 V12", 202395 },
                                                { 237L, "Aston Martin Vantage F1 Edition", 202396 },
                                                { 238L, "Aston Martin Vantage Safety Car", 202397 },
                                                { 239L, "Ferrari F8 Tributo", 202398 },
                                                { 240L, "Ferrari Roma", 202399 },
                                                { 241L, "McLaren 720S", 2023100 },
                                                { 242L, "McLaren Artura", 2023101 },
                                                { 243L, "Mercedes AMG GT Black Series Safety Car", 2023102 },
                                                { 244L, "Mercedes AMG GTR Pro", 2023103 },
                                                { 245L, "F1 Custom Team", 2023104 },
                                                { 246L, "Prema '21", 2023106 },
                                                { 247L, "Ferrari Uni-Virtuosi '21", 2023107 },
                                                { 248L, "Carlin '21", 2023108 },
                                                { 249L, "Hitech '21", 2023109 },
                                                { 250L, "Art GP '21", 2023110 },
                                                { 251L, "MP Motorsport '21", 2023111 },
                                                { 252L, "Charouz '21", 2023112 },
                                                { 253L, "Dams '21", 2023113 },
                                                { 254L, "Campos '21", 2023114 },
                                                { 255L, "BWT '21", 2023115 },
                                                { 256L, "Trident '21", 2023116 },
                                                { 257L, "Mercedes AMG GT Black Series", 2023117 },
                                                { 258L, "Prema '22", 2023118 },
                                                { 259L, "Virtuosi '22", 2023119 },
                                                { 260L, "Carlin '22", 2023120 },
                                                { 261L, "Hitech '22", 2023121 },
                                                { 262L, "Art GP '22", 2023122 },
                                                { 263L, "MP Motorsport '22", 2023123 },
                                                { 264L, "Charouz '22", 2023124 },
                                                { 265L, "Dams '22", 2023125 },
                                                { 266L, "Campos '22", 2023126 },
                                                { 267L, "Van Amersfoort Racing '22", 2023127 },
                                                { 268L, "Trident '22", 2023128 },
                                                { 1003L, "My Team '23", 2023255 }
                                            });

        migrationBuilder.InsertData(table: "Tracks",
                                    columns: new[] { "Id", "Name", "TrackNumber" },
                                    values: new object[,]
                                            {
                                                { 32L, "Las Vegas", 31 },
                                                { 33L, "Losail", 32 }
                                            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 128L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 129L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 130L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 131L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 132L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 133L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 134L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 135L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 136L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 137L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 138L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 139L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 140L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 141L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 142L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 143L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 144L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 145L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 216L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 217L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 218L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 219L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 220L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 221L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 222L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 223L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 224L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 225L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 226L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 227L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 228L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 229L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 230L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 231L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 232L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 233L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 234L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 235L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 236L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 237L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 238L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 239L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 240L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 241L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 242L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 243L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 244L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 245L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 246L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 247L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 248L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 249L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 250L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 251L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 252L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 253L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 254L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 255L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 256L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 257L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 258L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 259L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 260L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 261L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 262L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 263L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 264L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 265L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 266L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 267L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 268L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 1003L);

        migrationBuilder.DeleteData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 32L);

        migrationBuilder.DeleteData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 33L);
    }

    #endregion // Migration
}