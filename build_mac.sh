CONFIG='Release'
OUTPUT_PATH='./artifacts/packages'
SOLUTION='./openmedstack-framework.sln'

echo $CONFIG
echo "$OUTPUT_PATH"
echo "$SOLUTION"

echo 'CLEAN'
dotnet clean "$SOLUTION"

echo 'BUILD'
dotnet build "$SOLUTION" -c $CONFIG

echo 'PACK'
dotnet pack "$SOLUTION" -c $CONFIG -o "$OUTPUT_PATH" --include-symbols
