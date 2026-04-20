# CatalogoEstado Endpoint Documentation

## Overview
Created a complete REST API endpoint to retrieve reservation status values (CatalogoEstado) from the database. This allows frontend applications to dynamically fetch status options instead of hardcoding them.

## Endpoint Details

### URL
```
GET /api/catalog/estados
```

### Response Example
```json
[
  {
    "id_estado": 1,
    "descripcion": "Pendiente"
  },
  {
    "id_estado": 2,
    "descripcion": "Confirmada"
  },
  {
    "id_estado": 3,
    "descripcion": "Cancelada"
  },
  {
    "id_estado": 4,
    "descripcion": "Completada"
  }
]
```

### HTTP Status Codes
- **200 OK**: Successfully retrieved all status values
- **500 Internal Server Error**: Database error occurred

### Sample Request (cURL)
```bash
curl -X GET "https://localhost:7054/api/catalog/estados" \
  -H "Content-Type: application/json"
```

### Sample Request (JavaScript/Fetch)
```javascript
fetch('https://localhost:7054/api/catalog/estados')
  .then(response => response.json())
  .then(data => {
    console.log('Status options:', data);
    // Use data to populate dropdown or filter options
  })
  .catch(error => console.error('Error:', error));
```

---

## Implementation Details

### Created Files
1. **Models/CatalogoEstado.cs**
   - Simple data model with ID_estado and Descripcion properties
   - Maps to database CatalogoEstado table

2. **Interfaces/ICatalogRepository.cs**
   - Interface defining `GetAllEstadosAsync()` method
   - Follows repository pattern consistency

3. **Repositories/CatalogRepository.cs**
   - Implementation of ICatalogRepository
   - Uses Dapper to call stored procedure
   - Returns IEnumerable<CatalogoEstado>

4. **Controllers/CatalogController.cs**
   - API controller with single endpoint: `GET /api/catalog/estados`
   - Basic error handling with 500 response on exceptions

### Database Changes
- **Added Stored Procedure**: `SP_ConsultarCatalogoEstado`
  - Located in DatabaseSetup.sql (lines 212-222)
  - Retrieves all status values ordered by ID_estado
  - Uses `SELECT ID_estado, Descripcion FROM CatalogoEstado`

### Dependency Injection
- Registered in `Program.cs` (line 12):
  ```csharp
  builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
  ```

---

## Frontend Usage Examples

### Vue.js
```javascript
export default {
  data() {
    return {
      statusOptions: [],
      selectedStatus: null
    }
  },
  async mounted() {
    const response = await fetch('/api/catalog/estados');
    this.statusOptions = await response.json();
  }
}
```

### React
```javascript
const [statusOptions, setStatusOptions] = useState([]);

useEffect(() => {
  fetch('/api/catalog/estados')
    .then(res => res.json())
    .then(data => setStatusOptions(data));
}, []);
```

### Angular
```typescript
constructor(private http: HttpClient) {}

getStatusOptions(): Observable<CatalogoEstado[]> {
  return this.http.get<CatalogoEstado[]>('/api/catalog/estados');
}
```

---

## Status Values Reference

| ID | Description | Use Case |
|---|---|---|
| 1 | Pendiente | Initial reservation state, awaiting confirmation |
| 2 | Confirmada | Reservation confirmed by admin |
| 3 | Cancelada | Reservation cancelled by user or admin |
| 4 | Completada | Reservation completed (experience occurred) |

---

## Testing

### Via Swagger UI
1. Start the application (F5 in Visual Studio)
2. Navigate to https://localhost:7054/
3. Look for "Catalog" section in Swagger
4. Click "Try it out" on GET /api/catalog/estados
5. Click "Execute" to test

### Via Visual Studio Test Explorer
You can create unit tests for the endpoint:
```csharp
[TestMethod]
public async Task GetEstados_ReturnsOkResult()
{
    var controller = new CatalogController(mockRepository);
    var result = await controller.GetEstados();

    Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
}
```

---

## Notes

- ✅ Follows existing Repository pattern
- ✅ Uses Stored Procedures for database access consistency
- ✅ Integrated with dependency injection
- ✅ Includes basic error handling
- ✅ Ready for production use
- ⚠️ Consider adding caching (Redis) if called frequently
- ⚠️ Consider adding authorization if status values should be role-restricted

---

## Related Endpoints

This endpoint complements existing endpoints:
- `/api/experiencias` - Experience management
- `/api/reservations` - Reservation management (uses status values)
- `/api/comunidades` - Community management
- `/api/experienciaconcurrencia` - Experience schedule management

---

## Database Setup

Run the updated `DatabaseSetup.sql` to create:
1. ✅ CatalogoEstado table (if not exists)
2. ✅ Seed data (4 status values)
3. ✅ SP_ConsultarCatalogoEstado stored procedure

Execute in SQL Server Management Studio:
```sql
-- This script contains everything needed
EXEC sp_executesql N'...' -- Copy entire DatabaseSetup.sql content
```
