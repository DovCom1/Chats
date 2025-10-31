FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./Chats.API/Chats.API.csproj ./Chats.API/
COPY ./Chats.Core/Chats.Core.csproj ./Chats.Core/
COPY ./Chats.Infrastructure/Chats.Infrastructure.csproj ./Chats.Infrastructure/
COPY ./Chats.Service/Chats.Service.csproj ./Chats.Service/

RUN dotnet restore "Chats.API/Chats.API.csproj"

COPY . .
WORKDIR "/src/Chats.API"
RUN dotnet publish "Chats.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080

ENTRYPOINT ["dotnet", "Chats.API.dll"]