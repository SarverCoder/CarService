FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY CarService.sln ./
COPY src/CarService.Domain/CarService.Domain.csproj src/CarService.Domain/
COPY src/CarService.Application/CarService.Application.csproj src/CarService.Application/
COPY src/CarService.Infrastructure/CarService.Infrastructure.csproj src/CarService.Infrastructure/
COPY src/CarService.Api/CarService.Api.csproj src/CarService.Api/
RUN dotnet restore src/CarService.Api/CarService.Api.csproj
COPY src/ src/
RUN dotnet publish src/CarService.Api/CarService.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "CarService.Api.dll"]
