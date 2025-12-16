FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY DailyCost.sln ./
COPY src/DailyCost.Domain/DailyCost.Domain.csproj src/DailyCost.Domain/
COPY src/DailyCost.Application/DailyCost.Application.csproj src/DailyCost.Application/
COPY src/DailyCost.Infrastructure/DailyCost.Infrastructure.csproj src/DailyCost.Infrastructure/
COPY src/DailyCost.Api/DailyCost.Api.csproj src/DailyCost.Api/

RUN dotnet restore src/DailyCost.Api/DailyCost.Api.csproj

COPY src/ src/
RUN dotnet publish src/DailyCost.Api/DailyCost.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "DailyCost.Api.dll"]
