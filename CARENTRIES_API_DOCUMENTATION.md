# Car Entries REST API Documentation

## Overview
The Car Entries REST API provides endpoints for managing parking session records (vehicle entry and exit) in the Parking System Application. It supports CRUD operations (Create, Read, Update, Delete) and advanced filtering with computed fields for duration and payment calculations.

**Base URL:** `http://localhost:5000/api/carEntries`

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

## Computed Fields

All GET responses include computed fields that are automatically calculated based on entry and exit times:

- **stay_minutes** (int?): Total parking duration in minutes. NULL if vehicle is still parked
- **stay_hours** (decimal?): Total parking duration in decimal hours (e.g., 2.5 hours). NULL if vehicle is still parked
- **total_amount** (decimal?): Total amount to pay calculated as `ceiling(stay_hours) * parking_lot.price_per_hour`. NULL if vehicle is still parked. Hours are rounded UP (ceiling) so partial hours count as full hours

---

## HTTP Status Codes
- **200 OK**: Successful GET, PUT, or DELETE request
- **201 Created**: Successful POST request (resource created)
- **400 Bad Request**: Invalid request parameters or validation error
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server-side error

---

## Endpoints

### 1. Get All Car Entries
Retrieves all car entry records from the database with computed fields.

**Endpoint:** `GET /api/carEntries`

**Request:**
```bash
curl -X GET http://localhost:5000/api/carEntries
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Retrieved 3 car entries",
  "data": [
    {
      "consecutive": 1,
      "parkingId": 1,
      "automobileId": 1,
      "entryDateTime": "2026-04-14T08:00:00Z",
      "exitDateTime": "2026-04-14T10:30:00Z",
      "stayMinutes": 150,
      "stayHours": 2.5,
      "totalAmount": 7.50,
      "createdAt": "2026-04-14T08:00:00Z",
      "updatedAt": "2026-04-14T10:30:00Z"
    },
    {
      "consecutive": 2,
      "parkingId": 2,
      "automobileId": 2,
      "entryDateTime": "2026-04-14T09:00:00Z",
      "exitDateTime": null,
      "stayMinutes": null,
      "stayHours": null,
      "totalAmount": null,
      "createdAt": "2026-04-14T09:00:00Z",
      "updatedAt": "2026-04-14T09:00:00Z"
    }
  ],
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

---

### 2. Get Car Entry by Consecutive ID
Retrieves a single car entry by its consecutive ID with computed fields.

**Endpoint:** `GET /api/carEntries/{consecutive}`

**Parameters:**
- `consecutive` (path, required): The car entry consecutive ID (integer)

**Request:**
```bash
curl -X GET http://localhost:5000/api/carEntries/1
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Car entry retrieved successfully",
  "data": {
    "consecutive": 1,
    "parkingId": 1,
    "automobileId": 1,
    "entryDateTime": "2026-04-14T08:00:00Z",
    "exitDateTime": "2026-04-14T10:30:00Z",
    "stayMinutes": 150,
    "stayHours": 2.5,
    "totalAmount": 7.50,
    "createdAt": "2026-04-14T08:00:00Z",
    "updatedAt": "2026-04-14T10:30:00Z"
  },
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Car entry with consecutive ID 999 not found",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

---

### 3. Filter Car Entries by Vehicle Type and Date Range
Retrieves car entries filtered by vehicle type (sedan, 4x4, motorcycle) and optional date range.

**Endpoint:** `GET /api/carEntries/filter`

**Query Parameters:**
- `type` (optional): Vehicle type to filter by (sedan, 4x4, motorcycle)
- `dateStart` (optional): Start date for the filter (inclusive) - ISO 8601 format
- `dateEnd` (optional): End date for the filter (inclusive) - ISO 8601 format

**Request Examples:**
```bash
# Filter by vehicle type
curl -X GET "http://localhost:5000/api/carEntries/filter?type=sedan"

# Filter by date range
curl -X GET "http://localhost:5000/api/carEntries/filter?type=sedan&dateStart=2026-04-01&dateEnd=2026-04-30"

# Filter by vehicle type and date range
curl -X GET "http://localhost:5000/api/carEntries/filter?type=4x4&dateStart=2026-04-10T00:00:00&dateEnd=2026-04-14T23:59:59"
```

---

### 4. Filter Car Entries by Province and Date Range
Retrieves car entries filtered by province (parking location) and optional date range.

**Endpoint:** `GET /api/carEntries/filter`

**Query Parameters:**
- `province` (optional): Province name to filter by (case-insensitive partial match)
- `dateStart` (optional): Start date for the filter (inclusive) - ISO 8601 format
- `dateEnd` (optional): End date for the filter (inclusive) - ISO 8601 format

**Request Examples:**
```bash
# Filter by province
curl -X GET "http://localhost:5000/api/carEntries/filter?province=San%20Jose"

# Filter by province and date range
curl -X GET "http://localhost:5000/api/carEntries/filter?province=Alajuela&dateStart=2026-04-01&dateEnd=2026-04-30"

# Filter by date range only
curl -X GET "http://localhost:5000/api/carEntries/filter?dateStart=2026-04-10&dateEnd=2026-04-14"
```

**Response (200 OK - Type Filter):**
```json
{
  "success": true,
  "message": "Retrieved 2 car entries for vehicle type 'sedan' from 2026-04-01 to 2026-04-30",
  "data": [
    {
      "consecutive": 1,
      "parkingId": 1,
      "automobileId": 1,
      "entryDateTime": "2026-04-14T08:00:00Z",
      "exitDateTime": "2026-04-14T10:30:00Z",
      "stayMinutes": 150,
      "stayHours": 2.5,
      "totalAmount": 7.50,
      "createdAt": "2026-04-14T08:00:00Z",
      "updatedAt": "2026-04-14T10:30:00Z"
    }
  ],
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

**Response (400 Bad Request - Both Filters Provided):**
```json
{
  "success": false,
  "message": "Provide either 'type' or 'province', not both",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

**Response (400 Bad Request - Neither Filter Provided):**
```json
{
  "success": false,
  "message": "Provide either 'type' (vehicle type filter) or 'province' (province filter)",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

---

### 5. Create Car Entry
Creates a new car entry (parking session) record.

**Endpoint:** `POST /api/carEntries`

**Request Headers:**
- `Content-Type: application/json`

**Request Body:**
```json
{
  "parkingId": 1,
  "automobileId": 1,
  "entryDateTime": "2026-04-14T08:00:00Z",
  "exitDateTime": null
}
```

**Request (Vehicle Entering - No Exit Time):**
```bash
curl -X POST http://localhost:5000/api/carEntries \
  -H "Content-Type: application/json" \
  -d '{
    "parkingId": 1,
    "automobileId": 1,
    "entryDateTime": "2026-04-14T08:00:00Z",
    "exitDateTime": null
  }'
```

**Request (Complete Parking Session):**
```bash
curl -X POST http://localhost:5000/api/carEntries \
  -H "Content-Type: application/json" \
  -d '{
    "parkingId": 1,
    "automobileId": 2,
    "entryDateTime": "2026-04-14T08:00:00Z",
    "exitDateTime": "2026-04-14T10:30:00Z"
  }'
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "Car entry created successfully",
  "data": {
    "consecutive": 3,
    "parkingId": 1,
    "automobileId": 1,
    "entryDateTime": "2026-04-14T08:00:00Z",
    "exitDateTime": null,
    "stayMinutes": null,
    "stayHours": null,
    "totalAmount": null,
    "createdAt": "2026-04-14T12:34:56.789Z",
    "updatedAt": "2026-04-14T12:34:56.789Z"
  },
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

**Response (400 Bad Request - Invalid Parking ID):**
```json
{
  "success": false,
  "message": "Parking lot with ID 999 not found",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

### Field Validation for POST
- **parkingId**: Required, must be > 0, parking lot must exist in database
- **automobileId**: Required, must be > 0
- **entryDateTime**: Required, ISO 8601 datetime format
- **exitDateTime**: Optional, if provided must be after entryDateTime

---

### 6. Update Car Entry
Updates an existing car entry record (e.g., record vehicle exit time).

**Endpoint:** `PUT /api/carEntries/{consecutive}`

**Parameters:**
- `consecutive` (path, required): The car entry consecutive ID to update

**Request Headers:**
- `Content-Type: application/json`

**Request Body:**
```json
{
  "parkingId": 1,
  "automobileId": 1,
  "entryDateTime": "2026-04-14T08:00:00Z",
  "exitDateTime": "2026-04-14T10:30:00Z"
}
```

**Request (Record Vehicle Exit):**
```bash
curl -X PUT http://localhost:5000/api/carEntries/2 \
  -H "Content-Type: application/json" \
  -d '{
    "parkingId": 2,
    "automobileId": 2,
    "entryDateTime": "2026-04-14T09:00:00Z",
    "exitDateTime": "2026-04-14T11:45:00Z"
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Car entry updated successfully",
  "data": {
    "consecutive": 2,
    "parkingId": 2,
    "automobileId": 2,
    "entryDateTime": "2026-04-14T09:00:00Z",
    "exitDateTime": "2026-04-14T11:45:00Z",
    "stayMinutes": 165,
    "stayHours": 2.75,
    "totalAmount": 4.13,
    "createdAt": "2026-04-14T09:00:00Z",
    "updatedAt": "2026-04-14T12:34:56.789Z"
  },
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Car entry with consecutive ID 999 not found",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

### Field Validation for PUT
- **parkingId**: Required, must be > 0
- **automobileId**: Required, must be > 0
- **entryDateTime**: Required, ISO 8601 datetime format
- **exitDateTime**: Optional, if provided must be after entryDateTime

---

### 7. Delete Car Entry
Deletes a car entry record from the database.

**Endpoint:** `DELETE /api/carEntries/{consecutive}`

**Parameters:**
- `consecutive` (path, required): The car entry consecutive ID to delete

**Request:**
```bash
curl -X DELETE http://localhost:5000/api/carEntries/1
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Car entry with consecutive ID 1 deleted successfully",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Car entry with consecutive ID 999 not found",
  "data": null,
  "timestamp": "2026-04-14T12:34:56.789Z"
}
```

---

## Complete Example Workflow

### 1. Create a car entry (vehicle enters parking)
```bash
curl -X POST http://localhost:5000/api/carEntries \
  -H "Content-Type: application/json" \
  -d '{
    "parkingId": 1,
    "automobileId": 1,
    "entryDateTime": "2026-04-14T08:00:00Z",
    "exitDateTime": null
  }'
```

### 2. Retrieve the car entry
```bash
curl -X GET http://localhost:5000/api/carEntries/1
```

### 3. Update the car entry (vehicle exits parking)
```bash
curl -X PUT http://localhost:5000/api/carEntries/1 \
  -H "Content-Type: application/json" \
  -d '{
    "parkingId": 1,
    "automobileId": 1,
    "entryDateTime": "2026-04-14T08:00:00Z",
    "exitDateTime": "2026-04-14T10:30:00Z"
  }'
```

### 4. Filter by vehicle type for the date range
```bash
curl -X GET "http://localhost:5000/api/carEntries/filter?type=sedan&dateStart=2026-04-14&dateEnd=2026-04-14"
```

### 5. Filter by province for the date range
```bash
curl -X GET "http://localhost:5000/api/carEntries/filter?province=San%20Jose&dateStart=2026-04-14&dateEnd=2026-04-14"
```

### 6. Delete the car entry
```bash
curl -X DELETE http://localhost:5000/api/carEntries/1
```

---

## Error Handling

The API returns appropriate HTTP status codes and error messages:

| Scenario | Status | Message |
|----------|--------|---------|
| Valid request | 200, 201 | Operation successful |
| Missing required field | 400 | Field validation message |
| Invalid date range | 400 | dateStart cannot be greater than dateEnd |
| Both filters provided | 400 | Provide either 'type' or 'province', not both |
| No filters provided | 400 | Provide either 'type' or 'province' |
| Resource not found | 404 | Car entry with consecutive ID {id} not found |
| Server error | 500 | Error: {exception message} |

---

## Important Notes

### Computed Fields (stay_minutes, stay_hours, total_amount)
- These fields are **NULL** if `exitDateTime` is NULL (vehicle still parked)
- `stay_hours` uses ceiling rounding: 2.1 hours = 3 hours to pay
- `total_amount` = ceiling(stay_hours) × parking_lot.price_per_hour
- All values are calculated server-side from the Parking entity's PricePerHour

### Date Filtering
- Dates are in ISO 8601 format (e.g., 2026-04-14 or 2026-04-14T08:00:00Z)
- Date range filtering is inclusive on both ends
- Only entry date is filtered (entryDateTime), not exit time

### Filter Behavior
- **Type Filter**: Exact match (case-insensitive) of vehicle type
- **Province Filter**: Case-insensitive substring match
- Cannot use both type and province filters simultaneously

### Validation Rules
- ExitDateTime must be after EntryDateTime (if provided)
- ParkingId and AutomobileId must be > 0
- Foreign key references must exist in respective tables
