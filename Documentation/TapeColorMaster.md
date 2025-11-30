# Tape Color Master Implementation

## Overview
This document describes the implementation of the Tape Color Master module, which allows users to manage tape colors used in the production process.

## Backend Implementation

### Entities
- **TapeColorMaster**: Main entity representing a tape color with properties:
  - Id (int): Primary key
  - TapeColor (string): Name of the tape color
  - CreatedAt (DateTime): Creation timestamp
  - UpdatedAt (DateTime?): Last update timestamp
  - IsActive (bool): Soft delete flag
  - CreatedBy (string): User who created the record
  - UpdatedBy (string): User who last updated the record

### DTOs
- **TapeColorResponseDto**: Response DTO for tape color data
- **CreateTapeColorRequestDto**: Request DTO for creating a new tape color
- **UpdateTapeColorRequestDto**: Request DTO for updating an existing tape color
- **TapeColorSearchRequestDto**: Request DTO for searching tape colors

### Services
- **ITapeColorService**: Interface defining the contract for tape color operations
- **TapeColorService**: Implementation of the tape color service with CRUD operations

### Controllers
- **TapeColorController**: REST API controller exposing endpoints for tape color management

### Database
- Migration file created to add the TapeColorMasters table to the database
- Index on TapeColor for performance

## Frontend Implementation

### Components
- **TapeColorManagement**: Main page displaying all tape colors in a table
- **TapeColorForm**: Form component for creating and editing tape colors

### Hooks
- **useTapeColors**: Hook for fetching all tape colors
- **useTapeColor**: Hook for fetching a specific tape color by ID
- **useSearchTapeColors**: Hook for searching tape colors
- **useCreateTapeColor**: Hook for creating a new tape color
- **useUpdateTapeColor**: Hook for updating an existing tape color
- **useDeleteTapeColor**: Hook for deleting a tape color

### API Client
- Added tapeColorApi functions for all CRUD operations

### Routing
- Added routes for tape color management:
  - `/tape-colors`: List all tape colors
  - `/tape-colors/create`: Create a new tape color
  - `/tape-colors/:id/edit`: Edit an existing tape color

### Navigation
- Added "Tape Color" to the Master section in the sidebar navigation

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/TapeColor` | Get all tape colors |
| GET | `/api/TapeColor/{id}` | Get a specific tape color by ID |
| POST | `/api/TapeColor` | Create a new tape color |
| PUT | `/api/TapeColor/{id}` | Update an existing tape color |
| DELETE | `/api/TapeColor/{id}` | Delete a tape color (soft delete) |
| GET | `/api/TapeColor/search` | Search tape colors by various criteria |

## Usage

### Creating a Tape Color
1. Navigate to Tape Color Management from the sidebar
2. Click "Add Tape Color"
3. Fill in the tape color details:
   - Tape Color Name (required)
4. Click "Create Tape Color"

### Editing a Tape Color
1. Navigate to Tape Color Management
2. Click the edit icon for the tape color you want to modify
3. Update the tape color details
4. Click "Update Tape Color"

### Deleting a Tape Color
1. Navigate to Tape Color Management
2. Click the delete icon for the tape color you want to delete
3. Confirm the deletion in the dialog

### Searching Tape Colors
1. Navigate to Tape Color Management
2. Use the search input to filter tape colors by tape color name

## Validation Rules
- Tape Color Name: Required, max 200 characters

## Error Handling
- Duplicate Tape Color Name: Returns 400 Bad Request with error message
- Invalid Data: Returns 400 Bad Request with validation errors
- Not Found: Returns 404 Not Found for invalid IDs
- Server Errors: Returns 500 Internal Server Error with generic message