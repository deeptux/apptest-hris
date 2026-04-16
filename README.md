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

## Deployment (Render)

**Full checklist:** [`DEPLOYMENT.md`](DEPLOYMENT.md).

| Item | Notes |
|------|--------|
| **API Web Service** | Use **Docker** (not Node). **Dockerfile path:** `./Dockerfile` at repo root. Leave Render **build command** empty — the image build runs `dotnet publish` inside Docker. See [`DEPLOYMENT.md`](DEPLOYMENT.md) §0. |
| **PORT** | Render sets **`PORT`**; the API binds **`http://0.0.0.0:{PORT}`** when it is set ([`Program.cs`](src/Hris.Demo.Api/Program.cs)). |
| **.NET SDK** | For **local** / CI builds, use SDK **9** ([`global.json`](global.json)). The Docker image brings its own SDK for the API service. |
| **API secrets** | Set `Ai__Gemini__ApiKey` (and any other secrets) in the **API** web service environment on Render, not in git. |
| **CORS** | Set **`CORS_ORIGINS`** on the API to your Blazor static site origin(s), comma-separated, e.g. `https://your-client.onrender.com`. Overrides `Cors:Origins` in appsettings when set. |
| **Client → API URL** | Set `ApiBaseUrl` in `wwwroot/appsettings.Production.json` to your API public HTTPS URL before/during static site build. |

**Deployed URLs (fill in after first deploy; replace placeholders in this table):**

| Service | URL (placeholder) |
|---------|---------------------|
| Blazor static site | `https://YOUR-CLIENT.onrender.com` |
| ASP.NET Core API | `https://YOUR-API.onrender.com` |

**Smoke test:** Open the static site over HTTPS; in browser DevTools → Network, confirm API calls succeed with no CORS errors.

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
