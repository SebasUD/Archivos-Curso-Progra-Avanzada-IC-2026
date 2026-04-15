# Parkings REST API Documentation

## Overview
The Parkings REST API provides endpoints for managing parking lot data in the Parking System Application. It supports CRUD operations (Create, Read, Update, Delete) and advanced filtering capabilities.

**Base URL:** `http://localhost:5000/api/parkings`

---

## Response Format
All API responses follow a standardized JSON format:

```json
{
  "success": true,
  "message": "Descriptive message about the response",
  "data": {},
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

### Response Fields
- **success** (boolean): Indicates if the request was successful
- **message** (string): Descriptive message about the response
- **data** (object|array|null): The response payload
- **timestamp** (string): ISO 8601 UTC timestamp of the response

---

## HTTP Status Codes
- **200 OK**: Successful GET, PUT, or DELETE request
- **201 Created**: Successful POST request (resource created)
- **400 Bad Request**: Invalid request parameters or validation error
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server-side error

---

## Endpoints

### 1. Get All Parking Lots
Retrieves all parking lots from the database.

**Endpoint:** `GET /api/parkings`

**Request:**
```bash
curl -X GET http://localhost:5000/api/parkings
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Retrieved 5 parking lots",
  "data": [
    {
      "id": 1,
      "provinceName": "San José",
      "parkingName": "Parking Central Downtown",
      "pricePerHour": 2.50,
      "createdAt": "2026-04-10T08:00:00Z",
      "updatedAt": "2026-04-12T10:30:00Z"
    },
    {
      "id": 2,
      "provinceName": "Alajuela",
      "parkingName": "Alajuela Mall Parking",
      "pricePerHour": 1.50,
      "createdAt": "2026-04-11T09:15:00Z",
      "updatedAt": "2026-04-13T14:45:00Z"
    }
  ],
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

---

### 2. Get Parking Lot by ID
Retrieves a single parking lot by its ID.

**Endpoint:** `GET /api/parkings/{id}`

**Parameters:**
- `id` (path, required): The parking lot ID (integer)

**Request:**
```bash
curl -X GET http://localhost:5000/api/parkings/1
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Parking lot retrieved successfully",
  "data": {
    "id": 1,
    "provinceName": "San José",
    "parkingName": "Parking Central Downtown",
    "pricePerHour": 2.50,
    "createdAt": "2026-04-10T08:00:00Z",
    "updatedAt": "2026-04-12T10:30:00Z"
  },
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Parking lot with ID 999 not found",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

---

### 3. Filter Parking Lots
Retrieves parking lots with advanced filtering options.

**Endpoint:** `GET /api/parkings/filter`

**Query Parameters:**
- `province` (optional): Filter by province name (case-insensitive partial match)
- `name` (optional): Filter by parking name (case-insensitive partial match)
- `priceMin` (optional): Filter by minimum price per hour (decimal)
- `priceMax` (optional): Filter by maximum price per hour (decimal)

**Request Examples:**
```bash
# Filter by province
curl -X GET "http://localhost:5000/api/parkings/filter?province=San%20Jose"

# Filter by price range
curl -X GET "http://localhost:5000/api/parkings/filter?priceMin=1.00&priceMax=3.00"

# Filter by province and name
curl -X GET "http://localhost:5000/api/parkings/filter?province=Alajuela&name=Mall"

# Filter by all criteria
curl -X GET "http://localhost:5000/api/parkings/filter?province=San&name=downtown&priceMin=2&priceMax=5"
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Retrieved 2 parking lots matching the filter criteria",
  "data": [
    {
      "id": 1,
      "provinceName": "San José",
      "parkingName": "Parking Central Downtown",
      "pricePerHour": 2.50,
      "createdAt": "2026-04-10T08:00:00Z",
      "updatedAt": "2026-04-12T10:30:00Z"
    },
    {
      "id": 3,
      "provinceName": "San José",
      "parkingName": "Parking Westside",
      "pricePerHour": 2.75,
      "createdAt": "2026-04-12T11:00:00Z",
      "updatedAt": "2026-04-13T16:20:00Z"
    }
  ],
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

**Response (400 Bad Request - Invalid Price Range):**
```json
{
  "success": false,
  "message": "priceMin cannot be greater than priceMax",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

---

### 4. Create Parking Lot
Creates a new parking lot in the database.

**Endpoint:** `POST /api/parkings`

**Request Headers:**
- `Content-Type: application/json`

**Request Body:**
```json
{
  "provinceName": "Heredia",
  "parkingName": "Heredia Shopping Center Parking",
  "pricePerHour": 1.75
}
```

**Request:**
```bash
curl -X POST http://localhost:5000/api/parkings \
  -H "Content-Type: application/json" \
  -d '{
    "provinceName": "Heredia",
    "parkingName": "Heredia Shopping Center Parking",
    "pricePerHour": 1.75
  }'
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "Parking lot created successfully",
  "data": {
    "id": 5,
    "provinceName": "Heredia",
    "parkingName": "Heredia Shopping Center Parking",
    "pricePerHour": 1.75,
    "createdAt": "2026-04-14T12:34:56.789Z",
    "updatedAt": "2026-04-14T12:34:56.789Z"
  },
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

**Response (400 Bad Request - Missing Province):**
```json
{
  "success": false,
  "message": "Province name is required",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

**Response (400 Bad Request - Invalid Price):**
```json
{
  "success": false,
  "message": "Price per hour must be greater than 0",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

### Field Validation for POST
- **provinceName**: Required, maximum 100 characters
- **parkingName**: Required, maximum 150 characters
- **pricePerHour**: Required, must be greater than 0

---

### 5. Update Parking Lot
Updates an existing parking lot record.

**Endpoint:** `PUT /api/parkings/{id}`

**Parameters:**
- `id` (path, required): The parking lot ID to update

**Request Headers:**
- `Content-Type: application/json`

**Request Body:**
```json
{
  "provinceName": "San José",
  "parkingName": "Parking Central Downtown (Updated)",
  "pricePerHour": 3.00
}
```

**Request:**
```bash
curl -X PUT http://localhost:5000/api/parkings/1 \
  -H "Content-Type: application/json" \
  -d '{
    "provinceName": "San José",
    "parkingName": "Parking Central Downtown (Updated)",
    "pricePerHour": 3.00
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Parking lot updated successfully",
  "data": {
    "id": 1,
    "provinceName": "San José",
    "parkingName": "Parking Central Downtown (Updated)",
    "pricePerHour": 3.00,
    "createdAt": "2026-04-10T08:00:00Z",
    "updatedAt": "2026-04-14T12:34:56.789Z"
  },
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Parking lot with ID 999 not found",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

### Field Validation for PUT
- **provinceName**: Required, maximum 100 characters
- **parkingName**: Required, maximum 150 characters
- **pricePerHour**: Required, must be greater than 0

---

### 6. Delete Parking Lot
Deletes a parking lot record from the database.

**Endpoint:** `DELETE /api/parkings/{id}`

**Parameters:**
- `id` (path, required): The parking lot ID to delete

**Request:**
```bash
curl -X DELETE http://localhost:5000/api/parkings/1
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Parking lot with ID 1 deleted successfully",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Parking lot with ID 999 not found",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

---

## Complete Example Workflow

### 1. Create a parking lot
```bash
curl -X POST http://localhost:5000/api/parkings \
  -H "Content-Type: application/json" \
  -d '{
    "provinceName": "Cartago",
    "parkingName": "Cartago Central Parking",
    "pricePerHour": 1.25
  }'
```

### 2. Retrieve the created parking lot
```bash
curl -X GET http://localhost:5000/api/parkings/1
```

### 3. Filter parking lots by price range
```bash
curl -X GET "http://localhost:5000/api/parkings/filter?priceMin=1.00&priceMax=2.00"
```

### 4. Update the parking lot
```bash
curl -X PUT http://localhost:5000/api/parkings/1 \
  -H "Content-Type: application/json" \
  -d '{
    "provinceName": "Cartago",
    "parkingName": "Cartago Central Parking (Updated)",
    "pricePerHour": 1.50
  }'
```

### 5. Delete the parking lot
```bash
curl -X DELETE http://localhost:5000/api/parkings/1
```

---

## Error Handling

The API returns appropriate HTTP status codes and error messages:

| Scenario | Status | Message |
|----------|--------|---------|
| Valid request | 200, 201 | Operation successful |
| Missing required field | 400 | Field name is required |
| Validation error | 400 | Specific validation message |
| Resource not found | 404 | Resource with ID {id} not found |
| Server error | 500 | Error: {exception message} |

---

## Notes

- All timestamps are in UTC ISO 8601 format
- Prices should be provided as decimal values (e.g., 2.50, 1.75)
- Search/filter parameters are case-insensitive and support partial matching
- Province and Name filters perform case-insensitive substring matching
- Negative prices are not allowed
- Price range must have priceMin ≤ priceMax
