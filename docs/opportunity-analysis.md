# Opportunity Analysis — Loaner Equipment Tracker

Owner: Opportunity Analyst. Core question: *Is this worth doing, and where's the value?*
Status: brief (this engagement's primary purpose was to exercise the factory process).

## Problem & value
IT teams commonly track loaner equipment in spreadsheets/email, which loses custody visibility and
makes audits manual. The value is **accountability on demand** (who has what, what's overdue) plus
**IT time saved** and **manager self-service** — matching the two success measures (Adoption,
Audit readiness).

## Who benefits
- **IT staff**: less manual chasing; single source of truth for custody.
- **Managers**: self-serve visibility into their team's equipment; fewer "who has X?" pings to IT.
- **Employees**: clear request path and status.
- **Org/audit**: clean, reproducible custody report.

## Alternatives considered
- **Spreadsheet (status quo)**: cheapest, but no workflow, no roles, error-prone, poor audit trail.
- **Commercial ITAM/CMDB**: powerful but heavier and costlier than needed for loaner tracking;
  worth revisiting only if asset management scope expands well beyond loaners.
- **Custom app (chosen)**: right-sized; owns the exact workflow (manager approval) and integrates
  with the org's Microsoft/Entra identity later.

## Sizing signals (to confirm with real data)
Value scales with headcount, loaner volume, and audit frequency. At the assumed test scale
(tens of employees, ~dozens of items) the payoff is process quality and audit readiness rather than
raw hours; the case strengthens with volume.

## Recommendation
Proceed as an internal tool. Keep scope tight (loaner custody + approval + audit); resist scope
creep into full asset management unless a separate opportunity justifies it (Parking Lot).
