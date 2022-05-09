# Visual Scripting Support

> [!WARNING]
> There is an inherent inefficiency in utilizing visual scripting; in most cases, hand-crafted code will outperform generated code. While `GDX` operates on the principle of performance first, this does not preclude the idea of providing performant nodes for visual scripting graphs.

## Adding GDX To Visual Scripting

An assembly and its types need to be added to the unit list configuration of Visual Scripting to be usable. By default, this is a manual process available in the `Visual Scripting` section of the **Project Settings**. To speed this process up a bit and generally create a much nicer workflow, `GDX` provides a shortcutted workflow.

![Bolt Configuration](/images/manual/features/visual-scripting/bolt-setup.png)

A curated selection of functionality for `Visual Scripting` is available by clicking category checkboxes  (**A**), and then clicking the **Regenerate Units** button (**B**). This button replicates the same functionality available in the `Visual Scripting` section of the **Project Settings**.  The **Install Docs** button copies over a simplified version of XML documentation into a specific folder in your project so that `Visual Scripting` will have documentation for `GDX` based units.  Toggling the dropdown arrows (**C**) will display a list of the types added by this category; clicking on a type will open its documentation.