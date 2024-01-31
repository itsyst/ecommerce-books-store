FROM mcr.microsoft.com/dotnet/sdk:8.0 as base
WORKDIR /app
EXPOSE 5000
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:5000

# Copy everything
FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
COPY  Books/*.csproj  Books/ 
COPY  Books.Data/*.csproj  Books.Data/ 
COPY  Books.Domain/*.csproj  Books.Domain/ 
COPY  Books.Interfaces/*.csproj  Books.Interfaces/ 

# Restore as distinct layers
RUN dotnet restore Books/Books.csproj

# Copy everything
COPY . ./ 

# Build and publish a release
WORKDIR "/src/Books"
RUN dotnet build "Books.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Books.csproj" -c Release -o /app

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Books.dll", "--server.urls", "http://0.0.0.0:5000"]
