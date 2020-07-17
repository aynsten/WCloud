#用编译环境编译项目
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
COPY . /app
WORKDIR /app/src
#RUN dotnet restore
RUN dotnet publish WCloud.Admin.Consumer/WCloud.Admin.Consumer.csproj -c Release -o WCloud.Admin.Consumer/out

#编译结果copy到runtime容器运行
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/src/WCloud.Admin.Consumer/out .
ENTRYPOINT ["dotnet","WCloud.Admin.Consumer.dll"]