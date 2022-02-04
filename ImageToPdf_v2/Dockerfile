#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ImageToPdf_v2/ImageToPdf_v2.csproj", "ImageToPdf_v2/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "ImageToPdf_v2/ImageToPdf_v2.csproj"
COPY . .
WORKDIR "/src/ImageToPdf_v2"
RUN dotnet build "ImageToPdf_v2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImageToPdf_v2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageToPdf_v2.dll"]