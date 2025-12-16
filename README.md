# DailyCost

Implementation per DailyCost-PRD.md
Stack: ASP.NET Core 10 Web API + MySQL + Vue3/Vite

## Backend (API)
- Path: src/DailyCost.Api
- Config: src/DailyCost.Api/appsettings.json
  - ConnectionStrings:DefaultConnection (edit as needed)
  - Jwt:Secret (use >=32 chars random)
  - Database:AutoMigrate (enable in dev; in prod set false if no DDL permission and import scripts/schema.sql)
- Run: dotnet run --project src/DailyCost.Api
- Swagger (dev): http://localhost:5000/swagger

## Frontend (Web)
- Path: dailycost-web
- Dev: npm install && npm run dev (Vite proxies /api -> backend)
- Build: npm run build; deploy dist/ to /wwwroot/dailycost (base=/dailycost)

## Database init
- If no auto-migrate: run scripts/schema.sql
- Existing DB baseline migration id: 20251127024114_InitialCreate (aligned with code)

## Migrations
- Add: dotnet ef migrations add XxxChange -p src/DailyCost.Infrastructure -s src/DailyCost.Api -o Data/Migrations
- Apply: dotnet ef database update -p src/DailyCost.Infrastructure -s src/DailyCost.Api

## Deploy notes
- Backend: publish to /opt/dailycost/api; Kestrel on 127.0.0.1:5000; systemd service
- Frontend: /wwwroot/dailycost
- Nginx: serve /dailycost; proxy /dailycostapi -> 127.0.0.1:5000
