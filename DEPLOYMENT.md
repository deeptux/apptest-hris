# Render deployment — checklist

Use this with the root [`README.md`](README.md) **Deployment (GitHub Pages + Render)** section.

## 0) Client static site — GitHub Pages + custom domain

The Blazor WASM client is deployed by GitHub Actions on every push to `main`:

- Workflow: [`.github/workflows/deploy-client-pages.yml`](.github/workflows/deploy-client-pages.yml)
- **Canonical public demo URL:** `https://handrian.space/apptest-hris/` (custom domain; infra may proxy this path to the GitHub Pages deployment).
- **Upstream / alternate (GitHub Pages):** `https://deeptux.github.io/apptest-hris/`
- GitHub Pages source must be **GitHub Actions** (not branch-based Pages)
- This is a **project repo**, so production base path is **`/apptest-hris/`**. The workflow rewrites published `index.html` base href accordingly while local dev stays at `/`. The same build can be served from **both** origins above when the path segment matches.
- **Deep links / SPA:** GitHub Pages uses `404.html` copied from `index.html` for client-side routes. If deep links fail behind Cloudflare, check **proxy path, trailing slash, and fallback document** before changing Blazor routes.

## 1) API Web Service — Docker on Render

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

## 2) Secrets

- Do **not** commit `Ai:Gemini:ApiKey`, passwords, or tokens in:
  - `src/Hris.Demo.Api/appsettings.json`
  - `src/Hris.Demo.Api/appsettings.*.json` (tracked files)
  - `src/Hris.Demo.Client/wwwroot/appsettings.json`
- On **Render**, set API secrets as environment variables (e.g. `Ai__Gemini__ApiKey`).
- **Local:** `dotnet user-secrets` for the API project only.

### 2.1) Applicant profile uploads — AWS S3 (optional)

Applicant **avatar**, **cover**, and **PDF** uploads store bytes in **S3** and metadata in the API’s **SQLite** file (`ConnectionStrings__AppDb` / `Data/hris-files.db` under the API content root). **Uploads are not stored on the API’s local filesystem** as loose files — only the SQLite file (and S3 objects) hold data.

On the **Render Web Service** (API), set:

| Variable | Purpose |
|----------|---------|
| `Storage__Provider` | Set to `S3` to enable uploads/downloads. |
| `Storage__S3__BucketName` | Target bucket. |
| `Storage__S3__Region` | AWS region id (e.g. `ap-southeast-1`). |
| `Storage__S3__AccessKeyId` | IAM access key with `s3:PutObject`, `s3:GetObject`, `s3:HeadObject`, `s3:DeleteObject` on the bucket/prefix. |
| `Storage__S3__SecretAccessKey` | IAM secret (never commit). |
| `Storage__S3__UsePathStyle` | `true` only if you use path-style endpoints (e.g. some S3-compatible stores); default `false` for AWS. |
| `Storage__MaxImageBytes` | Optional override (default **1.5 MB** hard cap for stored images). |
| `Storage__MaxPdfBytes` | Optional override (default **10 MB** for PDFs). |
| `ConnectionStrings__AppDb` | Optional SQLite path; default is `Data Source=Data/hris-files.db` next to the published API. |

**S3 bucket CORS (required for browser → S3 PUT):** The Blazor site uploads **directly** to S3 using the pre-signed URL. Add a CORS rule on the bucket allowing **`PUT`** (and **`HEAD`** for verification) from every **static site origin** that serves the WASM UI (scheme + host, no path), for example `https://handrian.space` and `https://deeptux.github.io`. Include **`GET`** if you open signed object URLs in new tabs from the same origins. Example `AllowedMethods`: `GET`, `PUT`, `HEAD`. Typical `AllowedHeaders`: `*` (or at least `Content-Type`, `x-amz-*`).

**Note:** On Render’s ephemeral filesystem, the SQLite file is recreated unless you attach **persistent disk** or point `ConnectionStrings__AppDb` to external storage — metadata can reset on redeploy unless you plan for persistence.

## 3) Client `ApiBaseUrl` (production)

- Published Blazor WASM loads `wwwroot/appsettings.json`, then `wwwroot/appsettings.{Environment}.json` (see [`Program.cs`](src/Hris.Demo.Client/Program.cs)).
- For **Production** builds, [`appsettings.Production.json`](src/Hris.Demo.Client/wwwroot/appsettings.Production.json) should contain the **public HTTPS URL** of the API (no trailing slash required; the app normalizes it).
- For this repo, use `https://apptest-hris.onrender.com`. The Pages workflow enforces this in published output.
- Document the final URLs in [`README.md`](README.md) (deployment table).

## 4) API CORS

- After the static site origin(s) are known, allow each **origin** on the API (scheme + host + port — **no path**).
- Prefer the **`CORS_ORIGINS`** environment variable on Render: comma-separated list (spaces trimmed per entry). When both GitHub Pages and the custom domain serve the UI, include **both** origins, for example:
  - `CORS_ORIGINS=https://deeptux.github.io,https://handrian.space`
- If `CORS_ORIGINS` is unset, the API falls back to `Cors:Origins` in [`appsettings.json`](src/Hris.Demo.Api/appsettings.json) (localhost for dev).

## 5) `.gitignore`

- Confirm `bin/`, `obj/`, `/references/`, `/docs/`, and secret patterns remain as in [`.gitignore`](.gitignore). Do not commit build output.

## 6) SDK / build

- **Docker (API on Render):** The [`Dockerfile`](Dockerfile) uses **`mcr.microsoft.com/dotnet/sdk:9.0`** — no separate Render “.NET version” pick is needed for that service.
- **Non-Docker builds:** Use **.NET 9** locally or in CI ([`global.json`](global.json)).

## 7) Smoke test (after deploy)

1. Open the **static site** over **HTTPS**.
2. Browser **DevTools → Network**: load dashboard / queues; confirm API calls return **200** (not CORS errors).
3. If CORS fails, verify `CORS_ORIGINS` matches the static site origin exactly (scheme + host, no path).
4. Optional: **Simulated role → Applicant → Profile** — with S3 configured, upload a small image and a PDF; confirm `PUT` to `*.amazonaws.com` succeeds (bucket CORS) and `GET` download links open while valid.

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
