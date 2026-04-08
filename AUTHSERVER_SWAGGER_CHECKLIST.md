# AuthServer + Swagger Authorize Checklist

Muc tieu: Swagger UI co nut **Authorize** va login thanh cong cho tat ca service.

---

## 0) Khi nao can dung checklist nay
- Swagger UI khong thay nut Authorize.
- Authorize bi loi `invalid_request` (redirect_uri khong hop le).
- Authorize bi loi CORS.
- Token bi 401 `audience empty`.

---

## 1) Kiem tra Swagger JSON (moi service)
Mo:
```
https://localhost:<port>/swagger/v1/swagger.json
```
Phai co:
```json
"components": {
  "securitySchemes": { "oidc": { ... } }
},
"security": [ { "oidc": [] } ]
```

Neu `security` la `[ {} ]` thi Swagger **se khong hien** Authorize.

---

## 2) CORS o AuthServer
AuthServer phai **cho phep origin** noi Swagger UI dang chay.

Mo file:
```
apps/auth-server/src/saasLMS.AuthServer/appsettings.json
```
Trong `App:CorsOrigins` phai co:
- WebGateway (`https://localhost:44325`)
- PublicWebGateway (`https://localhost:44353`)
- Service hosts neu mo swagger truc tiep (vi du `https://localhost:44445`)

Neu thieu -> loi CORS khi goi `https://localhost:44322/connect/token`.

---

## 3) Cau hinh client Swagger trong AuthServer
Client dung chung: **`WebGateway_Swagger`**.

Client phai co:
- `ClientType = Public`
- `GrantType = authorization_code`
- `RedirectUris` dung

### Redirect URI can co
Neu mo swagger truc tiep o service:
```
https://localhost:<port>/swagger/oauth2-redirect.html
https://localhost:<port>/swagger/ui/oauth2-redirect.html
```

Neu mo swagger qua gateway:
```
https://localhost:44325/swagger/oauth2-redirect.html
https://localhost:44325/swagger/ui/oauth2-redirect.html
```

Phai add **day du** cho tat ca origin ban dung.

---

## 4) Scope cua client Swagger
Client `WebGateway_Swagger` phai duoc grant scope cho moi service can dung:
```
CourseCatalogService
EnrollmentService
AssessmentService
LearningProgressService
NotificationService
...
```

Neu thieu scope -> token se bi `audience empty` -> 401.

---

## 5) SwaggerClientId dong bo
Tat ca service host + gateway phai dung cung `SwaggerClientId`:
```
"AuthServer": {
  "SwaggerClientId": "WebGateway_Swagger"
}
```

---

## 6) Sau khi doi config
- **Restart AuthServer**
- Neu client da ton tai -> can update lai trong DB hoac xoa record de seeder tao moi.

---

## 7) Kiem tra nhanh khi loi
### Loi 401 (audience empty)
- Thieu scope o client hoac khong chon scope khi authorize.

### Loi invalid_request (redirect_uri)
- Thieu redirect URI trong client.

### Loi CORS (Failed to fetch)
- Thieu origin trong `AuthServer:App:CorsOrigins`.

---

## Ghi nho nhanh
- **Authorize khong hien** -> `security` trong swagger.json rong.
- **Authorize hien nhung login fail** -> CORS / Redirect URI / Scope.

