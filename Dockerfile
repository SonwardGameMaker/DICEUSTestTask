FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 3000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY TelegramBotDiseusTestApp/ TelegramBotDiseusTestApp/
WORKDIR /src/TelegramBotDiseusTestApp
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV PORT=3000
ENTRYPOINT ["dotnet", "TelegramBotDiseusTestApp.dll"]