#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app/work/zkpushtocc_v1
EXPOSE 80

# set CST time
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN echo 'Asia/Shanghai' >/etc/timezone

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["WebApi/WebApi.csproj", "WebApi/"]
COPY ["Utils/ZKSubscribeHelper/ZKSubscribeHelper.csproj", "Utils/ZKSubscribeHelper/"]
COPY ["NuGet.Config", "NuGet.Config"]
RUN dotnet restore --configfile "NuGet.Config"  "WebApi/WebApi.csproj"
COPY . .
WORKDIR "/src/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./work/zkpushtocc_v1

WORKDIR /app/work/zkpushtocc_v1
ENTRYPOINT ["dotnet", "WebApi.dll"]