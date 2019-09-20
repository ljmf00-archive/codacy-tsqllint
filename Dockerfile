FROM mcr.microsoft.com/dotnet/core/runtime:2.2-alpine

COPY src/Analyzer/bin/Release/netcoreapp2.2/publish/*.dll /opt/docker/bin/
COPY src/Analyzer/bin/Release/netcoreapp2.2/publish/Analyzer.runtimeconfig.json /opt/docker/bin/
COPY docs /docs/

RUN adduser -u 2004 -D docker
RUN chown -R docker:docker /docs

ENTRYPOINT [ "dotnet", "/opt/docker/bin/Analyzer.dll" ]