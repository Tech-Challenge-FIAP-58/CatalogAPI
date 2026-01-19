# syntax=docker/dockerfile:1

# =========================
# Base (runtime)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Se não estiver usando UID customizado, mantenha comentado
# USER $APP_UID


# =========================
# Build (restore + build)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# -------------------------
# Copia solução
# -------------------------
COPY FCG.Catalog.slnx ./

# -------------------------
# Copia apenas os csproj (cache de restore)
# -------------------------
COPY src/FCG.Catalog.WebApi/FCG.Catalog.WebApi.csproj src/FCG.Catalog.WebApi/
COPY src/FCG.Catalog.Application/FCG.Catalog.Application.csproj src/FCG.Catalog.Application/
COPY src/FCG.Core/FCG.Core.csproj src/FCG.Core/
COPY src/FCG.Catalog.Infra/FCG.Catalog.Infra.csproj src/FCG.Catalog.Infra/

# -------------------------
# Restore (projeto de entrada)
# -------------------------
RUN dotnet restore src/FCG.Catalog.WebApi/FCG.Catalog.WebApi.csproj

# -------------------------
# Copia o restante do código
# -------------------------
COPY . .

# -------------------------
# Build
# -------------------------
RUN dotnet build src/FCG.Catalog.WebApi/FCG.Catalog.WebApi.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/build


# =========================
# Publish
# =========================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish src/FCG.Catalog.WebApi/FCG.Catalog.WebApi.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false


# =========================
# Final (runtime)
# =========================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG.Catalog.WebApi.dll"]
