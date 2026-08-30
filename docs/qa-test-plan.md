# QA Test Plan & Results — Loaner Equipment Tracker (MVP)

Owner: QA. Status: **executed, all green (2026-08-30).**

## Scope
Validate the approved MVP loop: people/roles, request → manager approval → IT fulfillment →
loan tracking → check-in → overdue flagging → audit visibility, plus authentication and role
boundaries.

## Test approach
Two layers:
1. **Automated unit/workflow tests** — xUnit project `tests/LoanTracker.Tests`, exercising the
   real Razor Page handlers and domain model against an in-memory SQLite database (schema created
   via `EnsureCreated`, seeded with the production seeder). Run with `dotnet test`.
2. **Integration smoke test** — scripted HTTP walkthrough against the running app (login +
   antiforgery, role access, full approve→fulfill→checkin loop). Documented in the project journal.

## Automated coverage (18 tests, all passing)

| Area | Tests |
|------|-------|
| Seed integrity | user/equipment/loan/request counts; idempotent re-seed; manager relationships; **passwords hashed, not plaintext**; exactly one overdue loan |
| Approval workflow | manager approves own report; manager denies; **other manager cannot decide another team's request** (authorization guard) |
| Fulfillment | approved request → loan created, equipment → OnLoan, request → Fulfilled; **cannot fulfill with unavailable equipment** (guard) |
| Check-in | loan returned, equipment freed to Available |
| Requests | employee cancels own submitted request; **cannot cancel another user's request** (guard); new request is Submitted and routes to requester's manager |
| Overdue model | overdue when past due & unreturned; not overdue in future; returned never overdue; due-today not overdue (boundary) |

## Integration smoke results (manual/scripted)
- Login for Employee/Manager/IT; **bad password rejected**.
- Employee blocked from IT-only pages → Access-denied page, **no data leakage**.
- Full loop verified end-to-end; borrower sees fulfilled item on dashboard; audit report renders.

## Gaps / recommended for later (not blocking the test build)
- No end-to-end browser (UI) automation; role enforcement is covered at HTTP + handler level.
- No load/performance testing (out of scope for local single-PC test).
- Concurrency (two IT staff fulfilling the same item simultaneously) not tested — low risk at test
  scale; revisit before multi-user production.

## How to run
```
dotnet test
```
