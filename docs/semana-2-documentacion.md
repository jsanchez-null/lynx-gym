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
- **Responsable:** José Carlos Sánchez Castro (con Antigravity).
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

---

## 3. Próximos pasos (Semana 2 — parte de José Carlos)

- [ ] Inicializar `backend/Api` (ASP.NET Core Web API con Swagger, arquitectura en 3 capas).
- [ ] Crear `ApiResponse<T>`, los 5 Models y los DTOs en `backend/Api/Models/`.
- [ ] Implementar el `Middleware/` de errores estándar (`{ ok, data, error }`).
- [ ] Implementar `Auth/` (`POST /api/auth/login` con BCrypt, JWT a 24 h y control por roles).
- [ ] Implementar los endpoints de usuarios y membresías (`POST /api/usuarios`, `POST /api/admin/membresias`, `GET /api/membresias/mia`) y probarlos en Swagger.
