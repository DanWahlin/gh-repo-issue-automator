using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GhRepoIssueAutomator
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            string reposPath = "repos.txt";
            string promptsDir = "prompts";
            string? token = null;
            bool dryRun = false;

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
                        PrintUsage();
                        return 0;
                }
            }

            token ??= Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? Environment.GetEnvironmentVariable("GH_TOKEN");

            if (!ValidatePreconditions(token, reposPath, promptsDir))
            {
                return 1;
            }

            var repoLines = File.ReadAllLines(reposPath)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("#"))
                .ToList();

            var promptFiles = Directory.GetFiles(promptsDir)
                .Where(f => !Path.GetFileName(f).StartsWith("."))
                .ToList();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("gh-repo-issue-automator/1.0");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            foreach (var repoEntry in repoLines)
            {
                var (owner, repo) = ParseOwnerRepo(repoEntry);
                if (owner == null)
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

                    var payload = new { title = title, body = body };
                    var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                    try
                    {
                        var url = $"https://api.github.com/repos/{owner}/{repo}/issues";
                        using var response = await client.PostAsync(url, content);
                        var respContent = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            try
                            {
                                using var doc = JsonDocument.Parse(respContent);
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

                    await Task.Delay(500);
                }
            }

            Console.WriteLine("Done.");
            return 0;
        }

        static (string? owner, string? repo) ParseOwnerRepo(string input)
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

        static void PrintUsage()
        {
            Console.WriteLine("gh-repo-issue-automator - create GitHub issues from prompt files to multiple repos\n");
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run -- [--repos <path>] [--prompts <path>] [--token <token>] [--dry-run]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --repos, -r    Path to repos file (default: repos.txt). Each line should be 'owner/repo' or a GitHub URL. Lines starting with # are ignored.");
            Console.WriteLine("  --prompts, -p  Path to prompts folder (default: prompts). All files in the folder will be submitted as issues.");
            Console.WriteLine("  --token, -t    GitHub token. If omitted the GITHUB_TOKEN or GH_TOKEN environment variables will be used.");
            Console.WriteLine("  --dry-run      Don't actually POST issues; just show what would be done.");
            Console.WriteLine();
        }

        static bool ValidatePreconditions(string? token, string reposPath, string promptsDir)
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
    }
}
