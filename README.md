# <img src="https://github.com/CodeShayk/abacus.net/blob/master/images/ninja-icon-16.png" alt="ninja" style="width:30px;"/> Abacus v1.0.0
[![NuGet version](https://badge.fury.io/nu/abacus.svg)](https://badge.fury.io/nu/abacus) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/codeshayk/abacus/blob/master/License.md) [![build-master](https://github.com/codeshayk/abacus/actions/workflows/Build-Master.yml/badge.svg)](https://github.com/codeshayk/abacus/actions/workflows/Build-Master.yml) [![GitHub Release](https://img.shields.io/github/v/release/codeshayk/abacus?logo=github&sort=semver)](https://github.com/codeshayk/abacus/releases/latest)
[![CodeQL](https://github.com/codeshayk/abacus/actions/workflows/codeql.yml/badge.svg)](https://github.com/codeshayk/abacus/actions/workflows/codeql.yml) [![.Net](https://img.shields.io/badge/.Net-8.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

Simple Workflow framework in .Net
> Allows creating workflow templates with tasks transitions, triggered by outcomes of constituent tasks and entity actions linked to workflow (such as create, update or delete of linked entity).
### Design
![Abscus.Design](images/abscus.design.png)

### Concept
- `Workflow Template` defines a sequence of tasks linked to an action on entity of interest.
- `Task` is an unit of work that needs to be completed and can have an outcome associated upon completion.
- `Workflow Instance` is actual execution of workflow template triggered by an event on the instance of associated entity. Instance persists the current state of the workflow for the entity instance.
- Sequences of Tasks could be configured as multiple `transitions` with triggers driving transitions at different levels of branching.
- Tasks could be `triggered` by an `outcome` of previous task or a `domain event` on the workflow associated entity.

### Example Workflow
![Abscus.Example](images/abscus.example.png)

### Implementation
- Please visit [wiki](https://github.com/CodeShayk/Abacus/wiki) pages for details.
