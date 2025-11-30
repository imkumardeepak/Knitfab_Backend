# Dispatch Planning System

## Overview
The Dispatch Planning system manages the dispatch process for finished goods, including automatic LoadingNo generation.

## Features
- Automatic LoadingNo generation (LOAD{YY}{MM}{SERIAL})
- Dispatch planning and tracking
- Roll dispatch management
- Integration with existing storage capture system

## Getting Started

### Backend Setup
1. Ensure database migrations are applied
2. The system automatically creates LoadingNo when new dispatch planning records are created
3. Use the `/api/DispatchPlanning` endpoints to manage dispatch planning records

### Frontend Usage
1. The Dispatch Planning page displays all dispatch planning records with their LoadingNo
2. The Dispatch Details page shows LoadingNo for individual lots
3. LoadingNo is automatically generated when creating new dispatch planning records

## Example Usage

### Creating a New Dispatch Planning Record
```typescript
// Frontend example
const newDispatchPlanning = {
  lotNo: "LOT001",
  salesOrderId: 123,
  salesOrderItemId: 456,
  customerName: "ABC Customer",
  tape: "Tape Type A",
  totalRequiredRolls: 100,
  totalReadyRolls: 100,
  totalDispatchedRolls: 0,
  isFullyDispatched: false,
  vehicleNo: "TRUCK001",
  driverName: "John Doe",
  license: "DL12345",
  mobileNumber: "9876543210",
  remarks: "First dispatch"
  // LoadingNo will be auto-generated
};

// The API will automatically generate a LoadingNo like "LOAD25100001"
const response = await dispatchPlanningApi.createDispatchPlanning(newDispatchPlanning);
console.log(response.data.loadingNo); // Outputs: LOAD25100001
```

### LoadingNo Generation Logic
The LoadingNo is generated in the backend using the following format:
- **LOAD**: Fixed prefix
- **YY**: 2-digit current year (25 for 2025)
- **MM**: 2-digit current month (10 for October)
- **SERIAL**: 4-digit auto-incremented number (0001, 0002, etc.)

### Example Loading Numbers
1. First dispatch in October 2025: `LOAD25100001`
2. Second dispatch in October 2025: `LOAD25100002`
3. First dispatch in November 2025: `LOAD25110001`

## API Endpoints

### Dispatch Planning
- `GET /api/DispatchPlanning` - Get all dispatch plannings
- `GET /api/DispatchPlanning/{id}` - Get dispatch planning by ID
- `POST /api/DispatchPlanning` - Create a new dispatch planning (auto-generates LoadingNo)
- `PUT /api/DispatchPlanning/{id}` - Update a dispatch planning
- `DELETE /api/DispatchPlanning/{id}` - Delete a dispatch planning

### Dispatched Rolls
- `GET /api/DispatchPlanning/{id}/dispatched-rolls` - Get dispatched rolls by planning ID
- `POST /api/DispatchPlanning/dispatched-rolls` - Create a new dispatched roll

## Database Structure
The system uses two main tables:
1. `DispatchPlannings` - Stores dispatch planning records with LoadingNo
2. `DispatchedRolls` - Stores individual roll dispatch information

## LoadingNo Uniqueness
Each LoadingNo is unique within the system and follows a predictable sequence based on the year and month of creation.