## Sharingwith.me

**Links:** [LinkedIn](https://www.linkedin.com/in/micahahawinters/) · [Repository](https://github.com/Micahaha/sharingwith.me)

A Send Anywhere–style file sharing service built around short-lived, encrypted uploads with no accounts, no tracking, and no long-term storage.

Files are encrypted before storage. Access is granted through a temporary link or code, and content is deleted immediately after download or automatically after about six hours if never retrieved.

The system is designed to stay simple and disposable. It uses Azure Blob Storage for cheap, scalable storage while keeping encryption and lifecycle control entirely within the application layer.

This project exists to explore secure file handling, stateless backend design, automated cleanup, and privacy-first data lifecycles rather than replacing enterprise file transfer platforms.


## TODO / Roadmap

### Core
- [ ] Finalize encryption flow (key generation, storage, disposal)
- [ ] Add integrity checks (hash verification on download)
- [ ] Enforce single-use download links
- [ ] Configurable expiration time (default ~6 hours)
- [ ] Size limits per upload
- [ ] Better error handling for expired or already-downloaded files

### Storage & Cleanup
- [ ] Azure Blob lifecycle rules for auto-expiration
- [ ] Background worker for:
  - expired files
  - completed downloads
  - abandoned uploads
- [ ] Safe deletion verification (confirm blob removal)
- [ ] Cleanup retry logic for failed deletes

### Security Hardening
- [ ] Rate limiting on upload and download endpoints
- [ ] Brute-force protection on download codes
- [ ] Optional password-based encryption
- [ ] Time-safe comparisons for access codes
- [ ] Audit encryption and key-handling logic

### API & Backend
- [X] Streaming uploads/downloads (no full file buffering)
- [ ] Support resumable uploads
- [X] Improve API response consistency
- [ ] Add basic health checks
- [X] Logging (minimal, non-sensitive)

### UX / DX
- [X] Simple upload UI
- [ ] Drag-and-drop support
- [X] Progress indicators
- [ ] Clear expiration countdown for downloads
- [X] Copy-to-clipboard link/code

### Optional / Future Ideas
- [ ] Client-side encryption (browser-based)
- [X] Multi-file uploads (zip on the fly)
