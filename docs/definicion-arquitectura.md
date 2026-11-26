# Lynx Gym — Definición y arquitectura (versión del repo)

**Proyecto:** Lynx Gym — app de gym ficticio. **Equipo:** LYNXWARE — Paul Urías Sámano y José Carlos Sánchez Castro.
**Materia:** Programación de Dispositivos Móviles J303 — Ejercicio 19-20, Semana #1.
**Versión:** foto del 25 de noviembre de 2026, ya revisada y puesta en el Google Doc. El documento de trabajo es el Google Doc del equipo; este archivo es la versión guardada en el repo para que quede historial por semana.
**Google Doc:** "Definición y arquitectura de Lynx Gym" (Partes 1, 2 y 3).
**Repositorio:** https://github.com/jsanchez-null/lynx-gym

## 1. Qué es Lynx Gym

Aplicación para gimnasios que controla la membresía de los socios con un código QR fijo, muestra el aforo en tiempo real, las promos vigentes y la fecha de expiración de la membresía. El trabajador escanea el QR en recepción. El pago de la membresía se hace en el gym; la app solo registra el método (efectivo o transferencia) y consulta los datos.

## 2. Roles

- **Socio:** ve su QR, estado y fecha de expiración de su membresía, consulta el aforo y las promos.
- **Trabajador (Recepción):** escanea QR para validar y registrar entradas y salidas, consulta vigencia, registra socios y crea promos.
- **Administrador (dueño):** CRUD completo de socios y trabajadores, asigna y renueva membresías, publica promos y consulta ventas por tiempo, socios dentro y membresías activas.

## 3. Requerimientos (resumen)

- **RF01** Login con correo y contraseña, con redirección por rol. **RF02** Control de acceso por roles (RBAC).
- **RF03** Código QR único y fijo por socio. **RF04** Consulta de vigencia (estado, fecha de expiración, días restantes). **RF05** Aforo en tiempo real. **RF06** Promos vigentes.
- **RF07** Escaneo QR y validación de acceso. **RF08** Registro de entrada/salida y actualización del aforo.
- **RF09** CRUD de usuarios y trabajadores (Administrador); el Trabajador también puede registrar socios. **RF10** Asignar/renovar membresías: plan mensual, anual o familiar, con promo activa o "sin promo" guardada en la membresía. **RF11** CRUD de promos (Trabajador y Administrador). **RF12** Consulta de ventas y datos del dueño.
- **RNF01** Seguridad: BCrypt, HTTPS, JWT con expiración. **RNF02** Escaneo + validación + aforo en ≤ 2 segundos. **RNF03** Usabilidad: QR, vigencia y aforo en máximo 2 toques. **RNF04** Mantenibilidad: código modular.

## 4. Arquitectura

- **Frontend:** React Native con Expo y TypeScript. Navegación con React Navigation; estado con Context API (propuesta); comunicación con la API en `services/api.ts` con Axios y JWT.
- **Backend:** API REST en ASP.NET Core, en 3 capas: Controllers (rutas y validación básica), Services (reglas de negocio) y Repositories (única capa que toca la BD). Autenticación JWT, respuestas estándar `{ ok, data, error }`.
- **Base de datos:** SQL Server. Tablas: Usuarios, Membresias, Promos, Accesos, Pagos. `Membresias.promo_id` es nullable (nulo = sin promo); Pagos no lleva `promo_id`. "Socios dentro ahora" = Accesos con salida nula.
- **API:** 14 endpoints (ver `docs/semana-2-models-dtos.md` y la sección 2.4 del Doc).
- **Seguridad:** contraseñas con hash BCrypt, nunca en texto plano ni devueltas en respuestas; QR fijo, único por membresía; HTTPS en todas las llamadas; validación en el backend.

## 5. Plan de trabajo (resumen)

Ver `docs/cronograma.md`. Semana 1: definición y arquitectura. Semana 2: repo, BD y backend base. Semana 3: app del socio. Semana 4: trabajador y dueño. Semana 5: pruebas y entrega final.

**División del backend (acordado 25 nov):** Jose — autenticación/JWT, usuarios y membresías + Middleware. Paul — base de datos, accesos/aforo, promos y ventas del dueño. En común primero: Models/DTOs y formato estándar de respuesta (ver `docs/semana-2-models-dtos.md`). El frontend se hace entre los dos.
