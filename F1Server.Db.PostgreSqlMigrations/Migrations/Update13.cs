using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace F1Server.Db.PostgreSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update13 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name: "Users",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                                                           Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                                                           ProviderKey = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                                                           DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                                                           Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                                                           FirstLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                                                           LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                                                           LoginCount = table.Column<int>(type: "integer", nullable: false)
                                                       },
                                     constraints: table => table.PrimaryKey("PK_Users", x => x.Id),
                                     comment: "Users table containing information about authenticated users from external identity providers.");

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