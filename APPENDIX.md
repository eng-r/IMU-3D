# Architecture Diagrams Appendix

## System Architecture Overview

```
Angular (Three.js) <--SignalR-- Backend Hub
      ^                 |
      |  REST           v
      +----> ModeController -> ModeService
                             |
                             v
                     ImuSourceFactory
                             |
         ---------------------------------------------
         |                 |               |         |
   SpiralSim      CircularSim       UsbSource     Playback
         ---------------------------------------------
                             |
                             v
                     ImuStreamingService
                             |
                             v
                         SignalR Hub
                             |
                             v
                       Angular Viewer
```

## Data Flow Summary

### Real-time IMU (Spiral/Circular/USB)
```
Source → StreamingService → Hub → Angular
```

### Playback
```
MariaDB → PlaybackSource → StreamingService → Hub → Angular
```

## Database Schema Summary

### ImuSession
- Id
- Name
- StartedAt

### ImuSample
- Id
- Timestamp
- Yaw Pitch Roll
- Optional accelerometer and gyroscope values
```
