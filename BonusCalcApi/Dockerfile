FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

# Install dotnet-ef tool to run migrations
RUN dotnet tool install --global dotnet-ef --version 8.0.11

# Ensure dotnet tools are in the path
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy csproj and restore as distinct layers
COPY ./BonusCalcApi.sln ./
COPY ./BonusCalcApi/BonusCalcApi.csproj ./BonusCalcApi/

RUN dotnet restore ./BonusCalcApi/BonusCalcApi.csproj

# Copy everything else and build
COPY . ./
RUN dotnet build -c Release -o out
RUN dotnet publish -c Release -o out

EXPOSE ${PORT:-3000}
CMD ASPNETCORE_URLS=http://+:${PORT:-3000} dotnet ./out/BonusCalcApi.dll
