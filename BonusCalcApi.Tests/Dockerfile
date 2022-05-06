FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG SONAR_TOKEN
ENV SONAR_TOKEN=$SONAR_TOKEN

WORKDIR /app

# Enable sonarcloud
RUN apt-get update && apt-get install -y openjdk-11-jdk
RUN dotnet tool install -v quiet --global dotnet-coverage
RUN dotnet tool install -v quiet --global dotnet-sonarscanner
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
