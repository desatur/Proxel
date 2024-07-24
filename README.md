# Proxel
 Another high performance Minecraft Proxy written in .NET, this is a faster and more lightweight replacement for Waterfall/Velocity.
 <a href="https://star-history.com/#kohakow/Proxel&Date">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://api.star-history.com/svg?repos=kohakow/Proxel&type=Date&theme=dark" />
    <source media="(prefers-color-scheme: light)" srcset="https://api.star-history.com/svg?repos=kohakow/Proxel&type=Date" />
    <img alt="Star History Chart" src="https://api.star-history.com/svg?repos=kohakow/Proxel&type=Date" />
  </picture>
 </a>
## TODO/Features
- [x] Work in progress.
- [ ] Lightweight
  - [x] Use all CPU resources
    - [x] Async
    - [ ] SIMD
      - [ ] ParallelMath
      - [ ] Mathf
    - [ ] Multithreaded
      - [ ] Use ParallelFor when possible
      - [ ] Start methods in new thread when possible
  - [x] Low RAM usage
  - [ ] Ahead Of Time (AOT) Compilation
- [ ] Packet compression
  - [ ] Zlib
  - [ ] Set Compression packet (S→C)
- [ ] Handshaking
  - [x] Handshake (C→S)
  - [ ] Status (Next State 1)
  - [ ] Login (Next State 2)
  - [ ] Transfer (Next State 3)
- [ ] Login Start (C→S)
- [ ] Encryption Request (S→C)
- [ ] Encryption Response (C→S)
- [ ] Login Success (S→C)
- [ ] Login Acknowledged (C→S)
- [ ] Online mode
  - [ ] Auth
  - [ ] Skins
  - [ ] Capes
  - [ ] UUID transfers between servers
  - [ ] SessionID transfers between servers
  - [ ] Cosmetics transfers between servers
- [ ] Addon API
  - [ ] Dependency injection (AOT ?)
