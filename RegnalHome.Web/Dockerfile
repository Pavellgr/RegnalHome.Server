FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
RUN dotnet restore "RegnalHome.Web.csproj"
COPY . .
WORKDIR "/"
RUN dotnet build "RegnalHome.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RegnalHome.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RegnalHome.Web.dll"]

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
#Copy from Environment(build/publish)
COPY --from=publish app/publish/wwwroot . 