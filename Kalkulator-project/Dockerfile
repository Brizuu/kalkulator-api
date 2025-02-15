# Używanie obrazu bazowego z ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

# Używanie obrazu SDK do budowania aplikacji
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app

# Kopiowanie projektu .csproj do kontenera
COPY ["Kalkulator-project.csproj", "/app/"]

# Przywracanie zależności
RUN dotnet restore "/app/Kalkulator-project.csproj"

# Kopiowanie pozostałych plików do kontenera
COPY . .

# Zmiana katalogu roboczego na miejsce, gdzie znajduje się .csproj
WORKDIR "/app"

# Budowanie projektu z dodatkowymi logami
RUN dotnet build "/app/Kalkulator-project.csproj" -c $BUILD_CONFIGURATION -o /app/build --verbosity detailed

# Publikowanie aplikacji
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "/app/Kalkulator-project.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Finalna warstwa obrazu
FROM base AS final
ENV ASPNETCORE_HTTP_PORTS=5001
ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001
WORKDIR /app

# Kopiowanie opublikowanej aplikacji z warstwy publish
COPY --from=publish /app/publish .

# Ustawienie punktu wejścia
ENTRYPOINT ["dotnet", "Kalkulator-project.dll", "--enviroment=Development"]
