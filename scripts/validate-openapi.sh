#!/usr/bin/env sh
set -eu

repo_root=$(CDPATH= cd -- "$(dirname -- "$0")/.." && pwd)
npx --yes @redocly/cli@latest lint "$repo_root/specs/001-text-quiz-generator/contracts/quiz-api.openapi.yaml"
