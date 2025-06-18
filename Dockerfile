# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# WORKDIR /app
# EXPOSE 80
# EXPOSE 443

# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /src
# COPY ["MyDotnetApp.csproj", "./"]
# RUN dotnet restore "MyDotnetApp.csproj"
# COPY . .
# RUN dotnet build "MyDotnetApp.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "MyDotnetApp.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .

# # 创建数据保护密钥目录并设置权限
# RUN mkdir -p /app/keys && \
#     chmod 777 /app/keys

# # 添加等待脚本
# COPY wait-for-it.sh /app/wait-for-it.sh
# RUN chmod +x /app/wait-for-it.sh

# # 设置入口点
# ENTRYPOINT ["dotnet", "MyDotnetApp.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MyDotnetApp.csproj", "./"]
RUN dotnet restore "MyDotnetApp.csproj"
COPY . .
RUN dotnet build "MyDotnetApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyDotnetApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 创建目录并设置权限
RUN mkdir -p /root/.aspnet/DataProtection-Keys && \
    chmod 777 /root/.aspnet/DataProtection-Keys
ENV RUNNING_IN_DOCKER=true
ENTRYPOINT ["dotnet", "MyDotnetApp.dll"]