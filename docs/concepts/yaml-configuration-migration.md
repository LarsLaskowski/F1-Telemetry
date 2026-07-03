# Plan: Umstellung von Umgebungsvariablen auf YAML-Konfigurationsdatei

## Problem

Die gesamte Backend-Konfiguration (Datenbank, Observability, Telemetrie, Web-Hosting) erfolgt ausschließlich über `Environment.GetEnvironmentVariable()`. Es gibt keine `appsettings.json` oder andere Konfigurationsdateien. Die Umstellung auf eine YAML-Konfigurationsdatei verbessert die Wartbarkeit, Übersichtlichkeit und Versionierbarkeit der Konfiguration.

## Entscheidungen

- **Format**: YAML-Konfigurationsdatei (`f1server.yaml`)
- **Secrets**: Docker Secrets (`/run/secrets/`) bleiben für sensible Daten (DB-Passwort, InfluxDB Token)
- **Kein Fallback**: Umgebungsvariablen werden vollständig entfernt (kein Fallback)
- **Frontend**: Bleibt unverändert bei `envsubst` / `env.template.js`
- **Docker**: Dokumentation für Volume-Mount, keine docker-compose.yml

## YAML-Konfigurationsstruktur

```yaml
# f1server.yaml
database:
  type: 1                    # 1=MariaDB, 2=MSSQL, 3=PostgreSQL
  host: "localhost"
  name: "f1telemetry"
  user: "f1telemetry"
  # Passwort weiterhin über Docker Secret: /run/secrets/F1SERVER_DB_PASSWORD

web:
  enabled: true
  environment: "Production"  # Development / Production

observability:
  target: 2                  # 1=OpenTelemetry, 2=Console, other=disabled
  otlp:
    tracesUrl: ""
    metricsUrl: ""
    loggingUrl: ""

telemetry:
  logging: false
  influxDb:
    host: ""
    bucket: ""
    organization: ""
    # Token weiterhin über Docker Secret: /run/secrets/F1SERVER_TELEMETRY_TOKEN
```

## Betroffene Dateien

### Neue Dateien
- `F1Server/f1server.yaml` – Beispiel-Konfigurationsdatei
- `F1Server/f1server.Development.yaml` – Entwicklungs-Konfiguration (gitignored)
- `F1Server.Core/Configuration/F1ServerConfiguration.cs` – Stark typisiertes Konfigurationsmodell
- `F1Server.Core/Configuration/DatabaseConfiguration.cs` – DB-Konfiguration
- `F1Server.Core/Configuration/WebConfiguration.cs` – Web-Konfiguration
- `F1Server.Core/Configuration/ObservabilityYamlConfiguration.cs` – OTLP-Konfiguration
- `F1Server.Core/Configuration/TelemetryYamlConfiguration.cs` – Telemetrie-Konfiguration

### Zu ändernde Dateien
- `Directory.Packages.props` – NuGet-Paket `NetEscapades.Configuration.Yaml` hinzufügen
- `F1Server/F1Server.csproj` – NuGet-Referenz hinzufügen
- `F1Server/Program.cs` – YAML laden, `IConfiguration` aufbauen, alle `GetEnvironmentVariable` entfernen
- `F1Server.Db/Entity/F1ServerDbContext.cs` – Konfiguration per DI statt Env-Vars
- `F1Server.Db/F1Server.Db.csproj` – NuGet-Referenz ggf. ergänzen
- `F1Server.Observability/ObservabilityConfiguration.cs` – Auf Konfigurationsmodell umstellen
- `F1Server.WebApi/WebHosting.cs` – Konfiguration per DI, Secrets beibehalten
- `F1Server.Tests/TestInitializer.cs` – Test-Konfiguration via YAML statt Env-Var
- `Dockerfile` – ENV-Einträge entfernen, YAML-Mount-Punkt dokumentieren
- `.gitignore` – `f1server.Development.yaml` hinzufügen
- `README.md` – Dokumentation aktualisieren (Docker-Volume-Mount)

## Todos (parallelisierbar)

### Phase 1: Grundlagen (sequenziell)
1. **yaml-nuget** – NuGet-Paket `NetEscapades.Configuration.Yaml` in `Directory.Packages.props` und `F1Server.csproj` hinzufügen
2. **config-models** – Stark typisierte Konfigurationsklassen in `F1Server.Core/Configuration/` erstellen

### Phase 2: Refactoring (parallelisierbar – jeder Todo ist ein eigenständiger Agent)
3. **refactor-program** – `F1Server/Program.cs`: YAML-Datei laden, Configuration-Builder aufsetzen, alle `Environment.GetEnvironmentVariable()`-Aufrufe durch Konfigurationsmodell ersetzen. Die Konfigurationsmodelle per DI registrieren.
4. **refactor-dbcontext** – `F1Server.Db/Entity/F1ServerDbContext.cs`: `OnConfiguring()` und `DetectServerType()` umstellen, Konfiguration per Konstruktor-DI empfangen statt Env-Vars lesen.
5. **refactor-observability** – `F1Server.Observability/ObservabilityConfiguration.cs`: Auf Konfigurationsmodell umstellen, alle Env-Var-Zugriffe entfernen.
6. **refactor-webhosting** – `F1Server.WebApi/WebHosting.cs`: Konfiguration per DI.

### Phase 3: Infrastruktur & Tests (parallelisierbar)
7. **create-yaml-files** – `f1server.yaml` (Beispiel mit Defaults) und `f1server.Development.yaml` erstellen
8. **update-dockerfile** – `Dockerfile`: ENV-Zeilen entfernen, VOLUME für YAML-Datei konfigurieren, COPY der Default-YAML-Datei
9. **update-tests** – `F1Server.Tests/TestInitializer.cs`: Auf YAML-basierte Test-Konfiguration umstellen
10. **update-launchsettings** – `launchSettings.json`-Dateien: Umgebungsvariablen entfernen, da nicht mehr benötigt

### Phase 4: Dokumentation (parallelisierbar)
11. **update-readme** – `README.md`: Neue Konfigurationsanleitung, Docker-Volume-Mount-Beispiele
12. **update-gitignore** – `.gitignore`: `f1server.Development.yaml` aufnehmen

## Hinweise

- **Docker-Mount**: Die YAML-Datei wird in Docker per `-v /host/path/f1server.yaml:/app/f1server.yaml:ro` gemountet
- **Secrets-Pfad**: `/run/secrets/` bleibt unverändert für sensible Daten
- **YAML-Bibliothek**: `NetEscapades.Configuration.Yaml` integriert sich nahtlos in den `IConfiguration`-Stack von .NET
- **Kein Breaking Change für Frontend**: Angular-App bleibt vollständig unberührt
- **Test-Strategie**: Tests nutzen eine separate YAML-Datei mit InMemory-DB-Konfiguration (Type=99)
- **launchSettings.json**: Wird bereinigt – Entwickler verwenden stattdessen `f1server.Development.yaml`
