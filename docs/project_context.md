# Project Context: HotelGenericoApi

## 1. Visión General

API REST para sistema de gestión hotelera genérico. Desarrollada con **.NET 10**, **Entity Framework Core**, **SQL Server**. Proporciona endpoints para gestión de habitaciones, clientes, reservas, estancias, ventas, productos, comprobantes electrónicos y reportes. Incluye motor **Lua** para reglas de negocio (cálculo de IGV), **SignalR** para actualizaciones en tiempo real, y **JWT** para autenticación.

Originalmente desarrollada para el hotel "La Rica Noche", refactorizada como **Hotel Genérico** configurable mediante la tabla `configuracion_hotel`.

## 2. Stack Tecnológico

| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| .NET | 10.0 | Framework principal |
| C# | 13 | Lenguaje |
| ASP.NET Core | 10.0 | Framework Web API |
| Entity Framework Core | 10.0.7 | ORM |
| SQL Server Express | - | Base de datos local |
| NLua | 1.7.8 | Scripts Lua para reglas de negocio |
| Riok.Mapperly | 4.3.1 | Mapeo entidad-DTO (source generator) |
| QuestPDF | 2026.2.4 | Generación de PDFs |
| ClosedXML | 0.105.0 | Exportación a Excel |
| BCrypt.Net-Next | 4.1.0 | Hashing de contraseñas |
| Scalar.AspNetCore | 2.14.4 | Documentación OpenAPI |
| JWT Bearer (Microsoft.AspNetCore.Authentication.JwtBearer) | 10.0.7 | Autenticación |
| SignalR | - | WebSockets para tiempo real |
| Microsoft.AspNetCore.OpenApi | 10.0.4 | Soporte OpenAPI |

### Testing

| Herramienta | Versión | Propósito |
|------------|---------|-----------|
| xUnit | 2.9.3 | Framework de testing |
| Moq | 4.20.72 | Mocking de dependencias |
| EF Core InMemory | 10.0.7 | Base de datos en memoria para tests |
| coverlet.collector | 6.0.4 | Cobertura de código |

## 3. Estructura del Proyecto

```
HotelGenericoApi/
├── Controllers/           # Controladores REST (21)
│   ├── CatAfectacionIgvController.cs
│   ├── CatEstadoHabitacionController.cs
│   ├── CatEstadoSunatController.cs
│   ├── CatMetodoPagoController.cs
│   ├── CatRolUsuarioController.cs
│   ├── CatTipoComprobanteController.cs
│   ├── CatTipoDocumentoController.cs
│   ├── ClienteController.cs
│   ├── ComprobanteController.cs
│   ├── ConfiguracionHotelController.cs
│   ├── EstanciaController.cs
│   ├── HabitacionController.cs
│   ├── LuaTestController.cs
│   ├── PdfController.cs
│   ├── ProductoController.cs
│   ├── ReporteController.cs
│   ├── SetupController.cs
│   ├── TiposHabitacionController.cs
│   ├── UsuarioController.cs
│   └── VentaController.cs
├── Data/
│   └── HotelDbContext.cs   # DbContext de EF Core
├── DTOs/
│   ├── Request/            # DTOs de entrada (28)
│   └── Response/           # DTOs de salida (22)
├── Extensions/
│   └── IQueryableExtensions.cs
├── Hubs/
│   └── HabitacionHub.cs    # SignalR Hub
├── Mappings/               # Mapperly (12)
├── Models/                 # Entidades EF (22)
├── Scripts/                # Scripts Lua
│   ├── hotel_tax_rules.lua
│   └── validar_cliente.lua
├── Services/
│   ├── Interfaces/         # 21 interfaces
│   └── Implementations/    # 23 servicios
├── Tests/                  # Proyecto xUnit
├── DB.sql                  # Script de base de datos
├── Program.cs              # Punto de entrada
├── HotelGenericoApi.csproj # Archivo de proyecto
└── appsettings.json        # Configuración
```

## 4. Arquitectura

```
┌─────────────┐     ┌──────────────────┐     ┌──────────────┐
│  Cliente     │────▶│  ASP.NET Core    │────▶│  SQL Server  │
│  (React)     │◀────│  Web API         │◀────│  Express     │
└─────────────┘     └──────────────────┘     └──────────────┘
                           │
                    ┌──────┴──────┐
                    │   SignalR   │
                    │   Hub       │
                    └─────────────┘
```

- **Autenticación**: JWT Bearer con tokens de 8 horas
- **CORS**: Permitido desde `http://localhost:5173` con AllowCredentials
- **SeñalR**: Hub en `/hub/habitaciones` con reconexión automática
- **OpenAPI**: Documentación en `/scalar/v1` via Scalar (solo development)
- **ORM**: Entity Framework Core con SQL Server
- **Transacciones**: `IDbTransactionManager` para manejo transaccional explícito
- **Lua**: Motor NLua singleton para scripts de reglas de negocio

## 5. Base de Datos

### Tablas principales

| Tabla | Descripción |
|-------|-------------|
| configuracion_hotel | Datos del hotel (única fila, id=1) |
| cat_tipo_documento | Catálogo SUNAT: DNI(1), RUC(6), Pasaporte(7), Otros(0) |
| cat_metodo_pago | Catálogo SUNAT: Efectivo(005), Tarjeta(006), Yape/Plin(008) |
| cat_tipo_comprobante | Boleta(03), Factura(01) |
| cat_afectacion_igv | Gravado(10), Exonerado(20), Inafecto(30), Exportación(40) |
| cat_categoria_producto | Categorías de productos |
| cat_estado_habitacion | Estados con comportamiento (checkin/checkout/final) |
| cat_estado_sunat | Estados de comprobante: Pendiente(1), Enviado(2), Aceptado(3), Rechazado(4) |
| cat_rol_usuario | Administrador, Recepcionista, Limpieza |
| cat_transicion_estado | Máquina de estados de habitación |
| usuarios | Usuarios del sistema (bcrypt) |
| clientes | Clientes (incluye anónimo con documento 00000000) |
| tipos_habitacion | Tipos de habitación |
| tarifas | Precios por tipo + temporada |
| temporadas | Temporadas con multiplicador |
| habitaciones | Habitaciones del hotel |
| historial_estado_habitacion | Historial de cambios de estado |
| reservas | Reservas con fechas previstas |
| estancias | Check-in real |
| huespedes | Huéspedes adicionales en la habitación |
| productos | Productos con categoría y afectación IGV |
| items_estancia | Consumos durante la estancia |
| ventas | Ventas independientes |
| items_venta | Detalle de ventas |
| comprobantes | Comprobantes electrónicos SUNAT |
| cierre_caja_envios | Envíos de cierre de caja |

### Vistas

| Vista | Descripción |
|-------|-------------|
| v_cierre_caja_diario | Ingresos agrupados por fecha y método de pago |
| v_estado_habitaciones | Estado actual de cada habitación |

### Estados de habitación

| ID | Estado | CheckIn | CheckOut | Color UI |
|----|--------|---------|----------|----------|
| 1 | Disponible | ✓ | ✗ | success |
| 2 | Ocupada | ✗ | ✓ | warning |
| 3 | Limpieza | ✗ | ✗ | info |
| 4 | Mantenimiento | ✗ | ✗ | error |

### Transiciones válidas

| Estado Actual → Siguiente |
|--------------------------|
| Disponible (1) → Ocupada (2) |
| Disponible (1) → Mantenimiento (4) |
| Ocupada (2) → Limpieza (3) |
| Limpieza (3) → Disponible (1) |
| Mantenimiento (4) → Disponible (1) |

### Datos semilla

- Cliente anónimo: documento 00000000, tipo Otros
- Usuario admin inicial se crea via endpoint `/api/Setup` (si no existe ninguno)
- Temporadas: Alta (Jun-Ago, 1.20x), Baja (Sep-Nov, 0.85x)
- Tarifas: Una base de S/ 50 para tipo Matrimonial

## 6. Controladores y Endpoints

### Autenticación y Setup
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/Usuario/login` | Login, retorna JWT + datos usuario |
| GET | `/api/Setup` | Verifica si existe admin |
| POST | `/api/Setup` | Crea el primer administrador |

### Configuración del Hotel
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/ConfiguracionHotel` | Obtener configuración |
| PUT | `/api/ConfiguracionHotel` | Actualizar configuración |

### Habitaciones
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/Habitacion` | Listar todas |
| GET | `/api/Habitacion/{id}` | Obtener por ID |
| POST | `/api/Habitacion` | Crear |
| PUT | `/api/Habitacion/{id}` | Actualizar completa |
| PATCH | `/api/Habitacion/{id}` | Actualización parcial (ej. cambio estado) |
| DELETE | `/api/Habitacion/{id}` | Eliminar |
| GET | `/api/Habitacion/estado-actual` | Estado actual con acciones disponibles |

### Estancias
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/Estancia` | Listar todas |
| GET | `/api/Estancia/{id}` | Obtener por ID |
| POST | `/api/Estancia/checkin` | Realizar check-in |
| POST | `/api/Estancia/{id}/checkout` | Realizar check-out |
| POST | `/api/Estancia/{id}/consumo` | Registrar consumo |
| POST | `/api/Estancia/reserva` | Crear reserva |
| GET | `/api/Estancia/reserva/{id}` | Obtener reserva |
| GET | `/api/Estancia/reservas/{idHabitacion}` | Reservas de una habitación |
| GET | `/api/Estancia/{id}/consumos` | Consumos de una estancia |
| PUT | `/api/Estancia/{idEstancia}/consumo/{idItem}` | Actualizar consumo |
| DELETE | `/api/Estancia/{idEstancia}/consumo/{idItem}` | Eliminar consumo |

### Clientes
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/Cliente` | Listar (con búsqueda por documento) |
| GET | `/api/Cliente/{id}` | Obtener por ID |
| POST | `/api/Cliente` | Crear |
| PUT | `/api/Cliente/{id}` | Actualizar |
| DELETE | `/api/Cliente/{id}` | Eliminar |

### Productos
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/Producto` | Listar |
| GET | `/api/Producto/{id}` | Obtener |
| POST | `/api/Producto` | Crear |
| PUT | `/api/Producto/{id}` | Actualizar |
| DELETE | `/api/Producto/{id}` | Eliminar |

### Ventas
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/Venta` | Listar |
| GET | `/api/Venta/{id}` | Obtener |
| POST | `/api/Venta` | Crear (carrito) |

### Comprobantes
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/Comprobante` | Listar |
| GET | `/api/Comprobante/{id}` | Obtener detalle |
| GET | `/api/Comprobante/{id}/pdf` | Obtener PDF |
| POST | `/api/Comprobante/{id}/enviar-sunat` | Simular envío SUNAT |

### Reportes
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/Reporte/estado-habitaciones` | Estado de habitaciones |
| GET | `/api/Reporte/cierre-caja` | Cierre de caja (filtro por fecha) |
| GET | `/api/Reporte/cierre-caja/excel` | Exportar Excel |
| GET | `/api/Reporte/top-productos` | Productos más consumidos (filtro por días) |

### PDF
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/Pdf/CierreCaja` | PDF de cierre de caja |

### Catálogos
- CRUD completo para: `CatAfectacionIgv`, `CatEstadoHabitacion`, `CatEstadoSunat`, `CatMetodoPago`, `CatRolUsuario`, `CatTipoComprobante`, `CatTipoDocumento`, `TiposHabitacion`

### LuaTest
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/LuaTest/test-igv` | Prueba del motor Lua |

## 7. DTOs

### Request (28)
`LoginDto`, `CheckInDto`, `CheckOutDto`, `ReservaCreateDto`, `ConsumoEstanciaCreateDto`, `ActualizarConsumoDto`, `VentaCreateDto`, `HabitacionCreateDto`, `HabitacionUpdateDto`, `ClienteCreateDto`, `ClienteUpdateDto`, `ProductoCreateDto`, `ProductoUpdateDto`, `UsuarioCreateDto`, `UsuarioUpdateDto`, y CRUD DTOs para cada catálogo (Create/Update).

### Response (22)
`LoginResponseDto`, `HabitacionResponseDto`, `HabitacionEstadoActualDto`, `EstanciaResponseDto`, `ReservaResponseDto`, `ItemConsumoResponseDto`, `VentaResponseDto`, `ClienteResponseDto`, `ProductoResponseDto`, `ComprobanteResponseDto`, `CierreCajaResponseDto`, `CierreCajaEnvioDto`, `TopProductoDto`, `ConfiguracionHotelResponseDto`, `UsuarioResponseDto`, `PagedResult<T>`, y response DTOs para cada catálogo.

## 8. Servicios

### Interfaces (21)
`ICatAfectacionIgvService`, `ICatEstadoHabitacionService`, `ICatEstadoSunatService`, `ICatMetodoPagoService`, `ICatRolUsuarioService`, `ICatTipoComprobanteService`, `ICatTipoDocumentoService`, `ICierreCajaEnvioService`, `IClienteService`, `IComprobanteService`, `IConfiguracionHotelService`, `IDbTransactionManager`, `IEstanciaService`, `IHabitacionService`, `ILuaService`, `IPdfService`, `IProductoService`, `IReporteService`, `ITiposHabitacionService`, `IUsuarioService`, `IValidadorEstadoService`, `IVentaService`

### Implementaciones (23)
`LuaService` (singleton), `SqlServerTransactionManager`, `ValidadorEstadoService`, `SetupService`, y servicios CRUD/scoped para cada interfaz.

## 9. Mappings (Mapperly)

12 mappers source-generated que transforman entidades ↔ DTOs:
- Catálogos: CatAfectacionIgv, CatEstadoHabitacion, CatEstadoSunat, CatMetodoPago, CatRolUsuario, CatTipoComprobante, CatTipoDocumento
- Entidades: Cliente, Habitacion, Producto, TiposHabitacion, Usuario

## 10. Scripts Lua

### `hotel_tax_rules.lua`
Función `Calculate_igv_hotel(afectacion_codigo, base_imponible, tipo_comprobante)`:
- Si es boleta (03) + gravado (10): tasa = **10.5%** (hospedaje)
- Si es gravado normal: tasa = **18%**
- Exonerado/Inafecto/Exportación: tasa = **0%**
- Retorna `{ tasa, monto }`

### `validar_cliente.lua`
Función `Validar(documento, tipo)`:
- DNI: 8 dígitos
- RUC: 11 dígitos

## 11. Models/Entities (22)

`CatAfectacionIgv`, `CatCategoriaProducto`, `CatEstadoHabitacion`, `CatEstadoSunat`, `CatMetodoPago`, `CatRolUsuario`, `CatTipoComprobante`, `CatTipoDocumento`, `CatTransicionEstado`, `CierreCajaEnvio`, `Cliente`, `Comprobante`, `ConfiguracionHotel`, `Estancia`, `Habitacione`, `HistorialEstadoHabitacion`, `Huespede`, `ItemsEstancium`, `ItemsVentum`, `Producto`, `Reserva`, `Tarifa`, `Temporada`, `TiposHabitacion`, `Usuario`, `VCierreCajaDiario`, `VEstadoHabitacione`, `Venta`

## 12. SignalR

- **Hub**: `HabitacionHub` (vacío, hereda de Hub)
- **Endpoint**: `/hub/habitaciones`
- **Evento enviado**: `EstadoHabitacionCambiado` (enviado desde servicios cuando cambia estado de habitación)
- **Middleware**: CORS con AllowCredentials habilitado

## 13. Seguridad

- **Autenticación**: JWT Bearer Token
- **Claims**: NameIdentifier (id usuario), Role (nombre del rol)
- **CORS**: Solo `http://localhost:5173` con AllowCredentials
- **Contraseñas**: BCrypt.Net-Next
- **Endpoints**: Todos protegidos con `[Authorize]` excepto login y setup
- **Setup**: Endpoint `/api/Setup` verifica si hay admin, si no, permite crear primero

## 14. Configuración

### `appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HotelDB;User Id=sa;Password=***;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "elPapuSefu3decompras-esperayaregresa-tomaAsientoYesperemoslopapu-tardara3minutos",
    "Issuer": "HotelGenericoApi",
    "Audience": "HotelGenericoApiClient"
  }
}
```

## 15. Testing (xUnit)

- **Total tests**: 9
- **Framework**: xUnit 2.9.3 + Moq 4.20.72 + EF Core InMemory 10.0.7
- **Ubicación**: `/Tests/HotelGenericoApi.Tests.csproj`
- **Helpers auxiliares**:
  - `NoOpTransactionManager`: Implementación dummy de `IDbTransactionManager`
  - `TestDbContextFactory`: Crea `HotelDbContext` con InMemory database
  - `TestTransactionFactory`: Fábrica dummy de transacciones

### Tests unitarios incluidos
- `CheckIn_DeberiaCrearEstanciaYCambiarEstado`
- `CheckOut_DeberiaCambiarEstadoALimpieza`
- `CrearReserva_DeberiaCrearReserva`
- `CrearReserva_ConFechasSolapadas_DeberiaLanzarExcepcion`
- `RegistrarConsumo_DeberiaAgregarItem`
- `CambioEstado_TransicionValida_DeberiaPermitir`
- `CambioEstado_TransicionInvalida_DeberiaRechazar`
- (2 tests adicionales de validación de transiciones)

## 16. Decisiones Técnicas

| Decisión | Razón |
|----------|-------|
| NLua para IGV | Modificar tasas sin recompilar |
| Mapperly (source generator) | Rendimiento, cero reflection |
| ClosedXML | Alternativa gratuita vs EPPlus (licencia comercial) |
| QuestPDF | Generación de PDF 100% offline |
| Scalar | UI moderna para OpenAPI (vs Swagger UI) |
| BCrypt.Net-Next | Hashing seguro de contraseñas |
| InMemory DB en tests | Tests rápidos sin depender de SQL Server |
| Máquina de estados en BD | Configurable sin recompilar |

## 17. Flujo de Operaciones

### Check-In
1. Validar que habitación esté en estado Disponible (id=1)
2. Validar transición 1→2
3. Si hay reserva, marcarla como completada
4. Crear estancia con fechaCheckin=now
5. Calcular IGV via Lua
6. Generar comprobante automático
7. Cambiar habitación a Ocupada (id=2)
8. Notificar via SignalR

### Check-Out
1. Validar que habitación esté en estado Ocupada (id=2)
2. Validar transición 2→3
3. Liquidar consumos pendientes
4. Actualizar monto total de estancia
5. Cambiar habitación a Limpieza (id=3)
6. Notificar via SignalR

### Reserva
1. Validar que no haya solapamiento de fechas
2. Crear reserva con estado "Pendiente"
3. No cambia estado de habitación

## 18. Puertos

- **API**: `http://localhost:5054` (por defecto)
- **Entorno**: Development habilita OpenAPI/Scalar
