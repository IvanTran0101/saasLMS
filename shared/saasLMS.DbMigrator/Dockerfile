FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
WORKDIR "/src/shared/saasLMS.DbMigrator"
RUN dotnet publish -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY etc/docker/entrypoints/run-with-secrets.sh /app/run-with-secrets.sh
RUN chmod +x /app/run-with-secrets.sh
ENTRYPOINT ["/app/run-with-secrets.sh", "dotnet", "saasLMS.DbMigrator.dll"]

