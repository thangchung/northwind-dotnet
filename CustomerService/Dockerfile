FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CustomerService/CustomerService.csproj", "CustomerService/"]
COPY ["shared/N8T.Core/N8T.Core.csproj", "shared/N8T.Core/"]
COPY ["shared/N8T.Infrastructure.EfCore/N8T.Infrastructure.EfCore.csproj", "shared/N8T.Infrastructure.EfCore/"]
COPY ["shared/N8T.Infrastructure/N8T.Infrastructure.csproj", "shared/N8T.Infrastructure/"]
COPY ["shared/Northwind.IntegrationEvents/Northwind.IntegrationEvents.csproj", "shared/Northwind.IntegrationEvents/"]
RUN dotnet restore "CustomerService/CustomerService.csproj"
COPY . .
WORKDIR "/src/CustomerService"
RUN dotnet build "CustomerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CustomerService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CustomerService.dll"]
