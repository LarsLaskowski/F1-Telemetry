using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.MySqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update2 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(name: "FlashbacksUsed",
                                        table: "Sessions",
                                        type: "int",
                                        nullable: false,
                                        defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "FlashbacksUsed",
                                    table: "Sessions");
    }

    #endregion // Migration
}