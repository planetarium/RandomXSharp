# To correctly make a static-linked binary, we use Alpine Linux.
# The distro entirely uses musl instead of glibc which is not
# static-link-friendly.
FROM mcr.microsoft.com/dotnet/nightly/sdk:3.1-alpine3.13

RUN apk add --no-cache \
        bash=5.1.0-r0 \
        build-base=0.5-r2 \
        cmake=3.18.4-r1 \
        git=2.30.2-r0 \
        gcc=10.2.1_pre1-r3 \
        g++=10.2.1_pre1-r3 \
        patchelf=0.12-r0

SHELL ["/bin/bash", "-x", "-c"]

COPY . /usr/local/src/RandomXSharp
WORKDIR /usr/local/src/RandomXSharp

ENV BUILD_STATIC_LIBS=true
RUN ./build-librandomx.sh

ARG BUILD_NATIVE_ONLY

RUN if [[ "$BUILD_NATIVE_ONLY" = "" ]]; then \
        dotnet build -c Release; \
        dotnet pack -c Release -o dist; \
    fi
