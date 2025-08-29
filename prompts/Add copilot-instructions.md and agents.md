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

1. Specify coding practices, preferred technologies, or project requirements, so generated code follows your standards.
2. Set rules for code reviews, such as checking for security vulnerabilities or performance issues.
3. Provide instructions for generating commit messages or pull request titles and descriptions.

## agents.md Instructions

Create Your Agents.md File

Start by creating a file named AGENTS.md in your repository. 

1. Document Project Structure
Within your Agents.md file, clearly document your project's structure, architecture, and organization. This critical information helps OpenAI Codex understand the codebase layout and relationships between components.
2. Define Coding Conventions
Agents.md should thoroughly document your project's coding standards, style guidelines, naming conventions, and other practices. When OpenAI Codex has access to these conventions, it can generate code that seamlessly integrates with your existing codebase.
3. Include Testing Protocols
In your Agents.md file, provide clear instructions on how to run tests, what testing frameworks are used, and any specific testing requirements. This enables OpenAI Codex to generate not only functional code but also appropriate test cases.
4. Specify PR Guidelines
For teams using OpenAI Codex in collaborative environments, include instructions about Pull Request messages, formatting, and specific information that should be included when creating PRs. This helps ensure that code contributions follow your team's workflow.

