# La Rica Noche API

API REST del sistema **La Rica Noche**, diseñada para administrar la lógica principal del alojamiento y servir de soporte al frontend web.

Repositorio del frontend: https://github.com/MarcosZumaran/LaRicaNoche.Web

---

## Descripción

Este backend está desarrollado con **ASP.NET Core Web API** y está orientado a centralizar la lógica de negocio del sistema.

Se encarga de:

- exponer endpoints para el frontend
- manejar el acceso a datos
- controlar la lógica del negocio
- gestionar autenticación
- soportar operaciones relacionadas con comprobantes, ventas y reportes

---

## Tecnologías utilizadas

| Tecnología | Propósito |
|---|---|
| ASP.NET Core Web API | API principal |
| Entity Framework Core | Acceso y persistencia de datos |
| SQL Server | Motor de base de datos |
| Mapster | Mapeo de objetos |
| NLua | Ejecución de scripts dinámicos |

---

## Características

- Arquitectura orientada a servicios
- Separación entre controladores, lógica y acceso a datos
- Endpoints para los módulos principales del sistema
- Integración con base de datos SQL Server
- Base preparada para el consumo desde el frontend

---

## Requisitos previos

- .NET SDK compatible con el proyecto
- SQL Server
- Visual Studio, Visual Studio Code o un editor compatible
- Base de datos configurada correctamente

---

## Instalación

```bash
git clone https://github.com/MarcosZumaran/LaRicaNoche.Api.git
cd LaRicaNoche.Api
dotnet restore
```

---

## Configuración

Revisa el archivo `appsettings.json` y configura la cadena de conexión.

Ejemplo:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LaRicaNoche;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Si tu entorno usa usuario y contraseña, ajusta la cadena de conexión según corresponda.

---

## Ejecución

```bash
dotnet run
```

---

## Endpoints principales

Los módulos principales del sistema incluyen rutas orientadas a:

- autenticación
- habitaciones
- clientes
- estancias
- productos
- comprobantes
- reportes
- ventas

---

## Estructura general

```text
LaRicaNoche.Api/
├── Controllers/
├── Services/
├── Repositories/
├── Models/
├── DTOs/
├── Mappers/
├── Config/
└── Program.cs
```

---

## Relación con el frontend

Esta API fue creada para trabajar junto con la interfaz web del sistema.

Frontend del proyecto:  
https://github.com/MarcosZumaran/LaRicaNoche.Web

Flujo general:

```text
Frontend -> API -> Base de datos
```

---

## Nota importante

> Este proyecto fue creado con fines de práctica institucional y no tiene fines comerciales.

> La implementación puede seguir recibiendo mejoras en seguridad, validaciones y arquitectura.

---

## Autor

MarcosZumaran

Repositorio:  
https://github.com/MarcosZumaran/LaRicaNoche.Api
