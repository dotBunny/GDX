# Overview

## Automated Tests

Every commit triggers an assortment of automated tests that validate the GDX package. You can see the status of those tests registered against every commit in the **dev** branch on [GitHub](https://github.com/dotBunny/GDX/tree/dev). It starts with a simple "can we generate project files". This tests if any API usage is invalid across different package arrangements and Unity versions. If those are successful, each version will respectively trigger a subsequent set of unit tests (powered by UTR) further validating expected outcomes of direct API calls. Finally, assuming all of the above has passed successfully, a version will trigger a build that will then be ran to evaluate our Build Verification Tests.

## [Coverage](/reports/coverage/Report/index.html)

How much of the codebase is covered by unit tests.
[![Code Coverage](/reports/coverage/Report/badge_linecoverage.svg)](/reports/coverage/Report/index.html)

## [Duplication](/reports/duplicates/duplicates.xml)

Are we duplicating code?

## [Inspection](/reports/inspection/inspection.xml)

Static code analysis of numerous resharper-esque corrections.


## [Portability](/reports/portability/index.html)

A quick series of tests to identify what versions of .NET is required for GDX to function fully.