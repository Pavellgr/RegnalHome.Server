#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RegnalHome.Common/RegnalHome.Common.csproj", "RegnalHome.Common/"]
RUN dotnet restore "RegnalHome.Common/RegnalHome.Common.csproj"
COPY . .
WORKDIR "/src/RegnalHome.Common"
RUN dotnet build "RegnalHome.Common.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RegnalHome.Common.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RegnalHome.Web/RegnalHome.Web.csproj", "RegnalHome.Web/"]
RUN dotnet restore "RegnalHome.Web/RegnalHome.Web.csproj"
COPY . .
WORKDIR "/src/RegnalHome.Web"
RUN dotnet build "RegnalHome.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RegnalHome.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# build runtime image
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot/ .
COPY RegnalHome.Web/nginx.conf /etc/nginx/nginx.conf