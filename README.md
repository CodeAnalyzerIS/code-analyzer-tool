# Code Analyzer Tool (CAT) [![Build the code, run the code analyzer tool](https://github.com/CodeAnalyzerIS/code-analyzer-tool/actions/workflows/build-analyze.yaml/badge.svg)](https://github.com/CodeAnalyzerIS/code-analyzer-tool/actions/workflows/build-analyze.yaml) ![NuGet](https://img.shields.io/nuget/v/CodeAnalyzerTool.svg?label=nuget&color=blue&logo=nuget) [![dockerhub](https://img.shields.io/docker/v/alexanderwuytsis/code-analyzer-service.svg?label=dockerhub-image&color=blue&logo=docker)](https://hub.docker.com/repository/docker/alexanderwuytsis/code-analyzer-service/general) [![Github Container Registry](https://img.shields.io/docker/v/alexanderwuytsis/code-analyzer-service.svg?label=github-image&color=blue&logo=docker)](https://github.com/CodeAnalyzerIS/code-analyzer-tool/pkgs/container/code-analyzer-service)

This is a custom-made code analyzer tool that can analyze source code and identify code smells or other potential issues.  
This tool is created with a plugin architecture to allow the user to add his own plugins which can analyze different languages or files.  
We have added a built-in plugin that analyzes C# Code based on some built in rules, but you can always add more custom rules yourself.

![Homepage](Documentation/Screenshots/Home.PNG)
![ProjectDetails](Documentation/Screenshots/ProjectDetails.PNG)
![ProjectDetailsDrawer](Documentation/Screenshots/ProjectDetailsDrawer.png)
![RuleViolation](Documentation/Screenshots/RuleViolation.png)

## Features

- [x] Plugin system for extending language support
- [x] Expandable ruleset for built-in C# plugin (Roslyn)
- [x] Configurable through a config file
- [x] Analysis runnable through CI/CD pipeline
    - [x] Github Action Available for easy setup in Github workflow

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
