FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Копируем глобальные настройки (корневой Directory.Build.props) прежде чем делать restore
COPY ["Directory.Build.props", "./"]
# Копируем .csproj этого сервиса и его зависимостей (пути относительно корня репозитория: src/...)
COPY ["src/CategoryService/CategoryService.csproj", "CategoryService/"]
COPY ["src/ProductService/ProductService.csproj", "ProductService/"]
COPY ["src/Common/Loft.Common/Loft.Common.csproj", "Common/Loft.Common/"]

RUN dotnet restore "CategoryService/CategoryService.csproj"

# Копируем все исходники (включая остальные проекты и папки) в контейнер
COPY src/ .

# Переключаемся в папку проекта и собираем
WORKDIR "/src/CategoryService"
RUN dotnet build "CategoryService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/CategoryService"
RUN dotnet publish "CategoryService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CategoryService.dll"]