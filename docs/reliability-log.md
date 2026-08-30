# Reliability & Incident Log — Loaner Equipment Tracker

Owner: Debugging & Reliability Engineer. Activated on observed failures during the build.
Each entry: root cause (vs symptom), smallest safe fix, prevention.

## INC-1 — `dotnet` commands failed: "No .NET SDKs were found"
- **Symptom:** `dotnet --version` / project restore failed.
- **Root cause:** .NET runtime host present but **no SDK installed** on the host.
- **Fix:** installed .NET SDK 8.0.424 via `winget install Microsoft.DotNet.SDK.8`.
- **Prevention:** documented prerequisite in `src/LoanTracker/README.md`; verify with
  `dotnet --list-sdks` before building. Related paths: any future CI runner needs the SDK.

## INC-2 — NuGet restore failed: NU1100 "Unable to resolve Microsoft.EntityFrameworkCore.*"
- **Symptom:** package add/restore failed even with valid versions.
- **Root cause (not the version):** **no NuGet package source configured** (`dotnet nuget list
  source` → "No sources found"). The template restored only from bundled framework refs, masking
  the gap until the first real package fetch.
- **Fix:** registered `nuget.org` (`dotnet nuget add source https://api.nuget.org/v3/index.json`).
- **Prevention:** documented in engineering notes; a CI restore step would surface this immediately.

## INC-3 — Build error CS0246: `HtmlContentViewComponentResult` not found
- **Symptom:** compile failed in `RoleBadgesViewComponent`.
- **Root cause:** type referenced without its namespace
  (`Microsoft.AspNetCore.Mvc.ViewComponents`).
- **Fix:** fully-qualified the type name (smallest safe change).
- **Prevention:** covered indirectly by the build gate; the component renders in the app and is
  exercised on every authenticated page load.

## Notes
- No runtime/production incidents (test build only). The three items above were environment/build
  issues, all resolved; final state builds clean (0/0) and `dotnet test` is 18/18 green.
- Regression protection for domain/workflow behavior lives in `tests/LoanTracker.Tests` (18 tests),
  including authorization guards that previously could have regressed silently.
