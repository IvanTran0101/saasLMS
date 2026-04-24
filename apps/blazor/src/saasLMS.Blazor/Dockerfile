FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

RUN apt-get update \
    && apt-get install -y --no-install-recommends nodejs npm \
    && rm -rf /var/lib/apt/lists/*

COPY . .

# Bake ABP static libs into the image so production does not depend on a manager-host bind mount.
RUN dotnet tool install -g Volo.Abp.Cli --version 10.1.1 || true
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN abp install-libs --working-directory apps/blazor/src/saasLMS.Blazor

# ABP Blazor (host) project references the client project; publishing the host is enough.
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
