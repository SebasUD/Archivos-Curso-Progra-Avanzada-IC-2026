# Parking System REST API Documentation

**API Version:** 1.0  
**Base URL:** `http://localhost:5000/api`  
**Response Format:** JSON  
**Date:** April 14, 2026

---

## 📋 Overview

The Parking System REST API provides CRUD operations and advanced filtering for managing automobiles in the parking system database. All endpoints return standardized JSON responses with success/error handling.

---

## 🔗 Base URL

```
http://localhost:5000/api/automobiles
```

---

## 📊 Response Format

All endpoints return responses in the following format:

```json
{
  "success": true,
  "message": "Description of the operation",
  "data": { /* endpoint-specific data */ },
  "timestamp": "2026-04-14T10:30:00Z"
}
```

---

## 📍 Endpoints

### 1. **GET /automobiles** - Get All Automobiles

Retrieves all automobiles from the database.

#### Request
```http
GET /api/automobiles HTTP/1.1
Host: localhost:5000
```

#### Response (200 OK)
```json
{
  "success": true,
  "message": "Retrieved 8 automobiles",
  "data": [
    {
      "id": 1,
      "color": "Red",
      "year": 2020,
      "manufacturer": "Toyota",
      "type": "sedan",
      "createdAt": "2026-04-13T00:00:00Z",
      "updatedAt": "2026-04-13T00:00:00Z"
    },
    {
      "id": 2,
      "color": "Black",
      "year": 2019,
      "manufacturer": "Honda",
      "type": "sedan",
      "createdAt": "2026-04-13T00:00:00Z",
      "updatedAt": "2026-04-13T00:00:00Z"
    }
  ],
  "timestamp": "2026-04-14T10:30:00Z"
}
```

#### Response (500 Internal Server Error)
```json
{
  "success": false,
  "message": "Error: Database connection failed",
  "data": null,
  "timestamp": "2026-04-14T10:30:00Z"
}
```

---

### 2. **GET /automobiles/{id}** - Get Automobile by ID

Retrieves a specific automobile by its primary key.

#### Request
```http
GET /api/automobiles/1 HTTP/1.1
Host: localhost:5000
```

#### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | The automobile's primary key |

#### Response (200 OK)
```json
{
  "success": true,
  "message": "Automobile retrieved successfully",
  "data": {
    "id": 1,
    "color": "Red",
    "year": 2020,
    "manufacturer": "Toyota",
    "type": "sedan",
    "createdAt": "2026-04-13T00:00:00Z",
    "updatedAt": "2026-04-13T00:00:00Z"
  },
  "timestamp": "2026-04-14T10:30:00Z"
}
```

#### Response (404 Not Found)
```json
{
  "success": false,
  "message": "Automobile with ID 999 not found",
  "data": null,
  "timestamp": "2026-04-14T10:30:00Z"
}
```

---

### 3. **GET /automobiles/filter** - Filter Automobiles

Retrieves automobiles with advanced filtering by multiple criteria. All filter parameters are optional and use case-insensitive partial matching.

#### Request
```http
GET /api/automobiles/filter?color=Red&yearStart=2018&yearEnd=2022&manufacturer=Toyota&type=sedan HTTP/1.1
Host: localhost:5000
```

#### Query Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `color` | string | No | Filter by vehicle color (partial match, case-insensitive) |
| `yearStart` | integer | No | Filter by minimum manufacturing year (1900-2100) |
| `yearEnd` | integer | No | Filter by maximum manufacturing year (1900-2100) |
| `manufacturer` | string | No | Filter by manufacturer name (partial match, case-insensitive) |
| `type` | string | No | Filter by vehicle type (sedan, 4x4, motorcycle, case-insensitive) |

#### Example Requests

**Filter by color:**
```
GET /api/automobiles/filter?color=Red
```

**Filter by year range:**
```
GET /api/automobiles/filter?yearStart=2018&yearEnd=2022
```

**Filter by manufacturer:**
```
GET /api/automobiles/filter?manufacturer=Toyota
```

**Filter by type:**
```
GET /api/automobiles/filter?type=sedan
```

**Complex filter (multiple criteria):**
```
GET /api/automobiles/filter?color=Black&yearStart=2018&yearEnd=2021&manufacturer=Honda
```

#### Response (200 OK)
```json
{
  "success": true,
  "message": "Retrieved 2 automobiles matching the filter criteria",
  "data": [
    {
      "id": 1,
      "color": "Red",
      "year": 2020,
      "manufacturer": "Toyota",
      "type": "sedan",
      "createdAt": "2026-04-13T00:00:00Z",
      "updatedAt": "2026-04-13T00:00:00Z"
    },
    {
      "id": 3,
      "color": "Red",
      "year": 2021,
      "manufacturer": "Toyota",
      "type": "sedan",
      "createdAt": "2026-04-13T00:00:00Z",
      "updatedAt": "2026-04-13T00:00:00Z"
    }
  ],
  "timestamp": "2026-04-14T10:30:00Z"
}
```

#### Response (400 Bad Request)
```json
{
  "success": false,
  "message": "yearStart cannot be greater than yearEnd",
  "data": null,
  "timestamp": "2026-04-14T10:30:00Z"
}
```

---

### 4. **POST /automobiles** - Create Automobile

Creates a new automobile record in the database.

#### Request
```http
POST /api/automobiles HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "color": "Silver",
  "year": 2023,
  "manufacturer": "Tesla",
  "type": "sedan"
}
```

#### Request Body
| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| `color` | string | Yes | Max 50 characters, cannot be empty |
| `year` | integer | Yes | Must be between 1900 and 2100 |
| `manufacturer` | string | Yes | Max 100 characters, cannot be empty |
| `type` | string | Yes | Must be one of: `sedan`, `4x4`, `motorcycle` |

#### Response (201 Created)
```json
{
  "success": true,
  "message": "Automobile created successfully",
  "data": {
    "id": 9,
    "color": "Silver",
    "year": 2023,
    "manufacturer": "Tesla",
    "type": "sedan",
    "createdAt": "2026-04-14T10:30:00Z",
    "updatedAt": "2026-04-14T10:30:00Z"
  },
  "timestamp": "2026-04-14T10:30:00Z"
}
```

#### Response (400 Bad Request)
```json
{
  "success": false,
  "message": "Year must be between 1900 and 2100",
  "data": null,
  "timestamp": "2026-04-14T10:30:00Z"
}
```

#### Response (400 Bad Request - Invalid Type)
```json
{
  "success": false,
  "message": "Type must be one of: sedan, 4x4, motorcycle",
  "data": null,
  "timestamp": "2026-04-14T10:30:00Z"
}
```

---

### 5. **PUT /automobiles/{id}** - Update Automobile

Updates an existing automobile record.

#### Request
```http
PUT /api/automobiles/1 HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "color": "Blue",
  "year": 2021,
  "manufacturer": "Toyota",
  "type": "sedan"
}
```

#### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | The automobile's primary key |

#### Request Body
| Field | Type | Required | Constraints |
|-------|------|----------|-------------|
| `color` | string | Yes | Max 50 characters, cannot be empty |
| `year` | integer | Yes | Must be between 1900 and 2100 |
| `manufacturer` | string | Yes | Max 100 characters, cannot be empty |
| `type` | string | Yes | Must be one of: `sedan`, `4x4`, `motorcycle` |

#### Response (200 OK)
```json
{
  "success": true,
  "message": "Automobile updated successfully",
  "data": {
    "id": 1,
    "color": "Blue",
    "year": 2021,
    "manufacturer": "Toyota",
    "type": "sedan",
    "createdAt": "2026-04-13T00:00:00Z",
    "updatedAt": "2026-04-14T10:30:00Z"
  },
  "timestamp": "2026-04-14T10:30:00Z"
}
```

#### Response (404 Not Found)
```json
{
  "success": false,
  "message": "Automobile with ID 999 not found",
  "data": null,
  "timestamp": "2026-04-14T10:30:00Z"
}
```

#### Response (400 Bad Request)
```json
{
  "success": false,
  "message": "Type must be one of: sedan, 4x4, motorcycle",
  "data": null,
  "timestamp": "2026-04-14T10:30:00Z"
}
```

---

### 6. **DELETE /automobiles/{id}** - Delete Automobile

Deletes an automobile record from the database.

#### Request
```http
DELETE /api/automobiles/1 HTTP/1.1
Host: localhost:5000
```

#### Path Parameters
| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | The automobile's primary key |

#### Response (200 OK)
```json
{
  "success": true,
  "message": "Automobile with ID 1 deleted successfully",
  "data": null,
  "timestamp": "2026-04-14T10:30:00Z"
}
```

#### Response (404 Not Found)
```json
{
  "success": false,
  "message": "Automobile with ID 999 not found",
  "data": null,
  "timestamp": "2026-04-14T10:30:00Z"
}
```

---

## 🧪 cURL Examples

### Get All Automobiles
```bash
curl -X GET "http://localhost:5000/api/automobiles" \
  -H "Content-Type: application/json"
```

### Get Automobile by ID
```bash
curl -X GET "http://localhost:5000/api/automobiles/1" \
  -H "Content-Type: application/json"
```

### Filter Automobiles (multiple criteria)
```bash
curl -X GET "http://localhost:5000/api/automobiles/filter?color=Red&yearStart=2018&yearEnd=2022&manufacturer=Toyota" \
  -H "Content-Type: application/json"
```

### Create Automobile
```bash
curl -X POST "http://localhost:5000/api/automobiles" \
  -H "Content-Type: application/json" \
  -d '{
    "color": "Silver",
    "year": 2023,
    "manufacturer": "Tesla",
    "type": "sedan"
  }'
```

### Update Automobile
```bash
curl -X PUT "http://localhost:5000/api/automobiles/1" \
  -H "Content-Type: application/json" \
  -d '{
    "color": "Blue",
    "year": 2021,
    "manufacturer": "Toyota",
    "type": "sedan"
  }'
```

### Delete Automobile
```bash
curl -X DELETE "http://localhost:5000/api/automobiles/1" \
  -H "Content-Type: application/json"
```

---

## 📝 HTTP Status Codes

| Code | Meaning | Description |
|------|---------|-------------|
| **200** | OK | Request successful, data returned |
| **201** | Created | Resource created successfully |
| **400** | Bad Request | Invalid parameters or validation error |
| **404** | Not Found | Resource not found |
| **500** | Internal Server Error | Server-side error |

---

## 🔍 Field Validation Rules

### Color
- **Required:** Yes
- **Max Length:** 50 characters
- **Empty Check:** Not allowed

### Year
- **Required:** Yes
- **Range:** 1900 - 2100
- **Type:** Integer

### Manufacturer
- **Required:** Yes
- **Max Length:** 100 characters
- **Empty Check:** Not allowed

### Type
- **Required:** Yes
- **Allowed Values:** `sedan`, `4x4`, `motorcycle`
- **Case Handling:** Case-insensitive
- **Empty Check:** Not allowed

---

## 💾 Sample Automobile Records

Provided in the database:

| ID | Color | Year | Manufacturer | Type |
|----|-------|------|--------------|------|
| 1 | Red | 2020 | Toyota | sedan |
| 2 | Black | 2019 | Honda | sedan |
| 3 | Silver | 2021 | Ford | 4x4 |
| 4 | Blue | 2018 | BMW | sedan |
| 5 | White | 2022 | Yamaha | motorcycle |
| 6 | Gray | 2020 | Jeep | 4x4 |
| 7 | White | 2018 | Mercedes-Benz | sedan |
| 8 | Red | 2023 | Harley-Davidson | motorcycle |

---

## 🔐 Security Notes

- ✅ All endpoints use parameterized queries (EF Core)
- ✅ Input validation on all POST/PUT operations
- ✅ HTTP errors don't expose sensitive information
- ✅ Suitable for API gateway authentication (add JWT/Bearer token as needed)
- ✅ CORS enabled for cross-origin requests

---

## 🧩 Integration with Repository Pattern

This API uses the **Repository Pattern** for all database operations:

```csharp
public class AutomobilesController : ControllerBase
{
    private readonly IAutomobileRepository _repository;
    
    public AutomobilesController(IAutomobileRepository repository)
    {
        _repository = repository;
    }
    
    // All operations delegate to repository
    var automobiles = await _repository.GetAllFromDbAsync();
    var automobile = await _repository.GetByIdAsync(id);
    var filtered = await _repository.GetFilteredAsync(color, yearStart, yearEnd, manufacturer, type);
    await _repository.InsertAsync(automobile);
    await _repository.UpdateAsync(automobile);
    await _repository.DeleteAsync(id);
}
```

**Benefits:**
- Testable (mock the repository)
- Maintainable (separation of concerns)
- Reusable (other controllers can use same repository)
- Database-agnostic (swap implementation)

---

## 🚀 Getting Started

### 1. Start the API
```bash
cd ParkingSystemApp
dotnet restore
dotnet user-secrets set "ConnectionStrings:ParkingSystemDb" "Server=YOUR_HOST;Port=3306;Database=parking_system;Uid=YOUR_USER;Pwd=YOUR_PASSWORD;"
dotnet run
```

### 2. Verify Running
```bash
curl http://localhost:5000/api/automobiles
```

### 3. View Swagger Documentation (if enabled)
```
http://localhost:5000/swagger/index.html
```

---

## 📞 Troubleshooting

### "Connection Failed" Error
- Verify MySQL instance is running
- Check User Secrets configuration
- Test connection: `dotnet user-secrets list`

### "No Route Matches" Error
- Verify API is running on `http://localhost:5000`
- Check endpoint URL matches exactly
- Ensure HTTP method is correct (GET, POST, PUT, DELETE)

### "Validation Error" (400)
- Review field constraints above
- Check data types match specification
- Ensure required fields are provided

---

**Last Updated:** April 14, 2026

