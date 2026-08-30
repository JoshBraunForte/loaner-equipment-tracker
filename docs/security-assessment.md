# Security Assessment — Loaner Equipment Tracker (Test Build)

Owner: Security. Core question: *How would I attack it?*
Status: **assessment complete for the local test build; not cleared for production.**

## Context & posture
Local, single-PC test build. No SSO by explicit decision; seeded sample identities; SQLite file
DB. Security concerns receive highest consideration but humans retain final authority; several
items below are **accepted risks for the test phase** and must be revisited before any shared
deployment.

## Assets
- Employee/manager identities (sample data now; real PII once Entra ID is integrated).
- Equipment inventory and custody records (who has what) — the audit source of truth.
- Session/auth cookies.

## Trust boundaries
Browser ↔ ASP.NET Core app ↔ SQLite file. Single host; no external integrations yet.

## Findings

| # | Severity (test / prod) | Finding | Status / Mitigation |
|---|------------------------|---------|---------------------|
| S1 | Low / **High** | Shared, well-known seed password (`Password123!`) for all accounts | Accept for test. Prod: replace with Entra ID SSO (no local passwords). |
| S2 | Low / Med | App-managed cookie auth instead of enterprise IdP | By design for test. Path to Entra via Microsoft.Identity.Web already reserved in architecture. |
| S3 | Info | Passwords hashed with ASP.NET Core `PasswordHasher` (PBKDF2) | Implemented & unit-tested (no plaintext at rest). |
| S4 | Low / Med | HTTP only (no HTTPS/HSTS) on local run | Accept for localhost test. Prod: enforce HTTPS + HSTS. |
| S5 | Resolved | Authorization / horizontal access | Role checks on IT/Manager pages; **manager can only decide own reports**; **users can only cancel own requests** — verified by tests + smoke test (no data leak). |
| S6 | Resolved | CSRF | Razor Pages antiforgery tokens on all state-changing POSTs (login, approve/deny, fulfill, check-in, create/edit). |
| S7 | Low | No audit log of who approved/fulfilled/checked-in beyond current-state fields | Acceptable for MVP; `DecidedBy/At` captured. Consider immutable event log for compliance later. |
| S8 | Low / Med | No account lockout / rate limiting on login | Accept for test; add before internet-facing exposure. |
| S9 | Info | SQLite file unencrypted, world-readable on host | Fine on personal test PC; prod DB (SQL Server) needs at-rest encryption + access control. |

## Attack paths considered
- **Privilege escalation** (employee → IT actions): blocked by role policies; confirmed employee
  hitting `/Equipment`, `/Queue` gets Access-denied with no data returned.
- **Horizontal (IDOR)**: approving/cancelling by manipulating ids — blocked by ownership guards
  (manager-of-record, requester-of-record). Covered by tests S5.
- **CSRF**: mitigated by antiforgery (S6).
- **Credential attacks**: in-scope risk for prod (S1, S8); out-of-scope for test.

## Required before production (human decision to accept/deny each)
1. Replace seed auth with **Entra ID SSO** (removes S1, S2, S8).
2. Enforce **HTTPS/HSTS** (S4).
3. Move to **SQL Server** with at-rest encryption and least-privilege DB account (S9).
4. Add **audit/event logging** for custody changes (S7).
