# Lynx Gym — Semana 2: Models, DTOs y formato estándar de respuesta (borrador para acordar con Paul)

**Proyecto:** Lynx Gym — Equipo LYNXWARE (Paul Urías Sámano y José Carlos Sánchez Castro)
**Materia:** Programación de Dispositivos Móviles J303 — Ejercicio 19-20
**Estado:** Borrador preparado el miércoles 25 de noviembre de 2026. No está en el Google Doc ni en GitHub. Es el primer paso en común de la Semana 2, acordado hoy 25 nov: antes de dividir el backend, acordar los Models/DTOs y el formato estándar de respuesta, para que las dos mitades encajen.
**Fuentes:** Parte 2 del Doc — 2.3 (backend 3 capas, JWT, formato `{ ok, data, error }`), 2.4 (14 endpoints, RF01–RF12), 2.5 (ER y diccionario: Usuarios, Membresias, Promos, Accesos, Pagos), 2.6 (seguridad). Decisiones cerradas 23 nov: QR fijo, planes mensual/anual/familiar, promo activa o "sin promo" guardada en `Membresias.promo_id`, promo informativa en Pagos (Pagos NO lleva `promo_id`), pago en el gym en efectivo o transferencia, la app no cobra.
**Reparto acordado 25 nov:** Jose — Auth/JWT, usuarios y membresías + Middleware de errores. Paul — BD, accesos/aforo, promos y ventas del dueño. Frontend entre los dos.

---

## 0. Antes de picar código — Semana 1 todavía tiene 2 pendientes

1. **Repo vacío:** https://github.com/jsanchez-null/lynx-gym sigue vacío al 25 nov. Paul acaba de recibir el link; si lo clona ahora, no encuentra nada. Subir primero el arranque con `repo-starter/como-subirlo.md` (README, .gitignore, `app/`, `backend/`).
2. **Wireframes:** siguen solo en borrador en texto en `parte-3-y-wireframes-borrador.md` (Login, Mi Membresía, Promos, Detalle, Registrar socio). No se metieron al Doc hoy; falta confirmar quién los pasa en limpio.

Este archivo es el siguiente paso lógico después de eso: el contrato que los dos van a programar contra él.

---

## 1. Formato estándar de respuesta — lo usan TODOS los endpoints

De 2.3: `{ "ok": true/false, "data": ..., "error": "..." }`, con el código HTTP correcto. Nada de trazas ni consultas SQL en el error (2.6).

```csharp
// Models/DTOs/ApiResponse.cs — en backend/Api/Models/DTOs/
public class ApiResponse<T>
{
    public bool Ok { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
}
```

- Éxito: `Ok = true`, `Data` con el DTO, `Error = null`. HTTP 200 (201 al crear).
- Error: `Ok = false`, `Data = null`, `Error` con un mensaje corto para el usuario. HTTP según la tabla de abajo.
- El JSON sale en camelCase (`ok`, `data`, `error`) por la configuración de ASP.NET Core; en C# los nombres van en PascalCase como arriba.

### 1.1 Middleware de errores (parte de Jose)

Un solo middleware centraliza los errores, para que ningún controller devuelva otro formato:

| HTTP | Cuándo | `error` (ejemplo) |
|---|---|---|
| 400 | Faltan campos o vienen con formato inválido | `"Faltan campos obligatorios."` |
| 401 | Sin token, token vencido o inválido | `"Sesión no válida. Vuelve a iniciar sesión."` |
| 403 | Token válido pero rol sin permiso para esa ruta | `"No tienes permiso para esta acción."` |
| 404 | Usuario / membresía / promo no existe | `"No se encontró el recurso."` |
| 409 | Regla de negocio rota (ej.: check-in con un acceso ya abierto, membresía vencida) | `"La membresía ya tiene un acceso abierto."` |
| 500 | Error inesperado del servidor | `"Ocurrió un error interno."` (sin detalles) |

---

## 2. Models — calzados al ER de 2.5, sin campos inventados

```csharp
// Models/Usuario.cs
public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";          // único
    public string ContrasenaHash { get; set; } = "";  // BCrypt + salt, nunca en texto plano, nunca se devuelve
    public string Rol { get; set; } = "";             // "socio" | "trabajador" | "administrador"
    public string? Telefono { get; set; }             // opcional
    public DateTime CreadoEn { get; set; }
}

// Models/Membresia.cs
public class Membresia
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Tipo { get; set; } = "";            // "mensual" | "anual" | "familiar"
    public int? PromoId { get; set; }                 // nulo = "sin promo" (decisión promo B, 23 nov)
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }             // vigencia
    public string Estado { get; set; } = "";          // "activa" | "vencida" | "suspendida"
    public string CodigoQr { get; set; } = "";        // único, fijo por membresía; solo se guarda el código, no la imagen
}

// Models/Promo.cs
public class Promo
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string? Descripcion { get; set; }
    public decimal Descuento { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public int CreadoPor { get; set; }                // FK -> Usuarios (trabajador o administrador)
    public string Estado { get; set; } = "";          // "vigente" | "vencida"
}

// Models/Acceso.cs
public class Acceso
{
    public int Id { get; set; }
    public int MembresiaId { get; set; }
    public DateTime FechaHoraEntrada { get; set; }
    public DateTime? FechaHoraSalida { get; set; }    // nulo = el socio está dentro ahora
}

// Models/Pago.cs
public class Pago
{
    public int Id { get; set; }
    public int MembresiaId { get; set; }
    public decimal Monto { get; set; }                // monto pagado en el gym (con el descuento ya aplicado en caja)
    public DateOnly FechaPago { get; set; }
    public string Metodo { get; set; } = "";          // "efectivo" | "transferencia" — solo se indica, no se cobra en la app
    public int RegistradoPor { get; set; }            // FK -> Usuarios
    // OJO: Pagos NO lleva promo_id. Si la membresía fue con promo, se consulta en Membresias.PromoId.
}
```

Relaciones (2.5): un Usuario tiene una o varias Membresías; una Membresía tiene muchos Accesos y muchos Pagos, y puede tener una Promo o ninguna. "Socios dentro ahora" = Accesos con `FechaHoraSalida` nula. "Aforo" = conteo de esos accesos.

---

## 3. DTOs — contratos de entrada/salida por endpoint (2.4)

### 3.1 Auth — Jose (`POST /api/auth/login`, RF01)

```csharp
public class LoginRequest
{
    public string Correo { get; set; } = "";
    public string Contrasena { get; set; } = "";
}

public class LoginResponse
{
    public string Token { get; set; } = "";           // JWT
    public UsuarioDto Usuario { get; set; } = null!;
}

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";
    public string Rol { get; set; } = "";
    public string? Telefono { get; set; }
    // Sin ContrasenaHash y sin CreadoEn: nunca se devuelven (2.6).
}
```

JWT (2.3 / 2.6): claims `id`, `correo`, `rol`, con expiración. La app lo guarda y lo manda en cada petición; el backend valida el rol en cada ruta protegida (RF02).

### 3.2 Usuarios y membresías — Jose

```csharp
// POST /api/usuarios — Registrar socio (RF09, trabajador / administrador)
public class CrearUsuarioRequest
{
    public string Nombre { get; set; } = "";
    public string Correo { get; set; } = "";
    public string Contrasena { get; set; } = "";      // se guarda con hash, nunca en plano
    public string? Telefono { get; set; }
    // Rol: ver pregunta abierta nº 1 abajo.
}

// POST /api/admin/membresias — Asignar o renovar membresía (RF10, trabajador / administrador)
public class CrearMembresiaRequest
{
    public int UsuarioId { get; set; }
    public string Tipo { get; set; } = "";            // "mensual" | "anual" | "familiar"
    public int? PromoId { get; set; }                 // nulo = "sin promo"
    public decimal Monto { get; set; }                // monto pagado en el gym
    public string Metodo { get; set; } = "";          // "efectivo" | "transferencia"
}

// GET /api/membresias/mia — Mi tarjeta de membresía (RF03, RF04, socio dueño)
public class MembresiaDto
{
    public int Id { get; set; }
    public string Tipo { get; set; } = "";
    public string? PromoTitulo { get; set; }          // null = "sin promo"
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public string Estado { get; set; } = "";
    public int DiasRestantes { get; set; }            // calculado: FechaFin - hoy
    public string CodigoQr { get; set; } = "";        // solo el socio dueño y el trabajador que escanea lo ven (2.6)
}
```

Reglas de negocio en Services (2.3): al crear la membresía se elige una promo activa o "sin promo"; la promo queda guardada en la membresía. El pago solo registra método y monto, no cobra.

### 3.3 Promos, accesos, aforo y ventas — Paul (contratos para que encajen con lo de Jose)

```csharp
// GET /api/promos — solo vigentes (RF06). POST/PUT/DELETE /api/promos/{id} (RF11, trabajador / administrador)
public class PromoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string? Descripcion { get; set; }
    public decimal Descuento { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
}

// POST /api/accesos/check-in y /check-out (RF07, RF08, trabajador)
public class CheckInRequest { public string CodigoQr { get; set; } = ""; }
public class AccesoDto
{
    public int Id { get; set; }
    public int MembresiaId { get; set; }
    public DateTime FechaHoraEntrada { get; set; }
    public DateTime? FechaHoraSalida { get; set; }
}

// GET /api/aforo (RF05) -> ApiResponse<int> con el conteo.
// GET /api/accesos/activos (RF05, RF08) -> ApiResponse<List<AccesoDto>> con salida nula.

// GET /api/dueno/ventas?periodo=mes y GET /api/dueno/resumen (RF12, administrador)
public class VentasResumenDto
{
    public decimal Total { get; set; }
    public int MembresiasActivas { get; set; }
    public int SociosDentro { get; set; }
}
```

Todas estas respuestas también van envueltas en `ApiResponse<T>` (sección 1). Si Paul usa otro envoltorio, el front (`services/api.ts`, Axios + JWT) tendría que tratar dos formatos distintos.

---

## 4. Checklist Semana 2 de Jose (23–29 nov)

- [ ] Subir el arranque al repo vacío (ver `repo-starter/como-subirlo.md`) — primero, para que Paul pueda clonar algo.
- [ ] Acordar este archivo con Paul (Models/DTOs + `{ ok, data, error }`) antes de picar cada quien su mitad.
- [ ] Scaffold `backend/Api` con las carpetas de 2.3: Controllers / Services / Repositories / Models/DTOs / Auth / Middleware.
- [ ] Auth: `POST /api/auth/login` con JWT + roles, contraseña con BCrypt.
- [ ] Middleware de errores con la tabla 1.1.
- [ ] Endpoints de Jose: `POST /api/usuarios`, `POST /api/admin/membresias`, `GET /api/membresias/mia` — probados en Swagger.
- [ ] Paul en paralelo: 5 tablas en SQL Server + sus endpoints (accesos, promos, ventas).

---

## 5. Preguntas abiertas — no inventar, acordar con Paul

1. **Rol al registrar socio:** `POST /api/usuarios` ¿recibe el rol en el request o siempre crea "socio" y solo el administrador lo cambia después? (RF09 dice que el trabajador registra socios; no dice que asigne roles.)
2. **Fecha de inicio de la membresía:** ¿siempre es hoy al registrarla, o el trabajador puede elegir otra fecha? `CrearMembresiaRequest` no trae fecha por ahora.
3. **Renovar vs. asignar:** `POST /api/admin/membresias` ¿crea una membresía nueva cada vez (histórico) o actualiza la existente? El ER permite varias membresías por usuario (histórico por renovación).
4. **Expiración del JWT:** ¿cuántas horas? La Parte 2 solo dice "con expiración", sin número.
