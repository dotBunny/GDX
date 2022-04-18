# Frequently Asked Questions

## What does this license mean?

The big reason for switching to [BSL-1.0](https://choosealicense.com/licenses/bsl-1.0/) license is that you no longer need to include any notice in your distributed game about the inclusion of `GDX`. This is the clear difference with this new licensing model, compared to the [MIT](https://choosealicense.com/licenses/mit/) license, which requires notice in produced materials.

Therefore, you can drop the code in your project and start working. The only ask is to leave the source files headers alone. 
You can even distribute the `GDX` source code this way, so long as those headers are left in place.

## Why are exceptions not protected against?

_Garbage in, garbage out_ — Branching inside methods to handle scenarios that should have been protected against hire up in the call stack adds overhead and tech debt. `GDX` wants no part in **your** architectural problem.

> [!NOTE]
> There are some scenarios where exceptions are thrown to match existing expected behaviours.
