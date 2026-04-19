FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

RUN apt-get update \
    && apt-get install -y --no-install-recommends brotli \
    && rm -rf /var/lib/apt/lists/*

COPY . .

# Publish the WASM client explicitly so we always get the boot manifest.
# (.NET/ABP hosting setups sometimes end up publishing the server without `blazor.boot.json`.)
RUN dotnet publish apps/blazor/src/saasLMS.Blazor.Client/saasLMS.Blazor.Client.csproj \
    -c Release \
    -o /app/client-publish \
    -p:PublishTrimmed=false \
    -p:BlazorWebAssemblyEnableLinking=false \
    -p:WasmEnableLinking=false

# Some SDK/build combinations produce the boot json at an intermediate path but do not copy it
# to the publish output. Normalize it into the expected location.
RUN set -eux; \
    if [ ! -f /app/client-publish/wwwroot/_framework/blazor.boot.json ]; then \
      BOOT_JSON="$(find /src/apps/blazor/src/saasLMS.Blazor.Client/obj -type f -name blazor.publish.boot.json -print -o -type f -name blazor.boot.json -print | head -n 1 || true)"; \
      if [ -n "$BOOT_JSON" ]; then \
        mkdir -p /app/client-publish/wwwroot/_framework; \
        cp "$BOOT_JSON" /app/client-publish/wwwroot/_framework/blazor.boot.json; \
      fi; \
    fi; \
    if [ ! -f /app/client-publish/wwwroot/_framework/blazor.boot.json ]; then \
      echo "Client publish output is missing blazor.boot.json (and no intermediate was found)."; \
      echo "Boot candidates under publish output:"; \
      find /app/client-publish/wwwroot -maxdepth 3 -type f -name '*boot*.json*' -print | sed -n '1,200p' || true; \
      echo "Boot candidates under obj:"; \
      find /src/apps/blazor/src/saasLMS.Blazor.Client/obj -maxdepth 6 -type f -name '*boot*.json*' -print | sed -n '1,200p' || true; \
      exit 1; \
    fi

RUN dotnet publish apps/blazor/src/saasLMS.Blazor/saasLMS.Blazor.csproj \
    -c Release \
    -o /app/publish \
    -p:PublishTrimmed=false

RUN set -eux; \
    # Merge client wwwroot into server publish output.
    cp -a /app/client-publish/wwwroot/. /app/publish/wwwroot/; \
    # Ensure the boot manifest exists as an uncompressed file.
    if [ ! -f /app/publish/wwwroot/_framework/blazor.boot.json ]; then \
      if [ -f /app/publish/wwwroot/_framework/blazor.boot.json.br ]; then \
        brotli -d -c /app/publish/wwwroot/_framework/blazor.boot.json.br > /app/publish/wwwroot/_framework/blazor.boot.json; \
      elif [ -f /app/publish/wwwroot/_framework/blazor.boot.json.gz ]; then \
        gzip -dc /app/publish/wwwroot/_framework/blazor.boot.json.gz > /app/publish/wwwroot/_framework/blazor.boot.json; \
      fi; \
    fi; \
    if [ ! -f /app/publish/wwwroot/_framework/blazor.boot.json ]; then \
      echo "Missing /wwwroot/_framework/blazor.boot.json in final publish output."; \
      ls -la /app/publish/wwwroot/_framework | sed -n '1,200p' || true; \
      exit 1; \
    fi

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

COPY --from=build /app/publish .
COPY etc/docker/entrypoints/run-with-secrets.sh /app/run-with-secrets.sh
RUN chmod +x /app/run-with-secrets.sh

ENTRYPOINT ["/app/run-with-secrets.sh", "dotnet", "saasLMS.Blazor.dll"]
