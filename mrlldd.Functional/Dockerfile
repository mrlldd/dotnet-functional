FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /mrlldd.Functional

COPY . .
RUN dotnet restore
RUN dotnet build -c Release --no-restore
RUN dotnet test -c Release --no-restore --no-build
RUN dotnet pack -c Release -o /artifact --no-restore --no-build

ENTRYPOINT ["dotnet", "nuget", "push", "/artifact/*.nupkg"]