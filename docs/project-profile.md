# Project Profile

## Project Name
Loaner Equipment Tracker

A simple web application for IT to track loaner equipment: what equipment exists, who
currently has it, when it is due back, and which items are overdue.

## Project Type
Enterprise

## Primary Objective
Provide visibility and accountability: always know who currently has which loaner item and
what is overdue, so IT can answer these questions on demand for audits and reporting.

## Users
Three roles:
1. **Employees (borrowers)** — request equipment; presumably see what they currently have out.
2. **IT staff (operators)** — use the app for tracking: manage equipment inventory, fulfill
   requests, check items in/out, set due dates, record returns, act on overdue items.
3. **Managers** — see what equipment their own employees currently have.

## Success Measures
1. **Adoption** — IT uses the app instead of the old process; managers self-serve to see their
   employees' equipment instead of asking IT.
2. **Audit readiness** — the app can produce a clean, current who-has-what report on demand with
   no manual reconciliation.

## Constraints
- **No SSO** — this is a test build; do not integrate company SSO. A simple built-in login (or
  minimal auth) is acceptable for now.
- **Hosting** — initially runs locally on the user's PC (Windows 11) for testing. Not yet a
  production/shared deployment; portability to a shared host later is desirable but not required now.
- Other constraints (compliance, mandated stack, budget, timeline, integrations): none stated yet.

## Architecture Priorities
**Selected ordering (Option B — Test-adjusted, 2026-08-30):**
1. Speed of delivery
2. Simplicity
3. Flexibility
4. Scalability
5. Cost

Rationale: Enterprise intent retained, but ordering adjusted for the current local single-PC
test phase — Simplicity promoted above Scalability to avoid premature scaling work. Revisit when
moving toward a shared/production deployment.

---

### Personal default
1. Speed of delivery
2. Simplicity
3. Flexibility
4. Cost
5. Scalability

### Enterprise default
1. Speed of delivery
2. Flexibility
3. Scalability
4. Simplicity
5. Cost
