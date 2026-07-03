using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace F1Server.Db.PostgreSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update10 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(table: "Drivers",
                                    columns: new[] { "Id", "IsHumanDriver", "DriverGameId", "Name" },
                                    values: new object[,]
                                            {
                                                { 161L, 0, 20250, "Carlos Sainz" },
                                                { 162L, 0, 20252, "Daniel Ricciardo" },
                                                { 163L, 0, 20253, "Fernando Alonso" },
                                                { 164L, 0, 20254, "Felipe Massa" },
                                                { 165L, 0, 20257, "Lewis Hamilton" },
                                                { 166L, 0, 20259, "Max Verstappen" },
                                                { 167L, 0, 202510, "Nico Hülkenberg" },
                                                { 168L, 0, 202511, "Kevin Magnussen" },
                                                { 169L, 0, 202514, "Sergio Perez" },
                                                { 170L, 0, 202515, "Valtteri Bottas" },
                                                { 171L, 0, 202517, "Esteban Ocon" },
                                                { 172L, 0, 202519, "Lance Stroll" },
                                                { 173L, 0, 202520, "Arron Barnes" },
                                                { 174L, 0, 202521, "Martin Giles" },
                                                { 175L, 0, 202522, "Alex Murray" },
                                                { 176L, 0, 202523, "Lucas Roth" },
                                                { 177L, 0, 202524, "Igor Correia" },
                                                { 178L, 0, 202525, "Sophie Levasseur" },
                                                { 179L, 0, 202526, "Jonas Schiffer" },
                                                { 180L, 0, 202527, "Alain Forest" },
                                                { 181L, 0, 202528, "Jay Letourneau" },
                                                { 182L, 0, 202529, "Esto Saari" },
                                                { 183L, 0, 202530, "Yasar Atiyeh" },
                                                { 184L, 0, 202531, "Callisto Calabresi" },
                                                { 185L, 0, 202532, "Naota Izum" },
                                                { 186L, 0, 202533, "Howard Clarke" },
                                                { 187L, 0, 202534, "Lars Kaufmann" },
                                                { 188L, 0, 202535, "Marie Laursen" },
                                                { 189L, 0, 202536, "Flavio Nieves" },
                                                { 190L, 0, 202538, "Klimek Michalski" },
                                                { 191L, 0, 202539, "Santiago Moreno" },
                                                { 192L, 0, 202540, "Benjamin Coppens" },
                                                { 193L, 0, 202541, "Noah Visser" },
                                                { 194L, 0, 202550, "George Russell" },
                                                { 195L, 0, 202554, "Lando Norris" },
                                                { 196L, 0, 202558, "Charles Leclerc" },
                                                { 197L, 0, 202559, "Pierre Gasly" },
                                                { 198L, 0, 202562, "Alexander Albon" },
                                                { 199L, 0, 202570, "Rashid Nair" },
                                                { 200L, 0, 202571, "Jack Tremblay" },
                                                { 201L, 0, 202577, "Ayrton Senna" },
                                                { 202L, 0, 202580, "Guanya Zhou" },
                                                { 203L, 0, 202583, "Juan Manuel Correa" },
                                                { 204L, 0, 202590, "Michael Schumacher" },
                                                { 205L, 0, 202594, "Yuki Tsunoda" },
                                                { 206L, 0, 2025102, "Aidan Jackson" },
                                                { 207L, 0, 2025109, "Jenson Button" },
                                                { 208L, 0, 2025110, "David Coulthard" },
                                                { 209L, 0, 2025112, "Oscar Piastri" },
                                                { 210L, 0, 2025113, "Liam Lawson" },
                                                { 211L, 0, 2025116, "Richard Verschoor" },
                                                { 212L, 0, 2025123, "Enzo Fittipaldi" },
                                                { 213L, 0, 2025125, "Mark Webber" },
                                                { 214L, 0, 2025126, "Jacques Villeneuve" },
                                                { 215L, 0, 2025127, "Callie Mayer" },
                                                { 216L, 0, 2025132, "Logan Sargeant" },
                                                { 217L, 0, 2025136, "Jack Doohan" },
                                                { 218L, 0, 2025137, "Amaury Cordeel" },
                                                { 219L, 0, 2025138, "Dennis Hauger" },
                                                { 220L, 0, 2025145, "Zane Maloney" },
                                                { 221L, 0, 2025146, "Victor Martins" },
                                                { 222L, 0, 2025147, "Oliver Bearman" },
                                                { 223L, 0, 2025148, "Jak Crawford" },
                                                { 224L, 0, 2025149, "Isack Hadjar" },
                                                { 225L, 0, 2025152, "Roman Stanek" },
                                                { 226L, 0, 2025153, "Kush Maini" },
                                                { 227L, 0, 2025156, "Brendon Leigh" },
                                                { 228L, 0, 2025157, "David Tonizza" },
                                                { 229L, 0, 2025158, "Jarno Opmeer" },
                                                { 230L, 0, 2025159, "Lucas Blakeley" },
                                                { 231L, 0, 2025160, "Paul Aron" },
                                                { 232L, 0, 2025161, "Gabriel Bortoleto" },
                                                { 233L, 0, 2025162, "Franco Colapinto" },
                                                { 234L, 0, 2025163, "Taylor Barnard" },
                                                { 235L, 0, 2025164, "Joshua Dürksen" },
                                                { 236L, 0, 2025165, "Andrea-Kimi Antonelli" },
                                                { 237L, 0, 2025166, "Ritomo Miyata" },
                                                { 238L, 0, 2025167, "Rafael Villagómez" },
                                                { 239L, 0, 2025168, "Zak O’Sullivan" },
                                                { 240L, 0, 2025169, "Pepe Marti" },
                                                { 241L, 0, 2025170, "Sonny Hayes" },
                                                { 242L, 0, 2025171, "Joshua Pearce" },
                                                { 243L, 0, 2025172, "Callum Voisin" },
                                                { 244L, 0, 2025173, "Matias Zagazeta" },
                                                { 245L, 0, 2025174, "Nikola Tsolov" },
                                                { 246L, 0, 2025175, "Tim Tramnitz" },
                                                { 247L, 0, 2025185, "Luca Cortez" }
                                            });

        migrationBuilder.InsertData(table: "Teams",
                                    columns: new[] { "Id", "Name", "TeamGameId" },
                                    values: new object[,]
                                            {
                                                { 304L, "Mercedes", 20250 },
                                                { 305L, "Ferrari", 20251 },
                                                { 306L, "Red Bull Racing", 20252 },
                                                { 307L, "Williams", 20253 },
                                                { 308L, "Aston Martin", 20254 },
                                                { 309L, "Alpine", 20255 },
                                                { 310L, "RB", 20256 },
                                                { 311L, "Haas", 20257 },
                                                { 312L, "McLaren", 20258 },
                                                { 313L, "Sauber", 20259 },
                                                { 314L, "F1 Generic", 202541 },
                                                { 315L, "F1 Custom Team", 2025104 },
                                                { 316L, "Konnersport", 2025129 },
                                                { 317L, "APXGP '24", 2025142 },
                                                { 318L, "APXGP '25", 2025154 },
                                                { 319L, "Konnersport '24", 2025155 },
                                                { 320L, "Art GP '24", 2025158 },
                                                { 321L, "Campos '24", 2025159 },
                                                { 322L, "Rodin Motorsport '24", 2025160 },
                                                { 323L, "AIX Racing '24", 2025161 },
                                                { 324L, "DAMS '24", 2025162 },
                                                { 325L, "Hitech '24", 2025163 },
                                                { 326L, "MP Motorsport '24", 2025164 },
                                                { 327L, "Prema '24", 2025165 },
                                                { 328L, "Trident '24", 2025166 },
                                                { 329L, "Van Amersfoort Racing '24", 2025167 },
                                                { 330L, "Invicta '24", 2025168 },
                                                { 331L, "Mercedes '24", 2025185 },
                                                { 332L, "Ferrari '24", 2025186 },
                                                { 333L, "Red Bull Racing '24", 2025187 },
                                                { 334L, "Williams '24", 2025188 },
                                                { 335L, "Aston Martin '24", 2025189 },
                                                { 336L, "Alpine '24", 2025190 },
                                                { 337L, "RB '24", 2025191 },
                                                { 338L, "Haas '24", 2025192 },
                                                { 339L, "McLaren '24", 2025193 },
                                                { 340L, "Sauber '24", 2025194 },
                                                { 1005L, "My Team '25", 2025255 }
                                            });

        migrationBuilder.InsertData(table: "Tracks",
                                    columns: new[] { "Id", "LapReferenceTime", "Name", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime", "TrackNumber" },
                                    values: new object[,]
                                            {
                                                { 34L, 0L, "Silverstone (Reverse)", 0L, 0L, 0L, 39 },
                                                { 35L, 0L, "Austria (Reverse)", 0L, 0L, 0L, 40 },
                                                { 36L, 0L, "Zandvoort (Reverse)", 0L, 0L, 0L, 41 }
                                            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 161L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 162L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 163L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 164L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 165L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 166L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 167L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 168L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 169L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 170L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 171L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 172L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 173L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 174L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 175L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 176L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 177L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 178L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 179L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 180L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 181L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 182L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 183L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 184L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 185L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 186L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 187L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 188L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 189L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 190L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 191L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 192L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 193L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 194L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 195L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 196L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 197L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 198L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 199L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 200L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 201L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 202L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 203L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 204L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 205L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 206L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 207L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 208L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 209L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 210L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 211L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 212L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 213L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 214L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 215L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 216L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 217L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 218L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 219L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 220L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 221L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 222L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 223L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 224L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 225L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 226L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 227L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 228L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 229L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 230L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 231L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 232L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 233L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 234L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 235L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 236L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 237L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 238L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 239L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 240L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 241L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 242L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 243L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 244L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 245L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 246L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 247L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 304L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 305L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 306L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 307L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 308L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 309L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 310L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 311L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 312L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 313L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 314L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 315L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 316L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 317L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 318L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 319L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 320L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 321L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 322L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 323L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 324L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 325L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 326L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 327L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 328L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 329L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 330L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 331L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 332L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 333L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 334L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 335L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 336L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 337L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 338L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 339L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 340L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 1005L);

        migrationBuilder.DeleteData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 34L);

        migrationBuilder.DeleteData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 35L);

        migrationBuilder.DeleteData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 36L);
    }

    #endregion // Migration
}