FROM microsoft/dotnet:2.1-sdk-alpine
WORKDIR /docs

COPY ./MarkdownViewer/MarkdownViewer.csproj ./MarkdownViewer/MarkdownViewer.csproj
RUN dotnet restore ./MarkdownViewer/MarkdownViewer.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release

CMD /bin/sh