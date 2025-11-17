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

The diagram becomes clearer once we stop thinking of Three.js as a “front-end library” and instead view it as the *rendering engine* inside a larger, layered, real-time system. Angular is the application shell: it manages state, routing, dependency injection, component structure, and the UI lifecycle. Three.js is the visualization subsystem embedded within it, responsible only for turning incoming IMU data into a continuously updated 3D representation. Once that separation is understood, the architecture reads not as a stack of unrelated technologies but as a coherent flow of information from physical or simulated sources, through domain-level orchestration, into a reactive visualization layer.

At the heart of this architecture sits the Backend Hub, which exposes two distinct but complementary communication patterns. One is a standard REST interface that Angular uses for configuration-level actions, such as switching simulation modes or selecting which IMU source to activate. These interactions are transactional, infrequent, and deterministic. The other channel is a SignalR stream, which carries the high-frequency, low-latency IMU packets required for smooth three-dimensional visualization. REST configures the system; SignalR drives it.

The flow begins on the backend with a ModeController, the entry point for REST-based decisions. When Angular instructs the system to choose a mode—simulation, playback, USB device—the ModeController delegates that request to a ModeService, which encapsulates the business logic associated with validating, interpreting, and shifting between operating states. The ModeService then consults the ImuSourceFactory, a factory responsible for instantiating the correct IMU source according to the selected mode. This design ensures that the rest of the system remains agnostic to whether the data originates from a simulated environment, a real USB device, or a recorded file.

The ImuSourceFactory can produce four types of sources. SpiralSim and CircularSim generate mathematically parameterized IMU patterns, useful for testing without hardware. UsbSource taps into a physical IMU device, converting raw measurements into normalized orientation and acceleration packets. Playback replays historical sequences, allowing offline experimentation or regression testing. All four sources conform to the same interface, which is essential for polymorphism at scale.

Regardless of the source, IMU packets flow into the ImuStreamingService. This service performs aggregation, rate control, and packaging. It hides the heterogeneity of origins—simulated, live, or recorded—and emits a unified, time-aligned stream of IMU frames. These frames are forwarded to the SignalR Hub, which is the real-time boundary between backend and frontend. The hub pushes packets to all subscribed clients with minimal overhead, ensuring deterministic update rates in the viewer.

On the front end, Angular receives these streamed packets while simultaneously orchestrating UI state. Angular components interpret the IMU frames and pass only the minimal orientation and vector data to the Three.js scene. Three.js then serves as the rendering core, applying quaternion rotations, interpolating frames, and updating the 3D model so that the viewer sees a continuous and accurate representation of motion. Angular does not render the scene; it controls the lifecycle and the data flow. Three.js does not manage the application; it draws the world.

Interpreted this way, the diagram describes a closed control and visualization loop: configuration commands flow downward through REST to ModeController and ultimately to the factory that selects data sources; high-frequency IMU data flows upward through ImuStreamingService and SignalR into Angular; and visualization updates occur continuously within the Three.js renderer. The architecture is not a collection of libraries but a coordinated system in which each layer plays a distinct role in maintaining determinism, modularity, and extensibility.

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
