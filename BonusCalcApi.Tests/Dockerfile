FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telemetry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

WORKDIR /app

# Install dotnet-ef tool to run migrations
RUN dotnet tool install --global dotnet-ef --version 5.0.10

# Ensure dotnet tools are in the path
ENV PATH="$PATH:/root/.dotnet/tools"

# Enable sonarcloud
RUN apt-get update && apt-get install -y openjdk-11-jdk
RUN dotnet tool install -v quiet --global coverlet.console --version 1.7.2
RUN dotnet tool install -v quiet --global dotnet-sonarscanner --version 5.5.3
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy csproj and restore as distinct layers
COPY ./BonusCalcApi.sln ./
COPY ./BonusCalcApi/BonusCalcApi.csproj ./BonusCalcApi/
COPY ./BonusCalcApi.Tests/BonusCalcApi.Tests.csproj ./BonusCalcApi.Tests/

RUN dotnet restore ./BonusCalcApi/BonusCalcApi.csproj
RUN dotnet restore ./BonusCalcApi.Tests/BonusCalcApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out BonusCalcApi.Tests/BonusCalcApi.Tests.csproj
RUN dotnet build -c Release -o out BonusCalcApi/BonusCalcApi.csproj

CMD /app/ci.sh
