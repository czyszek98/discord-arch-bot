FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DiscordBotArch.csproj", "DiscordBotArch.csproj"]
RUN dotnet restore 'DiscordBotArch.csproj'

COPY ["src/*", "src" ]
RUN dotnet build 'DiscordBotArch.csproj' -c Release -o /App/build

FROM build as publish
RUN dotnet publish 'DiscordBotArch.csproj' -c Release -o /App/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update && \
    apt-get install -y ffmpeg && \
    rm -rf /var/lib/apt/lists/*
WORKDIR /app
RUN mkdir tmp
COPY --from=publish /App/publish .
ENTRYPOINT ["dotnet", "DiscordBotArch.dll"]