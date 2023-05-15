FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5006

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AuditCenter/AuditCenter.csproj", "AuditCenter/"]
COPY ["shared/N8T.Core/N8T.Core.csproj", "shared/N8T.Core/"]
COPY ["shared/N8T.Infrastructure.EfCore/N8T.Infrastructure.EfCore.csproj", "shared/N8T.Infrastructure.EfCore/"]
COPY ["shared/N8T.Infrastructure/N8T.Infrastructure.csproj", "shared/N8T.Infrastructure/"]
RUN dotnet restore "AuditCenter/AuditCenter.csproj"
COPY . .
WORKDIR "/src/AuditCenter"
RUN dotnet build "AuditCenter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuditCenter.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuditCenter.dll"]
