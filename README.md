# GitHub Repository Issue Automator

A small .NET CLI that reads a list of GitHub repositories and posts each file in a `prompts` folder as an issue to each repository. GitHub Copilot Coding Agent can then be assigned to the issue to process the prompts.

## Quick usage

1. Set a GitHub token in your environment: GITHUB_TOKEN (or GH_TOKEN), or pass `--token`. 

```bash
dotnet user-secrets init
dotnet user-secrets set "GITHUB_TOKEN" "your_token_here"
```

2. Add a `repos.md` file into the root of the project with lines like `owner/repo` (or full GitHub URLs). Lines starting with `#` are ignored. Example:

```markdown
# List target repositories (one per line). Examples:
# octocat/Hello-World

# Replace this example line with your target repos:
[org]/[repo_name]
```
3. Put two (or more) prompt files in the `prompts` folder; filenames will be used as issue titles and the file content as the issue body.
4. Run the app:

```
dotnet restore
dotnet run -- --repos repos.md --prompts prompts
```

Options

- `--repos, -r` Path to repos file (default `repos.md`).
- `--prompts, -p` Path to prompts folder (default `prompts`).
- `--token, -t` GitHub token (optional if GITHUB_TOKEN/GH_TOKEN env var is set).
- `--dry-run` Show what would be done without posting.

## Notes and considerations

- The program uses the prompt filename (without extension) as the issue title and the file contents as the issue body.
- The tool uses the GitHub REST API and requires a token with `public_repo` for public repositories.
