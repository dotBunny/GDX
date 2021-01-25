# Getting Started

## Installation
There are many options to getting `GDX` into your project; each comes with its benefits and drawbacks.

### Unity Package Manager
Installing via the package manager system (UPM) in Unity will provide an immutable (unable to be modified) copy of the package inside of your project.

> There is two available methods available for this option, pick one!

#### UPM: Package Manager Window
![UPM Git](/images/manual/getting-started/upm-gdx-github.gif)
The simplest and most straight forward method is to use Unity's built in workflow, once you have found and opened the **Package Manager** window (**Window > Package Manager**), follow these steps:
1. Find the **+** button in the upper left portion of the **Package Manager** window, clicking it should reveal a dropdown menu.
2. Select **Add package from git URL...**
3. In the presented text field, enter: `https://github.com/dotBunny/GDX.git`
4. After entering the URL, click **Add**.

#### UPM: Package Manifest ###
The above workflow provides a user interface for your project's `manifest.json` file. Alternatively,  you can enter the necessary line into that file manually by opening `Packages/manifest.json` in a text editor and adding the dependency your self:
```
{
  "dependencies": {
    "com.dotbunny.gdx": "https://github.com/dotBunny/GDX.git",
  }
}
```

### Asset Store
_Discussions to bring the package to the Asset Store are ongoing, we are looking to do this in the near future (1.5)._

### OpenUPM
The `GDX` package is registered with [OpenUPM](https://openupm.com/), and releases are automatically reflected in that registry. The package is registered as `com.dotbunny.gdx`.

For detailed instructions on how to use the [OpenUPM](https://openupm.com/) system and it's limitations, please consult their "[Installing a UPM package](https://openupm.com/docs/getting-started.html#installing-a-upm-package)" documentation.

### Cloned Repository
It is possible for the repository to be cloned into a sub-folder in your project's `Assets` folder. This option makes it easy to modify `GDX` to your liking, as well as access additional _in-development_ branches.
```bash
git clone https://github.com/dotBunny/GDX.git
```
This method is how typical development is done on `GDX`.