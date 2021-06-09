#!/bin/bash
set -e

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
  shared_lib_filename=librandomx.dylib
  ;;
*)
  shared_lib_filename=librandomx.so
  ;;
esac
destination="$(dirname "$0")/native/$shared_lib_filename"
if [[ -f "$destination" && "$CI" != "true" ]]; then
  echo "$destination already exists; this script will do nothing." > /dev/stderr
  exit 0
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
if command -v clang > /dev/null; then
  export CC=clang
  export CXX=clang++
fi
if [[ "$CI" = "true" ]]; then
  cmake -DARCH=native
  make
  ./randomx-tests
  $CC -o api-example -lm -lstdc++ src/tests/api-example1.c librandomx.a
  ./api-example
  make clean
fi
cmake -DARCH=native -DBUILD_SHARED_LIBS=ON
make
popd
cp "$tmpdir/$shared_lib_filename" "$destination"
echo "$destination has been made; check it out!" > /dev/stderr
