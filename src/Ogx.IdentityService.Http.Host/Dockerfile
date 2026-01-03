FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# Metadata labels
LABEL org.opencontainers.image.title="HsnSoft Ogx Identity Service" \
      org.opencontainers.image.description="HsnSoft Ogx Identity Service" \
      org.opencontainers.image.authors="HsnSoft <admin@hsnsoft.com>" \
      org.opencontainers.image.source="https://github.com/hsn-soft/ogx-service-identity"

USER root

WORKDIR /app
EXPOSE 8080

ENV DOTNET_EnableDiagnostics=0 \
    ASPNETCORE_URLS='http://+:8080;' \
    ASPNETCORE_ENVIRONMENT='docker'

RUN apt-get update && \
    apt-get install -y --no-install-recommends curl && \
    rm -rf /var/lib/apt/lists/*

# Health check endpoint (varsayılan: /health)
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl --fail http://localhost:8080/LivenessCheck || exit 1

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-stage
WORKDIR /build-source

# Non-root user setup
RUN groupadd -r -g 1001 servicegroup && \
    useradd -r -u 1001 serviceuser -g servicegroup

# Configuration files
COPY ["./nuget.config", "./"]
COPY ["./common.props", "./"]
COPY ["./common.version.props", "./"]
COPY ["./public_key.xml", "./"]

COPY ["./src/Ogx.IdentityService.Application/Ogx.IdentityService.Application.csproj", "./src/Ogx.IdentityService.Application/"]
COPY ["./src/Ogx.IdentityService.Domain/Ogx.IdentityService.Domain.csproj", "./src/Ogx.IdentityService.Domain/"]
COPY ["./src/Ogx.IdentityService.EntityFrameworkCore/Ogx.IdentityService.EntityFrameworkCore.csproj", "./src/Ogx.IdentityService.EntityFrameworkCore/"]
COPY ["./src/Ogx.IdentityService.Http.Host/Ogx.IdentityService.Http.Host.csproj", "./src/Ogx.IdentityService.Http.Host/"]

ENV NUGET_CERT_REVOCATION_MODE=offline

# NuGet restore with secrets
RUN --mount=type=secret,id=NUGET_USER \
    --mount=type=secret,id=NUGET_TOKEN \
    export NUGET_USER=$(cat /run/secrets/NUGET_USER) && \
    export NUGET_TOKEN=$(cat /run/secrets/NUGET_TOKEN) && \
    dotnet nuget update source github \
        --username $NUGET_USER \
        --password $NUGET_TOKEN \
        --store-password-in-clear-text

RUN dotnet restore "./src/Ogx.IdentityService.Http.Host/Ogx.IdentityService.Http.Host.csproj" --force --verbosity minimal

# Copy source code
COPY --chown=1001:1001 ["./src/Ogx.IdentityService.Application/.", "./src/Ogx.IdentityService.Application/"]
COPY --chown=1001:1001 ["./src/Ogx.IdentityService.Domain/.", "./src/Ogx.IdentityService.Domain/"]
COPY --chown=1001:1001 ["./src/Ogx.IdentityService.EntityFrameworkCore/.", "./src/Ogx.IdentityService.EntityFrameworkCore/"]
COPY --chown=1001:1001 ["./src/Ogx.IdentityService.Http.Host/.", "./src/Ogx.IdentityService.Http.Host/"]

# Build
RUN dotnet build "./src/Ogx.IdentityService.Http.Host/Ogx.IdentityService.Http.Host.csproj" --no-restore --no-incremental --verbosity minimal --configuration Release

# (Opsiyonel) Test
RUN dotnet test "./src/Ogx.IdentityService.Http.Host/Ogx.IdentityService.Http.Host.csproj" --no-restore --no-build --verbosity minimal --configuration Release --filter "category!=integration"

# Publish
RUN dotnet publish "./src/Ogx.IdentityService.Http.Host/Ogx.IdentityService.Http.Host.csproj" \
    --no-restore \
    --no-build \
    --configuration Release \
    --output /build-source/publish \
    /p:UseAppHost=false

# Final Runtime Image
FROM base AS final
WORKDIR /app
COPY --chown=1001:1001 --from=build-stage /build-source/publish .

# Runtime User
USER 1001

ENTRYPOINT ["dotnet", "Ogx.IdentityService.Http.Host.dll"]
CMD []