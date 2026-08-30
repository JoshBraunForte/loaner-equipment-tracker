# Project Journal

## Open Questions
- (none open)

- [RESOLVED 2026-08-30] How are employee/manager relationships known? → For the test, use **made-up
  sample/seed data** (employees each with a manager). Real data will come from **Entra ID** in a
  later process (see Deferred Features). Data model still needs an employee "manager" field.
- [RESOLVED 2026-08-30 via assumption] Scale for the test is small and PM's choice (local, seeded
  data). Working assumption below; flag if real scale is materially larger.
- [RESOLVED 2026-08-30] Does "request" imply approval? → Yes, **manager approval** (Option B). See Decisions.

## Decisions
- 2026-08-30 — Project name: "Loaner Equipment Tracker" (web app for IT to track loaner equipment).
- 2026-08-30 — Project Type: **Enterprise**. Sets default Architecture Priority ordering:
  Speed → Flexibility → Scalability → Simplicity → Cost.

- 2026-08-30 — Constraints: **No SSO** for the test build; **hosted locally** on the user's
  Windows 11 PC for testing. No compliance/stack/budget/timeline/integration constraints stated.
- 2026-08-30 — Success Measures: (1) Adoption; (2) Audit readiness.
- 2026-08-30 — Primary Objective: visibility & accountability (who has what, what's overdue) for
  audits/reporting.
- 2026-08-30 — Users: Employees (request), IT staff (operate/track), Managers (view own reports).
- 2026-08-30 — **Architecture Priority ordering: Option B (Test-adjusted)** —
  Speed → Simplicity → Flexibility → Scalability → Cost. Revisit at production/shared deployment.
- 2026-08-30 — **Request workflow: Option B — Manager approval.** Employee submits a request →
  their manager approves/denies → approved requests flow to IT to fulfill (assign item, set due date).
- 2026-08-30 — **Test data:** seed the app with **made-up sample data** (employees, managers,
  equipment, sample loans/requests). No real identity source for the test.
- 2026-08-30 — **MVP scope APPROVED** as proposed in `docs/product-roadmap.md` (in-app
  notifications only; email/reminders/richer reporting deferred to Phase 2; Entra ID/SSO Future).
- 2026-08-30 — **Tech stack: Option B — ASP.NET Core (C#) + Entity Framework Core + SQLite**
  for the local Windows test. Rationale: Simplicity now (file-based SQLite), Flexibility later
  (EF Core provider swap to SQL Server; Microsoft.Identity.Web path to Entra SSO). Dependency:
  .NET SDK must be installed locally.

- 2026-08-30 — **MVP built and smoke-tested.** ASP.NET Core Razor Pages app at `src/LoanTracker`
  builds clean (0 warnings/0 errors). Verified end-to-end: role-based login (Employee/Manager/IT),
  role access enforcement (employee blocked from IT pages → Access denied, no data leak), full
  request → manager approval → IT fulfillment → loan/check-in loop, overdue flagging, and the
  audit report. Run via `dotnet run --project src/LoanTracker/LoanTracker.csproj`; see
  `src/LoanTracker/README.md` for sample accounts (password `Password123!`).

## Assumptions
- Test scale is small: on the order of ~25–50 sample employees, a few managers, 1–3 IT staff,
  and ~40–75 equipment items. Enough to exercise approval routing and overdue reporting without
  performance concerns. Revisit at real deployment.
- The app manages its own login for the test (no SSO); auth/people model designed so Entra ID
  can replace it later.

## Issues & Findings
- 2026-08-30 — Manager-approval (Option B) pulls in scope that must be reflected in the roadmap:
  (a) request has states (Submitted → Approved/Denied → Fulfilled → Returned/Overdue);
  (b) requests must **route to the correct manager**, which depends on the employee→manager
  relationship — reinforces the need for that data (see Assumptions);
  (c) approvers/requesters need to be **notified** — for a local test, in-app status may suffice
  instead of email (open sub-question: is email needed?).
- 2026-08-30 — **Environment gap:** `dotnet --list-sdks` reported **no .NET SDK installed** on the
  PC. Resolved: installed **.NET SDK 8.0.424** via `winget install Microsoft.DotNet.SDK.8`.
- 2026-08-30 — **Environment gap #2:** no NuGet package source was configured (`dotnet nuget list
  source` → "No sources found"), so EF Core packages wouldn't restore. Resolved: registered
  `nuget.org` (https://api.nuget.org/v3/index.json).
- 2026-08-30 — Build stack pinned: EF Core **8.0.11** (Sqlite + Design) on **net8.0**,
  ASP.NET Core **Razor Pages** app at `src/LoanTracker`.

- 2026-08-30 — **Full-process pass complete.** All ten factory agent roles engaged; each owned
  artifact produced under `docs/`. QA: xUnit suite `tests/LoanTracker.Tests` (18 tests, all green)
  covering seed integrity, approval/fulfillment/check-in workflow, authorization guards, and
  overdue boundaries. Security Assessment completed (test-phase risks accepted; production gates
  listed). Reliability log captured 3 resolved env/build incidents. Solution file `LoanTracker.sln`
  ties app + tests.
  New artifacts: qa-test-plan, security-assessment, architecture, engineering-notes,
  operations-readiness, release-notes, opportunity-analysis, reliability-log, project-plan,
  project-status-report.

## Deferred Features
- **Entra ID integration** — pull employee/manager (and likely auth/SSO) data from Microsoft
  Entra ID in a later phase, replacing the seeded sample data. Design the people/auth model so
  this can be swapped in without rework. — deferred 2026-08-30 (human decision to defer)

## Parking Lot
