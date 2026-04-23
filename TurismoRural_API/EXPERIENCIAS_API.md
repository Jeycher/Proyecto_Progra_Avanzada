# Documentación de Endpoints: Experiencias y Experiencias Concurrencia

Este documento describe los endpoints de **Experiencias** y **ExperienciasConcurrencia** en la API de Turismo Rural. Está orientado a agentes de IA o desarrolladores frontend.

---

# PARTE 1: Experiencias

Una **Experiencia** es la actividad o producto turístico general (ej: "Tour en canopy", "Clase de surf"). Es la entidad padre de las concurrencias.

**Ruta base:** `/api/experiencias`

---

## 1. Obtener todas las experiencias

- **Método:** GET
- **Ruta:** `/api/experiencias`
- **Autenticación:** No especificada
- **Body:** No requiere
- **Respuesta exitosa (200 OK):**
```json
[
  {
    "iD_Experiencia": 1,
    "titulo": "Tour en canopy",
    "descripcion": "Tirolesa en bosque",
    "categoria": "Aventura",
    "usuarioIdRegistrador": 1,
    "iD_Comunidad": 1
  },
  {
    "iD_Experiencia": 2,
    "titulo": "Caminata volcán Arenal",
    "descripcion": "Tour guiado",
    "categoria": "Naturaleza",
    "usuarioIdRegistrador": 1,
    "iD_Comunidad": 4
  }
]
```

---

## 2. Obtener una experiencia por ID

- **Método:** GET
- **Ruta:** `/api/experiencias/{id}`
- **Parámetros de ruta:**
  - `id` (int): Identificador de la experiencia.
- **Body:** No requiere
- **Respuesta exitosa (200 OK):**
```json
{
  "iD_Experiencia": 1,
  "titulo": "Tour en canopy",
  "descripcion": "Tirolesa en bosque",
  "categoria": "Aventura",
  "usuarioIdRegistrador": 1,
  "iD_Comunidad": 1
}
```
- **Respuesta si no existe (404 Not Found)**

---

## 3. Crear una experiencia

- **Método:** POST
- **Ruta:** `/api/experiencias`
- **Body (JSON):**
```json
{
  "titulo": "Kayak en el río",
  "descripcion": "Recorrido en kayak por el río Sarapiquí",
  "categoria": "Aventura",
  "usuarioIdRegistrador": 1,
  "iD_Comunidad": 3
}
```
- **Campos requeridos:**
  - `titulo` (string, máx 50 chars): Nombre de la experiencia.
  - `usuarioIdRegistrador` (int): ID del usuario que registra la experiencia. Debe existir en la tabla `Usuario`.
  - `iD_Comunidad` (int): ID de la comunidad a la que pertenece. Debe existir en la tabla `Comunidad`.
- **Campos opcionales:**
  - `descripcion` (string, máx 150 chars): Descripción de la experiencia.
  - `categoria` (string, máx 150 chars): Categoría (ej: "Aventura", "Naturaleza", "Deportes").
- **Respuesta exitosa (200 OK):** Devuelve el objeto completo de la experiencia creada, incluyendo el `iD_Experiencia` asignado.
```json
{
  "iD_Experiencia": 4,
  "titulo": "Kayak en el río",
  "descripcion": "Recorrido en kayak por el río Sarapiquí",
  "categoria": "Aventura",
  "usuarioIdRegistrador": 1,
  "iD_Comunidad": 3
}
```

---

### 🔍 ¿Cómo funciona internamente el CREATE de una experiencia?

```
POST /api/experiencias
        │
        ▼
ExperienciasController.Create(ExperienciaCreateRequest model)
        │
        ├─ Valida que el body no sea nulo (400 Bad Request si es nulo)
        │
        ▼
ExperienciaRepository.CreateAsync(model)
        │
        ├─ PASO 1: Llama al SP SP_InsertarExperiencia
        │          Parámetros: @Titulo, @Descripcion, @Categoria,
        │                      @UsuarioIdRegistrador, @ID_Comunidad
        │          El SP hace INSERT INTO Experiencia (...) VALUES (...)
        │
        ├─ PASO 2: Recupera el ID generado con una consulta directa:
        │          SELECT TOP 1 ID_Experiencia FROM Experiencia
        │          WHERE Titulo = @Titulo AND UsuarioIdRegistrador = @UsuarioIdRegistrador
        │            AND ID_Comunidad = @ID_Comunidad
        │          ORDER BY ID_Experiencia DESC
        │
        ▼
Controller recibe el ID → llama a GetByIdAsync(id) → retorna el objeto creado (200 OK)
```

---

## 4. Actualizar una experiencia

- **Método:** PUT
- **Ruta:** `/api/experiencias/{id}`
- **Parámetros de ruta:**
  - `id` (int): Identificador de la experiencia a actualizar.
- **Body (JSON):** Enviar solo los campos que cambian. `titulo` es obligatorio si se envía (columna NOT NULL en BD).
```json
{
  "titulo": "Kayak avanzado en el río",
  "descripcion": "Nueva descripción actualizada",
  "categoria": "Deportes",
  "usuarioIdRegistrador": 1,
  "iD_Comunidad": 3
}
```
- **Descripción:** **No incluir el `id` en el body, solo en la URL.**
- **Respuesta exitosa (200 OK):** Devuelve el objeto completo actualizado.
```json
{
  "iD_Experiencia": 4,
  "titulo": "Kayak avanzado en el río",
  "descripcion": "Nueva descripción actualizada",
  "categoria": "Deportes",
  "usuarioIdRegistrador": 1,
  "iD_Comunidad": 3
}
```
- **Respuesta si no existe (404 Not Found)**

---

### 🔍 ¿Cómo funciona internamente el UPDATE de una experiencia?

```
PUT /api/experiencias/{id}
        │
        ▼
ExperienciasController.Update(int id, ExperienciaUpdateRequest model)
        │
        ├─ Valida que el body no sea nulo (400 Bad Request si es nulo)
        │
        ▼
ExperienciaRepository.UpdateAsync(int id, model)
        │
        ├─ Llama al SP SP_ActualizarExperiencia (SET NOCOUNT OFF)
        │  Parámetros: @ID_Experiencia, @Titulo, @Descripcion,
        │              @Categoria, @UsuarioIdRegistrador, @ID_Comunidad
        │
        │  El SP ejecuta:
        │    UPDATE Experiencia
        │    SET Titulo = @Titulo, Descripcion = @Descripcion,
        │        Categoria = @Categoria, UsuarioIdRegistrador = @UsuarioIdRegistrador,
        │        ID_Comunidad = @ID_Comunidad
        │    WHERE ID_Experiencia = @ID_Experiencia
        │
        ├─ Retorna affected > 0
        │    → false si no existe (0 filas afectadas) → 404 Not Found
        │    → true si se actualizó correctamente
        │
        ▼
Controller recibe true → llama a GetByIdAsync(id) → retorna el objeto actualizado (200 OK)
```

> ⚠️ **Nota sobre `Titulo`:** Aunque el DTO lo declara como `string?`, la columna `Titulo` en la BD es `NOT NULL`. Enviar `null` causaría un error 500. Siempre incluir `titulo` en el body del PUT.

---

## 5. Eliminar una experiencia

- **Método:** DELETE
- **Ruta:** `/api/experiencias/{id}`
- **Parámetros de ruta:**
  - `id` (int): Identificador de la experiencia a eliminar.
- **Body:** No requiere
- **Respuesta exitosa (204 No Content)**
- **Respuesta si no existe (404 Not Found)**

---

### 🔍 ¿Cómo funciona internamente el DELETE de una experiencia?

```
DELETE /api/experiencias/{id}
        │
        ▼
ExperienciasController.Delete(int id)
        │
        ▼
ExperienciaRepository.DeleteAsync(int id)
        │
        ├─ Llama al SP SP_EliminarExperiencia (SET NOCOUNT ON)
        │  Parámetro: @ID_Experiencia
        │
        │  El SP verifica existencia:
        │    IF NOT EXISTS → SELECT 0 AS Resultado; RETURN
        │    IF EXISTS     → DELETE ... ; SELECT 1 AS Resultado
        │
        ├─ Lee el resultado con QuerySingleAsync<int>
        │    → 0: no existía → retorna false → 404 Not Found
        │    → 1: eliminado  → retorna true  → 204 No Content
        │
```

> ⚠️ **Nota técnica:** El SP usa `SET NOCOUNT ON`, por lo que el repositorio usa `QuerySingleAsync<int>` en lugar de `ExecuteAsync` para leer el resultado `0`/`1` devuelto por el SP. Usar `ExecuteAsync` con `NOCOUNT ON` siempre retornaría `0` independientemente del resultado real.

---

# PARTE 2: Experiencias Concurrencia

Una **ExperienciaConcurrencia** es una sesión específica de una experiencia con fecha, precio y cupos definidos (ej: "Tour en canopy - 10 Abril 2026 a las 8am"). Es la entidad hija de Experiencia.

**Ruta base:** `/api/experienciasconcurrencia`

---

## 6. Obtener todas las concurrencias

- **Método:** GET
- **Ruta:** `/api/experienciasconcurrencia`
- **Body:** No requiere
- **Respuesta exitosa (200 OK):**
```json
[
  {
    "iD_Concurrencia": 1,
    "fecha": "2026-04-10T00:00:00",
    "detalle": "Canopy mañana",
    "precio": 50.00,
    "cupos_Disponibles": 10,
    "iD_Experiencia": 1
  },
  {
    "iD_Concurrencia": 2,
    "fecha": "2026-04-11T00:00:00",
    "detalle": "Canopy tarde",
    "precio": 55.00,
    "cupos_Disponibles": 8,
    "iD_Experiencia": 1
  }
]
```

---

## 7. Obtener una concurrencia por ID

- **Método:** GET
- **Ruta:** `/api/experienciasconcurrencia/{id}`
- **Parámetros de ruta:**
  - `id` (int): Identificador de la concurrencia.
- **Body:** No requiere
- **Respuesta exitosa (200 OK):**
```json
{
  "iD_Concurrencia": 1,
  "fecha": "2026-04-10T00:00:00",
  "detalle": "Canopy mañana",
  "precio": 50.00,
  "cupos_Disponibles": 10,
  "iD_Experiencia": 1
}
```
- **Respuesta si no existe (404 Not Found)**

---

## 8. Crear una concurrencia

- **Método:** POST
- **Ruta:** `/api/experienciasconcurrencia`
- **Body (JSON):**
```json
{
  "fecha": "2026-05-15T08:00:00",
  "detalle": "Sesión especial con guía experto",
  "precio": 75.00,
  "cupos_Disponibles": 12,
  "iD_Experiencia": 1
}
```
- **Campos requeridos:**
  - `fecha` (DateTime): Fecha de la sesión. Solo se guarda la parte de fecha (`DATE` en BD), la hora se descarta.
  - `precio` (decimal): Precio por persona. Se almacena como `DECIMAL(18,0)` (sin centavos).
  - `cupos_Disponibles` (int): Número de cupos disponibles para esta sesión.
  - `iD_Experiencia` (int): ID de la experiencia a la que pertenece. Debe existir en la tabla `Experiencia`.
- **Campos opcionales:**
  - `detalle` (string, máx 255 chars): Descripción adicional de la sesión.
- **Respuesta exitosa (200 OK):** Devuelve el objeto completo de la concurrencia creada.
```json
{
  "iD_Concurrencia": 6,
  "fecha": "2026-05-15T08:00:00",
  "detalle": "Sesión especial con guía experto",
  "precio": 75.00,
  "cupos_Disponibles": 12,
  "iD_Experiencia": 1
}
```

---

### 🔍 ¿Cómo funciona internamente el CREATE de una concurrencia?

```
POST /api/experienciasconcurrencia
        │
        ▼
ExperienciasConcurrenciaController.Create(ExperienciaConcurrenciaCreateRequest model)
        │
        ├─ Valida que el body no sea nulo (400 Bad Request si es nulo)
        │
        ▼
ExperienciaConcurrenciaRepository.CreateAsync(model)
        │
        ├─ PASO 1: Llama al SP SP_InsertarExperienciaConcurrencia
        │          Parámetros: @Fecha (solo DATE), @Detalle, @Precio,
        │                      @Cupos_Disponibles, @ID_Experiencia
        │          El SP hace INSERT INTO ExperienciaConcurrencia (...)
        │
        ├─ PASO 2: Recupera el ID con una consulta directa:
        │          SELECT TOP 1 ID_Concurrencia FROM ExperienciaConcurrencia
        │          WHERE Fecha = @Fecha AND Cupos_Disponibles = @Cupos_Disponibles
        │            AND ID_Experiencia = @ID_Experiencia
        │          ORDER BY ID_Concurrencia DESC
        │
        ├─ PASO 3 (corrección de precio):
        │          UPDATE ExperienciaConcurrencia SET Precio = @Precio
        │          WHERE ID_Concurrencia = @ID_Concurrencia
        │          ⚠️ Necesario porque el SP tiene limitación con DECIMAL(18,0)
        │             y a veces no persiste el precio correctamente.
        │
        ▼
Controller recibe el ID → llama a GetByIdAsync(id) → retorna el objeto creado (200 OK)
```

---

## 9. Actualizar una concurrencia

- **Método:** PUT
- **Ruta:** `/api/experienciasconcurrencia/{id}`
- **Parámetros de ruta:**
  - `id` (int): Identificador de la concurrencia a actualizar.
- **Body (JSON):** Todos los campos son opcionales, enviar solo los que cambian.
```json
{
  "fecha": "2026-05-15T10:00:00",
  "detalle": "Sesión modificada - nueva hora",
  "precio": 80.00,
  "cupos_Disponibles": 15,
  "iD_Experiencia": 1
}
```
- **Descripción:** **No incluir el `id` en el body, solo en la URL.**
- **Respuesta exitosa (200 OK):** Devuelve el objeto completo actualizado.
```json
{
  "iD_Concurrencia": 6,
  "fecha": "2026-05-15T10:00:00",
  "detalle": "Sesión modificada - nueva hora",
  "precio": 80.00,
  "cupos_Disponibles": 15,
  "iD_Experiencia": 1
}
```
- **Respuesta si no existe (404 Not Found)**

---

### 🔍 ¿Cómo funciona internamente el UPDATE de una concurrencia?

El proceso de actualización sigue este flujo:

```
PUT /api/experienciasconcurrencia/{id}
        │
        ▼
ExperienciasConcurrenciaController.Update(int id, ExperienciaConcurrenciaUpdateRequest model)
        │
        ├─ Valida que el body no sea nulo (400 Bad Request si es nulo)
        │
        ▼
ExperienciaConcurrenciaRepository.UpdateAsync(int id, model)
        │
        ├─ PASO 1: Llama al stored procedure SP_ActualizarExperienciaConcurrencia
        │          Parámetros enviados: @ID_Concurrencia, @Fecha, @Detalle,
        │                               @Precio, @Cupos_Disponibles, @ID_Experiencia
        │          El SP actualiza todos los campos enviados en la tabla ExperienciaConcurrencia.
        │
        ├─ PASO 2 (solo si model.Precio tiene valor):
        │          Ejecuta un UPDATE directo:
        │          UPDATE ExperienciaConcurrencia SET Precio = @Precio WHERE ID_Concurrencia = @id
        │          ⚠️ Esto es una corrección forzada del precio, ya que el SP de SQL Server
        │          tiene una limitación con el tipo DECIMAL que a veces no lo persiste correctamente.
        │
        ├─ PASO 3: Verifica existencia del registro:
        │          SELECT COUNT(1) FROM ExperienciaConcurrencia WHERE ID_Concurrencia = @id
        │          Retorna true si el SP afectó filas O si el registro existe.
        │
        ▼
Controller recibe true/false
        ├─ false → 404 Not Found
        └─ true  → Llama a GetByIdAsync(id) y retorna el objeto actualizado (200 OK)
```

#### Modelo de entrada (`ExperienciaConcurrenciaUpdateRequest`)

Todos los campos son **opcionales** (`nullable`). Solo se actualizarán los valores que se envíen:

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `fecha` | `DateTime?` | Nueva fecha y hora de la sesión |
| `detalle` | `string?` | Nueva descripción de la sesión |
| `precio` | `decimal?` | Nuevo precio por persona |
| `cupos_Disponibles` | `int?` | Nuevo número de cupos disponibles |
| `iD_Experiencia` | `int?` | ID de la experiencia padre (reasignación) |

> ⚠️ **Nota sobre el precio:** El sistema aplica el precio en **dos pasos**: primero a través del stored procedure y luego con un `UPDATE` directo. Esto garantiza que el precio quede correctamente guardado aunque el SP tenga limitaciones con el tipo `DECIMAL`.

> ⚠️ **Nota sobre `cupos_Disponibles`:** Aunque se puede modificar manualmente mediante este endpoint, los cupos también se actualizan automáticamente cuando se crean, modifican o eliminan reservas asociadas a esa concurrencia.

---

## 10. Eliminar una concurrencia

- **Método:** DELETE
- **Ruta:** `/api/experienciasconcurrencia/{id}`
- **Parámetros de ruta:**
  - `id` (int): Identificador de la concurrencia a eliminar.
- **Body:** No requiere
- **Respuesta exitosa (204 No Content)**
- **Respuesta si no existe (404 Not Found)**

---

### 🔍 ¿Cómo funciona internamente el DELETE de una concurrencia?

```
DELETE /api/experienciasconcurrencia/{id}
        │
        ▼
ExperienciasConcurrenciaController.Delete(int id)
        │
        ▼
ExperienciaConcurrenciaRepository.DeleteAsync(int id)
        │
        ├─ Llama al SP SP_EliminarExperienciaConcurrencia (SET NOCOUNT ON)
        │  Parámetro: @ID_Concurrencia
        │
        │  El SP verifica existencia:
        │    IF NOT EXISTS → SELECT 0 AS Resultado; RETURN
        │    IF EXISTS     → DELETE ... ; SELECT 1 AS Resultado
        │
        ├─ Lee el resultado con QuerySingleAsync<int>
        │    → 0: no existía → retorna false → 404 Not Found
        │    → 1: eliminado  → retorna true  → 204 No Content
```

> ⚠️ **Nota técnica:** Igual que en Experiencia, el SP usa `SET NOCOUNT ON`. El repositorio usa `QuerySingleAsync<int>` para leer el escalar `0`/`1` del SP.

---

# PARTE 3: Reservas

Una **Reserva** vincula a un usuario con una sesión específica (`ExperienciaConcurrencia`), registrando la cantidad de personas y el estado de la reserva.

**Ruta base:** `/api/reservations`

---

## 11. Obtener todas las reservas

- **Método:** GET
- **Ruta:** `/api/reservations`
- **Body:** No requiere
- **Respuesta exitosa (200 OK):**
```json
[
  {
    "iD_Reserva": 1,
    "fecha_Reserva": "2026-04-01T10:00:00",
    "cantidad_Personas": 2,
    "estado": 1,
    "iD_Usuario": 3,
    "iD_Concurrencia": 1
  }
]
```

---

## 12. Obtener una reserva por ID

- **Método:** GET
- **Ruta:** `/api/reservations/{id}`
- **Parámetros de ruta:**
  - `id` (int): Identificador de la reserva.
- **Body:** No requiere
- **Respuesta exitosa (200 OK):** Devuelve el objeto de la reserva.
- **Respuesta si no existe (404 Not Found)**

---

## 13. Obtener reservas por usuario

- **Método:** GET
- **Ruta:** `/api/reservations/by-user/{userId}`
- **Parámetros de ruta:**
  - `userId` (int): Identificador del usuario.
- **Body:** No requiere
- **Respuesta exitosa (200 OK):** Lista de reservas del usuario.

---

## 14. Crear una reserva

- **Método:** POST
- **Ruta:** `/api/reservations`
- **Body (JSON):**
```json
{
  "estado": 1,
  "iD_Usuario": 3,
  "cantidad_Personas": 2,
  "iD_Concurrencia": 1
}
```
- **Campos requeridos:**
  - `iD_Usuario` (int): ID del usuario que hace la reserva.
  - `iD_Concurrencia` (int): ID de la sesión a reservar.
  - `cantidad_Personas` (int): Número de personas.
  - `estado` (int): Estado inicial de la reserva (ej: `1` = activa).
- **Respuesta exitosa (201 Created):** Devuelve la ubicación del nuevo recurso y el body del modelo enviado.

---

## 15. Actualizar una reserva

- **Método:** PUT
- **Ruta:** `/api/reservations/{id}`
- **Parámetros de ruta:**
  - `id` (int): Identificador de la reserva a actualizar.
- **Body (JSON):**
```json
{
  "iD_Usuario": 3,
  "iD_Concurrencia": 2,
  "cantidad_Personas": 4,
  "estado": 2
}
```
- **Descripción:** **No incluir el `ID_Reserva` en el body, solo en la URL.**
- **Respuesta exitosa (200 OK):** Devuelve el objeto completo actualizado.
```json
{
  "iD_Reserva": 1,
  "fecha_Reserva": "2026-04-01T10:00:00",
  "cantidad_Personas": 4,
  "estado": 2,
  "iD_Usuario": 3,
  "iD_Concurrencia": 2
}
```
- **Respuesta si no existe (404 Not Found)**

---

### 🔍 ¿Cómo funciona internamente el UPDATE de una reserva?

```
PUT /api/reservations/{id}
        │
        ▼
ReservationsController.Update(int id, UpdateReservationDto model)
        │
        ├─ Valida que el body no sea nulo (400 Bad Request si es nulo)
        │
        ▼
ReservationRepository.UpdateAsync(int id, model)
        │
        ├─ Valida que id > 0 y model != null, retorna false si no se cumple
        │
        ├─ Llama al stored procedure sp_ActualizarReserva
        │   Parámetros enviados:
        │     @ID_Reserva       → id (de la URL)
        │     @ID_Usuario       → model.ID_Usuario
        │     @ID_Concurrencia  → model.iD_Concurrencia
        │     @Cantidad_Personas → model.Cantidad_Personas
        │     @Estado           → model.Estado
        │
        │   El SP ejecuta:
        │     UPDATE Reserva
        │     SET ID_Usuario = @ID_Usuario,
        │         ID_Concurrencia = @ID_Concurrencia,
        │         Cantidad_Personas = @Cantidad_Personas,
        │         Estado = @Estado
        │     WHERE ID_Reserva = @ID_Reserva
        │
        │   ⚠️ Nota: Fecha_Reserva NO se modifica en el update,
        │      conserva la fecha original del momento de creación.
        │
        ├─ Retorna result > 0
        │    → false si la reserva no existe (0 filas afectadas)
        │    → true  si la actualización fue exitosa
        │
        ▼
Controller recibe true/false
        ├─ false → 404 Not Found
        └─ true  → Llama a GetByIdAsync(id) y retorna el objeto actualizado (200 OK)
```

#### Modelo de entrada (`UpdateReservationDto`)

Todos los campos son **requeridos** (esta es una actualización completa del registro):

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `iD_Usuario` | `int` | ID del usuario asociado a la reserva |
| `iD_Concurrencia` | `int` | ID de la sesión (concurrencia) asociada |
| `cantidad_Personas` | `int` | Número de personas de la reserva |
| `estado` | `int` | Estado de la reserva (`1` = activa, `2` = cancelada, etc.) |

> ⚠️ **Nota sobre `Fecha_Reserva`:** Este campo es de solo lectura en el PUT. Se establece automáticamente al crear la reserva mediante `sp_CrearReserva` y no puede modificarse después.

> ⚠️ **Nota sobre errores silenciosos:** Si el SP falla internamente (ej: FK inválida), el repositorio atrapa la excepción y retorna `false`, lo que el controller interpreta como `404 Not Found`. Revisar los logs de error en la tabla de errores del sistema para diagnóstico.

---

## 16. Eliminar una reserva

- **Método:** DELETE
- **Ruta:** `/api/reservations/{id}`
- **Parámetros de ruta:**
  - `id` (int): Identificador de la reserva a eliminar.
- **Body:** No requiere
- **Respuesta exitosa (204 No Content)**
- **Respuesta si no existe (404 Not Found)**

---

# Estructura de los objetos

## Reserva (respuesta)

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `iD_Reserva` | int | Identificador único (auto-generado) |
| `fecha_Reserva` | DateTime? | Fecha y hora en que se creó la reserva (solo lectura) |
| `cantidad_Personas` | int? | Número de personas |
| `estado` | int? | Estado de la reserva (1 = activa, 2 = cancelada, etc.) |
| `iD_Usuario` | int? | ID del usuario que realizó la reserva |
| `iD_Concurrencia` | int? | ID de la sesión reservada |

## Experiencia (respuesta)

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `iD_Experiencia` | int | Identificador único (auto-generado) |
| `titulo` | string | Nombre de la experiencia |
| `descripcion` | string? | Descripción (opcional) |
| `categoria` | string? | Categoría (opcional) |
| `usuarioIdRegistrador` | int? | ID del usuario que la registró |
| `iD_Comunidad` | int? | ID de la comunidad asociada |

## ExperienciaConcurrencia (respuesta)

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `iD_Concurrencia` | int | Identificador único (auto-generado) |
| `fecha` | DateTime? | Fecha y hora de la sesión |
| `detalle` | string? | Descripción adicional (opcional) |
| `precio` | decimal | Precio por persona |
| `cupos_Disponibles` | int? | Cupos restantes para esta sesión |
| `iD_Experiencia` | int? | ID de la experiencia padre |

---

# Relación entre Experiencia y ExperienciaConcurrencia

```
Experiencia: Tour en canopy (ID: 1)
  ├─ Concurrencia 1: 2026-04-10 (10 cupos, $50) - Canopy mañana
  ├─ Concurrencia 2: 2026-04-11 (8 cupos, $55)  - Canopy tarde
  └─ Concurrencia 3: 2026-05-15 (12 cupos, $75) - Sesión especial

Experiencia: Clase de surf (ID: 3)
  ├─ Concurrencia 4: 2026-04-20 (12 cupos, $30) - Surf básico
  └─ Concurrencia 5: 2026-04-21 (10 cupos, $35) - Surf intermedio
```

> ⚠️ **Importante:** Los `cupos_Disponibles` de una concurrencia se gestionan automáticamente por el sistema de reservas. Al crear, actualizar o eliminar una reserva, los cupos se actualizan en tiempo real.

---

# Códigos de respuesta HTTP

| Código | Significado |
|--------|-------------|
| 200 | OK - Operación exitosa (GET, POST, PUT) |
| 204 | No Content - Eliminación exitosa (DELETE) |
| 400 | Bad Request - Body nulo o datos inválidos |
| 404 | Not Found - Recurso no encontrado |
| 500 | Internal Server Error - Error en el servidor |

---

**Actualizado:** 2024-06
