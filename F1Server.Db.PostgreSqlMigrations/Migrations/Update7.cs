using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace F1Server.Db.PostgreSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update7 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 9L,
                                    column: "Name",
                                    value: "Nico Hülkenberg");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 66L,
                                    column: "Name",
                                    value: "Jack Tremblay");

        migrationBuilder.InsertData(table: "Drivers",
                                    columns: new[] { "Id", "IsHumanDriver", "DriverGameId", "Name" },
                                    values: new object[,]
                                            {
                                                { 146L, 0, 157, "Zane Maloney" },
                                                { 147L, 0, 158, "Victor Martins" },
                                                { 148L, 0, 159, "Oliver Bearman" },
                                                { 149L, 0, 160, "Jak Crawford" },
                                                { 150L, 0, 161, "Isack Hadjar" },
                                                { 151L, 0, 162, "Arthur Leclerc" },
                                                { 152L, 0, 163, "Brad Benavides" },
                                                { 153L, 0, 164, "Roman Stanek" },
                                                { 154L, 0, 165, "Kush Maini" },
                                                { 155L, 0, 166, "James Hunt" },
                                                { 156L, 0, 167, "Juan Pablo Montoya" },
                                                { 157L, 0, 168, "Brendon Leigh" },
                                                { 158L, 0, 169, "David Tonizza" },
                                                { 159L, 0, 170, "Jarno Opmeer" },
                                                { 160L, 0, 171, "Lucas Blakeley" }
                                            });

        migrationBuilder.InsertData(table: "Nationalities",
                                    columns: new[] { "Id", "Name", "NationalityGameId" },
                                    values: new object[,]
                                            {
                                                { 89L, "Algerian", 88 },
                                                { 90L, "Bosnian", 89 },
                                                { 91L, "Filipino", 90 }
                                            });

        migrationBuilder.InsertData(table: "Teams",
                                    columns: new[] { "Id", "Name", "TeamGameId" },
                                    values: new object[,]
                                            {
                                                { 281L, "Mercedes", 20240 },
                                                { 282L, "Ferrari", 20241 },
                                                { 283L, "Red Bull Racing", 20242 },
                                                { 284L, "Williams", 20243 },
                                                { 285L, "Aston Martin", 20244 },
                                                { 286L, "Alpine", 20245 },
                                                { 287L, "RB", 20246 },
                                                { 288L, "Haas", 20247 },
                                                { 289L, "McLaren", 20248 },
                                                { 290L, "Sauber", 20249 },
                                                { 291L, "F1 Generic", 202441 },
                                                { 292L, "F1 Custom Team", 2024104 },
                                                { 293L, "Art GP '23", 2024143 },
                                                { 294L, "Campos '23", 2024144 },
                                                { 295L, "Carlin '23", 2024145 },
                                                { 296L, "PHM '23", 2024146 },
                                                { 297L, "Dams '23", 2024147 },
                                                { 298L, "Hitech '23", 2024148 },
                                                { 299L, "MP Motorsport '23", 2024149 },
                                                { 300L, "Prema '23", 2024150 },
                                                { 301L, "Trident '23", 2024151 },
                                                { 302L, "Van Amersfoort Racing '23", 2024152 },
                                                { 303L, "Virtuosi '23", 2024153 },
                                                { 1004L, "My Team '24", 2024255 }
                                            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 146L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 147L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 148L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 149L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 150L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 151L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 152L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 153L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 154L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 155L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 156L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 157L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 158L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 159L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 160L);

        migrationBuilder.DeleteData(table: "Nationalities",
                                    keyColumn: "Id",
                                    keyValue: 89L);

        migrationBuilder.DeleteData(table: "Nationalities",
                                    keyColumn: "Id",
                                    keyValue: 90L);

        migrationBuilder.DeleteData(table: "Nationalities",
                                    keyColumn: "Id",
                                    keyValue: 91L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 281L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 282L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 283L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 284L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 285L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 286L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 287L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 288L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 289L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 290L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 291L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 292L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 293L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 294L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 295L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 296L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 297L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 298L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 299L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 300L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 301L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 302L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 303L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 1004L);

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 9L,
                                    column: "Name",
                                    value: "Nico Hulkenburg");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 66L,
                                    column: "Name",
                                    value: "Jack Tremblayv");
    }

    #endregion // Migration
}