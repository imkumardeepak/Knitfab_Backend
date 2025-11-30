# LoadingNo Feature Documentation

## Overview
The LoadingNo feature automatically generates unique loading numbers for dispatch planning records following the format: `LOAD{YY}{MM}{SERIAL}`

## Format Details
- **LOAD**: Fixed prefix
- **YY**: 2-digit year (e.g., 25 for 2025)
- **MM**: 2-digit month (e.g., 10 for October)
- **SERIAL**: 4-digit serial number starting from 0001, auto-incremented

### Example Loading Numbers
- LOAD25100001 (First loading in October 2025)
- LOAD25100002 (Second loading in October 2025)
- LOAD25110001 (First loading in November 2025)

## Implementation Details

### Backend
1. **Database**: The `DispatchPlannings` table includes a `LoadingNo` column (VARCHAR(20))
2. **Model**: `DispatchPlanning` entity includes the `LoadingNo` property
3. **Repository**: `DispatchPlanningRepository` contains the `GenerateLoadingNoAsync()` method
4. **Service**: `DispatchPlanningService` uses the repository to generate LoadingNo during creation
5. **Controller**: `DispatchPlanningController` exposes API endpoints for dispatch planning operations

### Frontend
1. **DispatchPlanning Page**: Displays LoadingNo in the table view
2. **DispatchDetails Page**: Displays LoadingNo in the table view
3. **API Client**: `dispatchPlanningApi` provides methods to interact with backend endpoints

## Auto-generation Logic
The LoadingNo is generated using the following logic:
1. Get current year and month in YYMM format
2. Find the highest serial number for the current YYMM period
3. Increment the serial number by 1
4. Format as 4-digit number with leading zeros (0001, 0002, etc.)

## API Endpoints
- `GET /api/DispatchPlanning` - Get all dispatch plannings
- `GET /api/DispatchPlanning/{id}` - Get dispatch planning by ID
- `POST /api/DispatchPlanning` - Create a new dispatch planning (auto-generates LoadingNo)
- `PUT /api/DispatchPlanning/{id}` - Update a dispatch planning
- `DELETE /api/DispatchPlanning/{id}` - Delete a dispatch planning
- `GET /api/DispatchPlanning/{id}/dispatched-rolls` - Get dispatched rolls by planning ID
- `POST /api/DispatchPlanning/dispatched-rolls` - Create a new dispatched roll

## Database Schema
```sql
CREATE TABLE DispatchPlannings (
    Id INTEGER PRIMARY KEY,
    LotNo VARCHAR(100) NOT NULL,
    SalesOrderId INTEGER NOT NULL,
    SalesOrderItemId INTEGER NOT NULL,
    CustomerName VARCHAR(200) NOT NULL,
    Tape VARCHAR(100) NOT NULL,
    TotalRequiredRolls DECIMAL(18,3) NOT NULL,
    TotalReadyRolls DECIMAL(18,3) NOT NULL,
    TotalDispatchedRolls DECIMAL(18,3) NOT NULL,
    IsFullyDispatched BOOLEAN NOT NULL,
    DispatchStartDate TIMESTAMP,
    DispatchEndDate TIMESTAMP,
    VehicleNo VARCHAR(100) NOT NULL,
    DriverName VARCHAR(100) NOT NULL,
    License VARCHAR(50) NOT NULL,
    MobileNumber VARCHAR(20) NOT NULL,
    Remarks TEXT NOT NULL,
    LoadingNo VARCHAR(20) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL,
    UpdatedAt TIMESTAMP,
    IsActive BOOLEAN NOT NULL
);
```