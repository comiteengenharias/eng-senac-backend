# Etapa 1: build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

# (Opcional) copie seu .env, se usar
COPY .env /app/publish/

# Etapa 2: imagem de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ARG PORT=8080
ENV ASPNETCORE_URLS="https://+:${PORT}"
EXPOSE ${PORT}

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EngenhariasSenac.dll"]