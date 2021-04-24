# Frequently Asked Questions

## Why are exceptions not protected against?
_Garbage in, garbage out_ - Branching inside methods to handle scenarios that should have been protected against hire up in the call stack adds overhead and tech debt.