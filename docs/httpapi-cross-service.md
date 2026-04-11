# Cross-Service HTTP API Calls (ABP) - Notes

This note summarizes how one service calls another service over HTTP in ABP,
and the specific auth/scope issue we just fixed.

## 1. The Basic Flow
In ABP, a service (caller) can invoke another service (callee) via an HTTP
client proxy generated from the callee's application contracts.

High-level flow:
1. Callee exposes Application Service interfaces in its Application.Contracts
   project (e.g. `ICourseCatalogAppService`).
2. Callee's HttpApi module exposes endpoints using Conventional Controllers.
3. Caller registers HTTP client proxies using the callee's contracts assembly.
4. Caller calls the interface method; ABP resolves it to an HTTP request
   (endpoint + method + params) via `/api/abp/api-definition`.

## 2. Key Requirements for Successful Calls

### 2.1 Interface Matching
The caller must use the exact interface type declared by the callee's
Application.Contracts (namespace + type name). Otherwise ABP can't match
the method to the API description.

### 2.2 RemoteServiceName Alignment
Both sides must use the same `RemoteServiceName` string:
- The callee declares it using `[RemoteService(Name = "...")]`.
- The caller registers proxies with the same string.
- The callee's `api-definition` module must report the same name.

### 2.3 Authentication/Authorization
If the callee endpoint has `[Authorize]`, the caller must send a valid token.
Commonly this is done by:
- Adding `AbpHttpClientIdentityModelWebModule` in the caller host.
- Setting `RemoteServices:<name>:UseCurrentAccessToken = true` so the
  current user token is forwarded.

### 2.4 Token Audience and Scope
The token must include a valid `aud` (audience) for the callee service.
This is usually produced by the AuthServer/OpenIddict configuration:
- The callee service must exist as an API Resource/Scope.
- The Swagger client must be allowed to request that scope.
- The user logs in via Swagger to get a token that includes the scope.

If the token lacks `aud`, the callee returns:
```
401 Unauthorized
The audience '(null)' is invalid
```

## 3. The Issue We Hit
AssessmentService called CourseCatalogService:
- Endpoint: `GET /api/course-catalog/course-catalog/{id}/owner`
- Error: `401 Unauthorized` with `The audience '(null)' is invalid`

Root cause:
The Swagger client used by Assessment did not request the
`CourseCatalogService` scope, so the token did not include a valid audience
for CourseCatalogService.

## 4. The Fix We Applied
We added the callee scope to the Swagger auth flow for Assessment:
```
scopes: new[] { "AssessmentService", "CourseCatalogService" }
```

In addition, AuthServer was updated to:
- Create the `CourseCatalogService` scope (OpenIddict seed)
- Register CourseCatalogService as a resource
- Allow the Swagger client to request that scope

After re-seeding AuthServer and re-login in Swagger, tokens include the
correct audience and the call succeeds.

## 5. Checklist for Future Cross-Service Calls
- Use the callee's Application.Contracts interface directly
- Ensure RemoteServiceName matches on both sides
- Ensure the callee endpoint is exposed via HttpApi
- Forward access tokens (`UseCurrentAccessToken = true`)
- Ensure the token has `aud` for the callee (scopes + resources)
- Re-login in Swagger after any scope changes

