# Architecture Explanation: Domain, Infrastructure, and Naming Conventions

This document explains why the backend is structured into specific folders, why certain files exist, and the reasoning behind naming patterns such as `IImuSampleProvider`. The aim is to keep the system readable and maintainable while supporting future growth, including real hardware IMU integration, simulation modes, and database playback.

## 1. Why these folders exist

A well-organized backend helps keep unrelated responsibilities separated. Even though the initial step reads a single IMU sample from a JSON file, the long-term system will handle live USB IMU devices, simulated motion, and stored-session playback. A clean structure avoids large refactoring later.

The folders serve these roles:

```
Domain/         – Core concepts of the application (IMU sample)
Infrastructure/ – Implementations tied to external systems (JSON, USB, DB, etc.)
Controllers/    – Web API endpoints
```

## 2. What the Domain layer represents

The **Domain** layer holds the fundamental concepts of the system. These concepts should not depend on framework details, file systems, databases, or device drivers. They express the problem, not the implementation.

### ImuSample

A simple class that represents a single IMU reading: timestamp, yaw, pitch, and roll. It contains no serialization code, no USB logic, and no database logic. It reflects the real-world concept cleanly.

### IImuSampleProvider

An interface that communicates one requirement:

> “Something in the system must be capable of providing an IMU sample.”

It does not care where the data comes from. This makes it possible to plug in many different providers without changing the rest of the system.

## 3. Why the interface name starts with “I”

C# uses a long-standing convention: interfaces begin with **I**.

- `IImuSampleProvider` is an interface (abstraction)
- `JsonFileImuSampleProvider` is a class (implementation)

This pattern communicates intent clearly and allows the rest of the codebase to depend on abstractions rather than concrete types. It improves testability and supports dependency injection.

## 4. Why “sample” and “sample provider” are separate

These two concepts serve different roles:

### ImuSample
A structure holding data.  
It answers: *“What is an IMU reading?”*

### IImuSampleProvider
A description of behavior.  
It answers: *“How do I obtain an IMU reading?”*

### JsonFileImuSampleProvider
A concrete implementation.  
It answers: *“I obtain an IMU reading by reading a JSON file from disk.”*

Later, other implementations can be added:

- USB provider
- MariaDB playback provider
- simulated spiral motion provider
- simulated circular motion provider

The rest of the system stays unchanged.

## 5. What Infrastructure means

**Infrastructure** contains any component tightly coupled to external dependencies:

- JSON files  
- XML files  
- USB or serial ports  
- MariaDB via EF Core  
- network clients  
- configuration providers  

These pieces change frequently and should not pollute the core domain.

The provider that reads JSON from disk belongs here because it handles I/O, deserialization, and file paths.

## 6. Benefits of this structure

This structure supports future growth:

- adding new IMU input sources  
- recording and playing back sessions  
- integrating real USB IMU hardware  
- supporting multiple IMU types  
- extending the backend without breaking existing code  
- enabling front end features without architecture changes  

A small amount of structure now avoids obstacles later.

## Summary

| Concept | Purpose | Example |
|--------|---------|---------|
| Domain | Defines core data and behavior | `ImuSample`, `IImuSampleProvider` |
| Infrastructure | Actual implementations | `JsonFileImuSampleProvider` |
| Controllers | API surface to outside world | `ImuController` |
| “I” prefix | Interface naming convention | `IImuSampleProvider` |

This organization ensures that the system remains clear, flexible, and ready for more complex features.
