# Frequently Asked Questions

## What does this license mean?

The big reason for switching to [BSL-1.0](https://choosealicense.com/licenses/bsl-1.0/) license is that you no longer need to include any notice in your distributed game about the inclusion of `GDX`. This is the clear difference with this new licensing model, compared to the [MIT](https://choosealicense.com/licenses/mit/) license, which requires notice in produced materials.

Therefore, you can drop the code in your project and start working. The only ask is to leave the source files headers alone. 
You can even distribute the `GDX` source code this way, so long as those headers are left in place.

## Why are exceptions not protected against?

_Garbage in, garbage out_ — Branching inside methods to handle scenarios that should have been protected against hire up in the call stack adds overhead and tech debt. `GDX` wants no part in **your** architectural problem.

> [!NOTE]
> There are some scenarios where exceptions are thrown to match existing expected behaviours.

## Why doesn't GDX have <_insert feature here_>?

Let us reverse that question. Why haven't **you** added that feature to `GDX`?

In all honesty, we are a relatively small team of dedicated developers who have actual games/engines to make. Things get added ad-hoc as we find the time or need for them. We are always open to contributions and would encourage everyone to help their fellow game developer.