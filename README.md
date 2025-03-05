
# Basvaraj_Assignment


## Assumptions

- I am using https://www.nationalbanken.dk/api/currencyratesxml?lang=en-us for getting the Exchange Rates.
- For getting the all currencies exchange rates, I am assuming that only admin can do that.
- Assuming that token will be provided by another service and I am just verifying the token in the application.
- Application is not a microservice, in case of microservice I would have used Redis for storing those cached data.


## How to get valid token?

- You can get the valid auth token from this link: https://token.dev/.
- Open the link and enable the Base64 encoded flag.
- Paste the below secret key text in the Signing key text box (might be the case it should be same for you, but just to make sure I would recommend to paste the data).
```
NTNv7j0TuYARvmNMmWXo6fKvM4o6nv/aUi9ryX38ZH+L1bkrnD1ObOQ8JAUmHCBq7Iy7otZcyAagBLHVKvvYaIpmMuxmARQ97jUVG16Jkpkp1wXOPsrF9zwew6TpczyHkHgX5EuLg2MeBuiT/qJACs1J0apruOOJCg/gOtkjB4c=
```
- Now in the Payload you can change the expiry time of the token, which should be greater than the time you are testing it. Also, you can change if the user is the admin or not by setting the admin value as true or false. Maybe you can use the below Payload where the expiry has been set for 3 months later from the date I am writing this document.
```
{
  "sub": "1234567890",
  "name": "John Doe",
  "aud": "web-app",
  "admin": true,
  "iss": "https://valid-issuer.com/auth",
  "iat": 1741014407,
  "exp": 1749018007
}
```
![Token_Instructions](https://github.com/user-attachments/assets/c96e7ed3-d9a3-4bef-866f-d8dd6c4b0592)

- After you completed the changes now you can copy the token from JWT String text-box and use it in place of the bearer token in the postman app.

## How to test APIs?
- You can use the Postman to test application.
- Open the Postman app in you machine, click on the import button.
- Paste the CURL command provided below.
- Repeat step 2 and 3 for all of the CURL commands.
- After you import the request, you can start the application and make a request from the postman, if you get 401 or 403 as a response, maybe you need to check the token expiry or you need Admin access flag set in the payload.
![Postman_Instruction](https://github.com/user-attachments/assets/b1f16c0f-813d-42c8-b971-a73bf003c70e)


## CURL commands
- Get the DKK converted rate from a currecy. 
```
curl --location 'https://localhost:7046/Currency/GetDKKEquivalent/INR?value=302' \
--header 'Authorization: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYXVkIjoid2ViLWFwcCIsImFkbWluIjp0cnVlLCJpc3MiOiJodHRwczovL3ZhbGlkLWlzc3Vlci5jb20vYXV0aCIsImlhdCI6MTc0MTAxNDQwNywiZXhwIjoxNzQ5MDE4MDA3fQ.T4fkWwgrpBqUpxAUpFXJFRpVAe6Irn72IO2q9JbWvKY'
```
- Get all currency conversion data related to DKK.
```
curl --location 'https://localhost:7046/Currency/GetCurrencyRates' \
--header 'Authorization: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYXVkIjoid2ViLWFwcCIsImFkbWluIjp0cnVlLCJpc3MiOiJodHRwczovL3ZhbGlkLWlzc3Vlci5jb20vYXV0aCIsImlhdCI6MTc0MTAxNDQwNywiZXhwIjoxNzQ5MDE4MDA3fQ.T4fkWwgrpBqUpxAUpFXJFRpVAe6Irn72IO2q9JbWvKY'
```
- Get all currency conversion history. Please note that this request requires admin access (i.e admin key in the token should have the value as true).
```
curl --location 'https://localhost:7046/Currency/GetCurrencyFetchHistory?from=03%2F05%2F2025&to=03%2F06%2F2025&currency=&offset=0&limit=20' \
--header 'Authorization: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYXVkIjoid2ViLWFwcCIsImFkbWluIjp0cnVlLCJpc3MiOiJodHRwczovL3ZhbGlkLWlzc3Vlci5jb20vYXV0aCIsImlhdCI6MTc0MTAxNDQwNywiZXhwIjoxNzQ5MDE4MDA3fQ.T4fkWwgrpBqUpxAUpFXJFRpVAe6Irn72IO2q9JbWvKY'
```
