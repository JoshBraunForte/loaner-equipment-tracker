# Release Notes & Plan — Loaner Equipment Tracker

Owner: Release Manager.

## Release 0.1.0 — MVP (Test Build) — 2026-08-30
**Type:** internal test / demo build. **Environment:** local single-PC. **Not** a production release.

### Included
- Role-based access: Employee, Manager, IT Staff (seeded accounts, no SSO).
- Equipment inventory management (IT): add/edit, status Available/OnLoan/Retired.
- Request → **manager approval** → **IT fulfillment** workflow.
- Loan tracking, check-in/return, automatic overdue flagging.
- Role dashboards + in-app notification badges.
- On-demand "who has what" audit report (printable).
- Seeded sample data (7 users, 10 items, sample loans incl. one overdue, requests in mixed states).

### Quality gates (all met)
- `dotnet build`: 0 warnings / 0 errors.
- `dotnet test`: 18/18 passing.
- Integration smoke test: full loop + role enforcement + bad-login rejection verified.
- Security assessment completed; test-phase risks explicitly accepted.

### Known limitations (accepted for test)
- Shared demo password; HTTP-only; SQLite; no email; `EnsureCreated` (no migrations); no lockout.

### How to run
See `src/LoanTracker/README.md`.

## Forward release plan (indicative — scope owned by Product Manager)
- **0.2 (Phase 2):** email notifications, overdue reminders/escalation, richer reporting + CSV,
  equipment detail fields. Add EF Migrations. Requires a decision package before starting.
- **1.0 (Production):** Entra ID SSO, SQL Server, HTTPS/HSTS, chosen hosting + CI/CD + monitoring,
  audit/event logging. Gated by Security "required before production" list and Operations readiness.

## Rollback (test)
Stateless app; to revert a bad build, redeploy prior source. To reset data, delete the SQLite file.
