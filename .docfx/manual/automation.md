# Automation

Every commit to the [`dev`](https://github.com/dotBunny/GDX/tree/dev) branch runs through a barrage of automated testing to validate the expected behaviour and quality of the code being written inside of a private TeamCity installation. The results of all of the ran tests / build configurations are published as they complete back to GitHub and are viewable via the little indicator next to a commit.

## Automated Tests

All test that run inside of TeamCity are named in a very descriptive manner, GDX-`Test`-`UnityVersion`-`Variant`.

### GDX-GenerateProject

The first set of automated tests triggered are used to ensure that any API usage does not cause any compilation issues against the different versions of Unity supported. They additionally also try different inclusions of packages to ensure the `#ifdef` are exercised; this is the empty/packages variant definition. The run ends with the generation of the projects solution files via [`GDX.Editor.Automation.GenerateProjectFiles`](xref:GDX.Editor.Automation.GenerateProjectFiles) which are stored as artifacts for future use.

### GDX-EditMode

Next, assuming the previous test in the series has been successful, an associated *GDX-EditMode* test will run. These perform author-time unit testing (powered by UTR) validating expected outcomes from direct API usage calls.

### GDX-BVT

Finally, assuming the previous test in the series has been successful, multiple builds will be made to run build verification tests. Currently Windows and macOS are covered at this point, with both Mono and IL2CPP backends being exercised. This works out to 8 builds per supported Unity version.

## Documentation

In parallel to all of the automated testing, documentation is generated for the [`dev`](https://github.com/dotBunny/GDX/tree/dev) and gets posted to [https://gdx-dev.dotbunny.com/](https://gdx-dev.dotbunny.com/). Additionally, when a commit is made to the [`main`](https://github.com/dotBunny/GDX/tree/main) branch, documentation is likewise generated and posted to [https://gdx.dotbunny.com/](https://gdx.dotbunny.com/).

Some of the generated content which is more for developer reference can be found below.

Artifact | Description
:--- | :---
[Code Coverage](/reports/coverage/Report/index.html "Code Coverage") | How much of the codebase is covered by unit testing? This is what drives the badge on the front page.
[Inspection](/reports/inspection/inspection.xml "Inspection") | The results from static analysis of the codebase, including violations of its predetermined style guide.
[Duplication](/reports/duplicates/duplicates.xml "Duplication") | How much duplication of code is there? This one is tricky as we don't shy away from writing boilerplate code to enable performance.
[Portability](/reports/portability/index.html "Portability") | A quick series of tests to identify what versions of .NET is required for GDX to function fully.
