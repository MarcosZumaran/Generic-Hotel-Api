# La Rica Noche API

API REST para la gestión del hotel **"La Rica Noche"**, un pequeño alojamiento con 3 empleados y una única categoría de habitación. El sistema cubre reservas, estancias, control de habitaciones y reportes diarios.

**Estado actual:** API funcional con Lua scripting, CRUD de tipos de habitación. Próximamente se implementará la gestión de habitaciones, reservas, check‑in/out y emisión de comprobantes electrónicos.

Este proyecto esta enfocado en mis prácticas institucionales, por lo que se encuentra en desarrollo activo y es posible que sufra cambios significativos a medida que avanzo en el aprendizaje de nuevas tecnologías y mejores prácticas de desarrollo. o puede que lo abandoné por completo XD.

---

## Herramientas y tecnologías usadas

- .NET 10 (C# 13)
- ASP.NET Core Web API
- Entity Framework Core 10 (SQL Server)
- Mapster (mapeo objeto-objeto)
- NLua (scripts Lua para validaciones y reglas de negocio)
- Scalar (documentación interactiva de API)
- SQL Server (base de datos)

---

## Instalación y configuración

### Prerrequisitos

- .NET 10
- SQL Server

### Configuración de la base de datos

1. Ejecuta el script `DB.sql` contra tu servidor SQL Server para crear la base de datos con todas las tablas, catálogos y datos semilla 

   ```bash
   sqlcmd -S localhost -U sa -P 'adivinamesipuedes' -i DB.sql