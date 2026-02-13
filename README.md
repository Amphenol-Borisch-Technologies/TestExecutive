# TestExecutive
Minimalist test sequencer for functional test of electronic assemblies.

Sibling to TestPlan.
- https://github.com/Amphenol-Borisch-Technologies/TestPlan


# TestExecutive

TestExecutive is a lightweight, maintainable functional test framework built in C# and .NET 9. It provides a simple, modular structure for automating instruments, executing test sequences, and logging results using VISA.NET and SCPI.

## Why TestExecutive Exists

Commercial and open‑source test frameworks each have strengths, but they can also introduce complexity that isn’t always necessary for small to medium‑scale functional test systems. Full‑featured platforms such as NI TestStand and OpenTAP offer extensive capabilities, yet they may require additional licensing, infrastructure, or configuration beyond what many focused production environments need.

**TestExecutive was created to provide a right‑sized alternative** — a framework that is:

- straightforward to understand  
- easy to extend  
- built entirely on modern .NET  
- free of external licensing requirements  
- sized appropriately for functional test applications  

The goal is not to replace large commercial platforms, but to offer a simpler option when a full test‑management suite would be more than the project requires.

## Key Features

- Modular test‑step architecture  
- C# and .NET 9  
- VISA.NET instrument communication  
- SCPI‑based instrument control  
- Support for PXI or bench instruments  
- Configurable test sequences  
- Structured logging and result storage  
- Easy to extend with new instruments or test steps  
