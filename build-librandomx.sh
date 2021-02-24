#!/bin/bash
set -e

if ! command -v git > /dev/null; then
  echo "No git in the system; install git first." > /dev/stderr
  exit 1
fi

randomx_version=v1.1.8
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
if [[ -f "$destination" ]]; then
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
cmake -DARCH=native -DBUILD_SHARED_LIBS=ON
make
popd
cp "$tmpdir/$shared_lib_filename" "$destination"
echo "$destination has been made; check it out!" > /dev/stderr
