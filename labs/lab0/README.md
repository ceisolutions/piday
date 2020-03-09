# LAB 0 - GETTING SETUP
Before proceeding with the other labs, you'll need to get some items setup for your development environment.  There are two options for development: developing on your computer, developing directoy on the Raspberry PI.

### Requirements
- Git (to clone the PiDay repo)
- SSH (to connect to the Pi shell)
- SCP (to optionally transfer files)
- .NET 3.1 SDK (already on the Pi or on your computer)
- A code editor like VS Code (already on the Pi or on your computer)
- Docker (already on the Pi)
- Some downloaded Nuget packages

## Clone this repo with Git
You'll need to install a git client to clone this repo.

### Windows
| Task | Instructions |
|--------|--------------|
|1. Install Git | Install [Git for Windows](https://git-scm.com/downloads) or by using [Choclatey](https://chocolatey.org/packages/git) 
| 2. Clone | ```git clone https://github.com/ceisolutions/piday```

### macOS
| Task | Instructions |
|--------|--------------|
|1. Install Git | Install [Git for macOS](https://git-scm.com/downloads) or by using [Homebrew](https://formulae.brew.sh/formula/git#default) 
| 2. Clone | ```git clone https://github.com/ceisolutions/piday```

### GUIs
You can alternatively use a GUI client like [GitKraken](https://www.gitkraken.com/), [Sourcetree](https://www.sourcetreeapp.com/) or [Github Desktop](https://desktop.github.com/)


## Connecting with the Pi - SSH and BASH
You'll be directly interacting Pi through SSH to use the shell (BASH), to remotely debug or to copy files (SCP).  There are a few options to consider.

### Windows
NOTE: Newer Windows 10 versions may already come with SSH and SCP installed.

| Method | Instructions |
|--------|--------------|
|Git Bash| Install Git Bash with Git for Windows using [Choclatey](https://chocolatey.org/packages/git) or [directly](https://git-scm.com/downloads) installing it.  |
|WSL| If you already have the Windows Subsystem for Linux (WSL) it has the tools you need. 
|PuTTY + WinSCP| You can install [PuTTY](https://www.chiark.greenend.org.uk/~sgtatham/putty/latest.html) as a GUI SSH client and [Win SCP](https://winscp.net/eng/download.php) for file transfers|

### macOS
You're all set! macOS has SSH and SCP already installed.

## Install the .NET Core SDK
You'll need something to build the wonderful C# code you'll be creating. You'll need v3.1 for the best support on the Sense HAT library.  If you already have a newer version, that's OK, they can be installed side-by-side.

### Windows
Some versions of Visual Studio already come with the .NET Core SDK.  

| Method | Instructions |
|--------|--------------|
|Direct | Install by downloading from [Microsoft](https://dotnet.microsoft.com/download/dotnet-core/3.1) |
|Chocolatey | Check it out [here](https://chocolatey.org/packages/dotnetcore-sdk) or just try ```choco install dotnetcore-sdk```|

### macOS
The most assured way is to download and install from [Microsoft](https://dotnet.microsoft.com/download/dotnet-core/3.1).

## Install Visual Studio Code
Visual Studio Code is free and recommended but you can use any editor of your choice.

### Windows

| Method | Instructions |
|--------|--------------|
|Direct | Install by downloading from [Microsoft](https://code.visualstudio.com/) |
|Chocolatey | Check it out [here](https://chocolatey.org/packages/vscode) or just try ```choco install vscode```|

### macOS
| Method | Instructions |
|--------|--------------|
|Direct | Install by downloading from [Microsoft](https://code.visualstudio.com/) |
|Homebrew | ```brew install visual-studio-code```|


### Raspberry Pi
Since the Pi's are running Raspbian Lite, there won't be a pretty GUI.  The following are already installed on the Pi: [vim](https://www.vim.org/), [nano](https://www.nano-editor.org/), [micro](https://micro-editor.github.io/). Take your pick!

## Nuget Packages
Much of the lab work will be DISCONNECTED from the Internet (on the private PiNet) so it will be a good idea to download some Nuget packages into your cache.

To make this simple, there is a .csproj file in the folder which contains all the Nuget packages needed for the labs.  Restore these and they'll be waiting in the cache for you.

| Step | Command |
|-----|-----|
|1. Open a command prompt in this folder|
|2. Restore the Nuget packages | dotnet restore |

## Test Raspberry Pi Connection
Finally, try connecting to your Raspberry Pi by connecting to the PiNet Wifi (NO INTERNET!) and using your SSH tool to connect.

| Step | Command |
|-----|-----|
|1. Connect to the PiNet WiFi network| SSID: PiNet, PWD: piday2020|
|2. Determine your Pi's IP address | Check the RGB matrix on the Pi |
|3. Open an SSH connection |  ssh pi@192.168.2.nnn |
|4. Enter the default password | raspberry |



