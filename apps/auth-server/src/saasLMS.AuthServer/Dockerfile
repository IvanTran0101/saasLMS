FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
WORKDIR "/src/apps/auth-server/src/saasLMS.AuthServer"
RUN dotnet publish -c Release -o /app
RUN dotnet dev-certs https -v -ep /app/openiddict.pfx -p 78708b56-3dc7-46a0-9693-67e8ad61e109

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY etc/docker/entrypoints/run-with-secrets.sh /app/run-with-secrets.sh
RUN chmod +x /app/run-with-secrets.sh
ENTRYPOINT ["/app/run-with-secrets.sh", "dotnet", "saasLMS.AuthServer.dll"]
