# REST API Quick Start Guide

**Created:** April 14, 2026

---

## ⚡ Quick Start (3 Steps)

### Step 1: Configure Database Connection
```bash
cd ParkingSystemApp
dotnet user-secrets set "ConnectionStrings:ParkingSystemDb" "Server=YOUR_HOST;Port=3306;Database=parking_system;Uid=YOUR_USER;Pwd=YOUR_PASSWORD;"
```

### Step 2: Run the API
```bash
dotnet run
```

### Step 3: Test an Endpoint
```bash
curl http://localhost:5000/api/automobiles
```

---

## 🧪 Test All 6 Endpoints

### 1️⃣ GET All Automobiles
```bash
curl http://localhost:5000/api/automobiles
```
**Expected:** Returns array of 8 automobiles

---

### 2️⃣ GET Automobile by ID
```bash
curl http://localhost:5000/api/automobiles/1
```
**Expected:** Returns Toyota Red 2020 sedan

---

### 3️⃣ GET Filtered Automobiles
```bash
# Filter by color
curl "http://localhost:5000/api/automobiles/filter?color=Red"

# Filter by manufacturer
curl "http://localhost:5000/api/automobiles/filter?manufacturer=Toyota"

# Filter by year range
curl "http://localhost:5000/api/automobiles/filter?yearStart=2018&yearEnd=2022"

# Filter by type
curl "http://localhost:5000/api/automobiles/filter?type=sedan"

# Complex filter
curl "http://localhost:5000/api/automobiles/filter?color=Red&yearStart=2020&yearEnd=2023&manufacturer=Toyota"
```
**Expected:** Returns filtered automobiles matching criteria

---

### 4️⃣ POST Create Automobile
```bash
curl -X POST "http://localhost:5000/api/automobiles" \
  -H "Content-Type: application/json" \
  -d '{
    "color": "Gold",
    "year": 2024,
    "manufacturer": "Audi",
    "type": "sedan"
  }'
```
**Expected:** Returns created automobile with ID 9+

---

### 5️⃣ PUT Update Automobile
```bash
curl -X PUT "http://localhost:5000/api/automobiles/1" \
  -H "Content-Type: application/json" \
  -d '{
    "color": "Dark Blue",
    "year": 2020,
    "manufacturer": "Toyota",
    "type": "sedan"
  }'
```
**Expected:** Returns updated automobile

---

### 6️⃣ DELETE Automobile
```bash
curl -X DELETE "http://localhost:5000/api/automobiles/9"
```
**Expected:** Returns success message

---

## 📝 Common Requests

### Search by Multiple Criteria
```bash
# Find all 2020-2021 red sedans
curl "http://localhost:5000/api/automobiles/filter?color=red&yearStart=2020&yearEnd=2021&type=sedan"
```

### Create Entry
```bash
curl -X POST "http://localhost:5000/api/automobiles" \
  -H "Content-Type: application/json" \
  -d '{
    "color": "Green",
    "year": 2023,
    "manufacturer": "Volkswagen",
    "type": "sedan"
  }'
```

### Update Specific Field
```bash
curl -X PUT "http://localhost:5000/api/automobiles/5" \
  -H "Content-Type: application/json" \
  -d '{
    "color": "Black",
    "year": 2022,
    "manufacturer": "Yamaha",
    "type": "motorcycle"
  }'
```

---

## ✅ Validation Rules

### POST/PUT Body Validation

**Color** - Max 50 chars, required, not empty
```json
"color": "Red"
```

**Year** - 1900-2100
```json
"year": 2023
```

**Manufacturer** - Max 100 chars, required, not empty
```json
"manufacturer": "Toyota"
```

**Type** - Must be: `sedan`, `4x4`, `motorcycle`
```json
"type": "sedan"
```

---

## 🔍 Filter Query Parameters

**color** (optional) - Partial match, case-insensitive
```
?color=Red
```

**yearStart** (optional) - Min year 1900, Max 2100
```
?yearStart=2018
```

**yearEnd** (optional) - Min year 1900, Max 2100
```
?yearEnd=2022
```

**manufacturer** (optional) - Partial match, case-insensitive
```
?manufacturer=Toyota
```

**type** (optional) - One of: sedan, 4x4, motorcycle
```
?type=sedan
```

---

## 🎯 Example Workflows

### Scenario 1: Create and Update
```bash
# 1. Create new automobile
RESPONSE=$(curl -X POST "http://localhost:5000/api/automobiles" \
  -H "Content-Type: application/json" \
  -d '{"color":"Silver","year":2023,"manufacturer":"Tesla","type":"sedan"}')

# 2. Parse ID from response (example using jq)
# ID=$(echo $RESPONSE | jq '.data.id')

# 3. Update the automobile
curl -X PUT "http://localhost:5000/api/automobiles/9" \
  -H "Content-Type: application/json" \
  -d '{"color":"Blue","year":2023,"manufacturer":"Tesla","type":"sedan"}'
```

### Scenario 2: Filter and Count
```bash
# Find all sedan vehicles
curl "http://localhost:5000/api/automobiles/filter?type=sedan"

# Find all 4x4s manufactured before 2021
curl "http://localhost:5000/api/automobiles/filter?type=4x4&yearEnd=2020"

# Count motorcycles (pipe to jq if available)
curl "http://localhost:5000/api/automobiles/filter?type=motorcycle" | jq '.data | length'
```

### Scenario 3: Delete Old Records
```bash
# Delete automobile with ID 8
curl -X DELETE "http://localhost:5000/api/automobiles/8"

# Verify deletion
curl "http://localhost:5000/api/automobiles/8"
# Should return 404 Not Found
```

---

## 📊 Response Status Codes

| Status | Meaning | Example |
|--------|---------|---------|
| **200** | Success | GET, PUT, DELETE |
| **201** | Created | POST new automobile |
| **400** | Bad Request | Invalid year, missing field |
| **404** | Not Found | ID doesn't exist |
| **500** | Server Error | Database connection failed |

---

## 🛠️ Troubleshooting

### "Connection refused" or "Failed to connect"
```bash
# Check if API is running
curl http://localhost:5000/api/automobiles

# Check User Secrets
dotnet user-secrets list

# Verify credentials
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:ParkingSystemDb" "Server=YOUR_HOST;..."
```

### "404 Not Found" for valid ID
- Verify ID exists: `curl http://localhost:5000/api/automobiles`
- Check exact ID number
- Record may have been deleted

### "400 Bad Request" on POST/PUT
- Check all required fields are present
- Validate Type field is one of: sedan, 4x4, motorcycle
- Ensure Year is between 1900-2100
- Verify Color and Manufacturer are not empty

### "Year validation error"
Valid range: **1900 - 2100**
```bash
# ❌ Invalid
-d '{"color":"Red","year":1800,"manufacturer":"Test","type":"sedan"}'

# ✅ Valid
-d '{"color":"Red","year":2023,"manufacturer":"Test","type":"sedan"}'
```

---

## 📚 Documentation Links

- **Complete API Docs:** [API_DOCUMENTATION.md](API_DOCUMENTATION.md)
- **Implementation Details:** [REST_API_IMPLEMENTATION.md](REST_API_IMPLEMENTATION.md)
- **Project Analysis:** [PROJECT_ANALYSIS.md](PROJECT_ANALYSIS.md)

---

## 🚀 Production Checklist

- [ ] Test all 6 endpoints with curl/Postman
- [ ] Verify database connection
- [ ] Test validation (invalid year, type, etc.)
- [ ] Test filtering with multiple criteria
- [ ] Check error responses (404, 400, 500)
- [ ] Review logs during operations
- [ ] Verify CORS headers if needed
- [ ] Plan API authentication (JWT/API Key)
- [ ] Set up monitoring/logging
- [ ] Configure HTTPS for production

---

## 💡 Tips

1. **Use Postman** for easier testing with GUI
2. **Add `| jq` to curl** for pretty JSON: `curl ... | jq`
3. **Save credentials securely** in User Secrets
4. **Test filtering early** to verify partial matching works
5. **Check timestamps** in responses for data freshness
6. **Review logs** in console for debugging

---

**Status:** ✅ Ready to test and deploy

