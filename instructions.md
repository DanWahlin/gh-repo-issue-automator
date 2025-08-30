Generate the file `.github/copilot-instructions.md` for the current repository so that other AI coding agents can become productive quickly. If the file already exists then do not overwrite it.

Procedure you must follow
1. Scan the repository root and immediate subfolders to discover: README, build files (`*.csproj`, `package.json`, `Makefile`), the main source entry (e.g., `Program.cs`, `src/`), `prompts/`, `repos.txt`, and any CI workflows under `.github/workflows/`.
2. Produce a Markdown file with the sections and content described below.

Required sections (strict)
- Title line: `# Copilot instructions for <repo-name>` (derive repo name from `package.json`, csproj, git remote if available, etc.).
- Purpose: one-line summary of what the project does.
- Big picture: 3–6 bullets describing the core architecture or runtime flow (e.g., CLI arg parsing → load prompts → check issues → post issues).
- Key files to read first: list 4–8 file paths discovered, each with a one-sentence reason (e.g., `Program.cs` — main app logic; `README.md` — usage and setup).
- Important implementation/architecture notes: concise, factual items discovered (framework & version, top-level statements, implicit usings, notable helper functions, config/secrets approach).
- Developer workflows / commands: exact, tested commands to build, run (examples: `dotnet restore`, `dotnet run -- --dry-run`, `dotnet user-secrets set "GITHUB_TOKEN" "<token>"`). Use the repo's actual commands where present.
- Project-specific conventions & gotchas: non-standard patterns, truncation rules, pagination limitations, required token scopes, or anything that could cause hidden failures.
- Integration points & external dependencies: list APIs/endpoints, serialization libs, and packages used (give exact URLs or config keys when you find them).
- Suggested low-risk tasks: 3–5 concrete PR-sized tasks an agent can do first (e.g., add pagination for IssueExistsAsync, add title-from-first-line option, add unit tests for ParseOwnerRepo).
- Security & safety: explicit note to never commit secrets; 

Formatting & tone rules
- Keep output concise and authoritative. Use Markdown headings and short bullets. Use backticks for filenames, commands, and URLs.
- Prefer repository-specific facts. If a fact cannot be inferred, use a placeholder and label it clearly (e.g., "[no build command found]").
- Do not include any secret values or tokens from the repo or environment.
- If multiple plausible build/test commands exist, list them and indicate the recommended one.

Deliverable example skeleton. Use this structure but adapt as needed for the repository you're analyzing.

# Copilot instructions for <repo-name>

Purpose
- <one-line>

Big picture
- bullet 1
- bullet 2

Key files to read first
- `README.md` — reason
- `Program.cs` — reason

Important implementation/architecture notes
- bullet 1
- bullet 2

Developer workflows / commands
- `npm install`
- `dotnet restore`

Project-specific conventions & gotchas
- bullet 1
- bullet 2

Integration points & external dependencies
- bullet 1
- bullet 2

Suggested low-risk tasks an AI agent can do first
- bullet 1
- bullet 2

When changing behavior, check these integration points
- bullet 1
- bullet 2

Examples from the codebase
- bullet 1
- bullet 2

Security and safety
- bullet 1
- bullet 2