# build app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY RegnalHome.Web/RegnalHome.Web.csproj .
RUN dotnet restore "RegnalHome.Web.csproj"
COPY . .
RUN dotnet build "RegnalHome.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RegnalHome.Web.csproj" -c Release -o /app/publish

# build runtime image
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot/ .
COPY RegnalHome.Web/nginx.conf /etc/nginx/nginx.conf