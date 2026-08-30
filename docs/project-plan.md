# Project Plan — Loaner Equipment Tracker

Owner: Project Manager. Status as of 2026-08-30.

## Objective
Deliver a local test build proving the loaner-equipment custody loop (request → manager approval →
IT fulfillment → tracking → check-in → overdue → audit), and exercise the full Application Factory
process across all agents.

## Phases & status
| Phase | Owner(s) | Status |
|-------|----------|--------|
| Opportunity brief | Opportunity Analyst | ✅ Done (`opportunity-analysis.md`) |
| Project profile intake | Project Manager | ✅ Done (`project-profile.md`) |
| Product scope / roadmap | Product Manager | ✅ MVP approved & built (`product-roadmap.md`) |
| Architecture & stack decision | Enterprise Architect | ✅ Done (`architecture.md`) |
| Implementation | Software Engineer | ✅ Done (`src/LoanTracker`, `engineering-notes.md`) |
| QA / Quality Assessment | QA | ✅ Done — 18 tests green (`qa-test-plan.md`) |
| Security Assessment | Security | ✅ Done (`security-assessment.md`) |
| Reliability / incidents | Debugging & Reliability Eng. | ✅ 3 env/build incidents resolved (`reliability-log.md`) |
| Operational readiness | Operations | ✅ Test runbook done; prod deferred (`operations-readiness.md`) |
| Release 0.1.0 (test) | Release Manager | ✅ Gates met (`release-notes.md`) |

## Dependencies / critical path
Profile → Roadmap(MVP) → Stack decision → Build → QA/Security → Release. All satisfied for the
test build. Production (1.0) is gated on human decisions for hosting, Entra ID, SQL Server, HTTPS.

## Milestones
- M1 Profile approved — done.
- M2 MVP scope approved — done.
- M3 Stack approved — done.
- M4 MVP built & smoke-tested — done.
- M5 Full-process pass (QA + Security + all assessments) — done.
- M6 (future) Phase 2 kickoff — pending human go decision.

## Open decisions for the human (not blocking the test build)
1. Proceed to Phase 2 (email/reminders/reporting)? 
2. Commit to a production path (Entra ID + SQL Server + hosting)?
Both would be brought as Decision Packages when requested.
