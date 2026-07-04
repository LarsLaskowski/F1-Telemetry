# F1-Telemetry

F1-Telemetry is a service that receives UDP telemetry data from EA's F1 games
(F1 2019 and newer) and stores it in a relational database. A companion Angular
web interface visualizes the collected data. The service is built on .NET 10 and
ships as two Docker images:

| Image | Docker Hub | Purpose |
| --- | --- | --- |
| Telemetry service | [`networlddev/f1-telemetry`](https://hub.docker.com/r/networlddev/f1-telemetry) | Receives and processes telemetry packets, exposes the API |
| Web interface | [`networlddev/f1-telemetry-app`](https://hub.docker.com/r/networlddev/f1-telemetry-app) | Visualizes the stored data |

---

## Setup with Docker

You need Docker and a supported database (PostgreSQL, MariaDB/MySQL or Microsoft
SQL Server). Both images are published to Docker Hub, so no local build is
required.

### Ports

| Port | Protocol | Container | Description |
| --- | --- | --- | --- |
| `20777` | UDP | service | Telemetry port the F1 game sends packets to |
| `80` | TCP | service | REST API and SignalR hub |
| `80` | TCP | web app | Web interface (served by nginx) |

Configure the F1 game to send UDP telemetry to the host running the service on
port `20777`.

### docker-compose example

```yaml
services:
  f1-telemetry:
    image: networlddev/f1-telemetry:latest
    restart: unless-stopped
    ports:
      - "20777:20777/udp"   # telemetry from the game
      - "4820:80"           # REST API / SignalR
    environment:
      F1SERVER_DATABASE_TYPE: 3            # 1 = MariaDB, 2 = MSSQL, 3 = PostgreSQL
      F1SERVER_DB_HOST: db
      F1SERVER_DB_NAME: f1telemetry
      F1SERVER_DB_USER: f1
      F1SERVER_DB_PASSWORD: change-me
      F1SERVER_WEB: "true"
      F1SERVER_RUN_TELEMETRY_LOGGING: "false"
      TZ: Europe/Berlin
    volumes:
      - f1-logs:/var/f1-telemetry
    depends_on:
      - db

  f1-telemetry-app:
    image: networlddev/f1-telemetry-app:latest
    restart: unless-stopped
    ports:
      - "8080:80"
    environment:
      F1SERVER_HOST: f1-telemetry     # host of the service
      F1SERVER_PORT: "4820"           # published API port of the service

  db:
    image: postgres:17-alpine
    restart: unless-stopped
    environment:
      POSTGRES_DB: f1telemetry
      POSTGRES_USER: f1
      POSTGRES_PASSWORD: change-me
    volumes:
      - f1-db:/var/lib/postgresql/data

volumes:
  f1-logs:
  f1-db:
```

The database schema is created and migrated automatically on startup.

### Service configuration

| Variable | Description |
| --- | --- |
| `F1SERVER_DATABASE_TYPE` | Database provider: `1` (MariaDB/MySQL), `2` (MSSQL) or `3` (PostgreSQL). |
| `F1SERVER_DB_HOST` | Database host. |
| `F1SERVER_DB_NAME` | Database name. |
| `F1SERVER_DB_USER` | Database user. |
| `F1SERVER_DB_PASSWORD` | Database password. |
| `F1SERVER_DB_MSSQL_TRUST_SERVER_CERTIFICATE` | MSSQL only. Set to `false` to validate the server's TLS certificate instead of trusting it unconditionally. Defaults to `true`. |
| `F1SERVER_WEB` | Set to `true` to enable the REST API used by the web interface (port `80`). |
| `F1SERVER_RUN_TELEMETRY_LOGGING` | Set to `true` to write raw packet logs. Requires a volume mounted at `/var/f1-telemetry`. |
| `TZ` | Time zone used for timestamps (e.g. `Europe/Berlin`). |

### Web interface configuration

| Variable | Description |
| --- | --- |
| `F1SERVER_URL` | Full URL of the service, if it cannot be derived from host and port. |
| `F1SERVER_HOST` | Host name of the F1-Telemetry service. |
| `F1SERVER_PORT` | Published API port of the service (e.g. `4820` in the example above). |

### Health checks

Both images define a Docker `HEALTHCHECK`. The service reports on
`GET /api/serverhealth`, the web app on `GET /api/health`.

---

## Technical overview

F1-Telemetry is a multi-project .NET 10 solution. Incoming UDP packets are
parsed against the F1 packet contracts, dispatched by analyzers/factories to
specialized processors, and persisted through repositories. Multi-database
support is provided by provider-specific EF Core migration projects
(`F1Server.Db.MsSqlMigrations`, `F1Server.Db.MySqlMigrations`,
`F1Server.Db.PostgreSqlMigrations`). The `F1Server.WebApi` project exposes the
REST controllers and SignalR hubs that feed the Angular frontend.

### OpenTelemetry integration

Observability is implemented in `F1Server.Observability` around
`ObservabilityConfiguration`, which wires up traces, metrics and logs against the
[OpenTelemetry](https://opentelemetry.io/) SDK. All signals are tagged with the
resource attributes `service.name = F1-Telemetry`, `service.version = 1.0` and a
`service.instance.id` of either `Development` or `Production` (derived from
`ASPNETCORE_ENVIRONMENT`).

Observability is activated at startup only when `F1SERVER_OTLP_TARGET` is set to
a numeric value **and** at least one signal endpoint is configured. Each signal
is then enabled independently, depending on whether its endpoint variable is set.

#### Configuration

| Variable | Description |
| --- | --- |
| `F1SERVER_OTLP_TARGET` | Export target: `0` (NotSet), `1` (Console), `2` (Zipkin, obsolete), `3` (OpenTelemetry). |
| `F1SERVER_OTLP_TRACES_URL` | OTLP endpoint for traces. |
| `F1SERVER_OTLP_METRICS_URL` | OTLP endpoint for metrics. |
| `F1SERVER_OTLP_LOGGING_URL` | OTLP endpoint for logs. |
| `F1SERVER_OTLP_URL` | Legacy fallback used for the traces endpoint when `F1SERVER_OTLP_TRACES_URL` is not set. |

All OTLP exporters use the **gRPC** protocol, so endpoints should point at an
OTLP/gRPC collector (typically port `4317`), for example
`http://otel-collector:4317`.

#### Traces

Configured when `F1SERVER_OTLP_TRACES_URL` (or the legacy `F1SERVER_OTLP_URL`)
is set. Activity is captured from the sources `F1-Telemetry` and
`F1-Telemetry-WebAPI`, plus automatic **Entity Framework Core** instrumentation.
Traces are exported over OTLP/gRPC only when `F1SERVER_OTLP_TARGET` is `3`
(OpenTelemetry); for any other target they fall back to the console exporter.

#### Metrics

Configured when `F1SERVER_OTLP_METRICS_URL` is set and always exported over
OTLP/gRPC. Alongside runtime, process and event-counter instrumentation
(`Microsoft.Data.SqlClient.EventSource` and `Microsoft.EntityFrameworkCore`),
the application publishes its own meter `F1-Telemetry.Metrics`. All custom
instruments are prefixed with `f1telemetry.` and are seeded to zero on startup so
they are visible immediately. Key instruments include:

| Instrument | Type | Description |
| --- | --- | --- |
| `f1telemetry.packets_received` | Counter | Total packets received from the game. |
| `f1telemetry.packets_processed` | Counter | Total packets processed (tagged by `PacketType`). |
| `f1telemetry.packets_in_queue` | Gauge | Packets waiting to be processed. |
| `f1telemetry.packet_processing_time` | Histogram | Processing time per packet (ms). |
| `f1telemetry.processing_errors` | Counter | Packet processing errors. |
| `f1telemetry.db_errors` | Counter | Database errors. |
| `f1telemetry.packets_logged` | Counter | Packets written to the raw log. |
| `f1telemetry.<packet>_packets` | Counter | Per packet-type counters (e.g. `cartelemetry_packets`, `lapdata_packets`, `motion_packets`). |
| `f1telemetry.<packet>_packet_processing_time` | Gauge | Per packet-type processing time (ms). |

#### Logs

Configured when `F1SERVER_OTLP_LOGGING_URL` is set. A dedicated
`ILoggerFactory` exports structured logs over OTLP/gRPC (with formatted
messages, scopes and parsed state values) while also writing to the console.

---

## Project stats

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=networlddev_f1-telemetry&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=networlddev_f1-telemetry)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=networlddev_f1-telemetry&metric=bugs)](https://sonarcloud.io/summary/new_code?id=networlddev_f1-telemetry)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=networlddev_f1-telemetry&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=networlddev_f1-telemetry)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=networlddev_f1-telemetry&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=networlddev_f1-telemetry)
