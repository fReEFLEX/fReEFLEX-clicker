# fReEFLEX Clicker
[![License: Apache License 2.0](https://img.shields.io/badge/License-Apache%20License%202.0-yellow.svg)](LICENSE)

> fReEFLEX Clicker application. Measure your input latency with defined frametimes.

![](doc/window.png?raw=true "Clicker Window")

## 📁 Download
- [releases](https://github.com/fReEFLEX/fReEFLEX-clicker/releases)

## Contents
- [Download](#-download)
- [What is fReEFLEX?](#what-is-freeflex)
- [Usage](#usage)
    - [Serial Port](#serial-port)
- [Support fReEFLEX project](#-support-freeflex-project)
- [License](#-license)

## What is fReEFLEX?
fReEFLEX helps you to 
- Find the best settings for low input latency in 3D Games
- Optimize your OS for low input latency
- Measure processing delay of mice
- Measure the frequency of pulsed light sources, e.g. backlight strobing, dimmed LEDs.

The fReEFLEX project includes
- [Controller firmware](https://github.com/fReEFLEX/fReEFLEX-controller/releases) - copy this on your Raspberry Pi Pico
- [GUI](https://github.com/fReEFLEX/fReEFLEX-GUI/) - to operate the controller
- [3D Application](https://github.com/fReEFLEX/fReEFLEX-clicker/) - 3D application for E2E latency measurement 

## Usage
Download and unpack latest [release](https://github.com/fReEFLEX/fReEFLEX-clicker/releases). 
Start the DirectX or OpenGL clicker and start clicking anywhere in the window to produce a bright flash. 
>Running this application in windowed mode can add a multiple frame buffer and thus increase the latency.
>This is not true for every 3D game, e.g. windowed Unreal Engine might produce latencies similar to fullscreen. 
>Every game behaves differently when switching between windowed, borderless and fullscreen modes.
   
- `+` | `arrow up`: increase FPS by 10 
- `-` | `arrow down`: decrease max FPS by 10 
- `F`: toggle fullscreen
- `C`: toggle colors between black/white and grey
- `P`: open [Serial Port](#serial-port)

Read the [GUI Documentation](https://github.com/fReEFLEX/fReEFLEX-GUI/) to learn more about how to use this tool.

### Serial Port 
The Clicker application can establish a connection to the [fReEFLEX Controller](https://github.com/fReEFLEX/fReEFLEX-controller/) via serial port. This improves the results when measuring system latency.

## ☕ Support fReEFLEX project

[PayPal](https://paypal.me/Pmant)

## 👤 Author

[@Pmant](https://github.com/Pmant)

## 📝 License

Copyright © 2021 [Pmant](https://github.com/Pmant).

This project is [Apache License 2.0](LICENSE) licensed.


