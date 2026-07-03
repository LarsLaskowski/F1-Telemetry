using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Server.Db.MySqlMigrations.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(name: "Drivers",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                                           DriverGameId = table.Column<int>(type: "int", nullable: false),
                                                           Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                                                                       .Annotation("MySql:CharSet", "utf8mb4"),
                                                           IsHumanDriver = table.Column<int>(type: "int", nullable: false)
                                                       },
                                     constraints: table => table.PrimaryKey("PK_Drivers", x => x.Id))
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(name: "GameVersions",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                                           Version = table.Column<int>(type: "int", nullable: false),
                                                           Name = table.Column<string>(type: "longtext", nullable: true)
                                                                       .Annotation("MySql:CharSet", "utf8mb4"),
                                                           MajorVersion = table.Column<int>(type: "int", nullable: false),
                                                           MinorVersion = table.Column<int>(type: "int", nullable: false),
                                                           LastUsed = table.Column<DateTime>(type: "datetime", nullable: true)
                                                       },
                                     constraints: table => table.PrimaryKey("PK_GameVersions", x => x.Id))
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(name: "Nationalities",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                                           NationalityGameId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                                                                       .Annotation("MySql:CharSet", "utf8mb4")
                                                       },
                                     constraints: table => table.PrimaryKey("PK_Nationalities", x => x.Id))
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(name: "Teams",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                                           TeamGameId = table.Column<int>(type: "int", nullable: false),
                                                           Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                                                                       .Annotation("MySql:CharSet", "utf8mb4")
                                                       },
                                     constraints: table => table.PrimaryKey("PK_Teams", x => x.Id))
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(name: "Tracks",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                                           TrackNumber = table.Column<int>(type: "int", nullable: false),
                                                           Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                                                                       .Annotation("MySql:CharSet", "utf8mb4")
                                                       },
                                     constraints: table => table.PrimaryKey("PK_Tracks", x => x.Id))
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(name: "Sessions",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                                           SessionId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                                                           CreationTimestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                                                           FormulaType = table.Column<int>(type: "int", nullable: false),
                                                           TrackId = table.Column<long>(type: "bigint", nullable: false),
                                                           SessionType = table.Column<int>(type: "int", nullable: false),
                                                           GameVersionId = table.Column<long>(type: "bigint", nullable: false),
                                                           IsNetworkGame = table.Column<int>(type: "int", nullable: false),
                                                           ActiveCars = table.Column<int>(type: "int", nullable: false),
                                                           IsExtendedSession = table.Column<int>(type: "int", nullable: false),
                                                           AiDifficulty = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           SteeringAssistFirst = table.Column<int>(type: "int", nullable: false),
                                                           SteeringAssistLast = table.Column<int>(type: "int", nullable: false),
                                                           SteeringAssistChanged = table.Column<int>(type: "int", nullable: false),
                                                           BrakingAssistFirst = table.Column<int>(type: "int", nullable: false),
                                                           BrakingAssistLast = table.Column<int>(type: "int", nullable: false),
                                                           BrakingAssistChanged = table.Column<int>(type: "int", nullable: false),
                                                           GearBoxAssistFirst = table.Column<int>(type: "int", nullable: false),
                                                           GearBoxAssistLast = table.Column<int>(type: "int", nullable: false),
                                                           GearBoxAssistChanged = table.Column<int>(type: "int", nullable: false),
                                                           GameMode = table.Column<int>(type: "int", nullable: false),
                                                           RuleSet = table.Column<int>(type: "int", nullable: false),
                                                           SessionLength = table.Column<int>(type: "int", nullable: false),
                                                           IsFinished = table.Column<int>(type: "int", nullable: false)
                                                       },
                                     constraints: table =>
                                                  {
                                                      table.PrimaryKey("PK_Sessions", x => x.Id);
                                                      table.ForeignKey(name: "FK_Sessions_GameVersions_GameVersionId",
                                                                       column: x => x.GameVersionId,
                                                                       principalTable: "GameVersions",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Cascade);
                                                      table.ForeignKey(name: "FK_Sessions_Tracks_TrackId",
                                                                       column: x => x.TrackId,
                                                                       principalTable: "Tracks",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Cascade);
                                                  })
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(name: "Participants",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                                           SessionId = table.Column<long>(type: "bigint", nullable: false),
                                                           DriverId = table.Column<long>(type: "bigint", nullable: false),
                                                           NationalityId = table.Column<long>(type: "bigint", nullable: false),
                                                           IsHumanControlled = table.Column<int>(type: "int", nullable: false),
                                                           CarRaceNumber = table.Column<int>(type: "int", nullable: false),
                                                           TeamId = table.Column<long>(type: "bigint", nullable: false),
                                                           IsMyTeam = table.Column<int>(type: "int", nullable: true),
                                                           DriverName = table.Column<string>(type: "longtext", nullable: true)
                                                                             .Annotation("MySql:CharSet", "utf8mb4"),
                                                           ArrayIndex = table.Column<ushort>(type: "smallint unsigned", nullable: false)
                                                       },
                                     constraints: table =>
                                                  {
                                                      table.PrimaryKey("PK_Participants", x => x.Id);
                                                      table.ForeignKey(name: "FK_Participants_Drivers_DriverId",
                                                                       column: x => x.DriverId,
                                                                       principalTable: "Drivers",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Cascade);
                                                      table.ForeignKey(name: "FK_Participants_Nationalities_NationalityId",
                                                                       column: x => x.NationalityId,
                                                                       principalTable: "Nationalities",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Cascade);
                                                      table.ForeignKey(name: "FK_Participants_Sessions_SessionId",
                                                                       column: x => x.SessionId,
                                                                       principalTable: "Sessions",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Cascade);
                                                      table.ForeignKey(name: "FK_Participants_Teams_TeamId",
                                                                       column: x => x.TeamId,
                                                                       principalTable: "Teams",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Cascade);
                                                  })
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(name: "Laps",
                                     columns: table => new
                                                       {
                                                           Id = table.Column<long>(type: "bigint", nullable: false)
                                                                     .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                                           ParticipantId = table.Column<long>(type: "bigint", nullable: false),
                                                           SessionId = table.Column<long>(type: "bigint", nullable: false),
                                                           LapTime = table.Column<uint>(type: "int unsigned", nullable: false),
                                                           Sector1Time = table.Column<uint>(type: "int unsigned", nullable: false),
                                                           Sector2Time = table.Column<uint>(type: "int unsigned", nullable: false),
                                                           Sector3Time = table.Column<uint>(type: "int unsigned", nullable: false),
                                                           LapNumber = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           IsInvalid = table.Column<int>(type: "int", nullable: false),
                                                           IsCompleted = table.Column<int>(type: "int", nullable: false),
                                                           DriverStatus = table.Column<int>(type: "int", nullable: false),
                                                           PitStatus = table.Column<int>(type: "int", nullable: false),
                                                           ResultStatus = table.Column<int>(type: "int", nullable: false),
                                                           CarPosition = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                                                           TyreCompound = table.Column<int>(type: "int", nullable: false)
                                                       },
                                     constraints: table =>
                                                  {
                                                      table.PrimaryKey("PK_Laps", x => x.Id);
                                                      table.ForeignKey(name: "FK_Laps_Participants_ParticipantId",
                                                                       column: x => x.ParticipantId,
                                                                       principalTable: "Participants",
                                                                       principalColumn: "Id",
                                                                       onDelete: ReferentialAction.Cascade);
                                                  })
                        .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.InsertData(table: "Drivers",
                                    columns: new[] { "Id", "IsHumanDriver", "DriverGameId", "Name" },
                                    values: new object[,]
                                            {
                                                { 1L, 0, 0, "Carlos Sainz" },
                                                { 2L, 0, 1, "Daniil Kvyat" },
                                                { 3L, 0, 2, "Daniel Ricciardo" },
                                                { 4L, 0, 3, "Fernando Alonso" },
                                                { 5L, 0, 4, "Felipe Massa" },
                                                { 6L, 0, 6, "Kimi Räikkönen" },
                                                { 7L, 0, 7, "Lewis Hamilton" },
                                                { 8L, 0, 9, "Max Verstappen" },
                                                { 9L, 0, 10, "Nico Hulkenburg" },
                                                { 10L, 0, 11, "Kevin Magnussen" },
                                                { 11L, 0, 12, "Romain Grosjean" },
                                                { 12L, 0, 13, "Sebastian Vettel" },
                                                { 13L, 0, 14, "Sergio Perez" },
                                                { 14L, 0, 15, "Valtteri Bottas" },
                                                { 15L, 0, 17, "Esteban Ocon" },
                                                { 16L, 0, 19, "Lance Stroll" },
                                                { 17L, 0, 20, "Arron Barnes" },
                                                { 18L, 0, 21, "Martin Giles" },
                                                { 19L, 0, 22, "Alex Murray" },
                                                { 20L, 0, 23, "Lucas Roth" },
                                                { 21L, 0, 24, "Igor Correia" },
                                                { 22L, 0, 25, "Sophie Levasseur" },
                                                { 23L, 0, 26, "Jonas Schiffer" },
                                                { 24L, 0, 27, "Alain Forest" },
                                                { 25L, 0, 28, "Jay Letourneau" },
                                                { 26L, 0, 29, "Esto Saari" },
                                                { 27L, 0, 30, "Yasar Atiyeh" },
                                                { 28L, 0, 31, "Callisto Calabresi" },
                                                { 29L, 0, 32, "Naota Izum" },
                                                { 30L, 0, 33, "Howard Clarke" },
                                                { 31L, 0, 34, "Wilheim Kaufmann" },
                                                { 32L, 0, 35, "Marie Laursen" },
                                                { 33L, 0, 36, "Flavio Nieves" },
                                                { 34L, 0, 37, "Peter Belousov" },
                                                { 35L, 0, 38, "Klimek Michalski" },
                                                { 36L, 0, 39, "Santiago Moreno" },
                                                { 37L, 0, 40, "Benjamin Coppens" },
                                                { 38L, 0, 41, "Noah Visser" },
                                                { 39L, 0, 42, "Gert Waldmuller" },
                                                { 40L, 0, 43, "Julian Quesada" },
                                                { 41L, 0, 44, "Daniel Jones" },
                                                { 42L, 0, 45, "Artem Markelov" },
                                                { 43L, 0, 46, "Tadasuke Makino" },
                                                { 44L, 0, 47, "Sean Gelael" },
                                                { 45L, 0, 48, "Nyck De Vries" },
                                                { 46L, 0, 49, "Jack Aitken" },
                                                { 47L, 0, 50, "George Russell" },
                                                { 48L, 0, 51, "Maximilian Günther" },
                                                { 49L, 0, 52, "Nirei Fukuzumi" },
                                                { 50L, 0, 53, "Luca Ghiotto" },
                                                { 51L, 0, 54, "Lando Norris" },
                                                { 52L, 0, 55, "Sérgio Sette Câmara" },
                                                { 53L, 0, 56, "Louis Delétraz" },
                                                { 54L, 0, 57, "Antonio Fuoco" },
                                                { 55L, 0, 58, "Charles Leclerc" },
                                                { 56L, 0, 59, "Pierre Gasly" },
                                                { 57L, 0, 62, "Alexander Albon" },
                                                { 58L, 0, 63, "Nicholas Latifi" },
                                                { 59L, 0, 64, "Dorian Boccolacci" },
                                                { 60L, 0, 65, "Niko Kari" },
                                                { 61L, 0, 66, "Roberto Merhi" },
                                                { 62L, 0, 67, "Arjun Maini" },
                                                { 63L, 0, 68, "Alessio Lorandi" },
                                                { 64L, 0, 69, "Ruben Meijer" },
                                                { 65L, 0, 70, "Rashid Nair" },
                                                { 66L, 0, 71, "Jack Tremblayv" },
                                                { 67L, 0, 72, "Devon Butler" },
                                                { 68L, 0, 73, "Lukas Weber" },
                                                { 69L, 0, 74, "Antonio Giovinazzi" },
                                                { 70L, 0, 75, "Robert Kubica" },
                                                { 71L, 0, 76, "Alain Prost" },
                                                { 72L, 0, 77, "Ayrton Senna" },
                                                { 73L, 0, 78, "Nobuharu Matsushita" },
                                                { 74L, 0, 79, "Nikita Mazepin" },
                                                { 75L, 0, 80, "Guanya Zhou" },
                                                { 76L, 0, 81, "Mick Schumacher" },
                                                { 77L, 0, 82, "Callum Ilott" },
                                                { 78L, 0, 83, "Juan Manuel Correa" },
                                                { 79L, 0, 84, "Jordan King" },
                                                { 80L, 0, 85, "Mahaveer Raghunathan" },
                                                { 81L, 0, 86, "Tatiana Calderon" },
                                                { 82L, 0, 87, "Anthoine Hubert" },
                                                { 83L, 0, 88, "Guiliano Alesi" },
                                                { 84L, 0, 89, "Ralph Boschung" },
                                                { 85L, 0, 90, "Michael Schumacher" },
                                                { 86L, 0, 91, "Dan Ticktum" },
                                                { 87L, 0, 92, "Marcus Armstrong" },
                                                { 88L, 0, 93, "Christian Lundgaard" },
                                                { 89L, 0, 94, "Yuki Tsunoda" },
                                                { 90L, 0, 95, "Jehan Daruvala" },
                                                { 91L, 0, 96, "Gulherme Samaia" },
                                                { 92L, 0, 97, "Pedro Piquet" },
                                                { 93L, 0, 98, "Felipe Drugovich" },
                                                { 94L, 0, 99, "Robert Schwartzman" },
                                                { 95L, 0, 100, "Roy Nissany" },
                                                { 96L, 0, 101, "Marino Sato" },
                                                { 97L, 0, 102, "Aidan Jackson" },
                                                { 98L, 0, 103, "Casper Akkerman" },
                                                { 99L, 0, 109, "Jenson Button" },
                                                { 100L, 0, 110, "David Coulthard" },
                                                { 101L, 0, 111, "Nico Rosberg" },
                                                { 102L, 0, 112, "Oscar Piastri" },
                                                { 103L, 0, 113, "Liam Lawson" },
                                                { 104L, 0, 114, "Juri Vips" },
                                                { 105L, 0, 115, "Theo Pourchaire" },
                                                { 106L, 0, 116, "Richard Verschoor" },
                                                { 107L, 0, 117, "Lirim Zendeli" },
                                                { 108L, 0, 118, "David Beckmann" },
                                                { 109L, 0, 119, "Gianluca Petecof" },
                                                { 110L, 0, 120, "Matteo Nannini" },
                                                { 111L, 0, 121, "Alessio Deledda" },
                                                { 112L, 0, 122, "Bent Viscaal" },
                                                { 113L, 0, 123, "Enzo Fittipaldi" },
                                                { 114L, 0, 125, "Mark Webber" },
                                                { 115L, 0, 126, "Jacques Villeneuve" },
                                                { 116L, 0, 127, "Jake Hughes" },
                                                { 117L, 0, 128, "Frederik Vesti" },
                                                { 118L, 0, 129, "Olli Caldwell" },
                                                { 119L, 0, 130, "Logan Sargeant" },
                                                { 120L, 0, 131, "Cem Bolukbasi" },
                                                { 121L, 0, 132, "Ayuma Iwasa" },
                                                { 122L, 0, 133, "Clement Novolak" },
                                                { 123L, 0, 134, "Dennis Hauger" },
                                                { 124L, 0, 135, "Calan Williams" },
                                                { 125L, 0, 136, "Jack Doohan" },
                                                { 126L, 0, 137, "Amaury Cordeel" },
                                                { 127L, 0, 138, "Mika Hakkinen" },
                                                { 1000L, 1, 255, string.Empty }
                                            });

        migrationBuilder.InsertData(table: "Nationalities",
                                    columns: new[] { "Id", "Name", "NationalityGameId" },
                                    values: new object[,]
                                            {
                                                { 1L, "Unknown", (ushort)0 },
                                                { 2L, "American", (ushort)1 },
                                                { 3L, "Argentinean", (ushort)2 },
                                                { 4L, "Australian", (ushort)3 },
                                                { 5L, "Austrian", (ushort)4 },
                                                { 6L, "Azerbaijani", (ushort)5 },
                                                { 7L, "Bahraini", (ushort)6 },
                                                { 8L, "Belgian", (ushort)7 },
                                                { 9L, "Bolivian", (ushort)8 },
                                                { 10L, "Brazilian", (ushort)9 },
                                                { 11L, "British", (ushort)10 },
                                                { 12L, "Bulgarian", (ushort)11 },
                                                { 13L, "Cameroonian", (ushort)12 },
                                                { 14L, "Canadian", (ushort)13 },
                                                { 15L, "Chilean", (ushort)14 },
                                                { 16L, "Chinese", (ushort)15 },
                                                { 17L, "Colombian", (ushort)16 },
                                                { 18L, "Costa Rican", (ushort)17 },
                                                { 19L, "Croatian", (ushort)18 },
                                                { 20L, "Cypriot", (ushort)19 },
                                                { 21L, "Czech", (ushort)20 },
                                                { 22L, "Danish", (ushort)21 },
                                                { 23L, "Dutch", (ushort)22 },
                                                { 24L, "Ecuadorian", (ushort)23 },
                                                { 25L, "English", (ushort)24 },
                                                { 26L, "Emirian", (ushort)25 },
                                                { 27L, "Estonian", (ushort)26 },
                                                { 28L, "Finnish", (ushort)27 },
                                                { 29L, "French", (ushort)28 },
                                                { 30L, "German", (ushort)29 },
                                                { 31L, "Ghanaian", (ushort)30 },
                                                { 32L, "Greek", (ushort)31 },
                                                { 33L, "Guatemalan", (ushort)32 },
                                                { 34L, "Honduran", (ushort)33 },
                                                { 35L, "Hong Konger", (ushort)34 },
                                                { 36L, "Hungarian", (ushort)35 },
                                                { 37L, "Icelander", (ushort)36 },
                                                { 38L, "Indian", (ushort)37 },
                                                { 39L, "Indonesian", (ushort)38 },
                                                { 40L, "Irish", (ushort)39 },
                                                { 41L, "Israeli", (ushort)40 },
                                                { 42L, "Italian", (ushort)41 },
                                                { 43L, "Jamaican", (ushort)42 },
                                                { 44L, "Japanese", (ushort)43 },
                                                { 45L, "Jordanian", (ushort)44 },
                                                { 46L, "Kuwaiti", (ushort)45 },
                                                { 47L, "Latvian", (ushort)46 },
                                                { 48L, "Lebanese", (ushort)47 },
                                                { 49L, "Lithuanian", (ushort)48 },
                                                { 50L, "Luxembourger", (ushort)49 },
                                                { 51L, "Malaysian", (ushort)50 },
                                                { 52L, "Maltese", (ushort)51 },
                                                { 53L, "Mexican", (ushort)52 },
                                                { 54L, "Monegasque", (ushort)53 },
                                                { 55L, "New Zealander", (ushort)54 },
                                                { 56L, "Nicaraguan", (ushort)55 },
                                                { 57L, "Northern Irish", (ushort)56 },
                                                { 58L, "Norwegian", (ushort)57 },
                                                { 59L, "Omani", (ushort)58 },
                                                { 60L, "Pakistani", (ushort)59 },
                                                { 61L, "Panamanian", (ushort)60 },
                                                { 62L, "Paraguayan", (ushort)61 },
                                                { 63L, "Peruvian", (ushort)62 },
                                                { 64L, "Polish", (ushort)63 },
                                                { 65L, "Portuguese", (ushort)64 },
                                                { 66L, "Qatari", (ushort)65 },
                                                { 67L, "Romanian", (ushort)66 },
                                                { 68L, "Russian", (ushort)67 },
                                                { 69L, "Salvadoran", (ushort)68 },
                                                { 70L, "Saudi", (ushort)69 },
                                                { 71L, "Scottish", (ushort)70 },
                                                { 72L, "Serbian", (ushort)71 },
                                                { 73L, "Singaporean", (ushort)72 },
                                                { 74L, "Slovakian", (ushort)73 },
                                                { 75L, "Slovenian", (ushort)74 },
                                                { 76L, "South Korean", (ushort)75 },
                                                { 77L, "South African", (ushort)76 },
                                                { 78L, "Spanish", (ushort)77 },
                                                { 79L, "Swedish", (ushort)78 },
                                                { 80L, "Swiss", (ushort)79 },
                                                { 81L, "Thai", (ushort)80 },
                                                { 82L, "Turkish", (ushort)81 },
                                                { 83L, "Uruguayan", (ushort)82 },
                                                { 84L, "Ukrainian", (ushort)83 },
                                                { 85L, "Venezuelan", (ushort)84 },
                                                { 86L, "Barbadian", (ushort)85 },
                                                { 87L, "Welsh", (ushort)86 },
                                                { 88L, "Vietnamese", (ushort)87 }
                                            });

        migrationBuilder.InsertData(table: "Teams",
                                    columns: new[] { "Id", "Name", "TeamGameId" },
                                    values: new object[,]
                                            {
                                                { 1L, "Mercedes", 20190 },
                                                { 2L, "Ferrari", 20191 },
                                                { 3L, "Red Bull Racing", 20192 },
                                                { 4L, "Williams", 20193 },
                                                { 5L, "Racing Point", 20194 },
                                                { 6L, "Renault", 20195 },
                                                { 7L, "Toro Rosso", 20196 },
                                                { 8L, "Haas", 20197 },
                                                { 9L, "McLaren", 20198 },
                                                { 10L, "Alfa Romeo", 20199 },
                                                { 11L, "McLaren 1988", 201910 },
                                                { 12L, "McLaren 1991", 201911 },
                                                { 13L, "Williams 1992", 201912 },
                                                { 14L, "Ferrari 1995", 201913 },
                                                { 15L, "Williams 1996", 201914 },
                                                { 16L, "McLaren 1998", 201915 },
                                                { 17L, "Ferrari 2002", 201916 },
                                                { 18L, "Ferrari 2004", 201917 },
                                                { 19L, "Renault 2006", 201918 },
                                                { 20L, "Ferrari 2007", 201919 },
                                                { 21L, "Red Bull 2010", 201921 },
                                                { 22L, "Ferrari 1976", 201922 },
                                                { 23L, "ART Grand Prix", 201923 },
                                                { 24L, "Campos Vexatec Racing", 201924 },
                                                { 25L, "Carlin", 201925 },
                                                { 26L, "Charouz Racing System", 201926 },
                                                { 27L, "DAMS", 201927 },
                                                { 28L, "Russian Time", 201928 },
                                                { 29L, "MP Motorsport", 201929 },
                                                { 30L, "Pertamina", 201930 },
                                                { 31L, "McLaren 1990", 201931 },
                                                { 32L, "Trident", 201932 },
                                                { 33L, "BWT Arden", 201933 },
                                                { 34L, "McLaren 1976", 201934 },
                                                { 35L, "Lotus 1972", 201935 },
                                                { 36L, "Ferrari 1979", 201936 },
                                                { 37L, "McLaren 1982", 201937 },
                                                { 38L, "Williams 2003", 201938 },
                                                { 39L, "Brawn 2009", 201939 },
                                                { 40L, "Lotus 1978", 201940 },
                                                { 41L, "Art GP '19", 201942 },
                                                { 42L, "Campos '19", 201943 },
                                                { 43L, "Carlin '19", 201944 },
                                                { 44L, "Sauber Junior Charouz '19", 201945 },
                                                { 45L, "Dams '19", 201946 },
                                                { 46L, "Uni-Virtuosi '19", 201947 },
                                                { 47L, "MP Motorsport '19", 201948 },
                                                { 48L, "Prema '19", 201949 },
                                                { 49L, "Trident '19", 201950 },
                                                { 50L, "Arden '19", 201951 },
                                                { 51L, "Ferrari 1990", 201963 },
                                                { 52L, "McLaren 2010", 201964 },
                                                { 53L, "Ferrari 2010", 201965 },
                                                { 54L, "Mercedes", 20200 },
                                                { 55L, "Ferrari", 20201 },
                                                { 56L, "Red Bull Racing", 20202 },
                                                { 57L, "Williams", 20203 },
                                                { 58L, "Racing Point", 20204 },
                                                { 59L, "Renault", 20205 },
                                                { 60L, "Alpha Tauri", 20206 },
                                                { 61L, "Haas", 20207 },
                                                { 62L, "McLaren", 20208 },
                                                { 63L, "Alfa Romeo", 20209 },
                                                { 64L, "McLaren 1988", 202010 },
                                                { 65L, "McLaren 1991", 202011 },
                                                { 66L, "Williams 1992", 202012 },
                                                { 67L, "Ferrari 1995", 202013 },
                                                { 68L, "Williams 1996", 202014 },
                                                { 69L, "McLaren 1998", 202015 },
                                                { 70L, "Ferrari 2002", 202016 },
                                                { 71L, "Ferrari 2004", 202017 },
                                                { 72L, "Renault 2006", 202018 },
                                                { 73L, "Ferrari 2007", 202019 },
                                                { 74L, "McLaren 2008", 202020 },
                                                { 75L, "Red Bull 2010", 202021 },
                                                { 76L, "Ferrari 1976", 202022 },
                                                { 77L, "ART Grand Prix", 202023 },
                                                { 78L, "Campos Vexatec Racing", 202024 },
                                                { 79L, "Carlin", 202025 },
                                                { 80L, "Charouz Racing System", 202026 },
                                                { 81L, "DAMS", 202027 },
                                                { 82L, "Russian Time", 202028 },
                                                { 83L, "MP Motorsport", 202029 },
                                                { 84L, "Pertamina", 202030 },
                                                { 85L, "McLaren 1990", 202031 },
                                                { 86L, "Trident", 202032 },
                                                { 87L, "BWT Arden", 202033 },
                                                { 88L, "McLaren 1976", 202034 },
                                                { 89L, "Lotus 1972", 202035 },
                                                { 90L, "Ferrari 1979", 202036 },
                                                { 91L, "McLaren 1982", 202037 },
                                                { 92L, "Williams 2003", 202038 },
                                                { 93L, "Brawn 2009", 202039 },
                                                { 94L, "Lotus 1978", 202040 },
                                                { 95L, "F1 Generic car", 202041 },
                                                { 96L, "Art GP '19", 202042 },
                                                { 97L, "Campos '19", 202043 },
                                                { 98L, "Carlin '19", 202044 },
                                                { 99L, "Sauber Junior Charouz '19", 202045 },
                                                { 100L, "Dams '19", 202046 },
                                                { 101L, "Uni-Virtuosi '19", 202047 },
                                                { 102L, "MP Motorsport '19", 202048 },
                                                { 103L, "Prema '19", 202049 },
                                                { 104L, "Trident '19", 202050 },
                                                { 105L, "Arden '19", 202051 },
                                                { 106L, "Benetton 1994", 202053 },
                                                { 107L, "Benetton 1995", 202054 },
                                                { 108L, "Ferrari 2000", 202055 },
                                                { 109L, "Jordan 1991", 202056 },
                                                { 110L, "My Team '20", 2020255 },
                                                { 111L, "Mercedes", 20210 },
                                                { 112L, "Ferrari", 20211 },
                                                { 113L, "Red Bull Racing", 20212 },
                                                { 114L, "Williams", 20213 },
                                                { 115L, "Aston Martin", 20214 },
                                                { 116L, "Alpine", 20215 },
                                                { 117L, "Alpha Tauri", 20216 },
                                                { 118L, "Haas", 20217 },
                                                { 119L, "McLaren", 20218 },
                                                { 120L, "Alfa Romeo", 20219 },
                                                { 121L, "Art GP '19", 202142 },
                                                { 122L, "Campos '19", 202143 },
                                                { 123L, "Carlin '19", 202144 },
                                                { 124L, "Sauber Junior Charouz '19", 202145 },
                                                { 125L, "Dams '19", 202146 },
                                                { 126L, "Uni-Virtuosi '19", 202147 },
                                                { 127L, "MP Motorsport '19", 202148 },
                                                { 128L, "Prema '19", 202149 },
                                                { 129L, "Trident '19", 202150 },
                                                { 130L, "Arden '19", 202151 },
                                                { 131L, "Art GP '20", 202170 },
                                                { 132L, "Campos '20", 202171 },
                                                { 133L, "Carlin '20", 202172 },
                                                { 134L, "Charouz '20", 202173 },
                                                { 135L, "Dams '20", 202174 },
                                                { 136L, "Uni-Virtuosi '20", 202175 },
                                                { 137L, "MP Motorsport '20", 202176 },
                                                { 138L, "Prema '20", 202177 },
                                                { 139L, "Trident '20", 202178 },
                                                { 140L, "BWT '20", 202179 },
                                                { 141L, "Hitech '20", 202180 },
                                                { 142L, "Mercedes 2020", 202185 },
                                                { 143L, "Ferrari 2020", 202186 },
                                                { 144L, "Red Bull 2020", 202187 },
                                                { 145L, "Williams 2020", 202188 },
                                                { 146L, "Racing Point 2020", 202189 },
                                                { 147L, "Renault 2020", 202190 },
                                                { 148L, "Alpha Tauri 2020", 202191 },
                                                { 149L, "Haas 2020", 202192 },
                                                { 150L, "McLaren 2020", 202193 },
                                                { 151L, "Alfa Romeo 2020", 202194 },
                                                { 152L, "Prema '21", 2021106 },
                                                { 153L, "Uni-Virtuosi '21", 2021107 },
                                                { 154L, "Carlin '21", 2021108 },
                                                { 155L, "Hitech '21", 2021109 },
                                                { 156L, "Art GP '21", 2021110 },
                                                { 157L, "MP Motorsport '21", 2021111 },
                                                { 158L, "Charouz '21", 2021112 },
                                                { 159L, "Dams '21", 2021113 },
                                                { 160L, "Campos '21", 2021114 },
                                                { 161L, "BWT '21", 2021115 },
                                                { 162L, "Trident '21", 2021116 },
                                                { 163L, "Mercedes", 20220 },
                                                { 164L, "Ferrari", 20221 },
                                                { 165L, "Red Bull Racing", 20222 },
                                                { 166L, "Williams", 20223 },
                                                { 167L, "Aston Martin", 20224 },
                                                { 168L, "Alpine", 20225 },
                                                { 169L, "Alpha Tauri", 20226 },
                                                { 170L, "Haas", 20227 },
                                                { 171L, "McLaren", 20228 },
                                                { 172L, "Alfa Romeo", 20229 },
                                                { 173L, "Mercedes 2020", 202285 },
                                                { 174L, "Ferrari 2020", 202286 },
                                                { 175L, "Red Bull 2020", 202287 },
                                                { 176L, "Williams 2020", 202288 },
                                                { 177L, "Racing Point 2020", 202289 },
                                                { 178L, "Renault 2020", 202290 },
                                                { 179L, "Alpha Tauri 2020", 202291 },
                                                { 180L, "Haas 2020", 202292 },
                                                { 181L, "McLaren 2020", 202293 },
                                                { 182L, "Alfa Romeo 2020", 202294 },
                                                { 183L, "Aston Martin DB11 V12", 202295 },
                                                { 184L, "Aston Martin Vantage F1 Edition", 202296 },
                                                { 185L, "Aston Martin Vantage Safety Car", 202297 },
                                                { 186L, "Ferrari F8 Tributo", 202298 },
                                                { 187L, "Ferrari Roma", 202299 },
                                                { 188L, "McLaren 720S", 2022100 },
                                                { 189L, "McLaren Artura", 2022101 },
                                                { 190L, "Mercedes AMG GT Black Series Safety Car", 2022102 },
                                                { 191L, "Mercedes AMG GTR Pro", 2022103 },
                                                { 192L, "F1 Custom Team", 2022104 },
                                                { 193L, "Prema '21", 2022106 },
                                                { 194L, "Ferrari Uni-Virtuosi '21", 2022107 },
                                                { 195L, "Carlin '21", 2022108 },
                                                { 196L, "Hitech '21", 2022109 },
                                                { 197L, "Art GP '21", 2022110 },
                                                { 198L, "MP Motorsport '21", 2022111 },
                                                { 199L, "Charouz '21", 2022112 },
                                                { 200L, "Dams '21", 2022113 },
                                                { 201L, "Campos '21", 2022114 },
                                                { 202L, "BWT '21", 2022115 },
                                                { 203L, "Trident '21", 2022116 },
                                                { 204L, "Mercedes AMG GT Black Series", 2022117 },
                                                { 205L, "Prema '22", 2022118 },
                                                { 206L, "Virtuosi '22", 2022119 },
                                                { 207L, "Carlin '22", 2022120 },
                                                { 208L, "Hitech '22", 2022121 },
                                                { 209L, "Art GP '22", 2022122 },
                                                { 210L, "MP Motorsport '22", 2022123 },
                                                { 211L, "Charouz '22", 2022124 },
                                                { 212L, "Dams '22", 2022125 },
                                                { 213L, "Campos '22", 2022126 },
                                                { 214L, "Van Amersfoort Racing '22", 2022127 },
                                                { 215L, "Trident '22", 2022128 },
                                                { 1000L, "My Team '20", 2020255 },
                                                { 1001L, "My Team '21", 2021255 },
                                                { 1002L, "My Team '22", 2022255 }
                                            });

        migrationBuilder.InsertData(table: "Tracks",
                                    columns: new[] { "Id", "Name", "TrackNumber" },
                                    values: new object[,]
                                            {
                                                { 1L, "Melbourne", 0 },
                                                { 2L, "Paul Ricard", 1 },
                                                { 3L, "Shanghai", 2 },
                                                { 4L, "Sakhir (Bahrain)", 3 },
                                                { 5L, "Catalunya", 4 },
                                                { 6L, "Monaco", 5 },
                                                { 7L, "Montreal", 6 },
                                                { 8L, "Silverstone", 7 },
                                                { 9L, "Hockenheim", 8 },
                                                { 10L, "Hungaroring", 9 },
                                                { 11L, "Spa", 10 },
                                                { 12L, "Monza", 11 },
                                                { 13L, "Singapore", 12 },
                                                { 14L, "Suzuka", 13 },
                                                { 15L, "Abu Dhabi", 14 },
                                                { 16L, "Texas", 15 },
                                                { 17L, "Brazil", 16 },
                                                { 18L, "Austria", 17 },
                                                { 19L, "Sochi", 18 },
                                                { 20L, "Mexico", 19 },
                                                { 21L, "Baku (Azerbaijan)", 20 },
                                                { 22L, "Sakhir Short", 21 },
                                                { 23L, "Silverstone Short", 22 },
                                                { 24L, "Texas Short", 23 },
                                                { 25L, "Suzuka Short", 24 },
                                                { 26L, "Hanoi", 25 },
                                                { 27L, "Zandvoort", 26 },
                                                { 28L, "Imola", 27 },
                                                { 29L, "Portimão", 28 },
                                                { 30L, "Jeddah", 29 },
                                                { 31L, "Miami", 30 }
                                            });

        migrationBuilder.CreateIndex(name: "IX_Laps_ParticipantId",
                                     table: "Laps",
                                     column: "ParticipantId");

        migrationBuilder.CreateIndex(name: "IX_Participants_DriverId",
                                     table: "Participants",
                                     column: "DriverId");

        migrationBuilder.CreateIndex(name: "IX_Participants_NationalityId",
                                     table: "Participants",
                                     column: "NationalityId");

        migrationBuilder.CreateIndex(name: "IX_Participants_SessionId",
                                     table: "Participants",
                                     column: "SessionId");

        migrationBuilder.CreateIndex(name: "IX_Participants_TeamId",
                                     table: "Participants",
                                     column: "TeamId");

        migrationBuilder.CreateIndex(name: "IX_Sessions_GameVersionId",
                                     table: "Sessions",
                                     column: "GameVersionId");

        migrationBuilder.CreateIndex(name: "IX_Sessions_TrackId",
                                     table: "Sessions",
                                     column: "TrackId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Laps");

        migrationBuilder.DropTable(name: "Participants");

        migrationBuilder.DropTable(name: "Drivers");

        migrationBuilder.DropTable(name: "Nationalities");

        migrationBuilder.DropTable(name: "Sessions");

        migrationBuilder.DropTable(name: "Teams");

        migrationBuilder.DropTable(name: "GameVersions");

        migrationBuilder.DropTable(name: "Tracks");
    }

    #endregion // Migration
}