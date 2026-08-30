# Architecture — Loaner Equipment Tracker

Owner: Enterprise Architect. Core question: *How should it be designed?*
Status: current for MVP test build.

## Style
Single-process, server-rendered **ASP.NET Core Razor Pages** monolith with **EF Core** over
**SQLite**. Chosen for Simplicity/Speed (top priorities) while preserving a clean path to the
enterprise target (Entra ID + SQL Server). Deliberately not a SPA/API split — unnecessary for the
scope and slower to deliver.

## Components
```
Browser (Bootstrap UI)
   │  cookie auth
   ▼
ASP.NET Core Razor Pages  ── Pages/ (Account, Requests, Approvals, Queue, Loans, Equipment, Audit, Index)
   │                        ── ViewComponents/ (RoleBadges = in-app notifications)
   │                        ── Helpers/ (ClaimsExtensions)
   ▼
AppDbContext (EF Core)  ── Models/ (AppUser, Equipment, LoanRequest, Loan)
   ▼
SQLite (loantracker.db, created via EnsureCreated + DbSeeder)
```

## Domain model
- **AppUser** — Id, UserName, DisplayName, PasswordHash, Role {Employee|Manager|ItStaff},
  self-referencing `ManagerId` (employee → manager; seeded now, Entra later).
- **Equipment** — AssetTag (unique), Name, Category, Status {Available|OnLoan|Retired}, Notes.
- **LoanRequest** — Requester, ItemDescription/Category/Justification, Status
  {Submitted|Approved|Denied|Fulfilled|Cancelled}, manager decision (DecidedBy/At/Note),
  one-to-one link to the fulfilling Loan.
- **Loan** — Equipment, Borrower, optional Request, CheckedOutAt, DueDate, ReturnedAt.
  `IsOverdue` = not returned & past due (computed; not persisted).

Enums persist as text; unique indexes on UserName and AssetTag; delete behaviors set to Restrict
(SetNull for request↔loan) to protect custody history.

## Request lifecycle (state machine)
```
Submitted ──manager approve──▶ Approved ──IT fulfill──▶ Fulfilled ──check-in──▶ (loan returned)
   │                              
   ├──manager deny──▶ Denied     
   └──requester cancel──▶ Cancelled
```
Guards enforced in handlers: only the requester's manager may decide; only available equipment
may be assigned; only the requester may cancel, and only while Submitted.

## Cross-cutting
- **AuthN**: cookie scheme; seeded credentials; PBKDF2 password hashing.
- **AuthZ**: `AuthorizeFolder("/")` (login required everywhere except Account pages); role policies
  `ItStaff`/`Manager`; per-record ownership checks in handlers.
- **Notifications**: in-app only — nav badges (RoleBadges view component) + dashboard counts.
- **Antiforgery**: default Razor Pages tokens on all POSTs.

## Evolution path (design preserves these swaps)
1. **Identity** → replace cookie/seed auth with **Entra ID / Microsoft.Identity.Web**; `AppUser`
   + `ManagerId` populated from directory instead of seeder. UI/authorization unchanged.
2. **Database** → change EF provider from Sqlite to **SQL Server** (LocalDB/Express/Azure SQL);
   swap `EnsureCreated` for **EF Migrations**. Models unchanged.
3. **Notifications** → add email (Phase 2) behind an `INotificationService` abstraction.

## Key decisions (see project-journal.md)
- Enterprise type; priorities Speed → Simplicity → Flexibility → Scalability → Cost.
- Manager-approval workflow (Option B).
- ASP.NET Core + EF Core + SQLite (Option B) with SQLite-now / SQL-Server-later.
- `EnsureCreated` over Migrations for the test build (documented trade-off; migrations before prod).
