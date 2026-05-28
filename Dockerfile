FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

COPY RestaurantOrder.slnx ./
COPY src/RestaurantOrder.Domain/RestaurantOrder.Domain.csproj src/RestaurantOrder.Domain/
COPY src/RestaurantOrder.Application/RestaurantOrder.Application.csproj src/RestaurantOrder.Application/
COPY src/RestaurantOrder.Infrastructure/RestaurantOrder.Infrastructure.csproj src/RestaurantOrder.Infrastructure/
COPY src/RestaurantOrder.API/RestaurantOrder.API.csproj src/RestaurantOrder.API/
COPY tests/RestaurantOrder.UnitTests/RestaurantOrder.UnitTests.csproj tests/RestaurantOrder.UnitTests/
COPY tests/RestaurantOrder.IntegrationTests/RestaurantOrder.IntegrationTests.csproj tests/RestaurantOrder.IntegrationTests/

RUN dotnet restore RestaurantOrder.slnx

COPY . .

RUN dotnet publish src/RestaurantOrder.API/RestaurantOrder.API.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "RestaurantOrder.API.dll"]
