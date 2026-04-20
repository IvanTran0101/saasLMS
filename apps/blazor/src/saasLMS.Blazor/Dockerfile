FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .

# ABP Blazor (host) project references the client project; publishing the host is enough.
# Note: Our runtime uses `_framework/blazor.web.js` (Blazor Web App) so `blazor.boot.json`
# is not required to exist as a standalone file.
RUN dotnet publish apps/blazor/src/saasLMS.Blazor/saasLMS.Blazor.csproj \
    -c Release \
    -o /app/publish \
    -p:PublishTrimmed=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

COPY --from=build /app/publish .
COPY etc/docker/entrypoints/run-with-secrets.sh /app/run-with-secrets.sh
RUN chmod +x /app/run-with-secrets.sh

ENTRYPOINT ["/app/run-with-secrets.sh", "dotnet", "saasLMS.Blazor.dll"]
