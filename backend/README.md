# Backend — API Lynx Gym (ASP.NET Core, 3 capas)

Estructura según la sección 2.3 del documento de arquitectura.

- `Controllers/` — Rutas: reciben peticiones, validan lo básico, devuelven HTTP. Sin reglas de negocio.
- `Services/` — Reglas de negocio y validaciones (ej.: no permitir check-in con membresía vencida).
- `Repositories/` — Única capa que toca la base de datos (SQL Server).
- `Models/DTOs/` — Contratos de entrada/salida de la API.
- `Auth/` — JWT: login, generación y validación de token, roles.
- `Middleware/` — Manejo de errores y respuestas estándar `{ ok, data, error }`.

Endpoints (14, sección 2.4): `/api/auth/login`, `/api/membresias/mia`, `/api/promos`,
`/api/accesos/check-in`, `/api/accesos/check-out`, `/api/aforo`, `/api/accesos/activos`,
`/api/usuarios`, `/api/admin/membresias`, `/api/dueno/ventas`, `/api/dueno/resumen`.
