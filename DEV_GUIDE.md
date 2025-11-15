# Developer Guide

## Backend Structure
The backend follows a modular architecture. A background service manages the flow of orientation samples. A mode service maintains the active data source. The factory creates the N-based orientation providers.

### Key Components

### 1. ImuStreamingService
Runs continuously. Requests the current mode and forwards IMU samples to the SignalR hub.

### 2. ImuModeService
Stores the active mode and optional playback session.

### 3. ImuSourceFactory
Produces:
- Spiral simulator
- Circular simulator
- USB source
- Database playback source

### 4. Entity Framework Core
Uses the Pomelo provider for MariaDB.

## Frontend Structure
The Angular application consists of:
- A Three.js component for rendering the prism.
- A service for handling SignalR events.
- A REST client for mode changes.

## Build Instructions
Ensure that Node.js, Angular CLI, .NET 8 SDK, and MariaDB are installed. Restore dependencies for both frontend and backend, then run each subsystem independently.

## Coding Conventions
- Use dependency injection throughout backend services.
- Maintain separation between configuration (REST) and data streaming (SignalR).
- Store orientation sampling logic inside IMU source classes.
