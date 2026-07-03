using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.MySqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update10 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<uint>(name: "LapReferenceTime",
                                         table: "Tracks",
                                         type: "int unsigned",
                                         nullable: false,
                                         defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(name: "Sector1ReferenceTime",
                                         table: "Tracks",
                                         type: "int unsigned",
                                         nullable: false,
                                         defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(name: "Sector2ReferenceTime",
                                         table: "Tracks",
                                         type: "int unsigned",
                                         nullable: false,
                                         defaultValue: 0u);

        migrationBuilder.AddColumn<uint>(name: "Sector3ReferenceTime",
                                         table: "Tracks",
                                         type: "int unsigned",
                                         nullable: false,
                                         defaultValue: 0u);

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 1L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 76086u, 26213u, 17547u, 32326u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 2L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 88384u, 21725u, 27336u, 39323u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 3L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 90098u, 23633u, 27041u, 39424u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 4L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 87550u, 28228u, 37605u, 21717u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 5L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 70490u, 21268u, 28388u, 20834u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 6L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 69897u, 18286u, 33560u, 18051u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 7L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 68744u, 19103u, 21748u, 27893u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 8L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 84532u, 26940u, 34630u, 22962u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 9L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 70963u, 15279u, 34159u, 21525u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 10L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 74551u, 26892u, 26319u, 21340u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 11L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 100331u, 29592u, 42794u, 27945u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 12L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 78401u, 26003u, 26278u, 26120u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 13L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 94904u, 26392u, 36524u, 31988u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 14L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 86051u, 30889u, 38756u, 16406u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 15L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 81379u, 17113u, 35001u, 29265u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 16L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 91376u, 25031u, 36621u, 29724u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 17L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 67039u, 16654u, 33806u, 16579u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 18L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 62994u, 15716u, 28174u, 19054u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 19L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 89867u, 32606u, 31066u, 26195u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 20L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 75181u, 28221u, 27944u, 19016u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 21L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 99352u, 35288u, 40019u, 24045u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 22L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 53252u, 18566u, 18474u, 16212u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 23L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 51812u, 10960u, 16373u, 24479u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 24L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 30000u, 10000u, 10000u, 10000u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 25L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 30000u, 10000u, 10000u, 10000u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 26L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 93454u, 25342u, 40367u, 27745u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 27L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 67834u, 23711u, 23428u, 20695u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 28L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 73311u, 23564u, 25323u, 24515u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 29L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 75588u, 21567u, 29255u, 24766u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 30L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 85870u, 31244u, 27721u, 26905u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 31L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 85890u, 29419u, 31444u, 25027u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 32L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 90406u, 43317u, 25530u, 21559u });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 33L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 79850u, 26190u, 26501u, 27159u });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "LapReferenceTime",
                                    table: "Tracks");

        migrationBuilder.DropColumn(name: "Sector1ReferenceTime",
                                    table: "Tracks");

        migrationBuilder.DropColumn(name: "Sector2ReferenceTime",
                                    table: "Tracks");

        migrationBuilder.DropColumn(name: "Sector3ReferenceTime",
                                    table: "Tracks");
    }

    #endregion // Migration
}