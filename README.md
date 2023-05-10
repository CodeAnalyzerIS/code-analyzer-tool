# Code Analyzer Tool (CAT)

This is a custom-made code analyzer tool that can analyze source code and identify code smells or other potential issues.  
This tool is created with a plugin architecture to allow the user to add his own plugins which can analyze different languages or files.  
We have added a built-in plugin that analyzes C# Code based on some built in rules, but you can always add more custom rules yourself.

## Features

- [x] Plugin system for extending language support
- [x] Expandable ruleset for built-in C# plugin (Roslyn)
- [x] Configurable through a config file
- [x] Analysis runnable through CI/CD pipeline
    - [x] Github Action Available for easy setup in Github workflow

## Installation

To install the code analyzer tool, follow these steps:  
  
For local usage:  
1. Download the dotnet tool from nuget.org: `dotnet tool install --global CodeAnalyzerTool`  
	To install it in a different way, you can find the steps on the nuget.org page: https://www.nuget.org/packages/CodeAnalyzerTool  
  
For integration with github actions:  
1. You can use the following github action to install and run the tool: Code Analyzer Tool Action  
https://github.com/marketplace/actions/code-analyzer-tool-action  
Example usage in a workflow:  
```
name: Analyse the code
on: [push]
        
jobs:
    run-code-analyzer-tool:
        runs-on: ubuntu-latest
        steps:
            - name: Checkout the code
              uses: actions/checkout@v3.5.2
            - name: Run Code Analyzer Tool
              uses: CodeAnalyzerIS/code-analyzer-tool-action@v2.0.0
```
After running the tool you can check the results in the terminal or you can set-up the Code Analyzer Service which will provide the results in a webinterface  
https://github.com/CodeAnalyzerIS/code-analyzer-service

## Usage

The most common usage will be running the tool in the pipeline in combination with a hosted Code Analyzer Service 
   
The tool requires a config file which should be placed in the root of the repository (or directory) that will be analysed  
Explanation to set-up the config file can be found in the wiki: https://github.com/CodeAnalyzerIS/code-analyzer-tool/wiki/Configuration

## Example for Custom plugins

In this repository you can find an example for a custom plugin and the installation steps:  
https://github.com/CodeAnalyzerIS/custom-plugin-example

## Example for Custom Roslyn rule

This repository contains an example for a custom rule that can be added to the Roslyn Plugin which is built-in in the tool:  
https://github.com/CodeAnalyzerIS/custom-roslyn-rule-example

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
