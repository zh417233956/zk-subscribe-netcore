# 声明使用的基础镜像
FROM microsoft/dotnet:3.1-aspnetcore-runtime
# 将本地dotnet core 应用拷贝到 容器 /app/ 目录下
COPY . /app/work/zkpushtocc_v1/
# 设置工作目录
WORKDIR /app/work/zkpushtocc_v1/
# 设置导出端口
EXPOSE 5556
# 设置CST东八区时间
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN echo 'Asia/Shanghai' >/etc/timezone
# 指定应用入口点
ENTRYPOINT ["dotnet", "/app/work/zkpushtocc_v1/WebApi.dll"]