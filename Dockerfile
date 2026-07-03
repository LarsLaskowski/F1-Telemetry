ARG BASE_SDK="mcr.microsoft.com/dotnet/sdk:10.0-alpine"
ARG BASE_RUNTIME="mcr.microsoft.com/dotnet/aspnet:10.0-alpine"
ARG BASE_RUNTIME_DIGEST

FROM ${BASE_SDK} AS build

WORKDIR /src

# Copy Directory.Build.props and Directory.Packages.props first for Central Package Management
COPY ["./Directory.Build.props", "./"]
COPY ["./Directory.Packages.props", "./"]
COPY ["./nuget.config", "./"]

COPY ["./F1Server/F1Server.csproj", "./F1Server/"]
COPY ["./F1Server.Core/F1Server.Core.csproj", "./F1Server.Core/"]
COPY ["./F1Server.Db/F1Server.Db.csproj", "./F1Server.Db/"]
COPY ["./F1Server.Db.MsSqlMigrations/F1Server.Db.MsSqlMigrations.csproj", "./F1Server.Db.MsSqlMigrations/"]
COPY ["./F1Server.Db.MySqlMigrations/F1Server.Db.MySqlMigrations.csproj", "./F1Server.Db.MySqlMigrations/"]
COPY ["./F1Server.Db.PostgreSqlMigrations/F1Server.Db.PostgreSqlMigrations.csproj", "./F1Server.Db.PostgreSqlMigrations/"]
COPY ["./F1Server.Telemetry/F1Server.Telemetry.csproj", "./F1Server.Telemetry/"]
COPY ["./F1Server.Observability/F1Server.Observability.csproj", "./F1Server.Observability/"]
COPY ["./F1Server.Data/F1Server.Data.csproj", "./F1Server.Data/"]
COPY ["./F1Server.Service/F1Server.Service.csproj", "./F1Server.Service/"]
COPY ["./F1Server.WebApi/F1Server.WebApi.csproj", "./F1Server.WebApi/"]

RUN dotnet restore "./F1Server/F1Server.csproj" --configfile ./nuget.config

COPY . .

WORKDIR "/src"

RUN dotnet build "./F1Server/F1Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./F1Server/F1Server.csproj" -c Release -o /app /p:UseAppHost=false --no-restore

FROM ${BASE_RUNTIME} AS base
ARG BASE_RUNTIME
ARG BASE_RUNTIME_DIGEST

LABEL org.opencontainers.image.base.name="${BASE_RUNTIME}"
LABEL org.opencontainers.image.base.digest="${BASE_RUNTIME_DIGEST}"

EXPOSE 20777/udp
EXPOSE 80
EXPOSE 4820

RUN apk add curl
HEALTHCHECK --interval=5m --timeout=3s --retries=5 CMD curl --fail http://localhost/api/serverhealth || exit 1

WORKDIR /app
COPY --from=publish /app ./

ENV F1SERVER_RUN_TELEMETRY_LOGGING=false
ENV F1SERVER_DATABASE_TYPE=1
ENV F1SERVER_WEB=true
ENV F1SERVER_OTLP_TARGET=2
ENV TZ=

VOLUME ["/var/f1-telemetry"]
VOLUME ["/root/.aspnet"]

ENTRYPOINT ["dotnet", "F1Server.dll"]
