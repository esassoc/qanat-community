#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

RUN wget -qO- https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | bash
ARG pat
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS {\"endpointCredentials\": [{\"endpoint\":\"https://pkgs.dev.azure.com/esadatatechnology/_packaging/sitkatech/nuget/v3/index.json\", \"username\":\"build\", \"password\":\"${pat}\"}]}

WORKDIR /src
COPY ["Qanat.Swagger/Qanat.Swagger.csproj" , "Qanat.Swagger/"]
COPY ["./NuGet.Config", "Qanat.Swagger/"]

RUN dotnet restore --configfile "Qanat.Swagger/NuGet.Config" "./Qanat.Swagger/./Qanat.Swagger.csproj"
COPY . .
WORKDIR "/src/Qanat.Swagger"
RUN dotnet build "./Qanat.Swagger.csproj" --configfile NuGet.Config -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Qanat.Swagger.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Qanat.Swagger.dll"]	
