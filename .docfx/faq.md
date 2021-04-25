# Frequently Asked Questions

## TLDR; License
The big reason for switching to [BSL-1.0](https://choosealicense.com/licenses/bsl-1.0/) is that you no longer need to include any notice in your distributed game about the inclusion of GDX.

You can just drop the code in your project and start working; the only ask is that you leave the source files headers alone. 
You can even distribute the GDX source code this way, so long as those headers are left in place.

## Why are exceptions not protected against?
_Garbage in, garbage out_ — Branching inside methods to handle scenarios that should have been protected against hire up in the call stack adds overhead and tech debt.