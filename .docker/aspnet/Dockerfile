FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    wget curl apt-transport-https ca-certificates && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Download and install .NET SDK
RUN wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh && \
    chmod +x dotnet-install.sh && \
    ./dotnet-install.sh --channel 8.0 --install-dir /usr/share/dotnet && \
    rm dotnet-install.sh

USER $APP_UID
WORKDIR /app
EXPOSE 5000


RUN dotnet tool install --global dotnet-ef

# Make the global tools available in PATH
ENV PATH="$PATH:/home/app/.dotnet/tools"


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ../../Messy/Messy.csproj Messy/
RUN dotnet restore "Messy/Messy.csproj"
COPY ../../. .
WORKDIR "/src/Messy"
RUN dotnet build "Messy.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Messy.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src /src
ENTRYPOINT ["dotnet", "Messy.dll"]
