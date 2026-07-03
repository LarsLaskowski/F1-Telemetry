using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.MsSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update5 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<long>(name: "LapReferenceTime",
                                         table: "Tracks",
                                         type: "bigint",
                                         nullable: false,
                                         defaultValue: 0L);

        migrationBuilder.AddColumn<long>(name: "Sector1ReferenceTime",
                                         table: "Tracks",
                                         type: "bigint",
                                         nullable: false,
                                         defaultValue: 0L);

        migrationBuilder.AddColumn<long>(name: "Sector2ReferenceTime",
                                         table: "Tracks",
                                         type: "bigint",
                                         nullable: false,
                                         defaultValue: 0L);

        migrationBuilder.AddColumn<long>(name: "Sector3ReferenceTime",
                                         table: "Tracks",
                                         type: "bigint",
                                         nullable: false,
                                         defaultValue: 0L);

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 1L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 76086L, 26213L, 17547L, 32326L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 2L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 88384L, 21725L, 27336L, 39323L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 3L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 90098L, 23633L, 27041L, 39424L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 4L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 87550L, 28228L, 37605L, 21717L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 5L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 70490L, 21268L, 28388L, 20834L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 6L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 69897L, 18286L, 33560L, 18051L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 7L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 68744L, 19103L, 21748L, 27893L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 8L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 84532L, 26940L, 34630L, 22962L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 9L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 70963L, 15279L, 34159L, 21525L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 10L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 74551L, 26892L, 26319L, 21340L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 11L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 100331L, 29592L, 42794L, 27945L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 12L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 78401L, 26003L, 26278L, 26120L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 13L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 94904L, 26392L, 36524L, 31988L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 14L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 86051L, 30889L, 38756L, 16406L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 15L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 81379L, 17113L, 35001L, 29265L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 16L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 91376L, 25031L, 36621L, 29724L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 17L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 67039L, 16654L, 33806L, 16579L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 18L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 62994L, 15716L, 28174L, 19054L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 19L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 89867L, 32606L, 31066L, 26195L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 20L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 75181L, 28221L, 27944L, 19016L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 21L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 99352L, 35288L, 40019L, 24045L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 22L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 53252L, 18566L, 18474L, 16212L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 23L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 51812L, 10960L, 16373L, 24479L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 24L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 30000L, 10000L, 10000L, 10000L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 25L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 30000L, 10000L, 10000L, 10000L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 26L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 93454L, 25342L, 40367L, 27745L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 27L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 67834L, 23711L, 23428L, 20695L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 28L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 73311L, 23564L, 25323L, 24515L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 29L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 75588L, 21567L, 29255L, 24766L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 30L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 85870L, 31244L, 27721L, 26905L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 31L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 85890L, 29419L, 31444L, 25027L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 32L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 90406L, 43317L, 25530L, 21559L });

        migrationBuilder.UpdateData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 33L,
                                    columns: new[] { "LapReferenceTime", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime" },
                                    values: new object[] { 79850L, 26190L, 26501L, 27159L });
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