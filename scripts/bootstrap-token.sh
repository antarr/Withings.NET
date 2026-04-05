#!/bin/bash
# Bootstrap script to obtain a Withings OAuth refresh token.
#
# Starts a local server on port 8585 to capture the OAuth callback automatically.
#
# Prerequisites:
#   - .env file at the repo root with WITHINGS_CLIENT_ID, WITHINGS_CLIENT_SECRET, WITHINGS_CALLBACK_URL
#   - Callback URL set to http://localhost:8585/callback in Withings developer portal

set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"
ENV_FILE="$ROOT_DIR/.env"

if [ ! -f "$ENV_FILE" ]; then
    echo "Error: .env file not found at $ENV_FILE"
    exit 1
fi

# Load .env
export $(grep -v '^#' "$ENV_FILE" | grep -v '^$' | xargs)

if [ -z "$WITHINGS_CLIENT_ID" ] || [ -z "$WITHINGS_CLIENT_SECRET" ] || [ -z "$WITHINGS_CALLBACK_URL" ]; then
    echo "Error: WITHINGS_CLIENT_ID, WITHINGS_CLIENT_SECRET, and WITHINGS_CALLBACK_URL must be set in .env"
    exit 1
fi

# Build authorization URL
AUTH_URL="https://account.withings.com/oauth2_user/authorize2"
SCOPE="user.info,user.metrics,user.activity"
STATE="bootstrap_$(date +%s)"

ENCODED_CALLBACK=$(python3 -c "import urllib.parse; print(urllib.parse.quote('$WITHINGS_CALLBACK_URL', safe=''))")
FULL_URL="${AUTH_URL}?response_type=code&client_id=${WITHINGS_CLIENT_ID}&state=${STATE}&scope=${SCOPE}&redirect_uri=${ENCODED_CALLBACK}"

# Kill any leftover process on port 8585
lsof -ti:8585 | xargs kill -9 2>/dev/null || true

echo ""
echo "Starting local callback server on port 8585..."
echo "Opening Withings authorization page in your browser..."
echo ""

CODE_FILE=$(mktemp)
trap "rm -f $CODE_FILE" EXIT

# Start Python callback server in the background, writing code to temp file
python3 -c "
import http.server, urllib.parse, sys

class CallbackHandler(http.server.BaseHTTPRequestHandler):
    def do_GET(self):
        params = urllib.parse.parse_qs(urllib.parse.urlparse(self.path).query)
        code = params.get('code', [None])[0]
        self.send_response(200)
        self.send_header('Content-Type', 'text/html')
        self.end_headers()
        if code:
            self.wfile.write(b'<html><body><h2>Authorization successful!</h2><p>You can close this tab and return to the terminal.</p></body></html>')
            with open('$CODE_FILE', 'w') as f:
                f.write(code)
        else:
            self.wfile.write(b'<html><body><h2>Error: No code received</h2></body></html>')
    def log_message(self, *args):
        pass

server = http.server.HTTPServer(('localhost', 8585), CallbackHandler)
server.timeout = 120
server.handle_request()
" &

SERVER_PID=$!
sleep 1

# Open browser
open "$FULL_URL" 2>/dev/null || xdg-open "$FULL_URL" 2>/dev/null || echo "Open this URL in your browser: $FULL_URL"

echo "Waiting for authorization (up to 2 minutes)..."
echo ""

wait $SERVER_PID

AUTH_CODE=$(cat "$CODE_FILE" 2>/dev/null)

if [ -z "$AUTH_CODE" ]; then
    echo "Error: Failed to capture authorization code"
    exit 1
fi

echo "Authorization code received!"
echo ""
echo "Exchanging code for tokens..."

RESPONSE=$(curl -s -X POST "https://wbsapi.withings.net/v2/oauth2" \
    -d "action=requesttoken" \
    -d "grant_type=authorization_code" \
    -d "client_id=$WITHINGS_CLIENT_ID" \
    -d "client_secret=$WITHINGS_CLIENT_SECRET" \
    -d "code=$AUTH_CODE" \
    -d "redirect_uri=$WITHINGS_CALLBACK_URL")

# Parse response
STATUS=$(echo "$RESPONSE" | python3 -c "import sys,json; print(json.load(sys.stdin).get('status', 'unknown'))")

if [ "$STATUS" != "0" ]; then
    echo "Error: API returned status $STATUS"
    echo "$RESPONSE" | python3 -m json.tool 2>/dev/null || echo "$RESPONSE"
    exit 1
fi

REFRESH_TOKEN=$(echo "$RESPONSE" | python3 -c "import sys,json; print(json.load(sys.stdin)['body']['refresh_token'])")
USER_ID=$(echo "$RESPONSE" | python3 -c "import sys,json; print(json.load(sys.stdin)['body']['userid'])")

# Update .env
if grep -q "^WITHINGS_REFRESH_TOKEN=" "$ENV_FILE"; then
    sed -i '' "s|^WITHINGS_REFRESH_TOKEN=.*|WITHINGS_REFRESH_TOKEN=$REFRESH_TOKEN|" "$ENV_FILE"
else
    echo "WITHINGS_REFRESH_TOKEN=$REFRESH_TOKEN" >> "$ENV_FILE"
fi

if grep -q "^WITHINGS_USER_ID=" "$ENV_FILE"; then
    sed -i '' "s|^WITHINGS_USER_ID=.*|WITHINGS_USER_ID=$USER_ID|" "$ENV_FILE"
else
    echo "WITHINGS_USER_ID=$USER_ID" >> "$ENV_FILE"
fi

echo ""
echo "Success! Tokens saved to .env"
echo "  User ID:       $USER_ID"
echo "  Refresh Token:  ${REFRESH_TOKEN:0:20}..."
echo ""
echo "You can now run E2E tests:"
echo "  dotnet test --filter \"TestCategory=E2E\""
