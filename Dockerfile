FROM mcr.microsoft.com/dotnet/sdk:10.0 AS base
WORKDIR /packages
USER root

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-stage
WORKDIR /build-source

COPY ["./nuget.config", "./"]
COPY ["./common.props", "./"]
COPY ["./common.version.props", "./"]
COPY ["./common.test.props", "./"]

COPY ["./src/Ogx.IdentityService.Domain/Ogx.IdentityService.Domain.csproj", "./src/Ogx.IdentityService.Domain/"]
COPY ["./src/Ogx.IdentityService.EntityFrameworkCore/Ogx.IdentityService.EntityFrameworkCore.csproj", "./src/Ogx.IdentityService.EntityFrameworkCore/"]

# Update NuGet source with secret credentials
RUN --mount=type=secret,id=NUGET_SECRET \
    export NUGET_SECRET=$(cat /run/secrets/NUGET_SECRET) && \
    dotnet nuget update source github \
        --username hsnsh \
        --password $NUGET_SECRET \
        --store-password-in-clear-text

RUN dotnet restore "./src/Ogx.IdentityService.EntityFrameworkCore/Ogx.IdentityService.EntityFrameworkCore.csproj" --force --verbosity minimal

COPY ["./src/Ogx.IdentityService.Domain/.", "./src/Ogx.IdentityService.Domain/"]
COPY ["./src/Ogx.IdentityService.EntityFrameworkCore/.", "./src/Ogx.IdentityService.EntityFrameworkCore/"]

RUN dotnet build "./src/Ogx.IdentityService.EntityFrameworkCore/Ogx.IdentityService.EntityFrameworkCore.csproj" --no-restore --no-incremental --verbosity minimal --configuration Release

RUN dotnet test "./src/Ogx.IdentityService.EntityFrameworkCore/Ogx.IdentityService.EntityFrameworkCore.csproj" --no-restore --no-build --verbosity minimal --configuration Release --filter "category!=integration"

# Pack with version
RUN --mount=type=secret,id=VERSION_NUMBER \
    --mount=type=secret,id=ACTION_NUMBER \
    dotnet pack "./src/Ogx.IdentityService.Domain/Ogx.IdentityService.Domain.csproj" \
        --no-restore --no-build --configuration Release \
        --output ./packages \
        -p:PackageVersion=$(cat /run/secrets/VERSION_NUMBER).$(cat /run/secrets/ACTION_NUMBER)

RUN --mount=type=secret,id=VERSION_NUMBER \
    --mount=type=secret,id=ACTION_NUMBER \
    dotnet pack "./src/Ogx.IdentityService.EntityFrameworkCore/Ogx.IdentityService.EntityFrameworkCore.csproj" \
        --no-restore --no-build --configuration Release \
        --output ./packages \
        -p:PackageVersion=$(cat /run/secrets/VERSION_NUMBER).$(cat /run/secrets/ACTION_NUMBER)

# Final stage
FROM base AS final
WORKDIR /packages
COPY --from=build-stage /build-source/packages .

# Push to private NuGet source using secrets securely
RUN --mount=type=secret,id=NUGET_SOURCE \
    --mount=type=secret,id=NUGET_SECRET \
    dotnet nuget push *.nupkg \
        --source $(cat /run/secrets/NUGET_SOURCE) \
        --api-key $(cat /run/secrets/NUGET_SECRET) \
        --skip-duplicate
