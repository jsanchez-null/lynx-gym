-- =============================================================================
-- Lynx Gym — Esquema de Base de Datos (SQL Server)
-- Equipo: LYNXWARE | Materia: Programación de Dispositivos Móviles J303
-- Basado en: docs/definicion-arquitectura.md y docs/semana-2-models-dtos.md
-- =============================================================================

IF DB_ID('LynxGym') IS NULL
BEGIN
    CREATE DATABASE LynxGym;
END
GO

USE LynxGym;
GO

-- =============================================================================
-- 1. USUARIOS
-- Función: Almacena todas las cuentas del sistema (socios, trabajadores de
-- recepción y administradores/dueños). Se usa para el login (JWT), control
-- de acceso por roles (RBAC) y registro de quién crea promos o cobra pagos.
-- =============================================================================
IF OBJECT_ID('dbo.Usuarios', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios (
        Id              INT IDENTITY(1,1) PRIMARY KEY,
        Nombre          NVARCHAR(120) NOT NULL,
        Correo          NVARCHAR(150) NOT NULL UNIQUE,
        ContrasenaHash  NVARCHAR(255) NOT NULL, -- Hash BCrypt + salt (nunca texto plano)
        Rol             NVARCHAR(20)  NOT NULL CHECK (Rol IN ('socio', 'trabajador', 'administrador')),
        Telefono        NVARCHAR(20)  NULL,
        CreadoEn        DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

-- =============================================================================
-- 2. PROMOS
-- Función: Catálogo de promociones y descuentos del gimnasio. Permite a
-- trabajadores y administradores publicar ofertas vigentes que los socios
-- pueden consultar en la app y aplicar al asignar o renovar una membresía.
-- =============================================================================
IF OBJECT_ID('dbo.Promos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Promos (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        Titulo       NVARCHAR(120)  NOT NULL,
        Descripcion  NVARCHAR(500)  NULL,
        Descuento    DECIMAL(10, 2) NOT NULL CHECK (Descuento >= 0),
        FechaInicio  DATE           NOT NULL,
        FechaFin     DATE           NOT NULL,
        CreadoPor    INT            NOT NULL, -- Usuario (trabajador/admin) que creó la promo
        Estado       NVARCHAR(20)   NOT NULL CHECK (Estado IN ('vigente', 'vencida')),

        CONSTRAINT FK_Promos_Usuarios FOREIGN KEY (CreadoPor) REFERENCES dbo.Usuarios(Id)
    );
END
GO

-- =============================================================================
-- 3. MEMBRESIAS
-- Función: Controla los planes contratados por cada socio (mensual, anual o
-- familiar), su vigencia (FechaInicio / FechaFin), estado y código QR único
-- y fijo que el trabajador escanea en recepción.
-- Nota: PromoId es NULL cuando el socio contrató "sin promo".
-- =============================================================================
IF OBJECT_ID('dbo.Membresias', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Membresias (
        Id           INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId    INT           NOT NULL,
        Tipo         NVARCHAR(20)  NOT NULL CHECK (Tipo IN ('mensual', 'anual', 'familiar')),
        PromoId      INT           NULL, -- NULL = sin promo aplicada
        FechaInicio  DATE          NOT NULL,
        FechaFin     DATE          NOT NULL,
        Estado       NVARCHAR(20)  NOT NULL CHECK (Estado IN ('activa', 'vencida', 'suspendida')),
        CodigoQr     NVARCHAR(100) NOT NULL UNIQUE, -- Código único y fijo por membresía

        CONSTRAINT FK_Membresias_Usuarios FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id),
        CONSTRAINT FK_Membresias_Promos   FOREIGN KEY (PromoId)   REFERENCES dbo.Promos(Id)
    );
END
GO

-- =============================================================================
-- 4. ACCESOS
-- Función: Registra las entradas (check-in) y salidas (check-out) de los
-- socios al escanear su QR en recepción.
-- Regla de negocio: Si FechaHoraSalida es NULL, el socio está dentro ahora;
-- el conteo de registros con FechaHoraSalida IS NULL da el aforo en tiempo real.
-- =============================================================================
IF OBJECT_ID('dbo.Accesos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Accesos (
        Id                INT IDENTITY(1,1) PRIMARY KEY,
        MembresiaId       INT       NOT NULL,
        FechaHoraEntrada  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        FechaHoraSalida   DATETIME2 NULL, -- NULL = el socio sigue dentro del gimnasio

        CONSTRAINT FK_Accesos_Membresias FOREIGN KEY (MembresiaId) REFERENCES dbo.Membresias(Id)
    );
END
GO

-- =============================================================================
-- 5. PAGOS
-- Función: Lleva el historial de pagos realizados en caja del gimnasio
-- (efectivo o transferencia) al asignar o renovar una membresía. Sirve para
-- los reportes de ventas del dueño. La app solo registra el método, no cobra.
-- Nota: No lleva PromoId; si hubo promo, se consulta en Membresias.PromoId.
-- =============================================================================
IF OBJECT_ID('dbo.Pagos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Pagos (
        Id             INT IDENTITY(1,1) PRIMARY KEY,
        MembresiaId    INT            NOT NULL,
        Monto          DECIMAL(10, 2) NOT NULL CHECK (Monto >= 0),
        FechaPago      DATE           NOT NULL DEFAULT CAST(SYSUTCDATETIME() AS DATE),
        Metodo         NVARCHAR(20)   NOT NULL CHECK (Metodo IN ('efectivo', 'transferencia')),
        RegistradoPor  INT            NOT NULL, -- Usuario (trabajador/admin) que registró el pago

        CONSTRAINT FK_Pagos_Membresias FOREIGN KEY (MembresiaId)   REFERENCES dbo.Membresias(Id),
        CONSTRAINT FK_Pagos_Usuarios   FOREIGN KEY (RegistradoPor) REFERENCES dbo.Usuarios(Id)
    );
END
GO

