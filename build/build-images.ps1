param ($version='latest')

$currentFolder = $PSScriptRoot
$slnFolder = Join-Path $currentFolder "../"
### Apps Folders
$blazorAppFolder = Join-Path $slnFolder "apps/blazor/src/saasLMS.Blazor"
$authserverFolder = Join-Path $slnFolder "apps/auth-server/src/saasLMS.AuthServer"
$publicWebFolder = Join-Path $slnFolder "apps/public-web/src/saasLMS.PublicWeb"

### Microservice Folders
$identityServiceFolder = Join-Path $slnFolder "services/identity/src/saasLMS.IdentityService.HttpApi.Host"
$administrationServiceFolder = Join-Path $slnFolder "services/administration/src/saasLMS.AdministrationService.HttpApi.Host"
$saasServiceFolder = Join-Path $slnFolder "services/saas/src/saasLMS.SaasService.HttpApi.Host"
$productServiceFolder = Join-Path $slnFolder "services/product/src/saasLMS.ProductService.HttpApi.Host"

### Gateway Folders
$webGatewayFolder = Join-Path $slnFolder "gateways/web/src/saasLMS.WebGateway"
$webPublicGatewayFolder = Join-Path $slnFolder "gateways/web-public/src/saasLMS.PublicWebGateway"

### DB Migrator Folder
$dbmigratorFolder = Join-Path $slnFolder "shared/saasLMS.DbMigrator"

Write-Host "*** BUILDING BASE IMAGE ***" -ForegroundColor Green
Set-Location $slnFolder
docker build -f "Dockerfile.base" -t mycompanyname/saaslms-base:1.0 .

Write-Host "*** BUILDING DB MIGRATOR ***" -ForegroundColor Green
Set-Location $slnFolder
docker build -f "$dbmigratorFolder/Dockerfile" -t mycompanyname/saaslms-db-migrator:$version .

Write-Host "===== BUILDING MICROSERVICES =====" -ForegroundColor Yellow 
### IDENTITY-SERVICE
Write-Host "*** BUILDING IDENTITY-SERVICE ***" -ForegroundColor Green
Set-Location $slnFolder
docker build -f "$identityServiceFolder/Dockerfile" -t mycompanyname/saaslms-service-identity:$version .

### ADMINISTRATION-SERVICE
Write-Host "*** BUILDING ADMINISTRATION-SERVICE ***" -ForegroundColor Green
Set-Location $slnFolder
docker build -f "$administrationServiceFolder/Dockerfile" -t mycompanyname/saaslms-service-administration:$version .

### SAAS-SERVICE
Write-Host "*** BUILDING SAAS-SERVICE ***" -ForegroundColor Green
Set-Location $slnFolder
docker build -f "$saasServiceFolder/Dockerfile" -t mycompanyname/saaslms-service-saas:$version .

### PRODUCT-SERVICE
Write-Host "*** BUILDING PRODUCT-SERVICE ***" -ForegroundColor Green
Set-Location $slnFolder
docker build -f "$productServiceFolder/Dockerfile" -t mycompanyname/saaslms-service-product:$version .
Write-Host "===== COMPLETED BUILDING MICROSERVICES =====" -ForegroundColor Yellow 

Write-Host "===== BUILDING GATEWAYS =====" -ForegroundColor Yellow 
### WEB-GATEWAY
Write-Host "*** BUILDING WEB-GATEWAY ***" -ForegroundColor Green
Set-Location $slnFolder
docker build -f "$webGatewayFolder/Dockerfile" -t mycompanyname/saaslms-gateway-web:$version .

### PUBLICWEB-GATEWAY
Write-Host "*** BUILDING WEB-PUBLIC-GATEWAY ***" -ForegroundColor Green
Set-Location $slnFolder
docker build -f "$webPublicGatewayFolder/Dockerfile" -t mycompanyname/saaslms-gateway-web-public:$version .
Write-Host "===== COMPLETED BUILDING GATEWAYS =====" -ForegroundColor Yellow 

Write-Host "===== BUILDING APPLICATIONS =====" -ForegroundColor Yellow 
### AUTH-SERVER
Write-Host "*** BUILDING AUTH-SERVER ***" -ForegroundColor Green
Set-Location $slnFolder
docker build -f "$authserverFolder/Dockerfile" -t mycompanyname/saaslms-app-authserver:$version .

### PUBLIC-WEB
Write-Host "*** BUILDING WEB-PUBLIC ***" -ForegroundColor Green
Set-Location $slnFolder
docker build -f "$publicWebFolder/Dockerfile" -t mycompanyname/saaslms-app-public-web:$version .
### Blazor WEB App
if (Test-Path -Path $blazorAppFolder) {
    Write-Host "*** BUILDING BLAZOR WEB APPLICATION ***" -ForegroundColor Green
    Set-Location $slnFolder
    docker build -f "$blazorAppFolder/Dockerfile" -t mycompanyname/saaslms-app-blazor:$version .
}
Write-Host "===== COMPLETED BUILDING APPLICATIONS =====" -ForegroundColor Yellow

### ALL COMPLETED
Write-Host "ALL COMPLETED" -ForegroundColor Green
Set-Location $currentFolder