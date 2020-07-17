#用编译环境编译项目
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
COPY . /app
WORKDIR /app/src
#dotnet 2.x不再需要手动restore，但是也可以手动restore来自定义源
#RUN dotnet restore
RUN dotnet publish WCloud.MetroAd.Api/WCloud.MetroAd.Api.csproj -c Release -o WCloud.MetroAd.Api/out

#编译结果copy到runtime容器运行
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/src/WCloud.MetroAd.Api/out .
ENTRYPOINT ["dotnet","WCloud.MetroAd.Api.dll"]

#https://docs.microsoft.com/zh-cn/dotnet/core/tools/dotnet-publish?tabs=netcore21