#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RegnalHome.Server/RegnalHome.Server.csproj", "RegnalHome.Server/"]
COPY ["RegnalHome.Common/RegnalHome.Common.csproj", "RegnalHome.Common/"]
RUN dotnet restore "RegnalHome.Server/RegnalHome.Server.csproj"
COPY . .
WORKDIR "/src/RegnalHome.Server"
RUN dotnet build "RegnalHome.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RegnalHome.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RegnalHome.Server.dll"]

FROM base
COPY /src/cert.cer /usr/local/share/ca-certificates/cert.cer
RUN update-ca-certificates