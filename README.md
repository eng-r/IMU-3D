# IMU 3D Visualization System

This document describes the structure and operation of a system designed to visualize the orientation of a rectangular prism in three dimensions. Orientation updates originate either from a USB‑connected inertial measurement unit or from one of several simulated data sources. A backend service distributes these orientation samples to an Angular front end, which renders the prism using a Three.js scene. The same backend can replay recorded waveforms stored in a MariaDB database. The purpose of this document is to explain how the different parts of the system interact.

## Overview

The architecture is built around three main areas: the Angular front end, the .NET backend, and the MariaDB database. The front end displays the three‑dimensional prism and subscribes to real‑time orientation updates. The backend is responsible for generating or retrieving those updates and sending them to the front end through an event‑driven channel. The database stores sessions of orientation samples that can be replayed at a later time.

Two communication paths connect the front end and the backend. A REST interface is used for configuration tasks, such as changing the data source. A SignalR channel is used for continuous transmission of orientation samples. This keeps configuration separate from the high‑frequency data stream.

## Front End

The front end is written in Angular and uses Three.js to render a rectangular prism whose orientation is updated in real time. It maintains a SignalR connection to receive orientation samples from the backend. Each sample contains yaw, pitch, and roll values. These values are converted into a quaternion and applied to the prism mesh in the Three.js scene.

Users may select one of several data sources. When a user switches sources, the front end sends a REST request to the backend to update the active mode. The front end then continues receiving samples through the SignalR channel without further intervention.

## Backend

The backend is implemented in ASP.NET Core. A background service runs continuously, choosing the correct IMU source based on the current configuration and streaming the resulting samples to the SignalR hub. The configuration is stored in a small service that can be updated through a REST controller. When the configuration changes, the background service will adopt the new mode the next time it completes an iteration.

### IMU Sources

The system supports several sources of orientation data:

* Spiral simulator: produces a gradual multi‑axis rotation.
* Circular simulator: produces a periodic rotation with a consistent angular profile.
* USB source: intended for a physical IMU connected through a serial or USB interface.
* Database playback: reads previously recorded samples from the database and reproduces them using their original timing.

Each source produces orientation samples through a channel that the background service reads. This allows new sources to be added without modifying the rest of the pipeline.

## Database

MariaDB stores recorded IMU sessions. Each session contains many time‑stamped orientation samples. The schema uses one table for sessions and another table for the samples themselves.

When the system is placed in playback mode, a dedicated IMU source retrieves samples from the database, sorts them by time, and streams them to the background service while preserving their timing. This makes it possible to reproduce the original session as it unfolded.

## Data Flow Summary

Real‑time data passes directly through the system: the IMU source produces a sample, the background service forwards it to the SignalR hub, and the front end receives it immediately. Playback follows the same path, except that the samples originate in the database. Configuration travels in the opposite direction: the front end sends a REST request to the backend, which updates the configuration service that the background loop reads.

This system is intended to remain understandable while supporting real‑time visualization, multiple data sources, and session playback.

