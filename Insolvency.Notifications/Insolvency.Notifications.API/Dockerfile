#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Insolvency.Notifications.API/Insolvency.Notifications.API.csproj", "Insolvency.Notifications.API/"]
COPY ["Insolvency.Notifications.Messaging/Insolvency.Notifications.Messaging.csproj", "Insolvency.Notifications.Messaging/"]
COPY ["Insolvency.Notifications.Models/Insolvency.Notifications.Models.csproj", "Insolvency.Notifications.Models/"]
COPY ["Insolvency.Interfaces/Insolvency.Interfaces.csproj", "Insolvency.Interfaces/"]
COPY ["Insolvency.Identity.Models/Insolvency.Identity.Models.csproj", "Insolvency.Identity.Models/"]
COPY ["Insolvency.Models/Insolvency.Models.csproj", "Insolvency.Models/"]
COPY ["Insolvency.Data/Insolvency.Data.csproj", "Insolvency.Data/"]
RUN dotnet restore "Insolvency.Notifications.API/Insolvency.Notifications.API.csproj"
COPY . .
WORKDIR "/src/Insolvency.Notifications.API"
RUN dotnet build "Insolvency.Notifications.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Insolvency.Notifications.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Insolvency.Notifications.API.dll"]