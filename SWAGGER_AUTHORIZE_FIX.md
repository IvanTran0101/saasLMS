# Swagger Authorize Button Missing – Fix Summary

## Symptom
Some services (Enrollment, CourseCatalog, Assessment, LearningProgress, Notification) did not show the **Authorize** button in Swagger UI, while Identity/SaaS/Administration/Product did.

## Root Cause
Swagger JSON contained **`components.securitySchemes.oidc`**, but the top-level **`security`** section was generated as **`[ {} ]`**. Swagger UI hides the Authorize button when there is no valid security requirement.

Example from `swagger.json` (bad):
```json
"security": [ { } ]
```

This happened when ABP `AddAbpSwaggerGenWithOidc` did not inject a valid security requirement in **development**.

## Fix Applied
We preserved ABP conventions and existing call sites by updating the shared helper:

- **Keep** `ConfigureWithOidc(...)` signature intact.
- **In Development only**, switch from `AddAbpSwaggerGenWithOidc` to `AddAbpSwaggerGenWithOAuth`.
- **In non-development**, keep `AddAbpSwaggerGenWithOidc` unchanged.

This ensures Swagger JSON includes a non-empty security requirement and enables the Authorize button.

## Key Code Change
File:
- `shared/saasLMS.Shared.Hosting.AspNetCore/SwaggerConfigurationHelper.cs`

Behavior:
- **Dev**: `AddAbpSwaggerGenWithOAuth(authority, scopesDictionary, options => ...)`
- **Non-dev**: `AddAbpSwaggerGenWithOidc(authority, scopes, flows, discoveryEndpoint, options => ...)`

Notes:
- `scopes` is converted to a dictionary in dev because `AddAbpSwaggerGenWithOAuth` expects `Dictionary<string, string>`.
- We also removed `discoveryEndpoint` usage from local/dev call sites where it was just `AuthServer:MetadataAddress` (ABP recommends null unless issuer differs from discovery endpoint).

## Verification
After rebuild/restart:
1. `https://localhost:<port>/swagger/v1/swagger.json`
   - `components.securitySchemes.oidc` exists
   - `security` contains a non-empty object (no longer `{}`)
2. `https://localhost:<port>/swagger/ui/abp.swagger.js` is updated (no cache issue)
3. Swagger UI now shows **Authorize**.

## Files Changed
- `shared/saasLMS.Shared.Hosting.AspNetCore/SwaggerConfigurationHelper.cs`
- `apps/auth-server/src/saasLMS.AuthServer/saasLMSAuthServerModule.cs` (removed discoveryEndpoint for local/dev)

## Final Result
Authorize button now appears in all services’ Swagger UI without breaking existing service call sites.
