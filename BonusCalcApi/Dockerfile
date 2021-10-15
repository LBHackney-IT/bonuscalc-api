FROM mcr.microsoft.com/dotnet/core/sdk:3.1

WORKDIR /app

# Install EF Core tool
RUN dotnet tool install --global dotnet-ef --version 5.0.10
ENV PATH "$PATH:/root/.dotnet/tools"

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
