# Product Roadmap

> Status: **MVP APPROVED by human 2026-08-30.** Phase 2 / Future / Parking Lot remain proposed.
> Owner: Product Manager. Derived from `docs/project-profile.md` and decisions in
> `docs/project-journal.md`.

## Guiding intent
Primary objective is **visibility & accountability** — always know who has what and what's
overdue, for audits/reporting. Success = **Adoption** + **Audit readiness**. Priorities:
Speed → Simplicity → Flexibility → Scalability → Cost. Test build: local single-PC, no SSO,
seeded sample data.

## MVP (BUILT ✅ 2026-08-30 — `src/LoanTracker`)
Goal: prove the core loop end-to-end with seeded data on a local PC. All items below delivered
and smoke-tested.

- **People & roles (seeded):** Employees, Managers, IT staff. Each employee has a manager.
  Simple app login (no SSO). Sample data seeded on startup.
- **Equipment inventory:** IT can create/edit equipment items and see status
  (Available / On loan / Overdue).
- **Request → manager approval → fulfillment workflow (Option B):**
  Employee submits request → routed to their manager → manager approves/denies →
  approved request appears in IT queue → IT assigns an item and sets a due date.
- **Loan tracking:** who holds each item, due date; IT records returns (check-in).
- **Overdue visibility:** items past due date are flagged automatically.
- **Views by role:**
  - IT: full inventory, request queue, all loans, overdue list.
  - Manager: what their own employees currently have; approvals awaiting them.
  - Employee: what they currently have; status of their requests.
- **Audit report:** on-demand "who has what" (current holders + due/overdue) export/view.
- **Notifications (in-app only for test):** request/approval status shown in-app; no email.

## Phase 2 (proposed)
- Email notifications (request submitted / approved / due soon / overdue).
- Reminders & escalation for overdue items.
- Richer reporting (history, loan duration, per-department views), CSV export.
- Equipment detail: serial numbers, categories, condition/notes, attachments.

## Future
- **Entra ID integration** — pull employee/manager data and adopt SSO, replacing seeded data
  and app login. (Deferred by human decision 2026-08-30.)
- Shared/production hosting beyond the local PC.
- Self-service catalog / bookable equipment reservations.

## Parking Lot
- Whether managers should also be able to request on behalf of employees.
- Whether IT can override/skip manager approval for urgent loans.
- Barcode/QR scanning for check-in/out.

## Rejected (with rationale)
- (none yet)
