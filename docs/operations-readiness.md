# Operations Readiness — Loaner Equipment Tracker

Owner: Operations (run-state). Core question: *Is it ready to run, and how do we run it?*
Status: **ready for local test use; NOT production-ready** (by design).

## Current run-state (test)
- Runtime: .NET 8 on Windows 11, `dotnet run`, HTTP on localhost (auto-assigned port, e.g. 5112).
- Data: single SQLite file `loantracker.db` in the app content root; schema auto-created + seeded.
- Lifecycle: start = `dotnet run`; stop = Ctrl+C; reset = delete `loantracker.db*` and restart.
- Observability: console logging (EF command logging in Development).

## Runbook (test)
| Task | Action |
|------|--------|
| Start | `dotnet run --project src/LoanTracker/LoanTracker.csproj` |
| Reset data | stop app, delete `src/LoanTracker/loantracker.db*`, start again |
| Back up test data | copy `loantracker.db` while app stopped |
| Change port | set `ASPNETCORE_URLS` or edit `Properties/launchSettings.json` |

## Not yet in place (required before production — hand-offs)
- **Hosting**: shared host / container / IIS / Azure App Service — undecided (Future item).
- **HTTPS/TLS**: certificate + HSTS.
- **Database**: move to SQL Server; managed backups; migrations pipeline.
- **Identity**: Entra ID integration (removes local accounts).
- **Monitoring/alerting**: health checks, structured logs, uptime + overdue-job monitoring.
- **CI/CD**: build/test/deploy pipeline (tests already automatable via `dotnet test`).
- **Scheduled jobs**: if email reminders (Phase 2) are added, a scheduler/host is required.

## Recommendation
Operations has no production run-state to own yet. Re-engage when a deployment target is chosen
(the Future roadmap item), at which point Operations owns the production runbook, monitoring, and
backup/restore procedures.
