FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["src/PaymentService/PaymentService.csproj", "PaymentService/"]
COPY ["src/Common/Loft.Common/Loft.Common.csproj", "Common/Loft.Common/"]

RUN dotnet restore "PaymentService/PaymentService.csproj"

COPY src/ .

WORKDIR /src/PaymentService
RUN dotnet build "PaymentService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/PaymentService
RUN dotnet publish "PaymentService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.dll"]
