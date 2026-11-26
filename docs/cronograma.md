# Lynx Gym — Cronograma Semana 1 a Semana 5 (borrador para revisar)

**Proyecto:** Lynx Gym — Equipo LYNXWARE (Paul Urías Sámano y José Carlos Sánchez Castro)
**Materia:** Programación de Dispositivos Móviles J303 — Ejercicio 19-20
**Estado:** Revisado con Paul el 25 nov 2026 y puesto en el Google Doc como Parte 3.2.
**Fechas:** tentativas, contando 5 semanas desde el lunes 16 nov 2026 (Semana 1). La Semana 1 se está cerrando el 23-25 nov, así que la Semana 2 ya empezó en calendario; ajustar con Paul y con el profesor si les dan nuevas fechas.
**Responsables:** acordado el 25 nov. El backend se divide entre los dos (ver "División del backend" abajo); el frontend es lo más fácil y se hace entre los dos, sin reparto rígido.

---

## Resumen

| Semana | Fechas tentativas | Objetivo | Entregable |
|---|---|---|---|
| 1 | lun 16 – dom 22 nov (cierre 23-25 nov) | Definición y arquitectura | Doc completo: Partes 1, 2 y 3 + 5 wireframes |
| 2 | lun 23 – dom 29 nov | Base: repo, base de datos y backend base | Repo con arranque, BD creada, login con JWT funcionando |
| 3 | lun 30 nov – dom 6 dic | App del socio: membresía, QR, aforo y promos | Login, Mi Membresía, Promos y Registrar socio funcionando con la API |
| 4 | lun 7 – dom 13 dic | Flujo del trabajador y del dueño | Escáner QR (entrada/salida), Gestión y Ventas funcionando |
| 5 | lun 14 – dom 20 dic | Pruebas, correcciones y entrega final | App probada de punta a punta, demo y documento final |

---

## Semana 1 — Definición y arquitectura

**Fechas tentativas:** lun 16 – dom 22 nov 2026 (cierre 23-25 nov).
**Objetivo:** dejar por escrito qué se va a construir y cómo, sin código terminado.

- Paul: Parte 1 — descripción, problemática, objetivos, alcance, roles, RF01–RF11, RNF01–RNF04 y 3 casos de uso (Doc 18/11, revisado 23/11). Hecho.
- Jose: Parte 2 — arquitectura 2.1–2.7, 14 endpoints, diagrama ER y diccionario, seguridad, repo en GitHub (puesto en el Doc 23/11). Hecho.
- Equipo (pendiente de confirmar quién): 5 wireframes — Login, Mi Membresía, Promos, Detalle y Registrar socio — y Parte 3, tabla de responsabilidades. En borrador en `parte-3-y-wireframes-borrador.md`.
- Equipo (pendiente): subir el arranque del repo a https://github.com/jsanchez-null/lynx-gym (sigue vacío). Pasos en `repo-starter/como-subirlo.md`.
- **Entregable:** Google Doc "Definición y arquitectura de Lynx Gym" con Partes 1, 2 y 3 y los 5 wireframes.

## División del backend (acordado 25 nov)

El frontend es lo más fácil y se hace entre los dos, por pantallas y sin reparto rígido. El backend, que es lo más pesado, se divide por módulos, y cada quien hace su parte completa en las 3 capas (Controllers / Services / Repositories) para sus endpoints.

- **Jose — Autenticación, usuarios y membresías:** `Auth/` (login, JWT, roles, RF01, RF02), `Middleware/` (errores y respuestas estándar), y los endpoints de usuarios y membresías: `POST /api/auth/login`, `POST /api/usuarios`, `POST /api/admin/membresias`, `GET /api/membresias/mia` (RF03, RF04, RF09, RF10).
- **Paul — Base de datos, accesos, promos y ventas:** crear las 5 tablas en SQL Server y sus relaciones (2.5: Usuarios, Membresias, Promos, Accesos, Pagos), y los endpoints de accesos y aforo (`POST /api/accesos/check-in`, `POST /api/accesos/check-out`, `GET /api/aforo`, `GET /api/accesos/activos`, RF05, RF07, RF08), promos (`GET /api/promos`, `POST / PUT / DELETE /api/promos/{id}`, RF06, RF11) y ventas del dueño (`GET /api/dueno/ventas`, `GET /api/dueno/resumen`, RF12).
- **En común, primero (Semana 2):** acordar los Models/DTOs y el formato estándar de respuesta `{ ok, data, error }`, para que las dos mitades del backend encajen.
- **Frontend (entre los dos):** Login, Mi Membresía, Promos, Aforo, Escáner, Gestión y Ventas; el que termine su parte del backend ayuda con las pantallas que falten.

## Semana 2 — Base: repo, base de datos y backend base

**Fechas tentativas:** lun 23 – dom 29 nov 2026.
**Objetivo:** que el proyecto compile y la API responda, aunque la app todavía no tenga pantallas.

- Subir el arranque del repo (README, .gitignore, `app/src`, `backend/Api`).
- Crear la base de datos en SQL Server con las 5 tablas de 2.5: Usuarios, Membresias, Promos, Accesos y Pagos. **Paul.**
- Acordar entre los dos los Models/DTOs y el formato estándar de respuesta.
- Backend ASP.NET Core en 3 capas (Controllers / Services / Repositories): login con JWT y control por roles (RF01, RF02). **Jose.**
- Primeros endpoints con Swagger: `POST /api/auth/login`, `GET /api/membresias/mia`, `GET /api/promos`.
- **Entregable:** repo con el arranque subido, BD creada y login funcionando en Swagger.

## Semana 3 — App del socio: membresía, QR, aforo y promos

**Fechas tentativas:** lun 30 nov – dom 6 dic 2026.
**Objetivo:** que un socio pueda entrar a la app, ver su membresía y su QR, y consultar aforo y promos.

- Frontend React Native + Expo: Login (RF01), Mi Membresía con QR fijo, vigencia y días restantes (RF03, RF04), Aforo (RF05) y Lista de promos vigentes (RF06).
- Registro de socio y membresía (RF09, RF10): plan mensual, anual o familiar, promo activa o "sin promo", pago en efectivo o transferencia (la app solo registra el método, no cobra).
- Conectar las pantallas con la API (`services/api.ts`, Axios + JWT) y probar el flujo completo socio: login → membresía → QR → promos.
- **Responsables:** frontend entre los dos; backend de membresías y promos según la división de arriba (Jose membresías, Paul promos).
- **Entregable:** app con el flujo del socio funcionando contra la API.

## Semana 4 — Flujo del trabajador y del dueño

**Fechas tentativas:** lun 7 – dom 13 dic 2026.
**Objetivo:** cerrar el flujo que hace funcionar el gym: el trabajador escanea y el dueño consulta.

- Escáner QR del trabajador (RF07, RF08): check-in y check-out, registro en Accesos y actualización del aforo.
- Gestión de promos (RF11): crear, editar y eliminar promos, por Trabajador y Administrador.
- Panel del dueño (RF12): ventas por tiempo (día, semana o mes), membresías activas y socios dentro, a partir de los pagos registrados en el gym.
- Detalle de socio y membresía (RF04, RF09, RF10, RF12): plan, promo o "sin promo", último pago y accesos.
- **Responsables:** backend de accesos, promos y ventas, **Paul**; frontend entre los dos.
- **Entregable:** app completa de punta a punta: socio, trabajador y dueño.

## Semana 5 — Pruebas, correcciones y entrega final

**Fechas tentativas:** lun 14 – dom 20 dic 2026.
**Objetivo:** dejar la app estable y entregar el proyecto completo.

- Probar todos los RF (RF01–RF12) y los RNF (seguridad, rendimiento de escaneo ≤ 2 s, usabilidad): casos de prueba con membresía activa, vencida, QR ya dentro, promo y sin promo.
- Corregir errores, probar en un dispositivo real con Expo Go y preparar la demo: registro de socio → QR → escaneo de entrada → aforo → salida → ventas del dueño.
- Actualizar el documento si cambió algo (con los cambios documentados, como pide la guía) y preparar la entrega final.
- **Responsable:** los dos, revisión final conjunta.
- **Entregable:** app probada, demo lista y documento final entregado.

---

## Notas

- Este cronograma es la sección 3.2 (Plan de trabajo) que pide la guía: 5 semanas, entregable por semana y responsable por módulo principal. Las fechas son tentativas; el reparto del backend quedó acordado el 25 nov.
- Si el profesor mueve las fechas de entrega, las semanas se recorren igual: cada semana es un bloque, no una fecha fija.
- El repo https://github.com/jsanchez-null/lynx-gym sigue vacío al 25 nov; la Semana 2 empieza por subir el arranque.

## Fuentes

- Guía del Ejercicio 19-20: proyecto de 5 semanas, Parte 3.2 plan de trabajo con entregable por semana y responsable por módulo.
- Google Doc "Definición y arquitectura de Lynx Gym" (Partes 1 y 2, verificadas 23 nov) y `parte-2-lynx-gym-borrador-completo.md` (2.1-2.7, 14 endpoints, ER con Usuarios/Membresias/Promos/Accesos/Pagos).
- `parte-3-y-wireframes-borrador.md` (23 nov): 5 wireframes y tabla de la Parte 3 de la Semana 1.
- `repo-starter/` (README, .gitignore, `app/src`, `backend/Api`, `como-subirlo.md`).
