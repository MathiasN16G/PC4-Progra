
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY *.sln .
COPY EvaluadorInteligente/*.csproj EvaluadorInteligente/
RUN dotnet restore

COPY EvaluadorInteligente/. ./EvaluadorInteligente/
WORKDIR /src/EvaluadorInteligente
RUN dotnet publish -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EvaluadorInteligente.dll"]
