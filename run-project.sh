#!/bin/bash

# Build the projects
echo "Building projects..."
dotnet build

# Check if the build succeeded
if [ $? -ne 0 ]; then
  echo "Build failed. Exiting."
  exit 1
fi

echo "Build succeeded. Starting servers..."
# Start both projects in the background and capture their PIDs
dotnet run --project Backend/Backend.csproj &
BACKEND_PID=$!

dotnet run --project Frontend/Frontend.csproj &
FRONTEND_PID=$!

echo $BACKEND_PID
echo $FRONTEND_PID

# Function to kill both processes
cleanup() {
  echo "Stopping servers..."
  kill $BACKEND_PID $FRONTEND_PID
  wait $BACKEND_PID $FRONTEND_PID 2>/dev/null
  echo "Servers stopped."
}

# Trap Ctrl+C (SIGINT) and call cleanup
trap cleanup SIGINT

# Wait indefinitely to keep the script running
wait