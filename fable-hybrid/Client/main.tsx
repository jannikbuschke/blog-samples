import { createRoot } from "react-dom/client";
import { App } from "./app";

// Clear the existing HTML content
// document.body.innerHTML = '<div id="app"></div>';

// Render your React component instead
const r = document.getElementById("root");
const root = createRoot(r!);
root.render(<App />);
