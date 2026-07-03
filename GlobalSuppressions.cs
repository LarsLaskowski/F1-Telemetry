// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Minor Code Smell", "S1125:Boolean literals should not be redundant", Justification = "SonarCloud", Scope = "module")]
[assembly: SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static", Justification = "SonarCloud", Scope = "module")]

[assembly: SuppressMessage("Minor Code Smell", "S1192:String literals should not be duplicated", Justification = "SonarCloud", Scope = "namespace", Target = "F1Server.Db.Entity")]
[assembly: SuppressMessage("Minor Code Smell", "S1192:String literals should not be duplicated", Justification = "SonarCloud", Scope = "member", Target = "~M:F1Server.Program.CheckObservability(System.IServiceProvider)")]
[assembly: SuppressMessage("Minor Code Smell", "S1192:String literals should not be duplicated", Justification = "SonarCloud", Scope = "namespace", Target = "F1Server.Db.MySqlMigrations.Migrations")]
[assembly: SuppressMessage("Minor Code Smell", "S1192:String literals should not be duplicated", Justification = "SonarCloud", Scope = "namespace", Target = "F1Server.Db.MsSqlMigrations.Migrations")]
[assembly: SuppressMessage("Minor Code Smell", "S1192:String literals should not be duplicated", Justification = "SonarCloud", Scope = "namespace", Target = "F1Server.Db.PostgreSqlMigrations.Migrations")]

[assembly: SuppressMessage("Major Bug", "S1244:Floating point numbers should not be tested for equality", Justification = "SonarCloud", Scope = "module")]

[assembly: SuppressMessage("Major Code Smell", "S6968:Actions that return a value should be annotated with ProducesResponseTypeAttribute containing the return type", Justification = "SonarCloud", Scope = "module")]

[assembly: SuppressMessage("Maintainability", "S2094:Classes should not be empty", Justification = "SonarCloud", Scope = "module")]

[assembly: SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "Ignore DB-Migrations", Scope = "namespace", Target = "F1Server.Db.MySqlMigrations.Migrations")]
[assembly: SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "Ignore DB-Migrations", Scope = "namespace", Target = "F1Server.Db.MsSqlMigrations.Migrations")]
[assembly: SuppressMessage("Performance", "CA1861:Avoid constant arrays as arguments", Justification = "Ignore DB-Migrations", Scope = "namespace", Target = "F1Server.Db.PostgreSqlMigrations.Migrations")]