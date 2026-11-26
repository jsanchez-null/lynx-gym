# Lynx Gym — Documentación de Semana 2

**Proyecto:** Lynx Gym — Equipo LYNXWARE (Paul Urías Sámano, José Carlos Sánchez Castro y Erika Daniela Verdugo Rodríguez)  
**Materia:** Programación de Dispositivos Móviles J303 — Ejercicio 19-20  
**Objetivo de la Semana 2:** Base del proyecto (arranque del frontend en Expo, base de datos, contratos compartidos, estructura 3 capas del backend, middleware de errores y login con JWT).

---

## 1. Decisiones técnicas acordadas (26 nov 2026)

Antes de iniciar con el código del backend de José Carlos, se definieron los siguientes criterios sobre los puntos abiertos de [`docs/semana-2-models-dtos.md`](semana-2-models-dtos.md):

1. **Rol por defecto en `POST /api/usuarios`:** siempre crea usuarios con rol `"socio"`.
2. **Fecha de inicio de membresía:** siempre inicia el día en que se registra o renueva (fecha actual).
3. **Renovar vs. crear nueva membresía (`POST /api/admin/membresias`):** se **actualiza la membresía existente** del socio (si aún no tiene, se crea) y se registra **un nuevo pago en `Pagos`** en cada renovación.
   - *Motivo:* el `CodigoQr` es único y fijo por socio (funciona también como identificador en recepción para check-in, check-out, renovaciones y pagos). Mantener un único registro en `Membresias` evita duplicar o cambiar el QR, mientras `Pagos` y `Accesos` conservan el historial completo.
4. **Expiración del JWT:** **24 horas** (configurable en `appsettings.json`).

---

## 2. Registro de avances

### 26 nov 2026 — Arranque del frontend con plantilla Expo Tabs (`app/`)
- **Responsable:** José Carlos Sánchez Castro.
- **Qué se hizo:**
  - Se inicializó `app/` con la plantilla **Tabs** de Expo (`create-expo-app --template tabs`, con `expo-router` y TypeScript), pensada para probarse en **Expo Go** y contar con la barra de navegación inferior (*Bottom Tab Bar*) acorde a los diseños de referencia.
  - Se conservaron `app/README.md` y la estructura base de `app/src/`.
  - Se ajustó la configuración inicial en `app/package.json` y `app/app.json` (`name: "Lynx Gym"`, `slug: "lynx-gym"`, `scheme: "lynxgym"`).
  - Se mantuvo `node_modules/` ignorado por `.gitignore`.

### 26 nov 2026 — Base de datos SQL Server, Models compartidos y DTOs de Paul (`backend/`)
- **Responsable:** Paul Urías Sámano.
- **Qué se hizo:**
  - **Base de datos (`backend/Database/schema.sql`):** se organizó el script en `backend/Database/schema.sql` y se creó la base de datos **`LynxGym`** en `(localdb)\MSSQLLocalDB` (sin datos de prueba) con las 5 tablas y sus llaves foráneas: `Usuarios`, `Promos`, `Membresias`, `Accesos` y `Pagos`.
  - **Models compartidos (`backend/Api/Models/`):** se crearon `Usuario.cs`, `Membresia.cs`, `Promo.cs`, `Acceso.cs` y `Pago.cs` siguiendo `docs/semana-2-models-dtos.md`.
  - **DTOs (`backend/Api/Models/DTOs/`):** se crearon `ApiResponse.cs` (`{ ok, data, error }`), `PromoDto.cs`, `CheckInRequest.cs`, `AccesoDto.cs` y `VentasResumenDto.cs`.

### 26 nov 2026 — Base de `backend/Api` (ASP.NET Core 3 capas, Auth JWT, Middleware y Endpoints de José Carlos)
- **Responsable:** José Carlos Sánchez Castro.
- **Qué se hizo:**
  - Se inicializó el proyecto ASP.NET Core Web API (`net10.0`) en `backend/Api/` con **Swagger** (`Swashbuckle.AspNetCore`), **JWT** (`Microsoft.AspNetCore.Authentication.JwtBearer`), **BCrypt** (`BCrypt.Net-Next`) y **SQL Server** (`Microsoft.Data.SqlClient`).
  - Se completaron los contratos compartidos en `Models/` y `Models/DTOs/` (`ApiResponse<T>.Success/Fail`, `AuthDtos.cs`, `UsuarioDtos.cs`, `MembresiaDtos.cs`).
  - Se implementó `Middleware/ErrorHandlingMiddleware.cs` y `ApiException.cs` para estandarizar todos los errores (`400`, `401`, `403`, `404`, `409`, `500`) con `ApiResponse`.
  - Se implementó `Auth/JwtTokenService.cs` y `JwtOptions.cs` (claims `id`, `correo`, `rol`, expiración de 24 h).
  - Se implementaron las 3 capas (`Controllers/`, `Services/`, `Repositories/`) para los 4 endpoints asignados a José Carlos:
    - `POST /api/auth/login` (público, valida hash BCrypt y emite JWT).
    - `POST /api/usuarios` (roles `trabajador` / `administrador`, registra socios con rol `"socio"`).
    - `POST /api/admin/membresias` (roles `trabajador` / `administrador`, asigna o renueva membresía conservando el `CodigoQr` fijo y registrando el pago en `Pagos`).
    - `GET /api/membresias/mia` (autenticado, devuelve la tarjeta de membresía del socio con `diasRestantes` y `codigoQr`).
  - Se incluyeron repositorios `InMemory*` (activos por defecto cuando `Database:UseSqlServer = false`) para probar en Swagger de inmediato, junto con `SqlUsuarioRepository` y `SqlMembresiaRepository` listos para activar SQL Server (`(localdb)\MSSQLLocalDB`, BD `LynxGym`).
  - Se habilitó CORS (`ExpoDev`) para pruebas desde Expo Go en red local.
- **Usuarios de prueba (modo en memoria / Swagger):**
  - Admin: `admin@lynxgym.com` / `Admin123!`
  - Trabajador: `trabajador@lynxgym.com` / `Trabajador123!`
  - Socio: `socio@lynxgym.com` / `Socio123!` (incluye membresía demo con QR `LYNX-3-DEMOQR01`)

---

## 3. Próximos pasos (Semana 2)

- [x] Inicializar `backend/Api` (ASP.NET Core Web API con Swagger, arquitectura en 3 capas).
- [x] Crear `ApiResponse<T>`, los 5 Models y los DTOs en `backend/Api/Models/`.
- [x] Implementar el `Middleware/` de errores estándar (`{ ok, data, error }`).
- [x] Implementar `Auth/` (`POST /api/auth/login` con BCrypt, JWT a 24 h y control por roles).
- [x] Implementar los endpoints de usuarios y membresías (`POST /api/usuarios`, `POST /api/admin/membresias`, `GET /api/membresias/mia`).
- [x] Probar los endpoints en Swagger (`dotnet run` dentro de `backend/Api`). ✅ Funciona con repositorios en memoria.
- [x] Conectar con las 5 tablas de SQL Server de Paul (`Database:UseSqlServer = true`). ✅ BD `LynxGym` en `(localdb)\MSSQLLocalDB` creada y verificada.
- [x] Insertar datos de prueba en SQL Server (`backend/Database/seed.sql`): Admin Lynx, Recepcion Lynx y Alex Socio con hashes BCrypt.
