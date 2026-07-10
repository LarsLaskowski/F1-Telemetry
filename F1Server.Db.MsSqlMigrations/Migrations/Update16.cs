using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace F1Server.Db.MsSqlMigrations.Migrations;

/// <inheritdoc />
public partial class Update16 : Migration
{
    #region Migration

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "Tracks",
                                             type: "nvarchar(100)",
                                             maxLength: 100,
                                             nullable: false,
                                             defaultValue: string.Empty,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(100)",
                                             oldMaxLength: 100,
                                             oldNullable: true);

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "Teams",
                                             type: "nvarchar(100)",
                                             maxLength: 100,
                                             nullable: false,
                                             defaultValue: string.Empty,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(100)",
                                             oldMaxLength: 100,
                                             oldNullable: true);

        migrationBuilder.AlterColumn<string>(name: "DriverName",
                                             table: "Participants",
                                             type: "nvarchar(max)",
                                             nullable: false,
                                             defaultValue: string.Empty,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(max)",
                                             oldNullable: true);

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "Nationalities",
                                             type: "nvarchar(100)",
                                             maxLength: 100,
                                             nullable: false,
                                             defaultValue: string.Empty,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(100)",
                                             oldMaxLength: 100,
                                             oldNullable: true);

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "GameVersions",
                                             type: "nvarchar(max)",
                                             nullable: false,
                                             defaultValue: string.Empty,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(max)",
                                             oldNullable: true);

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "Drivers",
                                             type: "nvarchar(100)",
                                             maxLength: 100,
                                             nullable: false,
                                             defaultValue: string.Empty,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(100)",
                                             oldMaxLength: 100,
                                             oldNullable: true);

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 29L,
                                    column: "Name",
                                    value: "Naota Izumi");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 121L,
                                    column: "Name",
                                    value: "Ayumu Iwasa");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 122L,
                                    column: "Name",
                                    value: "Clement Novalak");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 185L,
                                    column: "Name",
                                    value: "Naota Izumi");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 239L,
                                    column: "Name",
                                    value: "Zak O'Sullivan");

        migrationBuilder.InsertData(table: "Drivers",
                                    columns: new[] { "Id", "IsHumanDriver", "DriverGameId", "Name" },
                                    values: new object[,]
                                            {
                                                { 248L, 0, 20260, "Carlos Sainz" },
                                                { 249L, 0, 20262, "Daniel Ricciardo" },
                                                { 250L, 0, 20263, "Fernando Alonso" },
                                                { 251L, 0, 20264, "Felipe Massa" },
                                                { 252L, 0, 20267, "Lewis Hamilton" },
                                                { 253L, 0, 20269, "Max Verstappen" },
                                                { 254L, 0, 202610, "Nico Hülkenberg" },
                                                { 255L, 0, 202611, "Kevin Magnussen" },
                                                { 256L, 0, 202614, "Sergio Perez" },
                                                { 257L, 0, 202615, "Valtteri Bottas" },
                                                { 258L, 0, 202617, "Esteban Ocon" },
                                                { 259L, 0, 202619, "Lance Stroll" },
                                                { 260L, 0, 202620, "Arron Barnes" },
                                                { 261L, 0, 202621, "Martin Giles" },
                                                { 262L, 0, 202622, "Alex Murray" },
                                                { 263L, 0, 202623, "Lucas Roth" },
                                                { 264L, 0, 202624, "Igor Correia" },
                                                { 265L, 0, 202625, "Sophie Levasseur" },
                                                { 266L, 0, 202626, "Jonas Schiffer" },
                                                { 267L, 0, 202627, "Alain Forest" },
                                                { 268L, 0, 202628, "Jay Letourneau" },
                                                { 269L, 0, 202629, "Esto Saari" },
                                                { 270L, 0, 202630, "Yasar Atiyeh" },
                                                { 271L, 0, 202631, "Callisto Calabresi" },
                                                { 272L, 0, 202632, "Naota Izumi" },
                                                { 273L, 0, 202633, "Howard Clarke" },
                                                { 274L, 0, 202634, "Lars Kaufmann" },
                                                { 275L, 0, 202635, "Marie Laursen" },
                                                { 276L, 0, 202636, "Flavio Nieves" },
                                                { 277L, 0, 202638, "Klimek Michalski" },
                                                { 278L, 0, 202639, "Santiago Moreno" },
                                                { 279L, 0, 202640, "Benjamin Coppens" },
                                                { 280L, 0, 202641, "Noah Visser" },
                                                { 281L, 0, 202650, "George Russell" },
                                                { 282L, 0, 202654, "Lando Norris" },
                                                { 283L, 0, 202658, "Charles Leclerc" },
                                                { 284L, 0, 202659, "Pierre Gasly" },
                                                { 285L, 0, 202662, "Alexander Albon" },
                                                { 286L, 0, 202670, "Rashid Nair" },
                                                { 287L, 0, 202671, "Jack Tremblay" },
                                                { 288L, 0, 202677, "Ayrton Senna" },
                                                { 289L, 0, 202680, "Guanya Zhou" },
                                                { 290L, 0, 202683, "Juan Manuel Correa" },
                                                { 291L, 0, 202690, "Michael Schumacher" },
                                                { 292L, 0, 202694, "Yuki Tsunoda" },
                                                { 293L, 0, 2026102, "Aidan Jackson" },
                                                { 294L, 0, 2026109, "Jenson Button" },
                                                { 295L, 0, 2026110, "David Coulthard" },
                                                { 296L, 0, 2026112, "Oscar Piastri" },
                                                { 297L, 0, 2026113, "Liam Lawson" },
                                                { 298L, 0, 2026116, "Richard Verschoor" },
                                                { 299L, 0, 2026123, "Enzo Fittipaldi" },
                                                { 300L, 0, 2026125, "Mark Webber" },
                                                { 301L, 0, 2026126, "Jacques Villeneuve" },
                                                { 302L, 0, 2026127, "Callie Mayer" },
                                                { 303L, 0, 2026132, "Logan Sargeant" },
                                                { 304L, 0, 2026136, "Jack Doohan" },
                                                { 305L, 0, 2026137, "Amaury Cordeel" },
                                                { 306L, 0, 2026138, "Dennis Hauger" },
                                                { 307L, 0, 2026145, "Zane Maloney" },
                                                { 308L, 0, 2026146, "Victor Martins" },
                                                { 309L, 0, 2026147, "Oliver Bearman" },
                                                { 310L, 0, 2026148, "Jak Crawford" },
                                                { 311L, 0, 2026149, "Isack Hadjar" },
                                                { 312L, 0, 2026152, "Roman Stanek" },
                                                { 313L, 0, 2026153, "Kush Maini" },
                                                { 314L, 0, 2026156, "Brendon Leigh" },
                                                { 315L, 0, 2026157, "David Tonizza" },
                                                { 316L, 0, 2026158, "Jarno Opmeer" },
                                                { 317L, 0, 2026159, "Lucas Blakeley" },
                                                { 318L, 0, 2026160, "Paul Aron" },
                                                { 319L, 0, 2026161, "Gabriel Bortoleto" },
                                                { 320L, 0, 2026162, "Franco Colapinto" },
                                                { 321L, 0, 2026163, "Taylor Barnard" },
                                                { 322L, 0, 2026164, "Joshua Dürksen" },
                                                { 323L, 0, 2026165, "Andrea-Kimi Antonelli" },
                                                { 324L, 0, 2026166, "Ritomo Miyata" },
                                                { 325L, 0, 2026167, "Rafael Villagómez" },
                                                { 326L, 0, 2026168, "Zak O'Sullivan" },
                                                { 327L, 0, 2026169, "Pepe Marti" },
                                                { 328L, 0, 2026170, "Sonny Hayes" },
                                                { 329L, 0, 2026171, "Joshua Pearce" },
                                                { 330L, 0, 2026172, "Callum Voisin" },
                                                { 331L, 0, 2026173, "Matias Zagazeta" },
                                                { 332L, 0, 2026174, "Nikola Tsolov" },
                                                { 333L, 0, 2026175, "Tim Tramnitz" },
                                                { 334L, 0, 2026185, "Luca Cortez" },
                                                { 335L, 0, 2026186, "Luke Browning" },
                                                { 336L, 0, 2026187, "Cian Shields" },
                                                { 337L, 0, 2026188, "Arvid Lindblad" },
                                                { 338L, 0, 2026189, "Dino Beganovic" },
                                                { 339L, 0, 2026190, "Leonardo Fornaroli" },
                                                { 340L, 0, 2026191, "Oliver Goethe" },
                                                { 341L, 0, 2026192, "Gabriele Minì" },
                                                { 342L, 0, 2026193, "Sebastián Montoya" },
                                                { 343L, 0, 2026194, "Alexander Dunne" },
                                                { 344L, 0, 2026195, "Max Esterson" },
                                                { 345L, 0, 2026196, "Sami Meguetounif" },
                                                { 346L, 0, 2026197, "John Bennett" }
                                            });

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 95L,
                                    column: "Name",
                                    value: "F1 Generic");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 222L,
                                    column: "Name",
                                    value: "RB");

        migrationBuilder.InsertData(table: "Teams",
                                    columns: new[] { "Id", "Name", "TeamGameId" },
                                    values: new object[,]
                                            {
                                                { 341L, "Mercedes", 20260 },
                                                { 342L, "Ferrari", 20261 },
                                                { 343L, "Red Bull Racing", 20262 },
                                                { 344L, "Williams", 20263 },
                                                { 345L, "Aston Martin", 20264 },
                                                { 346L, "Alpine", 20265 },
                                                { 347L, "RB", 20266 },
                                                { 348L, "Haas", 20267 },
                                                { 349L, "McLaren", 20268 },
                                                { 350L, "Sauber", 20269 },
                                                { 351L, "F1 Generic", 202641 },
                                                { 352L, "F1 Custom Team", 2026104 },
                                                { 353L, "Konnersport", 2026129 },
                                                { 354L, "APXGP '24", 2026142 },
                                                { 355L, "APXGP '25", 2026154 },
                                                { 356L, "Konnersport '24", 2026155 },
                                                { 357L, "Art GP '24", 2026158 },
                                                { 358L, "Campos '24", 2026159 },
                                                { 359L, "Rodin Motorsport '24", 2026160 },
                                                { 360L, "AIX Racing '24", 2026161 },
                                                { 361L, "DAMS '24", 2026162 },
                                                { 362L, "Hitech '24", 2026163 },
                                                { 363L, "MP Motorsport '24", 2026164 },
                                                { 364L, "Prema '24", 2026165 },
                                                { 365L, "Trident '24", 2026166 },
                                                { 366L, "Van Amersfoort Racing '24", 2026167 },
                                                { 367L, "Invicta '24", 2026168 },
                                                { 368L, "Mercedes '24", 2026185 },
                                                { 369L, "Ferrari '24", 2026186 },
                                                { 370L, "Red Bull Racing '24", 2026187 },
                                                { 371L, "Williams '24", 2026188 },
                                                { 372L, "Aston Martin '24", 2026189 },
                                                { 373L, "Alpine '24", 2026190 },
                                                { 374L, "RB '24", 2026191 },
                                                { 375L, "Haas '24", 2026192 },
                                                { 376L, "McLaren '24", 2026193 },
                                                { 377L, "Sauber '24", 2026194 },
                                                { 378L, "Art GP '25", 2026465 },
                                                { 379L, "Campos '25", 2026466 },
                                                { 380L, "Rodin Motorsport '25", 2026467 },
                                                { 381L, "AIX Racing '25", 2026468 },
                                                { 382L, "DAMS '25", 2026469 },
                                                { 383L, "Hitech '25", 2026470 },
                                                { 384L, "MP Motorsport '25", 2026471 },
                                                { 385L, "Prema '25", 2026472 },
                                                { 386L, "Trident '25", 2026473 },
                                                { 387L, "Van Amersfoort Racing '25", 2026474 },
                                                { 388L, "Invicta '25", 2026475 },
                                                { 389L, "Mercedes '26", 2026476 },
                                                { 390L, "Ferrari '26", 2026477 },
                                                { 391L, "Red Bull Racing '26", 2026478 },
                                                { 392L, "Williams '26", 2026479 },
                                                { 393L, "Aston Martin '26", 2026480 },
                                                { 394L, "Alpine '26", 2026481 },
                                                { 395L, "RB '26", 2026482 },
                                                { 396L, "Haas '26", 2026483 },
                                                { 397L, "McLaren '26", 2026484 },
                                                { 398L, "Audi '26", 2026485 },
                                                { 399L, "Cadillac '26", 2026486 },
                                                { 1006L, "My Team '26", 2026255 }
                                            });

        migrationBuilder.InsertData(table: "Tracks",
                                    columns: new[] { "Id", "LapReferenceTime", "Name", "Sector1ReferenceTime", "Sector2ReferenceTime", "Sector3ReferenceTime", "TrackNumber" },
                                    values: new object[] { 37L, 0L, "Madrid", 0L, 0L, 0L, 42 });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 248L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 249L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 250L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 251L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 252L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 253L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 254L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 255L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 256L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 257L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 258L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 259L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 260L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 261L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 262L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 263L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 264L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 265L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 266L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 267L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 268L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 269L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 270L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 271L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 272L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 273L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 274L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 275L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 276L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 277L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 278L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 279L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 280L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 281L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 282L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 283L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 284L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 285L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 286L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 287L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 288L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 289L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 290L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 291L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 292L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 293L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 294L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 295L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 296L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 297L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 298L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 299L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 300L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 301L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 302L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 303L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 304L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 305L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 306L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 307L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 308L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 309L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 310L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 311L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 312L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 313L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 314L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 315L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 316L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 317L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 318L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 319L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 320L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 321L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 322L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 323L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 324L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 325L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 326L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 327L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 328L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 329L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 330L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 331L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 332L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 333L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 334L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 335L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 336L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 337L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 338L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 339L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 340L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 341L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 342L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 343L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 344L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 345L);

        migrationBuilder.DeleteData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 346L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 341L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 342L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 343L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 344L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 345L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 346L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 347L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 348L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 349L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 350L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 351L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 352L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 353L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 354L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 355L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 356L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 357L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 358L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 359L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 360L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 361L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 362L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 363L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 364L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 365L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 366L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 367L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 368L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 369L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 370L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 371L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 372L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 373L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 374L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 375L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 376L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 377L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 378L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 379L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 380L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 381L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 382L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 383L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 384L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 385L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 386L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 387L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 388L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 389L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 390L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 391L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 392L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 393L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 394L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 395L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 396L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 397L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 398L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 399L);

        migrationBuilder.DeleteData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 1006L);

        migrationBuilder.DeleteData(table: "Tracks",
                                    keyColumn: "Id",
                                    keyValue: 37L);

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "Tracks",
                                             type: "nvarchar(100)",
                                             maxLength: 100,
                                             nullable: true,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(100)",
                                             oldMaxLength: 100);

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "Teams",
                                             type: "nvarchar(100)",
                                             maxLength: 100,
                                             nullable: true,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(100)",
                                             oldMaxLength: 100);

        migrationBuilder.AlterColumn<string>(name: "DriverName",
                                             table: "Participants",
                                             type: "nvarchar(max)",
                                             nullable: true,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "Nationalities",
                                             type: "nvarchar(100)",
                                             maxLength: 100,
                                             nullable: true,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(100)",
                                             oldMaxLength: 100);

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "GameVersions",
                                             type: "nvarchar(max)",
                                             nullable: true,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(name: "Name",
                                             table: "Drivers",
                                             type: "nvarchar(100)",
                                             maxLength: 100,
                                             nullable: true,
                                             oldClrType: typeof(string),
                                             oldType: "nvarchar(100)",
                                             oldMaxLength: 100);

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 29L,
                                    column: "Name",
                                    value: "Naota Izum");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 121L,
                                    column: "Name",
                                    value: "Ayuma Iwasa");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 122L,
                                    column: "Name",
                                    value: "Clement Novolak");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 185L,
                                    column: "Name",
                                    value: "Naota Izum");

        migrationBuilder.UpdateData(table: "Drivers",
                                    keyColumn: "Id",
                                    keyValue: 239L,
                                    column: "Name",
                                    value: "Zak O’Sullivan");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 95L,
                                    column: "Name",
                                    value: "F1 Generic car");

        migrationBuilder.UpdateData(table: "Teams",
                                    keyColumn: "Id",
                                    keyValue: 222L,
                                    column: "Name",
                                    value: "Alpha Tauri");
    }

    #endregion // Migration
}