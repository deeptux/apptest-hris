# Render deployment — checklist

Use this with the root [`README.md`](README.md) **Deployment (Render)** section.

## 0) API Web Service — Docker on Render

Use a **Web Service** with **Docker** (not **Node** or a blank shell where `dotnet` is missing). The repo root [`Dockerfile`](Dockerfile) builds and runs **`Hris.Demo.Api`**.

| Render field | Value |
|----------------|--------|
| **Environment** | **Docker** |
| **Dockerfile path** | `./Dockerfile` (repo root) |
| **Root directory** | `.` (repository root) |
| **Branch** | `main` (or your deploy branch) |
| **Auto-deploy** | On — deploy when you push to that branch |
| **Build command** | Leave **empty** — the Dockerfile runs `dotnet restore` / `dotnet publish` inside the image. Do not set a Node/native build that runs `dotnet` on Render’s host. |
| **`PORT`** | Set automatically by Render; the API listens on **`http://0.0.0.0:{PORT}`** when `PORT` is present ([`Program.cs`](src/Hris.Demo.Api/Program.cs)). |

Still set **`CORS_ORIGINS`**, secrets (`Ai__Gemini__ApiKey`, etc.), and configure the Blazor client **`ApiBaseUrl`** as in the sections below.

**Local Docker check** (requires [Docker Desktop](https://www.docker.com/products/docker-desktop/) or Engine):

```bash
docker build -t hris-api-test .
docker run --rm -e PORT=10000 -p 10000:10000 hris-api-test
curl -sS http://localhost:10000/api/Branding
```

You should see JSON branding (HTTP 200). If you get a redirect, try `curl -L`.

## 1) Secrets

- Do **not** commit `Ai:Gemini:ApiKey`, passwords, or tokens in:
  - `src/Hris.Demo.Api/appsettings.json`
  - `src/Hris.Demo.Api/appsettings.*.json` (tracked files)
  - `src/Hris.Demo.Client/wwwroot/appsettings.json`
- On **Render**, set API secrets as environment variables (e.g. `Ai__Gemini__ApiKey`).
- **Local:** `dotnet user-secrets` for the API project only.

## 2) Client `ApiBaseUrl` (production)

- Published Blazor WASM loads `wwwroot/appsettings.json`, then `wwwroot/appsettings.{Environment}.json` (see [`Program.cs`](src/Hris.Demo.Client/Program.cs)).
- For **Production** builds, [`appsettings.Production.json`](src/Hris.Demo.Client/wwwroot/appsettings.Production.json) should contain the **public HTTPS URL** of the API (no trailing slash required; the app normalizes it).
- Replace the placeholder `https://YOUR-API-SERVICE.onrender.com` with your real API service URL **before** or **during** CI/publish (e.g. `sed`, script, or manual edit).
- Document the final URLs in [`README.md`](README.md) (deployment table).

## 3) API CORS

- After the static site URL is known (e.g. `https://<client>.onrender.com`), allow it on the API.
- Prefer the **`CORS_ORIGINS`** environment variable on Render: comma-separated list, no spaces (or trim handled). Example:
  - `CORS_ORIGINS=https://apptest-hris-client.onrender.com`
- If `CORS_ORIGINS` is unset, the API falls back to `Cors:Origins` in [`appsettings.json`](src/Hris.Demo.Api/appsettings.json) (localhost for dev).

## 4) `.gitignore`

- Confirm `bin/`, `obj/`, `/references/`, `/docs/`, and secret patterns remain as in [`.gitignore`](.gitignore). Do not commit build output.

## 5) SDK / build

- **Docker (API on Render):** The [`Dockerfile`](Dockerfile) uses **`mcr.microsoft.com/dotnet/sdk:9.0`** — no separate Render “.NET version” pick is needed for that service.
- **Non-Docker builds:** Use **.NET 9** locally or in CI ([`global.json`](global.json)).

## 6) Smoke test (after deploy)

1. Open the **static site** over **HTTPS**.
2. Browser **DevTools → Network**: load dashboard / queues; confirm API calls return **200** (not CORS errors).
3. If CORS fails, verify `CORS_ORIGINS` matches the static site origin exactly (scheme + host, no path).

## Appendix: Purging `docs/` from all Git history (optional)

The **`docs/`** folder is **local-only** (see `.gitignore`). A normal `git rm -r --cached docs/` only removes it from **new** commits; to remove it from **every** past commit:

1. Install [`git-filter-repo`](https://github.com/newren/git-filter-repo) (e.g. `pip install git-filter-repo`).
2. Commit or stash any work; working tree clean.
3. From repo root:
   ```bash
   git filter-repo --path docs/ --invert-paths --force
   ```
   `git filter-repo` **removes the `origin` remote** — re-add it:
   ```bash
   git remote add origin https://github.com/deeptux/apptest-hris.git
   git push --force origin main
   ```
4. Your local **`docs/`** files are unchanged if they were already gitignored; only history is rewritten.

Coordinate with collaborators before force-pushing; they must re-clone or reset to the new `main`.

### Single-commit `main` (fresh public history)

The public repo may use **one** root commit (orphan branch + `git add -A` + commit) so GitHub shows a clean “initial import” only. That **does not delete your code** — it only drops **older Git commits** (messages, bisect history). Local-only folders (e.g. `docs/`, `.cursor/` in `.gitignore`) stay on your machine and are not part of that commit.

### Removing `.cursor/` from history

Same idea as `docs/`: `git filter-repo --path .cursor/ --invert-paths --force`, then restore any local editor rules under `.cursor/` if your tool removed them, and keep **`/.cursor/`** in `.gitignore` so they are not re-committed.
