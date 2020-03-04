FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app
COPY . .
RUN dotnet publish src/mservicesample.Membership.Api -c release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT dotnet src/mservicesample.Membership.Api.dll


#multistage slim build
# ARG VERSION=3.1-alpine3.10
# FROM mcr.microsoft.com/dotnet/core/sdk:$VERSION AS build-env
# WORKDIR /app
# COPY . .
# RUN dotnet publish src/mservicesample.Membership.Api -c release -o out
# RUN ping google.com

# FROM mcr.microsoft.com/dotnet/core/aspnet:$VERSION
# RUN adduser \
#   --disabled-password \
#   --home /app \
#   --gecos '' app \
#   && chown -R app /app
# USER app

# WORKDIR /app
# COPY --from=build-env /app/output .
# ENV DOTNET_RUNNING_IN_CONTAINER=true \
#   ASPNETCORE_URLS=http://+:8080   
# ENV ASPNETCORE_ENVIRONMENT docker

# EXPOSE 8080
# ENTRYPOINT dotnet mservicesample.Membership.Api.dll