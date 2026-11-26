# Lynx Gym

App de gym ficticio — Equipo LYNXWARE (Paul Urías Sámano y José Carlos Sánchez Castro).
Materia: Programación de Dispositivos Móviles J303 — Ejercicio 19-20.

La app controla la membresía de los socios con un código QR fijo, muestra el aforo,
las promos vigentes y la vigencia de la membresía. El trabajador escanea el QR en
recepción. El pago se hace en el gym; la app solo registra el método (efectivo o
transferencia) y consulta los datos. Documento de definición y arquitectura:
Google Doc del equipo ("Definición y arquitectura de Lynx Gym").

## Stack

- Frontend: React Native con Expo y TypeScript (`app/`)
- Backend: API REST en ASP.NET Core, 3 capas (`backend/Api/`)
- Base de datos: SQL Server (tablas Usuarios, Membresias, Promos, Accesos, Pagos)
- Autenticación: JWT (ver 2.3 y 2.6 del documento)

## Estructura

```
app/       # React Native + Expo (ver app/README.md)
backend/   # ASP.NET Core API en 3 capas (ver backend/README.md)
docs/      # Documento y diagramas del proyecto
```

## Desarrollo

- Backend: `dotnet run` dentro de `backend/Api` (Swagger para probar los endpoints).
- App: Expo Go en un dispositivo real o emulador.

Semana #1: solo se pide el documento de definición y arquitectura, no código terminado.
Este arranque es la estructura para las siguientes semanas.
