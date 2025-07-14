FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ProjectManagement.Api/ProjectManagement.Api.csproj ./ProjectManagement.Api/
COPY ProjectManagement.Application/ProjectManagement.Application.csproj ./ProjectManagement.Application/
COPY ProjectManagement.Infrastructure/ProjectManagement.Infrastructure.csproj ./ProjectManagement.Infrastructure/

RUN dotnet restore ./ProjectManagement.Api/ProjectManagement.Api.csproj

COPY . .

WORKDIR /src/ProjectManagement.Api
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
EXPOSE 7013
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "ProjectManagement.Api.dll"]
