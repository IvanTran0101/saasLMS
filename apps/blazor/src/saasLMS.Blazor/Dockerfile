FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet publish apps/blazor/src/saasLMS.Blazor/saasLMS.Blazor.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

COPY --from=build /app/publish .
COPY etc/docker/entrypoints/run-with-secrets.sh /app/run-with-secrets.sh
RUN chmod +x /app/run-with-secrets.sh

# Use the same entrypoint as DbMigrator/base image to map Docker Swarm secrets -> env vars.
ENTRYPOINT ["/app/run-with-secrets.sh", "dotnet", "saasLMS.Blazor.dll"]
