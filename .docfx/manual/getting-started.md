# Getting Started

## Installation

There are many options to getting `GDX` into your project; each comes with its benefits and drawbacks.

### Unity Package Manager

Installing via the package manager system (UPM) in Unity will provide an immutable (unable to be modified) copy of the package inside of your project.

> [!TIP]
> There are two available UPM methods. You only need to **pick one**!

#### UPM: Package Manager Window

![UPM Git](/images/manual/getting-started/upm-gdx-github.gif)
The simplest and most straight forward method is to use Unity's built in workflow, once you have found and opened the **Package Manager** window (**Window > Package Manager**), follow these steps:

1. Find the **+** button in the upper left portion of the **Package Manager** window, clicking it should reveal a dropdown menu.
2. Select **Add package from git URL...**
3. In the presented text field, enter: `https://github.com/dotBunny/GDX.git`
4. After entering the URL, click **Add**.

#### UPM: Package Manifest ###

The above workflow provides a user interface for your project's `manifest.json` file. Alternatively,  you can enter the necessary line into that file manually by opening `Packages/manifest.json` in a text editor and adding the dependency your self:

```json
{
  "dependencies": {
    "com.dotbunny.gdx": "https://github.com/dotBunny/GDX.git",
  }
}
```

> [!NOTE]
> You can also include branches or releases with this method by appending `#branch` to the end of the definition. Check out Unity's [UPM](https://docs.unity3d.com/Manual/upm-git.html) page for more details.

### Asset Store

> [!NOTE]
> Discussions to bring the package to the Asset Store are ongoing, we are looking to do this in the near future.

### OpenUPM

The `GDX` package is registered with [OpenUPM](https://openupm.com/), and releases are automatically reflected in that registry. The package is registered as `com.dotbunny.gdx`.

For detailed instructions on how to use the [OpenUPM](https://openupm.com/) system and it's limitations, please consult their "[Installing a UPM package](https://openupm.com/docs/getting-started.html#installing-a-upm-package)" documentation.

### Cloned Repository

It is possible for the repository to be cloned into a sub-folder in your project's `Assets` folder. This option makes it easy to modify `GDX` to your liking, as well as access additional _in-development_ branches.

```bash
git clone https://github.com/dotBunny/GDX.git
```

This method is how typical development is done on `GDX`.

## Project Settings

Upon installation, `GDX` _should_ work out of the box.

Further configuration of `GDX` is done by opening the **Project Settings** window (**Edit > Project Settings...**), and selecting the `GDX` section. You will be presented with a view similar to the image below, with numerous configuration options sorted by feature.

![Project Settings](/images/manual/getting-started/gdx-config.png)

Each feature has a collapsable section (**A**) that is expandable and collapsable by clicking the **+**/**-** buttons to the header's left. Features dynamically activate based on the Unity version and packages found in the project (**B**).

A section header's background can inform you of the status of the feature. A green background indicates that the feature is active, where a yellow background means that it is currently disabled. Not all features are enabled by default and require an opt-in (**C**). A blue background indicates an always-on feature, most commonly just functionality that has some level of configuration.

### Persitent Storage

Project-specific configuration values are stored in `.\Generated\GDX.generated.cs` (configurable) using an override pattern which automagically updates the [Config](xref:GDX.Config) when the [Core](xref:GDX.Core) statically initializes.

> [!NOTE]
> This process works with `DOTS_RUNTIME` builds, however there is an additional need to call `GDX.Core.InitializeOnMainThread()` for thread specific initialization.
