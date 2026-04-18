# apptest-hris

C# demo app for an HRIS focused on **Recruitment, Selection & Placement (RSP)** — Blazor WebAssembly client, ASP.NET Core API, shared DTOs, and mock data for narrative demos.

**Repository:** [https://github.com/deeptux/apptest-hris](https://github.com/deeptux/apptest-hris)

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download) — version is hinted by [`global.json`](global.json) (`rollForward` allows newer patches).

## Run locally

1. **API** (serves REST + CORS for the client):

   ```bash
   dotnet run --project src/Hris.Demo.Api
   ```

   Default HTTPS URL is shown in the console (often `https://localhost:7209`).

2. **Client** (Blazor WASM):

   ```bash
   dotnet run --project src/Hris.Demo.Client
   ```

3. Ensure the client’s [`src/Hris.Demo.Client/wwwroot/appsettings.json`](src/Hris.Demo.Client/wwwroot/appsettings.json) `ApiBaseUrl` matches the API URL (see environment-specific files below).

4. Optional AI job description features: set `Ai:Gemini:ApiKey` via **user secrets** (local) or **environment variables** on the host (e.g. `Ai__Gemini__ApiKey` on Render). **Never commit real keys** in tracked `appsettings` files. See API [`AiController`](src/Hris.Demo.Api/Controllers/AiController.cs). Extra notes may live under local **`docs/`** (not published; e.g. `implementation-ai-job-description.md`).

### Client configuration layers

At runtime the WASM host loads, in order:

1. `wwwroot/appsettings.json` — defaults (e.g. local API URL for development).
2. `wwwroot/appsettings.{Environment}.json` if present — e.g. [`appsettings.Production.json`](src/Hris.Demo.Client/wwwroot/appsettings.Production.json) after `dotnet publish` (Production) so the deployed site points at your public API HTTPS URL.

Replace the placeholder in `appsettings.Production.json` before publishing to production, or generate that file in CI with the real API URL.

**Simulated role & storage:** The header **Simulated role** (HR / Approver / Hiring manager / **Applicant**) is **in-memory** only (refresh resets). **Applicant** mode shows a **single pinned demo persona** (Ana Reyes — see [`ApplicantDemoPersona`](src/Hris.Demo.Client/Services/ApplicantDemoPersona.cs) and API [`MockRspStore`](src/Hris.Demo.Api/Services/MockRspStore.cs)). If you later persist UI prefs in **`localStorage`**, remember values are **per-origin** (`github.io` vs `handrian.space` do not share).

## Deployment (GitHub Pages + Render)

**Full checklist:** [`DEPLOYMENT.md`](DEPLOYMENT.md).

| Item | Notes |
|------|--------|
| **Client static hosting** | GitHub Pages via workflow [`.github/workflows/deploy-client-pages.yml`](.github/workflows/deploy-client-pages.yml). Every push to `main` publishes the Blazor WASM client. Optional: **Cloudflare** (or similar) can proxy a custom host to the same static output. |
| **Canonical public demo URL** | **`https://handrian.space/apptest-hris/`** (custom domain + path). |
| **Upstream / alternate (GitHub Pages)** | `https://deeptux.github.io/apptest-hris/` — same app base path **`/apptest-hris/`**; one production build works for both when infra matches. |
| **Project-repo base path** | Production uses base href **`/apptest-hris/`** (not `/`). The workflow rewrites published `index.html` accordingly; local `dotnet run` stays at `/`. |
| **API Web Service** | Use **Docker** (not Node). **Dockerfile path:** `./Dockerfile` at repo root. Leave Render **build command** empty — the image build runs `dotnet publish` inside Docker. See [`DEPLOYMENT.md`](DEPLOYMENT.md) §1. |
| **PORT** | Render sets **`PORT`**; the API binds **`http://0.0.0.0:{PORT}`** when it is set ([`Program.cs`](src/Hris.Demo.Api/Program.cs)). |
| **.NET SDK** | For **local** / CI builds, use SDK **9** ([`global.json`](global.json)). The Docker image brings its own SDK for the API service. |
| **API secrets** | Set `Ai__Gemini__ApiKey` (and any other secrets) in the **API** web service environment on Render, not in git. |
| **CORS** | Set **`CORS_ORIGINS`** on the API to every **browser origin** that serves the WASM UI, comma-separated, **origin only** (no path). Example when both hosts are used: `https://deeptux.github.io,https://handrian.space`. |
| **Client → API URL** | Production uses `wwwroot/appsettings.Production.json` with `ApiBaseUrl: https://apptest-hris.onrender.com` (also enforced in the Pages workflow publish output). The browser calls the API **directly** (cross-origin); do not put the API behind an extra Worker for this demo unless you intentionally change that model. |

**Deployed URLs:**

| Service | URL |
|---------|---------------------|
| Blazor static site (canonical) | `https://handrian.space/apptest-hris/` |
| Blazor static site (GitHub Pages upstream) | `https://deeptux.github.io/apptest-hris/` |
| ASP.NET Core API (Render) | `https://apptest-hris.onrender.com` |

**Smoke test:** Open the static site over HTTPS; in browser DevTools → Network, confirm API calls succeed with no CORS errors. Switch **Simulated role** to **Applicant** → should land on **`/applicant`** with applicant-only nav; switch back to **HR** → **`/`** and full HRIS nav.

## Solution layout

| Path | Role |
|------|------|
| `src/Hris.Demo.Api` | ASP.NET Core Web API |
| `src/Hris.Demo.Client` | Blazor WebAssembly PWA |
| `src/Hris.Demo.Shared` | Shared models/DTOs |
| `docs/` | **Not in git** — local canon, progress log, AI notes (see `.gitignore`). Public deploy checklist: [`DEPLOYMENT.md`](DEPLOYMENT.md). |
| `references/` | **Not in git** — local reference files only (see `.gitignore`) |
| `global.json` | Pins **.NET 9** SDK for local/CI |
| `Dockerfile` | Multi-stage build for **`Hris.Demo.Api`** (Render Web Service **Docker** runtime) |
| `.dockerignore` | Keeps Docker build context small (excludes Client, `bin/`, `obj/`, etc.) |
| `.env.example` | Documents env var names (secrets belong in host config, not committed `.env`) |

## License

This project is licensed under the **GNU General Public License v3.0** — see [`LICENSE`](LICENSE) on the default branch.
