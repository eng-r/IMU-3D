# Setting Up Angular for 3D Visualization (with Three.js)

This document explains, step by step, how to set up an Angular application for 3D visualization using **Three.js**. It is written with the IMU‑3D style project in mind, where 3D objects may later be driven by real‑time sensor data (e.g., an IMU).

---

## 1. Prerequisites

Before starting, install the basic tooling on Windows:

1. **Node.js (LTS)**  
   Download and install from: https://nodejs.org  
   During installation, make sure `npm` (Node Package Manager) is included.

NOTE: **Chocolatey** Node.js refers to the installation and management of Node.js on a Windows operating system using the Chocolatey package manager.
Chocolatey is a package manager for Windows, similar to apt-get on Linux or Homebrew on macOS. It allows users to easily install, update, and manage software packages from the command line.
Node.js is a JavaScript runtime environment that allows developers to run JavaScript code outside of a web browser. It is widely used for building server-side applications, command-line tools, and more. In essence, "Chocolatey Node.js" means using Chocolatey to efficiently install, update, and manage Node.js on a Windows machine.


2. **Angular CLI**  
   After Node.js is installed, open *PowerShell* or *Command Prompt*:

   ```bash
   npm install -g @angular/cli
   ```

3. Confirm the tools:

   ```bash
   node -v
   npm -v
   ng version
   ```

You should see version numbers printed without errors.

---

## 2. Create a New Angular Project

Choose a folder where you keep your projects, then run:

```bash
ng new imu-3d-frontend
```

Angular CLI will ask a few questions:

- **Add Angular routing?** – We can answer `Yes` as we can later define routes for different IMU 3D views.  

[Angular](https://v17.angular.io/guide/architecture-components) routing (managed by the Angular Router), is a mechanism that enables navigation between different views or components within an Angular application without requiring a full page reload. It allows for the creation of **single-page applications** (SPAs) by mapping specific URLs to corresponding components. A component is the basic building block of an application's user interface, while a view is the part of the screen that a component controls. A component is a TypeScript class that includes an HTML template for the view, the logic for the view's behavior, and optional CSS styles to control its appearance.

- **Stylesheet format?** – Choose what you like (`CSS`, `SCSS`, etc.). `SCSS` is often convenient for larger projects.

We are building a technical visualization UI (3D IMU model, rotating cube, telemetry panel), likely one main view, maybe later a few dashboard screens;
minimal UI complexity; we are not building a commercial UX-heavy app with complex theming. Given this purpose, the best choice is:
```Sass (SCSS)```

SCSS looks exactly like CSS, but with superpowers: SCSS is just CSS plus variables (e.g., primary-color), nesting, mixins (reusable patterns), functions.

Move into the project folder:

```bash
cd imu-3d-frontend
```

To check that everything runs:

```bash
ng serve
```

Then open a browser at:

```text
http://localhost:4200
```

You should see the default Angular welcome page.

---

## 3. Install Three.js (Core 3D Library)

Three.js will be the main library used for 3D rendering.

From the root of the Angular project (`imu-3d-frontend`):

```bash
npm install three
npm install --save-dev @types/three
```

- `three` is the runtime library.
- `@types/three` gives TypeScript type definitions so the Angular project understands Three.js types (e.g., `Scene`, `PerspectiveCamera`, `Mesh`).

---

## 4. Create a 3D Visualization Component

Create a dedicated Angular component to host the 3D scene, for example `imu-visualizer`:

```bash
ng generate component components/imu-visualizer
```

Angular will create files similar to:

```text
src/app/components/imu-visualizer/imu-visualizer.component.ts
src/app/components/imu-visualizer/imu-visualizer.component.html
src/app/components/imu-visualizer/imu-visualizer.component.scss
```

You will place the 3D canvas in the HTML file and the Three.js logic in the TypeScript file.

---

## 5. Add a Canvas Container to the Component Template

Edit:

```text
src/app/components/imu-visualizer/imu-visualizer.component.html
```

Replace its content with:

```html
<div class="imu-visualizer-container">
  <canvas #rendererCanvas></canvas>
</div>
```

Add some simple styles so the canvas occupies reasonable space:

```text
src/app/components/imu-visualizer/imu-visualizer.component.scss
```

```scss
.imu-visualizer-container {
  width: 100%;
  height: 100%;
  display: block;
  overflow: hidden;
}

canvas {
  width: 100%;
  height: 100%;
  display: block;
}
```

The `#rendererCanvas` template reference will be linked from the TypeScript code so Three.js can render into that canvas.

---

## 6. Basic Three.js Setup in Angular

Open:

```text
src/app/components/imu-visualizer/imu-visualizer.component.ts
```

Replace the content with a minimal Three.js scene setup:

```ts
import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  ViewChild
} from '@angular/core';

import * as THREE from 'three';

@Component({
  selector: 'app-imu-visualizer',
  templateUrl: './imu-visualizer.component.html',
  styleUrls: ['./imu-visualizer.component.scss']
})
export class ImuVisualizerComponent implements AfterViewInit, OnDestroy {

  @ViewChild('rendererCanvas', { static: false })
  rendererCanvas!: ElementRef<HTMLCanvasElement>;

  private renderer!: THREE.WebGLRenderer;
  private scene!: THREE.Scene;
  private camera!: THREE.PerspectiveCamera;
  private cube!: THREE.Mesh;

  private animationFrameId: number | null = null;

  ngAfterViewInit(): void {
    this.initScene();
    this.startRenderingLoop();
  }

  ngOnDestroy(): void {
    if (this.animationFrameId !== null) {
      cancelAnimationFrame(this.animationFrameId);
    }
    if (this.renderer) {
      this.renderer.dispose();
    }
  }

  private initScene(): void {
    const canvas = this.rendererCanvas.nativeElement;

    // Renderer
    this.renderer = new THREE.WebGLRenderer({ canvas, antialias: true });
    this.renderer.setPixelRatio(window.devicePixelRatio);
    this.renderer.setSize(canvas.clientWidth, canvas.clientHeight, false);

    // Scene
    this.scene = new THREE.Scene();
    this.scene.background = new THREE.Color(0x202020);

    // Camera
    const fov = 60;
    const aspect = canvas.clientWidth / canvas.clientHeight;
    const near = 0.1;
    const far = 1000;

    this.camera = new THREE.PerspectiveCamera(fov, aspect, near, far);
    this.camera.position.set(0, 0, 5);

    // Simple geometry: a cube as a placeholder for IMU orientation
    const geometry = new THREE.BoxGeometry(1, 1, 1);
    const material = new THREE.MeshStandardMaterial({ color: 0x00ff80 });
    this.cube = new THREE.Mesh(geometry, material);
    this.scene.add(this.cube);

    // Light
    const light = new THREE.DirectionalLight(0xffffff, 1);
    light.position.set(5, 5, 5);
    this.scene.add(light);

    // Handle window resizing
    window.addEventListener('resize', () => this.onWindowResize());
  }

  private startRenderingLoop(): void {
    const render = () => {
      // Rotate the cube as a simple demo
      this.cube.rotation.x += 0.01;
      this.cube.rotation.y += 0.01;

      this.renderer.render(this.scene, this.camera);

      this.animationFrameId = requestAnimationFrame(render);
    };

    render();
  }

  private onWindowResize(): void {
    if (!this.renderer || !this.camera || !this.rendererCanvas) {
      return;
    }

    const canvas = this.rendererCanvas.nativeElement;
    const width = canvas.clientWidth;
    const height = canvas.clientHeight;

    this.camera.aspect = width / height;
    this.camera.updateProjectionMatrix();

    this.renderer.setSize(width, height, false);
  }
}
```

What this code does:

- Creates a `WebGLRenderer` bound to the `<canvas>` element.
- Sets up a `Scene`, a `PerspectiveCamera`, and a simple cube mesh.
- Adds a directional light so the cube is visible with shading.
- Starts an animation loop using `requestAnimationFrame` that rotates the cube and renders each frame.
- Handles window resize events to keep the 3D view properly scaled.

Later, you can replace the cube’s static rotation with orientation updates derived from real IMU data (e.g., roll, pitch, yaw).

---

## 7. Add the Component to the Main App Template

Open:

```text
src/app/app.component.html
```

Replace its content with:

```html
<div class="app-container">
  <h1>IMU‑3D Angular Frontend</h1>
  <app-imu-visualizer></app-imu-visualizer>
</div>
```

You may also add some styles in:

```text
src/app/app.component.scss
```

```scss
.app-container {
  width: 100vw;
  height: 100vh;
  margin: 0;
  padding: 0;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

h1 {
  margin: 0.5rem;
  padding: 0;
  font-size: 1.2rem;
  color: #dddddd;
}

app-imu-visualizer {
  flex: 1;
}
```

This gives you a full‑window layout with a header and the 3D view filling the rest of the screen.

---

## 8. Run the Angular 3D Frontend

From the project root:

```bash
ng serve
```

Then open:

```text
http://localhost:4200
```

You should see the page with the title **“IMU‑3D Angular Frontend”** and a rotating 3D cube rendered via Three.js.

---

## 9. Connecting to a Backend Later (IMU Data)

Once your backend (e.g., .NET 8 Web API) provides an endpoint like:

```text
http://localhost:5000/api/imu/sample
```

you can:

1. Create an Angular service to call that endpoint via `HttpClient`.
2. Map IMU data (e.g., quaternion or Euler angles) to cube rotation:
   - `cube.rotation.x = pitch;`
   - `cube.rotation.y = yaw;`
   - `cube.rotation.z = roll;`

The same general structure remains:

- Angular controls lifecycle and UI.
- Three.js handles rendering and 3D math.
- The backend feeds real‑time or polled IMU data over HTTP (later possibly WebSockets).

---

## 10. Summary

You now have:

1. A working Angular project created via Angular CLI.  
2. Three.js installed and wired into an Angular component.  
3. A dedicated `ImuVisualizerComponent` that renders a simple 3D cube.  
4. A layout that can be extended into a full IMU‑3D visualization dashboard.  

From here, typical next steps are:

- Add services to fetch IMU data from the backend.  
- Replace the cube with a more realistic 3D model (e.g., device or coordinate frame).  
- Implement camera controls (orbit, pan, zoom) and UI overlays (plots, numeric data).  
- Add routing for different views (e.g., live view, playback, calibration).

This foundation is enough to start experimenting with 3D visualization for your IMU‑3D project in Angular.
