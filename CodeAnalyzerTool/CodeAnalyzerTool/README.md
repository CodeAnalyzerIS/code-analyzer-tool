# Code Analyzer Tool
This is a dotnet tool used to analyze your code either in the pipeline or locally.  
The command to run the tool is: code-analyzer-tool

A custom github action is also created to run the tool in a github workflow

Custom rules can be added as plugins, same goes for custom compilers.  
Currently Roslyn is the only built in compiler.  
Source code and documentation can be found here:  
https://github.com/CodeAnalyzerIS/code-analyzer-tool