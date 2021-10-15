FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

WORKDIR /app

# Install EF Core tool
RUN dotnet tool install --global dotnet-ef --version 5.0.10
ENV PATH "$PATH:/root/.dotnet/tools"

# Copy csproj and restore as distinct layers
COPY ./BonusCalcApi.sln ./
COPY ./BonusCalcApi/BonusCalcApi.csproj ./BonusCalcApi/
COPY ./BonusCalcApi.Tests/BonusCalcApi.Tests.csproj ./BonusCalcApi.Tests/

RUN dotnet restore ./BonusCalcApi/BonusCalcApi.csproj
RUN dotnet restore ./BonusCalcApi.Tests/BonusCalcApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out BonusCalcApi.Tests/BonusCalcApi.Tests.csproj

CMD dotnet test
