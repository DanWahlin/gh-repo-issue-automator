# Copilot instructions for gh-repo-issue-automator

Purpose
- Help an AI coding agent become productive quickly in this small .NET 9 CLI that posts prompt files as GitHub issues to multiple repositories.

Big picture (what the program does)
- CLI parses args (repos file, prompts folder, optional token, --dry-run).
- Loads repo list from `repos.txt` and all files in the `prompts/` folder.
- For each repo+prompt: check whether an issue with the same title already exists; if not, POST a new issue to the GitHub REST API.
- Token resolution order: CLI `--token`, then environment variables `GITHUB_TOKEN`/`GH_TOKEN`, then dotnet user-secrets (project secrets).

Key files to read first
- `Program.cs` — entire app lives here (top-level statements; helper methods: ParseArgs, ValidatePreconditions, LoadRepoLines, LoadPromptFiles, CreateHttpClient, PostIssuesToRepos, IssueExistsAsync, PostIssue, ParseOwnerRepo, PrintUsage).
- `gh-repo-issue-automator.csproj` — target framework (net9.0), implicit usings, package refs (UserSecrets added).
- `README.md` — quick usage and developer hints (user-secrets example, dry-run recommendation).
- `repos.txt` — example repo list format (owner/repo or full URL).
- `prompts/` — sample prompt files; filename → issue title, file contents → issue body.

Important implementation/architecture notes
- The project uses C# top-level statements and implicit usings (see csproj). Expect Program.cs to be a flat file with local helper functions.
- CLI options are grouped into a `CliOptions` record (see ParseArgs → CliOptions).
- Nullability is enabled and handled explicitly: several methods return nullable tuples for owner/repo and the code checks them.
- HTTP calls use a shared HttpClient configured with GitHub headers and a 500ms delay between issue POSTs to reduce burst rate.
- Issue existence check: `IssueExistsAsync` fetches `GET /repos/{owner}/{repo}/issues?state=all&per_page=100` and does a case-insensitive exact-title comparison of the first 100 issues. This is a deliberate/simple implementation but has pagination limitations.

Developer workflows / commands
- Build and run locally:
  - `dotnet restore`
  - `dotnet build`
  - `dotnet run -- --repos repos.txt --prompts prompts` (use `--dry-run` to preview without POSTing)
- Local secret setup (already used in project):
  - `dotnet user-secrets init`
  - `dotnet user-secrets set "GITHUB_TOKEN" "<token>"`
  - Note: user-secrets are not environment variables; the app explicitly reads user-secrets via `ConfigurationBuilder().AddUserSecrets(...)`.
- If you modify `gh-repo-issue-automator.csproj` to add packages, run `dotnet restore` again.

Project-specific conventions & gotchas
- Titles are derived from the prompt filename (without extension) and truncated to 200 chars. If you change this logic, update both the PostIssue and IssueExistsAsync title handling to keep parity.
- The code checks closed issues because `state=all` is used — closed issues will block new identical titles. If you want to allow reposting closed items, change that behavior.
- `IssueExistsAsync` only checks up to 100 issues (one page). For repositories with many issues, implement proper pagination (follow `Link` header) or use the GitHub Search API.
- The `UserSecretsId` (if present in csproj) is safe to commit — it is not a secret. Do not commit actual tokens.
- `.gitignore` is present and build artifacts were removed from git; keep it when adding files.

Integration points & external dependencies
- GitHub REST API — POST issues at `https://api.github.com/repos/{owner}/{repo}/issues` and GET issues at `.../issues?state=all&per_page=100`.
- Uses `System.Text.Json` for serialization and deserialization.
- Uses `Microsoft.Extensions.Configuration.UserSecrets` to read developer secrets.

Suggested low-risk tasks an AI agent can do first
- Add pagination to `IssueExistsAsync` (follow `Link` header) to ensure robust checking.
- Add an option to use the prompt file's first non-empty line as the title (with tests for trimming and truncation).
- Add a `--concurrency` flag to limit parallel POSTs (and use SemaphoreSlim) instead of sequential delay.
- Add unit tests for `ParseOwnerRepo` and `ParseArgs` with xUnit (no tests currently present).

When changing behavior, check these integration points
- If you alter how titles are generated, ensure IssueExistsAsync compares the exact same transformed title.
- If you add labels/assignees, update the JSON payload in `PostIssue` and ensure the token has appropriate permissions.
- If you add pagination or use the Search API, respect GitHub rate limits (observe headers and back off).

Examples from the codebase
- Posting issues: `POST https://api.github.com/repos/{owner}/{repo}/issues` with payload `{"title": "...", "body": "..."}` in `PostIssue`.
- Existing-check: `GET https://api.github.com/repos/{owner}/{repo}/issues?state=all&per_page=100` in `IssueExistsAsync`.

Security and safety
- Use `--dry-run` when validating changes to avoid creating noisy issues.
- Never write PATs into source files. For local dev, use `dotnet user-secrets` or environment variables.

If anything above is unclear or you want the file to emphasize other tasks (e.g. testing, CI setup, or adding GitHub Actions), tell me what to add and I will iterate on this instruction file.
