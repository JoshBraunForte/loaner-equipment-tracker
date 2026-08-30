# Application Factory Operating Model — V1

## Core Principle

AI agents analyze, recommend, challenge, create artifacts, and coordinate work.
Humans make major decisions.

## Decision Escalation

The Project Manager presents only one decision requiring human guidance at a time.

Each decision must include:
1. Decision required
2. Brief context
3. Practical options
4. Concise advantages / disadvantages
5. Relevant agent viewpoints
6. Collective recommendation
7. Confidence level
8. Final option:
   **I need more information and a conversation vs selecting one of your recommendations.**

The Project Manager may consolidate analysis but may not hide meaningful dissent.

## Artifact Governance

- Artifact owners maintain their artifacts.
- Downstream agents may challenge artifacts but may not silently rewrite approved decisions.
- Changes requiring human approval are escalated through the Project Manager.
- The Project Journal records decisions, open questions, assumptions, findings, deferred features, and parking-lot items.
- Deferred scope is never discarded without an explicit human decision.

## Factory Distribution

ApplicationFactory is centrally versioned.
Projects consume a snapshot of approved factory assets using bootstrap/update tooling.
Each project records:
- factory version
- update date
- source
- locally modified factory files

Updates are deliberate rather than automatic.
