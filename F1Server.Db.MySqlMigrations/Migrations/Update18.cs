using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.MySqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update18 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name: "Users",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                                           Provider = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                                                                           .Annotation("MySql:CharSet", "utf8mb4"),
                                                           ProviderKey = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                                                                              .Annotation("MySql:CharSet", "utf8mb4"),
                                                           DisplayName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                                                                              .Annotation("MySql:CharSet", "utf8mb4"),
                                                           Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                                                                        .Annotation("MySql:CharSet", "utf8mb4"),
                                                           FirstLogin = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                                                           LastLogin = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                                                           LoginCount = table.Column<int>(type: "int", nullable: false)
                                                       },
                                     constraints: table => table.PrimaryKey("PK_Users", x => x.Id),
                                     comment: "Users table containing information about authenticated users from external identity providers.")
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(name: "IX_Users_Email",
                                     table: "Users",
                                     column: "Email");

        migrationBuilder.CreateIndex(name: "IX_Users_Provider",
                                     table: "Users",
                                     column: "Provider");

        migrationBuilder.CreateIndex(name: "IX_Users_ProviderKey_Provider",
                                     table: "Users",
                                     columns: new[] { "ProviderKey", "Provider" },
                                     unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Users");
    }

    #endregion // Migration
}