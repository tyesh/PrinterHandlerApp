<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

PrinterHandler is a C# application designed to use a custom protocol to lauch a windows exe and send text to the default installed printer.


### Built With

This section should list any major frameworks/libraries used to bootstrap your project. Leave any add-ons/plugins for the acknowledgements section. Here are a few examples.

* [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)

<p align="right">(<a href="#top">back to top</a>)</p>



<!-- GETTING STARTED -->
## Getting Started

Once the application is compiled, you can use the --help command to list the options availables and a brief description of use.

### Prerequisites

Before running the app, follow this steps:
1. Install the [.NET SDK]([https://docs.microsoft.com/en-us/dotnet/csharp/](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks))
3. In the root of the project, generate the exe with dotnet run.
4. In the folter of the exe (/bin/debug/.net), run the exe as administrator with the option --configure-uri-scheme. This will add a custom protocol to the windows registry.

Dependecues:
1. Win32.Registry
2. DragonFruit
3. Drawing.Common

This repository only contains the barebones of the application and needs to be compiled. To compile the application run the command dotnet run. 
After the compilation end, the application is ready for condiguration-  ```

### Configuration

The option --configure-uri-scheme enable to application to set a registry for the custom protocol that will execute the application and run from it. The configuration needs to be executed from administrator.
Once the application is configured, you can start the application with printHandler://

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- Test Page -->
## Test Page

To test the print page, execute the exe with the option --print-test-page

<p align="right">(<a href="#top">back to top</a>)</p>
