# ?? Sistema de Auditoría - Documentación Completa

## ?? Resumen Ejecutivo

Sistema de auditoría completo que registra **TODA** la actividad de la aplicación automáticamente:
- Usuario (ID, Username, Rol)
- Fecha/hora exacta (UTC)
- Acción realizada
- Entidad afectada
- IP del cliente
- Duración de peticiones
- Errores y excepciones

---

## ??? Arquitectura

```
HTTP Request
    ?
ExceptionHandlingMiddleware (captura errores)
    ?
AuditMiddleware (registra peticiones)
    ?
Controllers ? Services ? Repositories
    ?
Base de Datos (Tabla Audits)
```

---

## ?? Componentes Implementados

### 1. Dominio
- **`Audit.cs`** - Entidad con todas las propiedades de auditoría

### 2. Repositorio
- **`AuditRepository.cs`** - Acceso a datos
- **`IAuditRepository.cs`** - Interfaz

### 3. Servicio
- **`AuditService.cs`** - Lógica de negocio
- **`IAuditService.cs`** - Interfaz

### 4. Middleware
- **`AuditMiddleware.cs`** - Registra TODAS las peticiones HTTP automáticamente
- **`ExceptionHandlingMiddleware.cs`** - Registra excepciones

### 5. Controller
- **`AuditController.cs`** - API REST para consultas (solo Admin, read-only)

---

## ?? Configuración

### Program.cs

```csharp
// Servicios
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IAuditService, AuditService>();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    // Global: 100 requests/minuto
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(/*...*/);
    
    // Auditoría: 10 requests/minuto (política estricta)
    options.AddPolicy("AuditPolicy", /*...*/);
});

// Middlewares (orden importante)
app.UseMiddleware<ExceptionHandlingMiddleware>();  // Primero
app.UseMiddleware<AuditMiddleware>();              // Segundo
app.UseRateLimiter();                             // Tercero
```

### Base de Datos

```sql
CREATE TABLE Audits (
    Id INT PRIMARY KEY IDENTITY,
    Timestamp DATETIME2 NOT NULL,
    UserId INT NULL,
    Username NVARCHAR(200) NOT NULL,
    UserRole NVARCHAR(50) NOT NULL,
    Action NVARCHAR(50) NOT NULL,
    EntityType NVARCHAR(100) NOT NULL,
    EntityId INT NULL,
    Details NVARCHAR(4000) NULL,
    IpAddress NVARCHAR(50) NULL,
    HttpMethod NVARCHAR(10) NULL,
    RequestPath NVARCHAR(500) NULL,
    
    INDEX IX_Audits_UserId (UserId),
    INDEX IX_Audits_Timestamp (Timestamp),
    INDEX IX_Audits_EntityType_EntityId (EntityType, EntityId)
);
```

---

## ?? Uso

### 1. Auditoría Automática (Ya funciona)

```csharp
// No necesitas hacer nada, se registra automáticamente
[HttpGet("{id}")]
public async Task<IActionResult> GetRoom(int id)
{
    return Ok(await _service.GetByIdAsync(id));
}
```

### 2. Auditoría Manual (en servicios)

```csharp
public class ReservationService
{
    private readonly IAuditService _auditService;

    // Crear
    await _auditService.LogCreateAsync("Reservation", reservation.Id, reservation);
    
    // Actualizar
    await _auditService.LogUpdateAsync("Reservation", id, updatedData);
    
    // Eliminar
    await _auditService.LogDeleteAsync("Reservation", id);
    
    // CheckIn
    await _auditService.LogCheckInAsync(reservationId, checkInTime);
    
    // CheckOut
    await _auditService.LogCheckOutAsync(reservationId, checkOutTime);
    
    // Extender
    await _auditService.LogExtendAsync(reservationId, oldEnd, newEnd);
    
    // Cancelar
    await _auditService.LogCancelAsync(reservationId);
    
    // Personalizado
    await _auditService.LogAsync(
        action: "StatusChange",
        entityType: "Room",
        entityId: roomId,
        details: new { OldStatus = "Available", NewStatus = "OutOfService" });
}
```

### 3. Consultar Auditorías

#### API REST (Solo Admin)

```bash
GET /api/audit?page=1&pageSize=50
GET /api/audit/user/{userId}?page=1
GET /api/audit/daterange?from=2024-01-01&to=2024-01-31
GET /api/audit/action/Exception
GET /api/audit/entity/Reservation
```

#### Código C#

```csharp
var audits = await _auditRepository.GetAllAsync(ct);
var userAudits = await _auditRepository.GetByUserIdAsync(userId, ct);
var errors = await _auditRepository.GetByActionAsync("Exception", ct);
```

---

## ?? Seguridad

### Protecciones Implementadas

| Característica | Implementación | Nivel |
|---------------|---------------|-------|
| Autorización Solo Admin | `[Authorize(Roles = "Admin")]` | ?? CRÍTICO |
| Rate Limiting | 10 requests/minuto | ?? ALTO |
| Solo Lectura | Sin POST/PUT/DELETE | ?? CRÍTICO |
| Paginación | Máximo 100 registros | ?? MEDIO |
| Logging de Accesos | Todo acceso se registra | ?? ALTO |
| Auditoría de Auditorías | Trazabilidad completa | ?? ALTO |

### ?? IMPORTANTE - Antes de Producción

**DESCOMENTAR en `AuditController.cs`:**

```csharp
[Authorize(Roles = "Admin")] // ? DESCOMENTAR ESTA LÍNEA
public class AuditController : ControllerBase
```

**Sin esto, cualquier usuario podrá acceder a las auditorías.** ??

---

## ?? Tipos de Acciones

### Automáticas (por Middleware)
- `Read` - GET
- `Create` - POST
- `Update` - PUT
- `PartialUpdate` - PATCH
- `Delete` - DELETE
- `Login` - Peticiones a /login
- `Exception` - Errores
- `{METHOD}_Error` - Errores en cualquier método

### Manuales (por IAuditService)
- `CheckIn` - Check-in de reserva
- `CheckOut` - Check-out de reserva
- `Extend` - Extensión de reserva
- `Cancel` - Cancelación
- `AccessAudit` - Consulta de auditorías
- Cualquier acción personalizada

---

## ?? Queries Útiles

### 1. Top 10 usuarios más activos
```sql
SELECT TOP 10
    Username,
    COUNT(*) as TotalActions,
    MAX(Timestamp) as LastAction
FROM Audits
WHERE Timestamp >= DATEADD(MONTH, -1, GETUTCDATE())
GROUP BY Username
ORDER BY TotalActions DESC
```

### 2. Errores recientes
```sql
SELECT TOP 50
    Timestamp,
    Username,
    EntityType,
    Details
FROM Audits
WHERE Action = 'Exception'
  AND Timestamp >= DATEADD(DAY, -7, GETUTCDATE())
ORDER BY Timestamp DESC
```

### 3. Actividad de un usuario
```sql
SELECT
    Timestamp,
    Action,
    EntityType,
    EntityId,
    HttpMethod,
    RequestPath
FROM Audits
WHERE UserId = @UserId
ORDER BY Timestamp DESC
```

### 4. Accesos sospechosos a auditorías
```sql
SELECT
    Username,
    IpAddress,
    COUNT(*) as AccessCount,
    MIN(Timestamp) as FirstAccess,
    MAX(Timestamp) as LastAccess
FROM Audits
WHERE Action = 'AccessAudit'
  AND Timestamp >= DATEADD(HOUR, -1, GETUTCDATE())
GROUP BY Username, IpAddress
HAVING COUNT(*) > 5
```

### 5. Limpieza de datos antiguos (ejecutar periódicamente)
```sql
-- Eliminar auditorías de más de 1 año
DELETE FROM Audits
WHERE Timestamp < DATEADD(YEAR, -1, GETUTCDATE())
```

---

## ?? Mantenimiento

### Tareas Periódicas

#### Diarias
- [ ] Revisar errores del día
```sql
SELECT COUNT(*) FROM Audits WHERE Action = 'Exception' AND Timestamp >= CAST(GETUTCDATE() AS DATE)
```

#### Semanales
- [ ] Revisar usuarios más activos
- [ ] Verificar accesos anómalos a auditorías
- [ ] Revisar performance (peticiones lentas en Details.DurationMs)

#### Mensuales
- [ ] Limpiar datos antiguos (> 1 año)
- [ ] Analizar patrones de uso
- [ ] Optimizar índices si es necesario

### Monitoreo de Logs

```bash
# Buscar errores
grep "ERROR GLOBAL" logs/audit-*.log

# Buscar accesos a auditorías
grep "ACCESO AUDITORÍA" logs/audit-*.log

# Buscar rate limiting
grep "429" logs/audit-*.log
```

---

## ?? Testing

### Probar Rate Limiting
```bash
# Ejecutar 15 requests (debe bloquear después de 10)
for i in {1..15}; do
  curl -X GET "https://localhost:5001/api/audit?page=1"
  echo "Request $i"
done
```

### Verificar Auditoría Automática
```bash
# Hacer cualquier petición
curl -X GET "https://localhost:5001/api/rooms/1"

# Verificar en BD
SELECT TOP 1 * FROM Audits ORDER BY Timestamp DESC
```

---

## ?? Configuración por Entorno

### appsettings.Development.json
```json
{
  "Logging": {
    "LogLevel": {
      "API_GestionDeSalas_Jaume_Sere.Middleware.AuditMiddleware": "Information"
    }
  }
}
```

### appsettings.Production.json
```json
{
  "Logging": {
    "LogLevel": {
      "API_GestionDeSalas_Jaume_Sere.Middleware.AuditMiddleware": "Warning"
    }
  }
}
```

---

## ?? Troubleshooting

### Problema: No se registran auditorías
- [ ] Verificar que `AuditMiddleware` está registrado en `Program.cs`
- [ ] Verificar que `IAuditService` está registrado en DI
- [ ] Verificar conexión a base de datos
- [ ] Revisar logs de errores

### Problema: Rate limiting no funciona
- [ ] Verificar que `app.UseRateLimiter()` está llamado
- [ ] Verificar que `[EnableRateLimiting("AuditPolicy")]` está en el controller
- [ ] Revisar configuración de políticas

### Problema: Usuarios no-admin pueden acceder
- [ ] **Verificar que `[Authorize(Roles = "Admin")]` está descomentado**
- [ ] Verificar que JWT está configurado
- [ ] Verificar que el claim "role" está en el token

---

## ?? Checklist de Despliegue

### Pre-Producción
- [ ] Descomentar `[Authorize(Roles = "Admin")]` en `AuditController`
- [ ] Ejecutar migración para crear tabla `Audits`
- [ ] Verificar índices en base de datos
- [ ] Configurar HTTPS obligatorio
- [ ] Probar rate limiting
- [ ] Validar que solo admins pueden acceder
- [ ] Configurar job de limpieza de datos antiguos

### Post-Despliegue
- [ ] Verificar logs en tiempo real
- [ ] Monitorear performance
- [ ] Configurar alertas para errores
- [ ] Revisar primeros registros de auditoría
- [ ] Validar que la auditoría funciona correctamente

---

## ?? Soporte y Referencias

### Archivos Clave
- **Dominio:** `LogicaNegocio/Dominio/Audit.cs`
- **Servicio:** `LogicaAplicacion/Services/AuditService.cs`
- **Repositorio:** `Infrastructure/LogicaDatos/EntityFramework/Repositorios/AuditRepository.cs`
- **Middleware:** `API GestionDeSalas-Jaume&Sere/Middleware/AuditMiddleware.cs`
- **Controller:** `API GestionDeSalas-Jaume&Sere/Controllers/AuditController.cs`

### Claims Soportados (JWT)
- `userId` o `sub` o `nameidentifier`
- `username` o `name`
- `role`

### Rate Limits
- **Global:** 100 requests/minuto por IP
- **Auditoría:** 10 requests/minuto por IP
- **Respuesta:** 429 Too Many Requests

---

## ? Resumen Final

**El sistema de auditoría está completamente implementado y listo para usar.**

? Auditoría automática de todas las peticiones  
? Auditoría manual para operaciones específicas  
? API REST para consultas (solo Admin)  
? Rate limiting para protección  
? Logging estructurado completo  
? Paginación obligatoria  
? Solo lectura (read-only)  
? Preparado para producción  

**Acción requerida:** Descomentar `[Authorize(Roles = "Admin")]` antes de producción. ??
