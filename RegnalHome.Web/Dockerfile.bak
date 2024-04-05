#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RegnalHome.Web/RegnalHome.Web.csproj", "RegnalHome.Web/"]
RUN dotnet restore "RegnalHome.Web/RegnalHome.Web.csproj"
COPY . .
WORKDIR "/src/RegnalHome.Web"
RUN dotnet build "RegnalHome.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RegnalHome.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
EXPOSE 443
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY "RegnalHome.Web/nginx.conf" /etc/nginx/nginx.conf