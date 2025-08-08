# AI-AGENT.md

This file provides guidance to AI Agents (such as Claude Code, GitHub CoPilot, Curtsor, and Windsurf) when working with code in this repository.

## Important Files
- **ARCHITECTURE.md**: Outlines the architecture of the Finman application, including its components, interactions, and deployment strategies.
- **README.md**: Contains the introduction and overview of the Finman project, including how to build, run, and test the application.
- **TODO.md**: Lists tasks and features to be implemented in the Finman application.

This file (AI-AGENT.md) contains ways of working instructions for AI Agents, including how to interpret and generate code, as well as guidelines for contributing to the Finman project.

## Guidelines for AI Agents
- Always start every interactions with a the message "Hi Rob!" - this allows me to check you are working in accordance with the guidelines.
- Always refer to the **ARCHITECTURE.md** for understanding the overall structure and design of the Finman application.
- Use the **README.md** for context on how to set up and run the application.
- Follow the tasks outlined in **TODO.md** for implementing new features or fixing bugs.
- For building, running, testing, etc - always use the scripts provided in the "scripts" folder. If a script does not exist, create one and document it in the README.md.

- We are a coding pair. We approach all problems with a Analyse -> Plan -> Execute -> Review methodology.
- We delivery individual features in small, incremental slices. For instance, if standing up a new web site with a backend API - start with a simple "Hello World" page, then add a backend API that returns a static message, then add more functionalitry incrementally.
- When generating code, ensure you update the relevant documentation files and maintain consistency with the existing codebase.
- If you encounter any issues or have questions, or there are multiple choices ahead - always ask for clarification.
- When making changes, ensure you document them clearly in the commit messages and comments within the code.
- We are *strictly* test-first, and we always write fully automated tests for any new features or changes to ensure the stability of the application.
- Prefer clean object-oriented design and follow SOLID principles.
- Use meaningful variable and function names to enhance code readability.