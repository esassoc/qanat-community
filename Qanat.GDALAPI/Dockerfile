#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM ghcr.io/osgeo/gdal:ubuntu-full-3.8.5 as base
WORKDIR /app
EXPOSE 80

RUN apt-get update && \
    apt-get install -y wget

# Download Microsoft signing key and repository
RUN wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb

# Install Microsoft signing key and repository
RUN dpkg -i packages-microsoft-prod.deb

# Clean up
RUN rm packages-microsoft-prod.deb

# Install the .NET runtime
RUN apt-get update && \
    apt-get install -y aspnetcore-runtime-8.0

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Qanat.GDALAPI/Qanat.GDALAPI.csproj", "Qanat.GDALAPI/"]
RUN dotnet restore "Qanat.GDALAPI/Qanat.GDALAPI.csproj"
COPY . .
WORKDIR "/src/Qanat.GDALAPI"
RUN dotnet build "Qanat.GDALAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Qanat.GDALAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Qanat.GDALAPI.dll"]