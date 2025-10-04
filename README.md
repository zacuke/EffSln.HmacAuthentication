# API Authentication

## Headers Required
- `X-API-Key`: API key from config
- `X-Timestamp`: UTC ISO 8601 (5-minute window)
- `X-Signature`: HMAC-SHA256 signature

## Signature Generation
```
signature = base64(hmac_sha256(secret, timestamp + method + path + query + body))
```
 
## Endpoints
- `GET /api/events/next?clientId=&liveMode=`
- `POST /api/events/register`

## Config
```
  "ApiKeys": {
    "sk_live_key1234": "whsec_prod_abcde",
    "sk_live_key1234": "whsec_test_abcde"
  }
  ```