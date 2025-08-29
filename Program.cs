// Add config usings
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

return await RunAsync(args);

async Task<int> RunAsync(string[] args)
{
    var opts = ParseArgs(args);

    string reposPath = opts.ReposPath;
    string promptsDir = opts.PromptsDir;
    string? token = opts.Token;
    bool dryRun = opts.DryRun;
    bool showHelp = opts.ShowHelp;

    if (showHelp)
    {
        PrintUsage();
        return 0;
    }

    // load user-secrets (fallback to environment variables separately)
    var config = new ConfigurationBuilder()
        .AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly(), optional: true)
        .Build();

    token ??= Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? Environment.GetEnvironmentVariable("GH_TOKEN")
             ?? config["GITHUB_TOKEN"] ?? config["GH_TOKEN"];

    if (!ValidatePreconditions(token, reposPath, promptsDir))
    {
        return 1;
    }

    var repoLines = LoadRepoLines(reposPath);
    var promptFiles = LoadPromptFiles(promptsDir);

    if (repoLines.Count == 0)
    {
        Console.Error.WriteLine("No repositories found in repos file.");
        return 1;
    }

    if (promptFiles.Count == 0)
    {
        Console.Error.WriteLine("No prompt files found in prompts folder.");
        return 1;
    }

    using var client = CreateHttpClient(token!);

    await PostIssuesToRepos(client, repoLines, promptFiles, dryRun);

    Console.WriteLine("Done.");
    return 0;
}

CliOptions ParseArgs(string[] args)
{
    string reposPath = "repos.md";
    string promptsDir = "prompts";
    string? token = null;
    bool dryRun = false;
    bool showHelp = false;

    for (int i = 0; i < args.Length; i++)
    {
        var a = args[i];
        switch (a)
        {
            case "--repos":
            case "-r":
                if (i + 1 < args.Length) reposPath = args[++i];
                break;
            case "--prompts":
            case "-p":
                if (i + 1 < args.Length) promptsDir = args[++i];
                break;
            case "--token":
            case "-t":
                if (i + 1 < args.Length) token = args[++i];
                break;
            case "--dry-run":
                dryRun = true;
                break;
            case "--help":
            case "-h":
                showHelp = true;
                break;
        }
    }

    return new CliOptions(reposPath, promptsDir, token, dryRun, showHelp);
}

bool ValidatePreconditions(string? token, string reposPath, string promptsDir)
{
    if (string.IsNullOrWhiteSpace(token))
    {
        Console.Error.WriteLine("No GitHub token provided. Set GITHUB_TOKEN or GH_TOKEN, or pass --token <token>.");
        return false;
    }

    if (!File.Exists(reposPath))
    {
        Console.Error.WriteLine($"Repos file not found: {reposPath}");
        return false;
    }

    if (!Directory.Exists(promptsDir))
    {
        Console.Error.WriteLine($"Prompts folder not found: {promptsDir}");
        return false;
    }

    return true;
}

List<string> LoadRepoLines(string reposPath)
{
    return File.ReadAllLines(reposPath)
        .Select(l => l.Trim())
        .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#"))
        .ToList();
}

List<string> LoadPromptFiles(string promptsDir)
{
    return Directory.GetFiles(promptsDir)
        .Where(f => !Path.GetFileName(f).StartsWith("."))
        .ToList();
}

HttpClient CreateHttpClient(string token)
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.UserAgent.ParseAdd("gh-repo-issue-automator/1.0");
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    return client;
}

async Task PostIssuesToRepos(HttpClient client, List<string> repoLines, List<string> promptFiles, bool dryRun)
{
    foreach (var repoEntry in repoLines)
    {
        var (owner, repo) = ParseOwnerRepo(repoEntry);
        if (owner == null || repo == null)
        {
            Console.WriteLine($"Skipping invalid repo entry: {repoEntry}");
            continue;
        }

        Console.WriteLine($"Target repo: {owner}/{repo}");

        foreach (var promptFile in promptFiles)
        {
            var fileName = Path.GetFileName(promptFile);
            var title = Path.GetFileNameWithoutExtension(fileName);
            var body = File.ReadAllText(promptFile);

            if (title.Length > 200) title = title.Substring(0, 200);

            Console.WriteLine($" -> Posting issue '{title}' from '{fileName}' {(dryRun ? "(dry-run)" : "")}" );

            if (dryRun)
            {
                continue;
            }

            // check for existing issue with same title
            if (await IssueExistsAsync(client, owner, repo, title))
            {
                Console.WriteLine($"   Skipping: issue with title '{title}' already exists in {owner}/{repo}.");
                continue;
            }

            await PostIssue(client, owner, repo, title, body);

            await Task.Delay(500);
        }
    }
}

async Task<bool> IssueExistsAsync(HttpClient client, string owner, string repo, string title)
{
    try
    {
        var url = $"https://api.github.com/repos/{owner}/{repo}/issues?state=all&per_page=100";
        using var response = await client.GetAsync(url);
        var respContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.Error.WriteLine($"   Warning: could not check existing issues: {(int)response.StatusCode} {response.ReasonPhrase}");
            Console.Error.WriteLine($"   Response: {respContent}");
            return false;
        }

        using var doc = System.Text.Json.JsonDocument.Parse(respContent);
        if (doc.RootElement.ValueKind == System.Text.Json.JsonValueKind.Array)
        {
            foreach (var item in doc.RootElement.EnumerateArray())
            {
                if (item.TryGetProperty("title", out var t))
                {
                    var existingTitle = t.GetString();
                    if (!string.IsNullOrEmpty(existingTitle) && string.Equals(existingTitle, title, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"   Exception checking existing issues: {ex.Message}");
    }

    return false;
}

async Task PostIssue(HttpClient client, string owner, string repo, string title, string body)
{
    var payload = new { title = title, body = body };
    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

    try
    {
        var url = $"https://api.github.com/repos/{owner}/{repo}/issues";
        using var response = await client.PostAsync(url, content);
        var respContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(respContent);
                if (doc.RootElement.TryGetProperty("html_url", out var urlProp))
                {
                    Console.WriteLine($"   Created: {urlProp.GetString()}");
                }
                else
                {
                    Console.WriteLine("   Created (no url returned).");
                }
            }
            catch
            {
                Console.WriteLine("   Issue created.");
            }
        }
        else
        {
            Console.Error.WriteLine($"   Failed: {(int)response.StatusCode} {response.ReasonPhrase}");
            Console.Error.WriteLine($"   Response: {respContent}");
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"   Exception posting: {ex.Message}");
    }
}

(string? owner, string? repo) ParseOwnerRepo(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return (null, null);

    if (Uri.TryCreate(input, UriKind.Absolute, out var uri))
    {
        var segments = uri.AbsolutePath.Trim('/').Split('/');
        if (segments.Length >= 2) return (segments[0], segments[1]);
        return (null, null);
    }

    var parts = input.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length >= 2) return (parts[0], parts[1]);
    return (null, null);
}

void PrintUsage()
{
    Console.WriteLine("gh-repo-issue-automator - create GitHub issues from prompt files to multiple repos\n");
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run -- [--repos <path>] [--prompts <path>] [--token <token>] [--dry-run]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --repos, -r    Path to repos file (default: repos.md). Each line should be 'owner/repo' or a GitHub URL. Lines starting with # are ignored.");
    Console.WriteLine("  --prompts, -p  Path to prompts folder (default: prompts). All files in the folder will be submitted as issues.");
    Console.WriteLine("  --token, -t    GitHub token. If omitted the GITHUB_TOKEN or GH_TOKEN environment variables will be used.");
    Console.WriteLine("  --dry-run      Don't actually POST issues; just show what would be done.");
    Console.WriteLine();
}

record CliOptions(string ReposPath, string PromptsDir, string? Token, bool DryRun, bool ShowHelp);
