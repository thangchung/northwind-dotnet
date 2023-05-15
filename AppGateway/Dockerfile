FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AppGateway/AppGateway.csproj", "AppGateway/"]
RUN dotnet restore "AppGateway/AppGateway.csproj"
COPY . .
WORKDIR "/src/AppGateway"
RUN dotnet build "AppGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppGateway.dll"]
