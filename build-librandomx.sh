#!/bin/bash
set -e

if [[ "$CI" = "true" ]]; then
  set -vx
fi

if ! command -v git > /dev/null; then
  echo "No git in the system; install git first." > /dev/stderr
  exit 1
fi

randomx_version=v1.1.9
if [[ "$1" != "" ]]; then
  randomx_version="$1"
fi

case "$(uname -s)" in
Darwin)
lib_filename=librandomx.dylib
;;
*)
lib_filename=librandomx.so
;;
esac
destination="$(dirname "$0")/native/$lib_filename"
if [[ -f "$destination" && "$CI" != "true" ]]; then
  echo "$destination already exists; this script will do nothing." > /dev/stderr
  exit 0
fi
if [[ ! -d "$(dirname "$0")/native" ]]; then
  mkdir -p "$(dirname "$0")/native"
fi

tmpdir="$(mktemp -d)"
cleanup_tmpdir() {
  rm -rf "$tmpdir"
}
trap cleanup_tmpdir EXIT

git clone \
  --depth=1 \
  --branch "$randomx_version" \
  git://github.com/tevador/RandomX.git \
  "$tmpdir"
pushd "$tmpdir"
if [[ "$(uname -s)" = Linux ]] && command -v gcc > /dev/null; then
  export CC=gcc
  export CXX=g++
elif [[ "$(uname -s)" = Darwin ]] && command -v clang > /dev/null; then
  export CC=clang
  export CXX=clang++
fi
if [[ "$BUILD_STATIC_LIBS" != "" ]]; then
  cmake \
    -DARCH=native \
    -DCMAKE_C_FLAGS="-static -no-pie -static-libgcc" \
    -DCMAKE_CXX_FLAGS="-static -no-pie -static-libgcc -static-libstdc++ -fno-rtti"
  make
  gcc \
    -o librandomx.so \
    -shared \
    -fno-rtti \
    -Wl,--whole-archive \
    "$(gcc --print-file-name=libc.a)" \
    librandomx.a \
    -static-libgcc \
    -static-libstdc++ \
    -lstdc++ \
    -Wl,--no-whole-archive
else
  cmake -DARCH=native -DBUILD_SHARED_LIBS=ON
  make
fi
popd
cp "$tmpdir/$lib_filename" "$destination"
echo "$destination has been made; check it out!" > /dev/stderr
