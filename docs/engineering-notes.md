# Engineering Notes — Loaner Equipment Tracker

Owner: Software Engineer. Core question: *How should it be implemented?*

## Solution layout
```
LoanTracker.sln
├─ src/LoanTracker/            ASP.NET Core Razor Pages app (net8.0)
│  ├─ Models/                  domain entities + enums
│  ├─ Data/                    AppDbContext, DbSeeder
│  ├─ Helpers/                 ClaimsExtensions
│  ├─ ViewComponents/          RoleBadges (nav notification badges)
│  ├─ Pages/                   Account, Requests, Approvals, Queue, Loans, Equipment, Audit, Index
│  └─ README.md                run instructions + sample accounts
└─ tests/LoanTracker.Tests/    xUnit domain/workflow tests (18, all passing)
```

## Build / run / test
```
dotnet build            # 0 warnings / 0 errors
dotnet run  --project src/LoanTracker/LoanTracker.csproj
dotnet test             # 18 passing
```
First run creates + seeds `loantracker.db`. Delete `loantracker.db*` to reset.

## Conventions & decisions
- Razor Pages with per-page handlers; state changes are POST handlers with antiforgery.
- Authorization: folder-level login requirement + role attributes + in-handler ownership guards.
- Enums stored as text for readable data; computed `IsOverdue`/`IsReturned` kept out of EF queries
  (queries use explicit `ReturnedAt == null && DueDate < today`).
- `Upsert` page pattern for equipment create/edit to reduce duplication.

## Environment setup performed (was missing on the host)
- Installed **.NET SDK 8.0.424** (winget `Microsoft.DotNet.SDK.8`).
- Registered **nuget.org** package source (none was configured).
- Pinned EF Core **8.0.11** (Sqlite + Design).

## Known trade-offs / TODO before production
- `EnsureCreated` → switch to EF **Migrations**.
- Add `INotificationService` seam for email (Phase 2).
- Add integration (WebApplicationFactory) tests for auth flows to complement handler tests.
- Introduce a thin service layer if workflow rules grow (keeps page handlers thin).
