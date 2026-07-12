#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

cd "$ROOT_DIR"

echo "Installing backend dependencies..."
dotnet restore backend/TaskManager.slnx

echo "Installing frontend dependencies..."
cd frontend
npm install
cd "$ROOT_DIR"

echo "Setup completed."
