# Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Tüm dosyaları kopyala
COPY ./MHRS-OtomatikRandevu ./

# Restore + Publish
RUN dotnet restore "MHRS-OtomatikRandevu.csproj"
RUN dotnet publish "MHRS-OtomatikRandevu.csproj" -c Release -o /app/publish

# Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MHRS-OtomatikRandevu.dll"]