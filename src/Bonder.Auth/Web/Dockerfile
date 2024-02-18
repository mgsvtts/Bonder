#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Bonder.Auth/Web/Web.csproj", "src/Bonder.Auth/Web/"]
COPY ["src/Bonder.Auth/Application/Application.csproj", "src/Bonder.Auth/Application/"]
COPY ["src/Bonder.Auth/Domain/Domain.csproj", "src/Bonder.Auth/Domain/"]
COPY ["src/Bonder.Shared/Shared.Domain/Shared.Domain.csproj", "src/Bonder.Shared/Shared.Domain/"]
COPY ["src/Bonder.Auth/Infrastructure/Infrastructure.csproj", "src/Bonder.Auth/Infrastructure/"]
COPY ["src/Bonder.Shared/Shared.Proto/Shared.Proto.csproj", "src/Bonder.Shared/Shared.Proto/"]
COPY ["src/Bonder.Auth/Presentation/Presentation.csproj", "src/Bonder.Auth/Presentation/"]
RUN dotnet restore "./src/Bonder.Auth/Web/./Web.csproj"
COPY . .
WORKDIR "/src/src/Bonder.Auth/Web"
RUN dotnet build "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]