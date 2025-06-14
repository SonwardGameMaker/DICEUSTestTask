FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 3000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TelegramBotDiseusTestApp/TelegramBotDiseusTestApp.csproj", "./"]
RUN dotnet restore "TelegramBotDiseusTestApp/TelegramBotDiseusTestApp.csproj"
COPY . .
RUN dotnet publish "TelegramBotDiseusTestApp/TelegramBotDiseusTestApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ENTRYPOINT ["dotnet", "TelegramBotDiseusTestApp/TelegramBotDiseusTestApp.dll"]