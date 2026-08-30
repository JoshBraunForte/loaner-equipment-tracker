# Loaner Equipment Tracker — Test Build

ASP.NET Core (Razor Pages) + Entity Framework Core + SQLite. Local, single-PC test build.
See `../../docs/product-roadmap.md` for the approved MVP scope and `../../docs/project-journal.md`
for decisions.

## Prerequisites
- .NET SDK 8.x (installed: 8.0.424)

## Run
```
dotnet run --project src/LoanTracker/LoanTracker.csproj
```
On first run the app creates `loantracker.db` (SQLite) and seeds sample data, then prints the URL
(e.g. `http://localhost:5112`). Open it in a browser and sign in.

To reset to a clean data set, stop the app and delete `loantracker.db*`, then run again.

## Sample accounts (password for all: `Password123!`)
| Username | Role | Can do |
|----------|------|--------|
| `it1` | IT Staff | Inventory, fulfillment queue, loans + check-in, overdue, audit report |
| `mgr1` | Manager | Approve/deny requests from Emma & Evan; see their team's equipment |
| `mgr2` | Manager | Approve/deny requests from Olivia & Oscar; see their team's equipment |
| `emp1` (Emma) | Employee | Request equipment; see own loans/requests. Manager: mgr1 |
| `emp2` (Evan) | Employee | Manager: mgr1 (has an overdue monitor in seed data) |
| `emp3` (Olivia) | Employee | Manager: mgr2 |
| `emp4` (Oscar) | Employee | Manager: mgr2 |

## Try the core loop
1. Sign in as **emp1**, submit a new request → it routes to mgr1.
2. Sign in as **mgr1**, open **Approvals**, approve it.
3. Sign in as **it1**, open **Queue**, assign an available item and a due date.
4. The item now shows on emp1's dashboard and on IT's **Loans** page; **check in** returns it.
5. **Audit** produces the on-demand "who has what" report (with a Print button).

## Notes / scope
- Auth is a simple seeded cookie login (no SSO) per the test constraint. Passwords are hashed
  (ASP.NET Core `PasswordHasher`). Entra ID / SSO is a deferred future item.
- Notifications are in-app only (nav badges + dashboard counts). Email is Phase 2.
- Schema is created via EF `EnsureCreated()` (no migrations) to keep the test build simple.
