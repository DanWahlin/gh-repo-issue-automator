gh-repo-issue-automator

A small .NET 9 CLI that reads a list of GitHub repositories and posts each file in a `prompts` folder as an issue to each repository.

Quick usage

1. Set a GitHub token in your environment: GITHUB_TOKEN (or GH_TOKEN), or pass `--token`.
2. Populate `repos.txt` with lines like `owner/repo` (or full GitHub URLs). Lines starting with `#` are ignored.
3. Put two (or more) prompt files in the `prompts` folder; filenames will be used as issue titles and the file content as the issue body.
4. Run the app:

dotnet restore
dotnet run -- --repos repos.txt --prompts prompts

Options

- `--repos, -r` Path to repos file (default `repos.txt`).
- `--prompts, -p` Path to prompts folder (default `prompts`).
- `--token, -t` GitHub token (optional if GITHUB_TOKEN/GH_TOKEN env var is set).
- `--dry-run` Show what would be done without posting.

Notes and considerations

- By default the program uses the filename (without extension) as the issue title and the file contents as the issue body. If you prefer titles to come from the first line of the prompt files, or if you want labels/assignees/milestones added, tell me and I can add support for that.
- The tool uses the GitHub REST API and requires a token with `repo` scope for private repositories or `public_repo` for public repositories.
