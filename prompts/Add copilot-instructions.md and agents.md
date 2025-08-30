Title: Add copilot-instructions.md and agents.md files

# Plan

We need to add specific files and content into this repository. To do that, think through and analyze the following plan. After careful analysis, perform each step exactly as stated.

1. Scan the repository to get a complete understanding of what it contains.
2. Perform the following steps to create the appropriate files. If a file already exists DO NOT PERFORM that step. We do not want to overwrite existing `copilot-instructions.md` or `agents.md` files. 
    - Create a GitHub Copilot instructions file named `copilot-instructions.md` in the `.github` directory at the root of the repository if the file does not exist. See "## copilot-instructions.md Instructions" below and follow that general template format based upon what you found in this repository. 
    - Create an `agents.md` file at the root of the repository if it does not exist. See "## agents.md Instructions" below and follow that general template format based upon what you found in this repository.  In addition to the provided template below, research the additional template available at https://github.com/gakeez/agents_md_collection/tree/main/examples so that you understand your options for creating the `agents.md` file.

**RULES**:
- ONLY add the `copilot-instructions.md` and `agents.md` files if they do not already exist. DO NOT modify any other files.
- DO NOT COPY the actual content from the templates that are mentioned below. Use the templates to understand the format that should be used to create the files.

## copilot-instructions.md Instructions 

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

## agents.md Instructions

# Create high‑quality AGENTS.md file

You are a code agent. Your task is to create a complete, accurate AGENTS.md at the root of this repository that follows the public guidance at https://agents.md/.

AGENTS.md is an open format designed to provide coding agents with the context and instructions they need to work effectively on a project.

## What is AGENTS.md?

AGENTS.md is a Markdown file that serves as a "README for agents" - a dedicated, predictable place to provide context and instructions to help AI coding agents work on your project. It complements README.md by containing detailed technical context that coding agents need but might clutter a human-focused README.

## Key Principles

- **Agent-focused**: Contains detailed technical instructions for automated tools
- **Complements README.md**: Doesn't replace human documentation but adds agent-specific context
- **Standardized location**: Placed at repository root (or subproject roots for monorepos)
- **Open format**: Uses standard Markdown with flexible structure
- **Ecosystem compatibility**: Works across 20+ different AI coding tools and agents

## File Structure and Content Guidelines

### 1. Required Setup

- Create the file as `AGENTS.md` in the repository root
- Use standard Markdown formatting
- No required fields - flexible structure based on project needs

### 2. Essential Sections to Include

#### Project Overview

- Brief description of what the project does
- Architecture overview if complex
- Key technologies and frameworks used

#### Setup Commands

- Installation instructions
- Environment setup steps
- Dependency management commands
- Database setup if applicable

#### Development Workflow

- How to start development server
- Build commands
- Watch/hot-reload setup
- Package manager specifics (npm, pnpm, yarn, etc.)

#### Testing Instructions

- How to run tests (unit, integration, e2e)
- Test file locations and naming conventions
- Coverage requirements
- Specific test patterns or frameworks used
- How to run subset of tests or focus on specific areas

#### Code Style Guidelines

- Language-specific conventions
- Linting and formatting rules
- File organization patterns
- Naming conventions
- Import/export patterns

#### Build and Deployment

- Build commands and outputs
- Environment configurations
- Deployment steps and requirements
- CI/CD pipeline information

### 3. Optional but Recommended Sections

#### Security Considerations

- Security testing requirements
- Secrets management
- Authentication patterns
- Permission models

#### Monorepo Instructions (if applicable)

- How to work with multiple packages
- Cross-package dependencies
- Selective building/testing
- Package-specific commands

#### Pull Request Guidelines

- Title format requirements
- Required checks before submission
- Review process
- Commit message conventions

#### Debugging and Troubleshooting

- Common issues and solutions
- Logging patterns
- Debug configuration
- Performance considerations

## Example Template

Use this as a starting template and customize based on the specific project:

```markdown
# AGENTS.md

## Project Overview

[Brief description of the project, its purpose, and key technologies]

## Setup Commands

- Install dependencies: `[package manager] install`
- Start development server: `[command]`
- Build for production: `[command]`

## Development Workflow

- [Development server startup instructions]
- [Hot reload/watch mode information]
- [Environment variable setup]

## Testing Instructions

- Run all tests: `[command]`
- Run unit tests: `[command]`
- Run integration tests: `[command]`
- Test coverage: `[command]`
- [Specific testing patterns or requirements]

## Code Style

- [Language and framework conventions]
- [Linting rules and commands]
- [Formatting requirements]
- [File organization patterns]

## Build and Deployment

- [Build process details]
- [Output directories]
- [Environment-specific builds]
- [Deployment commands]

## Pull Request Guidelines

- Title format: [component] Brief description
- Required checks: `[lint command]`, `[test command]`
- [Review requirements]

## Additional Notes

- [Any project-specific context]
- [Common gotchas or troubleshooting tips]
- [Performance considerations]
```

## Working Example from agents.md

Here's a real example from the agents.md website:

```markdown
# Sample AGENTS.md file

## Dev environment tips

- Use `pnpm dlx turbo run where <project_name>` to jump to a package instead of scanning with `ls`.
- Run `pnpm install --filter <project_name>` to add the package to your workspace so Vite, ESLint, and TypeScript can see it.
- Use `pnpm create vite@latest <project_name> -- --template react-ts` to spin up a new React + Vite package with TypeScript checks ready.
- Check the name field inside each package's package.json to confirm the right name—skip the top-level one.

## Testing instructions

- Find the CI plan in the .github/workflows folder.
- Run `pnpm turbo run test --filter <project_name>` to run every check defined for that package.
- From the package root you can just call `pnpm test`. The commit should pass all tests before you merge.
- To focus on one step, add the Vitest pattern: `pnpm vitest run -t "<test name>"`.
- Fix any test or type errors until the whole suite is green.
- After moving files or changing imports, run `pnpm lint --filter <project_name>` to be sure ESLint and TypeScript rules still pass.
- Add or update tests for the code you change, even if nobody asked.

## PR instructions

- Title format: [<project_name>] <Title>
- Always run `pnpm lint` and `pnpm test` before committing.
```

## Implementation Steps

1. **Analyze the project structure** to understand:

   - Programming languages and frameworks used
   - Package managers and build tools
   - Testing frameworks
   - Project architecture (monorepo, single package, etc.)

2. **Identify key workflows** by examining:

   - package.json scripts
   - Makefile or other build files
   - CI/CD configuration files
   - Documentation files

3. **Create comprehensive sections** covering:

   - All essential setup and development commands
   - Testing strategies and commands
   - Code style and conventions
   - Build and deployment processes

4. **Include specific, actionable commands** that agents can execute directly

5. **Test the instructions** by ensuring all commands work as documented

6. **Keep it focused** on what agents need to know, not general project information

## Best Practices

- **Be specific**: Include exact commands, not vague descriptions
- **Use code blocks**: Wrap commands in backticks for clarity
- **Include context**: Explain why certain steps are needed
- **Stay current**: Update as the project evolves
- **Test commands**: Ensure all listed commands actually work
- **Consider nested files**: For monorepos, create AGENTS.md files in subprojects as needed

## Monorepo Considerations

For large monorepos:

- Place a main AGENTS.md at the repository root
- Create additional AGENTS.md files in subproject directories
- The closest AGENTS.md file takes precedence for any given location
- Include navigation tips between packages/projects

## Final Notes

- AGENTS.md works with 20+ AI coding tools including Cursor, Aider, Gemini CLI, and many others
- The format is intentionally flexible - adapt it to your project's needs
- Focus on actionable instructions that help agents understand and work with your codebase
- This is living documentation - update it as your project evolves

When creating the AGENTS.md file, prioritize clarity, completeness, and actionability. The goal is to give any coding agent enough context to effectively contribute to the project without requiring additional human guidance.