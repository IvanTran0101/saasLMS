## syntax=docker/dockerfile:1.7

ARG DOTNET_SDK_IMAGE=mcr.microsoft.com/dotnet/sdk:10.0
ARG DOTNET_RUNTIME_IMAGE=mcr.microsoft.com/dotnet/aspnet:10.0

FROM ${DOTNET_SDK_IMAGE} AS build
WORKDIR /src

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
    NUGET_XMLDOC_MODE=skip

COPY . .

RUN dotnet publish "services/administration/src/saasLMS.AdministrationService.HttpApi.Host/saasLMS.AdministrationService.HttpApi.Host.csproj" \
    -c Release \
    -o /app/publish \
    -p:UseAppHost=false \
    -p:ContinuousIntegrationBuild=true

# Hard safety: appsettings.secrets.json is dev-only and must not ship in production images.
RUN rm -f /app/publish/appsettings.secrets.json

FROM ${DOTNET_RUNTIME_IMAGE} AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80 \
    ENTRY_DLL=saasLMS.AdministrationService.HttpApi.Host.dll
EXPOSE 80

COPY --from=build /app/publish/ ./
COPY "etc/docker/entrypoints/run-with-secrets.sh" "/app/run-with-secrets.sh"
RUN chmod +x "/app/run-with-secrets.sh"

ENTRYPOINT ["/app/run-with-secrets.sh"]
CMD ["sh", "-c", "test -n \"$ENTRY_DLL\" || { echo 'ENTRY_DLL is required.'; exit 1; }; exec dotnet \"$ENTRY_DLL\""]
