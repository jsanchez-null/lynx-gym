# App — Lynx Gym (React Native + Expo, TypeScript)

Estructura según la sección 2.2 del documento de arquitectura.

- `src/screens/` — Login, MiMembresia, Promos, Aforo, Escaner, Gestion, Ventas, Perfil
- `src/components/` — TarjetaMembresia, CodigoQR, TarjetaPromo, ContadorAforo, etc.
- `src/services/` — `api.ts`: toda la comunicación con el backend (Axios + JWT). Ninguna pantalla llama a la API directamente.
- `src/state/` — Sesión y membresía actual (Context API, propuesta).
- `src/navigation/` — Flujo entre pantallas según el rol (React Navigation).
- `src/utils/` — Formato de fechas, moneda, validaciones.

Navegación: Login → Mi Membresía (tarjeta + QR fijo, vigencia) → Promos → Aforo.
Trabajador: Escáner → Resultado, Registrar socio. Administrador: Gestión y Ventas.
