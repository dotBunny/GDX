# Bootstrap

One less publicized part of the GDX initiative is the [Bootstrap](https://github.com/dotBunny/GDX.Bootstrap) system we have in place for development outside of existing projects. Cloning this repository allows you to test and develop features in isolation quickly.

Once cloned, on a Windows machine, all you need to do is run **script-setup.bat** and it will make the magic happen.

## Repository Layout

### Scripts

#### [script-setup.bat](https://github.com/dotBunny/GDX.Bootstrap/blob/main/script-setup.bat)

This will download GDX into each of the development projects and symlink the .editorconfig into the right spot for your IDE to pick it up when generating a project from Unity.

#### [script-docs.bat](https://github.com/dotBunny/GDX.Bootstrap/blob/main/script-docs.bat)

This will download the necessary components to build the documentation locally. Most people do not need this to work on the project.

### Templates

Underneath this folder are empty Unity projects used to test what including the package at different versions does, it helps with compatibility testing mostly.

### Projects

#### GDX_Development
Located in `Projects\GDX_Development`, this is the **primary** development project that should be used when contributing to the project. Unit tests can be ran from the Unity Editor and this project is used in [Automation](/manual/contributing/automation.html) as well. 

The project includes a root menu item addition (of its namesake) to the Unity editor which allows build and BVT testing to be driven from the editor (on Windows).

##### BVT

The **Build Vertification Test** suite allows for tests to be run in builds and validated across numerous platforms. These are meant to run without any other dependencies and therefore appropriate for actual runtime deployment. The `Bootstrap` MonoBehaviour file drives this in our builds asynchronously.

##### Demo

This is where content lives that is used in our _discovery_ videos about `GDX`.



#### GDX_Entities
This project has been rather dormant whilst we await a proper Entities 1.0 release cycle. It will change as things become publicly available.