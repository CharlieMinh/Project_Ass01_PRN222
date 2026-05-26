# PRN222 Project Progress

Project: Scientific Journal Publication Trend Tracking System  
Technology: ASP.NET Core MVC, SQL Server, Entity Framework Core  
Last updated: 2026-05-26

## Current Status

The project has completed the foundation setup, authentication milestone, Admin/User authorization foundation, all Step 8 core CRUD modules, Step 9 many-to-many relationship management, Step 10 paper search/detail, Step 12 dashboard/charts, and Step 13 bookmark/follow/notification personalization. The application can run locally, connect to SQL Server, register users, log in, log out, display role-based navigation, let Admin/Researcher manage core academic data, assign the key academic relationships, search/view paper details, show trend dashboards, bookmark papers, follow keywords/topics/journals, and view notifications.

Local URL:

```text
http://localhost:5111
```

## Completed Steps

### Step 1: Create ASP.NET Core MVC Project

Status: Completed before Codex implementation.

Notes:
- Existing solution structure is multi-layered:
  - `Scientific.WebAppMVC`
  - `Scientific.Entities`
  - `Scientific.Repositories`
  - `Scientific.Services`

### Step 2: Run SQL Script To Create Database

Status: Completed before Codex implementation.

Database:

```text
ScientificJournalTrendDB
```

### Step 3: Scaffold Models + DbContext With EF Core

Status: Completed before Codex implementation.

Main DbContext:

```text
Scientific.Entities/Models/ScientificJournalTrendDBContext.cs
```

### Step 4: Test Database Connection

Status: Verified during Codex implementation.

Notes:
- Database exists locally.
- User and role data were checked through SQL Server.
- Existing users and roles were found.

### Step 5: Create Shared Layout And Menu

Status: Completed.

Implemented:
- Updated shared layout navigation.
- Added user menu items:
  - Home
  - Search Papers
  - Trend Dashboard
  - Trending Topics
  - My Bookmarks
  - My Following
  - Notifications
- Added authentication menu:
  - Login
  - Register
  - Profile
  - Logout
- Added Admin dropdown menu shown only for Admin users.
- Kept link to existing `JournalsMinhPvs` controller so MinhPV's current work is still reachable.

Main file:

```text
Scientific.WebAppMVC/Views/Shared/_Layout.cshtml
```

### Step 6: Code Login / Register / Logout

Status: Completed and manually tested successfully.

Implemented:
- `AccountController`
- Login page
- Register page
- Logout action
- Profile page
- Access Denied page
- Cookie authentication
- Claims-based login with:
  - User id
  - Full name
  - Email
  - Roles
- Register creates a new active user.
- Register stores password as SHA256 hash.
- Login supports SHA256 hashes for new users.
- Login also supports existing BCrypt hashes in the current seed data.

Main files:

```text
Scientific.WebAppMVC/Controllers/AccountController.cs
Scientific.WebAppMVC/ViewModels/LoginViewModel.cs
Scientific.WebAppMVC/ViewModels/RegisterViewModel.cs
Scientific.WebAppMVC/Views/Account/Login.cshtml
Scientific.WebAppMVC/Views/Account/Register.cshtml
Scientific.WebAppMVC/Views/Account/Profile.cshtml
Scientific.WebAppMVC/Views/Account/AccessDenied.cshtml
Scientific.Services/UsersHuyDdSevice.cs
Scientific.Repositories/UserRepository.cs
```

Demo login accounts reset for testing:

```text
Admin email: trungvd@admin.sjt.vn
Admin password: Admin@123

User email: anhptl@student.hcmus.edu.vn
User password: User@123
```

### Step 7: Code Admin/User Authorization

Status: Completed.

Implemented:
- Cookie authentication registered in `Program.cs`.
- `UseAuthentication()` added before `UseAuthorization()`.
- Centralized role names in `AppRoles`.
- Centralized authorization policy names in `AppPolicies`.
- Registered authorization policies:
  - `AdminOnly`
  - `AcademicUser`
  - `DataManager`
- Admin-only `AdminController`.
- Admin-only authorization added to `JournalsMinhPvsController`.
- Admin layout menu now checks role through `AppRoles.Admin`.
- Current DB role model is supported:
  - `Admin`
  - `Researcher`
  - `Lecturer`
  - `Student`
  - `LecturerStudent`
- Unauthenticated users are redirected to `/Account/Login`.
- Unauthorized users are sent to `/Account/AccessDenied`.

Main files:

```text
Scientific.WebAppMVC/Program.cs
Scientific.WebAppMVC/Authorization/AppRoles.cs
Scientific.WebAppMVC/Authorization/AppPolicies.cs
Scientific.WebAppMVC/Controllers/AdminController.cs
Scientific.WebAppMVC/Controllers/JournalsMinhPvsController.cs
Scientific.WebAppMVC/Views/Shared/_Layout.cshtml
```

## Verification

Build result:

```text
dotnet build
Build succeeded.
7 Warning(s)
0 Error(s)
```

Current warnings:
- Existing nullable warnings in `Scientific.Repositories/GenericRepository.cs`.
- Existing nullable warning in `Scientific.Repositories/JournalRepository.cs`.
- These warnings are not from the new authorization implementation and do not block running the app.

Smoke test result:

```text
/Account/Login       -> 200 OK
/Account/Register    -> 200 OK
/Admin without login -> redirects to /Account/Login?ReturnUrl=/Admin
/JournalsMinhPvs without login -> redirects to /Account/Login?ReturnUrl=/JournalsMinhPvs
```

Manual test result:
- Login works.
- Register works.
- Logout works.
- Admin/user menu behavior works.
- Admin can open `/Users`.
- Admin can open `/Roles`.

### Step 8A: Core CRUD - Users / Roles

Status: Completed.

Implemented:
- Admin-only `UsersController`.
- Admin-only `RolesController`.
- User list with search by name, email, or organization.
- User details page.
- User create page.
- User edit page.
- User delete confirmation page.
- User lock/unlock action.
- User create/edit supports role assignment.
- User create/edit stores password with SHA256 hash.
- Role list with assigned-user count.
- Role details page with assigned users.
- Role create page.
- Role edit page.
- Role delete confirmation page.
- Role delete is blocked when users are assigned to that role.
- User delete is blocked safely when the account is the current Admin or related records prevent deletion.

Main files:

```text
Scientific.WebAppMVC/Controllers/UsersController.cs
Scientific.WebAppMVC/Controllers/RolesController.cs
Scientific.WebAppMVC/ViewModels/UserAdminFormViewModel.cs
Scientific.WebAppMVC/ViewModels/RoleFormViewModel.cs
Scientific.WebAppMVC/ViewModels/RoleOptionViewModel.cs
Scientific.WebAppMVC/Views/Users/*
Scientific.WebAppMVC/Views/Roles/*
```

Verification:

```text
dotnet build
Build succeeded.
0 Warning(s)
0 Error(s)

Admin login: trungvd@admin.sjt.vn / Admin@123
/Users -> 200 OK
/Roles -> 200 OK
```

### Step 8B: Core CRUD - Publisher / Category / Journal

Status: Completed.

Implemented:
- Admin-only `PublishersController`.
- Admin-only `CategoriesController`.
- Admin-only `JournalsController`.
- Publisher CRUD:
  - Index
  - Create
  - Edit
  - Delete
  - Details
- Category CRUD:
  - Index
  - Create
  - Edit
  - Delete
  - Details
- Journal CRUD:
  - Index
  - Create
  - Edit
  - Delete
  - Details
- Journal create/edit uses a Publisher dropdown.
- Journal detail displays:
  - Journal name
  - Publisher
  - ISSN / EISSN
  - Impact factor
  - Ranking
  - Country
  - Paper count
- Delete actions are protected with friendly messages when related records block deletion.

Main files:

```text
Scientific.WebAppMVC/Controllers/PublishersController.cs
Scientific.WebAppMVC/Controllers/CategoriesController.cs
Scientific.WebAppMVC/Controllers/JournalsController.cs
Scientific.WebAppMVC/ViewModels/JournalFormViewModel.cs
Scientific.WebAppMVC/Views/Publishers/*
Scientific.WebAppMVC/Views/Categories/*
Scientific.WebAppMVC/Views/Journals/*
```

### Step 8C: Core CRUD - Author / Paper

Status: Completed.

Implemented:
- Admin/Researcher `AuthorsController`.
- Admin/Researcher `PapersController`.
- Author CRUD:
  - Index
  - Create
  - Edit
  - Delete
  - Details
- Paper CRUD:
  - Index
  - Create
  - Edit
  - Delete
  - Details
- Paper create/edit uses a Journal dropdown.
- Paper create sets `CreatedBy_HuyDD` from the current logged-in user when available.
- Paper create auto-fills `PublicationYear` from `PublicationDate` when year is not entered.
- Paper detail displays journal, DOI, publication date/year, authors, keywords, and abstract.
- Assigning authors/keywords to papers is intentionally left for Step 9 many-to-many relationship management.

Main files:

```text
Scientific.WebAppMVC/Controllers/AuthorsController.cs
Scientific.WebAppMVC/Controllers/PapersController.cs
Scientific.WebAppMVC/ViewModels/PaperFormViewModel.cs
Scientific.WebAppMVC/Views/Authors/*
Scientific.WebAppMVC/Views/Papers/*
```

### Step 8D: Core CRUD - Keyword / Topic

Status: Completed.

Implemented:
- Admin/Researcher `KeywordsController`.
- Admin/Researcher `TopicsController`.
- Keyword CRUD:
  - Index
  - Create
  - Edit
  - Delete
  - Details
- Topic CRUD:
  - Index
  - Create
  - Edit
  - Delete
  - Details
- Keyword and Topic names are checked for duplicates.
- Delete actions are protected with friendly messages when related records block deletion.
- TrendRecords CRUD is intentionally left out of Step 8 because trend data will be created by seed/service later.

Main files:

```text
Scientific.WebAppMVC/Controllers/KeywordsController.cs
Scientific.WebAppMVC/Controllers/TopicsController.cs
Scientific.WebAppMVC/Views/Keywords/*
Scientific.WebAppMVC/Views/Topics/*
```

### Step 8 Final Checklist

Status: Passed.

| Check | Result |
|---|---|
| Admin can open CRUD pages | Passed |
| Normal user cannot open Admin/DataManager CRUD pages | Passed |
| Create Publisher | Passed |
| Create Journal with Publisher dropdown | Passed |
| Create Author | Passed |
| Create Paper with Journal dropdown | Passed |
| Create Keyword | Passed |
| Create Topic | Passed |
| Build has no errors | Passed |

Verification details:

```text
dotnet build
Build succeeded.
0 Warning(s)
0 Error(s)

Admin test account:
trungvd@admin.sjt.vn / Admin@123

Admin route smoke test:
/Publishers        -> 200 OK
/Publishers/Create -> 200 OK
/Categories        -> 200 OK
/Categories/Create -> 200 OK
/Journals          -> 200 OK
/Journals/Create   -> 200 OK
/Authors           -> 200 OK
/Authors/Create    -> 200 OK
/Papers            -> 200 OK
/Papers/Create     -> 200 OK
/Keywords          -> 200 OK
/Keywords/Create   -> 200 OK
/Topics            -> 200 OK
/Topics/Create     -> 200 OK

Temporary create-post test:
Publisher -> created successfully
Category  -> created successfully
Journal   -> created successfully with Publisher
Author    -> created successfully
Paper     -> created successfully with Journal
Keyword   -> created successfully
Topic     -> created successfully
Temporary test records were cleaned from the database after verification.

Normal user test account:
anhptl@student.hcmus.edu.vn / User@123

Normal user access:
/Publishers -> AccessDenied
/Authors    -> AccessDenied
/Papers     -> AccessDenied
/Keywords   -> AccessDenied
/Users      -> AccessDenied
/Roles      -> AccessDenied
```

## Next Steps

### Step 8: Code Core CRUD By Member

Status: Completed.

Completed:

1. Users / Roles CRUD for Admin.
2. Publishers / Journals / Categories CRUD.
3. Authors / Papers CRUD.
4. Keywords / Topics CRUD.

### Step 9: Code Many-To-Many Relationships

Status: Completed.

Implemented:
- User - Role assignment.
- Journal - Category assignment.
- Paper - Author assignment.
- Paper - Keyword assignment.
- Topic - Keyword assignment.

#### 9.1 User - Role

Implemented:
- Route: `/Users/AssignRoles/{userId}`
- Admin can assign one or more roles to a user with checkboxes.
- Save behavior clears existing roles and inserts the selected roles again.
- At least one role is required.
- User list and detail pages now include an Assign Roles action.

Main files:

```text
Scientific.WebAppMVC/Controllers/UsersController.cs
Scientific.WebAppMVC/Views/Users/AssignRoles.cshtml
```

#### 9.2 Journal - Category

Implemented:
- Route: `/Journals/AssignCategories/{journalId}`
- Admin can assign multiple categories to a journal with checkboxes.
- Save behavior clears existing categories and inserts the selected categories again.
- Journal list and detail pages now include an Assign Categories action.
- Journal detail now displays assigned categories.

Main files:

```text
Scientific.WebAppMVC/Controllers/JournalsController.cs
Scientific.WebAppMVC/Views/Journals/AssignCategories.cshtml
Scientific.WebAppMVC/Views/Journals/Details.cshtml
```

#### 9.3 Paper - Author

Implemented:
- Route: `/Papers/AssignAuthors/{paperId}`
- Admin/Researcher can assign multiple authors to a paper with checkboxes.
- First version uses the simple approach:
  - AuthorOrder is assigned automatically by selected author ID order.
  - IsCorrespondingAuthor is set to false by default.
- Paper list and detail pages now include an Assign Authors action.
- Paper detail already displays assigned authors.

Main files:

```text
Scientific.WebAppMVC/Controllers/PapersController.cs
Scientific.WebAppMVC/Views/Papers/AssignAuthors.cshtml
Scientific.WebAppMVC/Views/Papers/Details.cshtml
```

#### 9.4 Paper - Keyword

Implemented:
- Route: `/Papers/AssignKeywords/{paperId}`
- Admin/Researcher can assign multiple keywords to a paper with checkboxes.
- Save behavior clears existing keywords and inserts the selected keywords again.
- Paper list and detail pages now include an Assign Keywords action.
- Paper detail displays assigned keywords.

Main files:

```text
Scientific.WebAppMVC/Controllers/PapersController.cs
Scientific.WebAppMVC/Views/Papers/AssignKeywords.cshtml
Scientific.WebAppMVC/Views/Papers/Details.cshtml
```

#### 9.5 Topic - Keyword

Implemented:
- Route: `/Topics/AssignKeywords/{topicId}`
- Admin/Researcher can assign multiple keywords to a topic with checkboxes.
- Save behavior clears existing keywords and inserts the selected keywords again.
- Topic list and detail pages now include an Assign Keywords action.
- Topic detail now displays assigned keywords.

Main files:

```text
Scientific.WebAppMVC/Controllers/TopicsController.cs
Scientific.WebAppMVC/Views/Topics/AssignKeywords.cshtml
Scientific.WebAppMVC/Views/Topics/Details.cshtml
```

Shared files:

```text
Scientific.WebAppMVC/ViewModels/AssignmentOptionViewModel.cs
Scientific.WebAppMVC/ViewModels/ManyToManyAssignmentViewModel.cs
Scientific.WebAppMVC/Views/Shared/_AssignmentForm.cshtml
```

#### Step 9 Final Checklist

Status: Passed.

| Check | Result |
|---|---|
| User can have roles | Passed |
| Journal can have many categories | Passed |
| Paper can have many authors | Passed |
| Paper can have many keywords | Passed |
| Topic can have many keywords | Passed |
| Paper detail displays authors/keywords | Passed |
| Journal detail displays categories | Passed |
| Topic detail displays keywords | Passed |
| Build has no errors | Passed |

Verification details:

```text
dotnet build
Build succeeded.
0 Warning(s)
0 Error(s)

Temporary assign-post test:
/Users/AssignRoles/{id}         -> UserRoles_HuyDD count = 2
/Journals/AssignCategories/{id} -> JournalCategories_MinhPV count = 1
/Papers/AssignAuthors/{id}      -> PaperAuthors_BaoTG count = 1
/Papers/AssignKeywords/{id}     -> PaperKeywords_LuanNTK count = 1
/Topics/AssignKeywords/{id}     -> TopicKeywords_LuanNTK count = 1

Temporary test records were cleaned from the database after verification.
```

### Step 9: Code Many-To-Many Relationship Management

Status: Completed.

Completed:
- Assign role to user.
- Assign category to journal.
- Assign author to paper.
- Assign keyword to paper.
- Assign keyword to topic.

### Step 10: Code Search Paper + Paper Detail

Status: Completed.

Completed:
- Added `/Papers/Search` with GET query string filters:
  - Search text across title, abstract, and keyword name.
  - Author name.
  - Journal dropdown.
  - Publication year.
- Added paper search result cards with detail links, journal/year/citation metadata, open access badge, and login-only bookmark button placeholder.
- Reworked `/Papers/Details/{id}` to use a dedicated detail view model.
- Paper detail now shows title, DOI, journal, publisher, publication metadata, authors, keywords, abstract, metrics, paper URL, PDF URL, and login-only bookmark state.
- Detail action now creates `PaperMetrics` when missing and increments `ViewCount` on each valid detail visit.
- Search and detail are accessible for normal demo users, while paper CRUD and many-to-many assignment actions remain protected by the `DataManager` policy.

Files changed:
- `Scientific.WebAppMVC/Controllers/PapersController.cs`
- `Scientific.WebAppMVC/ViewModels/Papers/PaperSearchViewModel.cs`
- `Scientific.WebAppMVC/ViewModels/Papers/PaperResultViewModel.cs`
- `Scientific.WebAppMVC/ViewModels/Papers/PaperDetailViewModel.cs`
- `Scientific.WebAppMVC/Views/Papers/Search.cshtml`
- `Scientific.WebAppMVC/Views/Papers/Details.cshtml`
- `Scientific.WebAppMVC/Views/_ViewImports.cshtml`

Verification:
- `dotnet build Scientific.WebAppMVC/Scientific.WebAppMVC.csproj` succeeded with 0 errors.
- `GET /Papers/Search` without filters returns 200 and shows the empty search form.
- Search by title/text, abstract text, keyword name, author name, journal id, publication year, and combined filters returns 200.
- `GET /Papers/Details/1` returns 200 and shows detail sections plus metrics.
- `GET /Papers/Details/999999` returns 404.
- `PaperMetrics.ViewCount` increments after opening a valid detail page.

Known limitations:
- Bookmark action itself is still planned for Step 13. Step 10 only shows the bookmark button/state where required.

## Notes For Future Updates

This file should be updated after each completed milestone with:
- What was implemented.
- Which files were changed.
- What was tested.
- Known limitations or next actions.

## Debug Notes

### 2026-05-25: MSB3027 / MSB3021 DLL File Lock During Build

Status: Resolved.

Symptom:

```text
MSB3027 Could not copy ... Scientific.Entities.dll / Scientific.Repositories.dll / Scientific.Services.dll.
Exceeded retry count of 10.
The file is locked by: Scientific.WebAppMVC (PID).

MSB3021 Unable to copy file ...
The process cannot access the file because it is being used by another process.
```

Root cause:
- The ASP.NET Core app was still running.
- The running `Scientific.WebAppMVC` process locked DLL files in `Scientific.WebAppMVC/bin/Debug/net8.0`.
- Visual Studio / MSBuild could not overwrite those DLL files during build.
- This was not a C# compile error and not caused by Step 9 code.

Fix used:

```powershell
Stop-Process -Id 3940 -Force
dotnet build .\Scientific.WebAppMVC\Scientific.WebAppMVC.csproj
```

Verification:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

How to debug next time:
- Stop the running app in Visual Studio before building.
- Check Task Manager for `Scientific.WebAppMVC.exe` or `dotnet.exe`.
- Or run:

```powershell
Get-Process | Where-Object { $_.ProcessName -eq 'Scientific.WebAppMVC' -or $_.ProcessName -eq 'dotnet' }
Stop-Process -Id <PID> -Force
```

## Repository Notes

### 2026-05-25: Publish Project To GitHub

Status: Completed.

Repository:
- https://github.com/CharlieMinh/Project_Ass01_PRN222.git

Completed:
- Added root `.gitignore` to exclude Visual Studio local files, build outputs, user-local config, and temporary files.
- Created initial local commit for the full PRN222 project source.
- Connected local Git repo to GitHub remote `origin`.
- Merged the remote initial `README.md` commit from GitHub.
- Pushed branch `main` to GitHub.

Verification:
- `git push -u origin main` completed successfully.
- Remote tracking is set: local `main` tracks `origin/main`.

## Milestone Notes

### Step 11: Seed Data

Status: Skipped by project decision.

Reason:
- Seed/sample data was already inserted manually into the database tables.
- The project moved directly to Step 12.

### Step 12: Dashboard + Chart.js

Status: Completed locally and ready to be tracked through the `developer` branch.

Completed:
- Added `DashboardController` with `/Dashboard/Index`.
- Added `TrendsController` with:
  - `/Trends/Keyword/{id}`
  - `/Trends/Topic/{id}`
  - `/Trends/TrendingTopics`
- Added dashboard view models for statistic cards, trend charts, top papers, trend detail, and trending item rankings.
- Added statistic cards:
  - Total Papers
  - Total Authors
  - Total Journals
  - Total Keywords
  - Total Topics
- Added line chart for publication trend over time by keyword.
- Added filter controls for keyword, from year, and to year.
- Added bar charts for top trending keywords and top trending topics using latest trend year.
- Added top papers ranking table with title, journal, publication year, citation count, and bookmark count.
- Added trend detail pages for keyword/topic with paper count and trend score charts.
- Added local Chart.js vendor file under `wwwroot/lib/chart.js` so dashboard charts do not depend on CDN availability during demo.

Files changed:
- `Scientific.WebAppMVC/Controllers/DashboardController.cs`
- `Scientific.WebAppMVC/Controllers/TrendsController.cs`
- `Scientific.WebAppMVC/ViewModels/Dashboard/TrendDashboardViewModel.cs`
- `Scientific.WebAppMVC/ViewModels/Dashboard/TopPaperViewModel.cs`
- `Scientific.WebAppMVC/ViewModels/Dashboard/TrendDetailViewModel.cs`
- `Scientific.WebAppMVC/ViewModels/Dashboard/TrendingItemViewModel.cs`
- `Scientific.WebAppMVC/Views/Dashboard/Index.cshtml`
- `Scientific.WebAppMVC/Views/Trends/Keyword.cshtml`
- `Scientific.WebAppMVC/Views/Trends/Topic.cshtml`
- `Scientific.WebAppMVC/Views/Trends/TrendingTopics.cshtml`
- `Scientific.WebAppMVC/Views/Trends/_TrendDetail.cshtml`
- `Scientific.WebAppMVC/Views/_ViewImports.cshtml`
- `Scientific.WebAppMVC/wwwroot/lib/chart.js/chart.umd.min.js`
- `Scientific.WebAppMVC/wwwroot/lib/chart.js/LICENSE.md`

Verification:
- `dotnet build Scientific.WebAppMVC/Scientific.WebAppMVC.csproj` succeeded with 0 warnings and 0 errors after stopping the running app process that locked the executable.
- `/Dashboard/Index` returns 200.
- `/Dashboard/Index?keywordId=1&fromYear=2020&toYear=2024` returns 200.
- `/Trends/TrendingTopics` returns 200.
- `/Trends/Keyword/1` returns 200.
- `/Trends/Topic/1` returns 200.
- `/lib/chart.js/chart.umd.min.js` returns 200.
- Browser DOM check confirmed dashboard headings, statistic card labels, three chart canvases, top papers section, and trending topics table rows render.

Known notes:
- Dashboard uses existing `TrendRecords` data. No migration or schema change was created.
- Default dashboard keyword falls back to an AI-related keyword if available, otherwise to the highest latest trend score keyword.
- The latest trend year in the current database is used for top keyword/topic rankings.
- Changes are managed through the `developer` branch instead of pushing directly to `main`.

### Step 13: Bookmark / Follow / Notification

Status: Completed locally and ready to be tracked through the `developer` branch.

Completed:
- Added `BookmarksController`:
  - `GET /Bookmarks/Index`
  - `POST /Bookmarks/Add/{paperId}`
  - `POST /Bookmarks/Remove/{paperId}`
- Added bookmark list page for logged-in users.
- Connected paper search result cards to real bookmark POST actions.
- Connected paper detail bookmark button to add/remove actions.
- Bookmark add checks login, paper existence, and duplicate bookmarks before insert.
- Bookmark remove updates the related paper metric safely.
- `PaperMetrics.BookmarkCount` is incremented/decremented when bookmarking changes.
- Added `FollowingController`:
  - Follow/unfollow keyword.
  - Follow/unfollow topic.
  - Follow/unfollow journal.
  - `GET /Following/Index` for My Following.
- Added follow/unfollow controls:
  - Keyword trend detail page.
  - Topic trend detail page.
  - Paper detail journal section.
- Added `NotificationsController`:
  - `GET /Notifications/Index`
  - `POST /Notifications/MarkAsRead/{id}`
  - `POST /Notifications/MarkAllAsRead`
- Added notification list page with unread state and related paper/trend links.
- Added basic notification generation:
  - When a paper is created, users following that paper's journal receive a notification.
  - When new keywords are assigned to a paper, users following those keywords receive notifications.
- Added My Following link to the authenticated user menu.

Files changed:
- `Scientific.WebAppMVC/Controllers/BookmarksController.cs`
- `Scientific.WebAppMVC/Controllers/FollowingController.cs`
- `Scientific.WebAppMVC/Controllers/NotificationsController.cs`
- `Scientific.WebAppMVC/Controllers/PapersController.cs`
- `Scientific.WebAppMVC/Controllers/TrendsController.cs`
- `Scientific.WebAppMVC/ViewModels/Bookmarks/*`
- `Scientific.WebAppMVC/ViewModels/Following/*`
- `Scientific.WebAppMVC/ViewModels/Notifications/*`
- `Scientific.WebAppMVC/ViewModels/Papers/PaperDetailViewModel.cs`
- `Scientific.WebAppMVC/ViewModels/Dashboard/TrendDetailViewModel.cs`
- `Scientific.WebAppMVC/Views/Bookmarks/Index.cshtml`
- `Scientific.WebAppMVC/Views/Following/Index.cshtml`
- `Scientific.WebAppMVC/Views/Notifications/Index.cshtml`
- `Scientific.WebAppMVC/Views/Papers/Search.cshtml`
- `Scientific.WebAppMVC/Views/Papers/Details.cshtml`
- `Scientific.WebAppMVC/Views/Trends/Keyword.cshtml`
- `Scientific.WebAppMVC/Views/Trends/Topic.cshtml`
- `Scientific.WebAppMVC/Views/Shared/_Layout.cshtml`
- `Scientific.WebAppMVC/Views/_ViewImports.cshtml`

Verification:
- `dotnet build Scientific.WebAppMVC/Scientific.WebAppMVC.csproj` succeeded with 0 warnings and 0 errors.
- `GET /Papers/Search` returns 200.
- `GET /Trends/TrendingTopics` returns 200.
- `GET /Bookmarks/Index` redirects unauthenticated users to `/Account/Login`.
- `GET /Following/Index` redirects unauthenticated users to `/Account/Login`.
- `GET /Notifications/Index` redirects unauthenticated users to `/Account/Login`.
- In-app browser check confirmed `/Papers/Search` renders and protected bookmark routes redirect to login.

Known notes:
- Notification generation is intentionally simple and non-realtime for the first demo version.
- Topic trending notification generation is not wired yet because there is no manual TrendRecords update workflow in the current app.
- Changes are managed through the `developer` branch instead of pushing directly to `main`.

### 2026-05-26: Playwright E2E Test For Steps 1-13

Status: Completed locally. The `e2e-tests/` folder is intentionally ignored and kept only on the local machine.

Completed:
- Added Playwright e2e script:
  - `e2e-tests/prn222-step13-e2e.js`
- Added local Playwright package files in `e2e-tests` for running the script from terminal.
- Added `.gitignore` rules for:
  - `node_modules/`
  - `test-results/`
  - `e2e-tests/`
- The script verifies 13 end-to-end steps:
  1. Home page loads.
  2. Register unique user.
  3. Profile shows registered user and role.
  4. Normal user is denied DataManager CRUD.
  5. Logout.
  6. Admin login and CRUD access.
  7. Admin creates a paper with journal.
  8. Admin assigns keyword to paper.
  9. Dashboard and trending topics render.
  10. User searches the created paper.
  11. Paper detail loads and bookmark works.
  12. My Bookmarks and My Following work.
  13. Notification is generated for followed journal and marked read.
- Each step takes a screenshot under `test-results/prn222-e2e-*`.
- The script prints a terminal summary with PASSED/FAILED per step.

Bug found and fixed:
- Initial e2e run showed notification `Mark All as Read` / `Mark Read` returned success but left `IsRead = false`.
- Fixed `NotificationsController` to use EF Core `ExecuteUpdateAsync` for direct SQL updates on notification read state.

Verification:

```text
dotnet build Scientific.WebAppMVC/Scientific.WebAppMVC.csproj
Build succeeded.
0 Warning(s)
0 Error(s)

node ./e2e-tests/prn222-step13-e2e.js
Step 01 PASSED
Step 02 PASSED
Step 03 PASSED
Step 04 PASSED
Step 05 PASSED
Step 06 PASSED
Step 07 PASSED
Step 08 PASSED
Step 09 PASSED
Step 10 PASSED
Step 11 PASSED
Step 12 PASSED
Step 13 PASSED
```

### 2026-05-26: MinhPV Source Naming Alignment

Status: Completed locally on branch `developer`. Not pushed in this step.

Purpose:
- Align source-code naming with the team ownership rule for MinhPV's assigned area.
- Focus MinhPV's scope on Journal + Category Management rather than broad project features.

MinhPV responsibility:
- CRUD publisher.
- CRUD journal.
- CRUD category.
- Assign journal to category.

Database tables:
- `Publishers`
- `Journals_MinhPV`
- `Categories_MinhPV`
- `JournalCategories_MinhPV`

Renamed source files/folders:
- `Controllers/PublishersController.cs` -> `Controllers/Publishers_MinhPVController.cs`
- `Controllers/JournalsController.cs` -> `Controllers/Journals_MinhPVController.cs`
- `Controllers/CategoriesController.cs` -> `Controllers/Categories_MinhPVController.cs`
- `Views/Publishers` -> `Views/Publishers_MinhPV`
- `Views/Journals` -> `Views/Journals_MinhPV`
- `Views/Categories` -> `Views/Categories_MinhPV`
- `ViewModels/JournalFormViewModel.cs` -> `ViewModels/Journal_MinhPVFormViewModel.cs`

Class/reference updates:
- `Publishers_MinhPVController`
- `Journals_MinhPVController`
- `Categories_MinhPVController`
- `Journal_MinhPVFormViewModel`
- Admin menu now points to:
  - `/Publishers_MinhPV`
  - `/Journals_MinhPV`
  - `/Categories_MinhPV`
- Admin dashboard journal card now points to `Journals_MinhPV`.
- The older scaffolded journal controller was renamed to `Journals_MinhPVLegacyController` to avoid leaving a journal controller with inconsistent MinhPV naming.

Verification:

```text
dotnet build Scientific.WebAppMVC/Scientific.WebAppMVC.csproj
Build succeeded.
0 Warning(s)
0 Error(s)

Unauthenticated route check:
/Journals_MinhPV          -> redirects to Login
/Categories_MinhPV        -> redirects to Login
/Publishers_MinhPV        -> redirects to Login
/Journals_MinhPV/Create   -> redirects to Login
/Categories_MinhPV/Create -> redirects to Login
/Publishers_MinhPV/Create -> redirects to Login

Admin browser check:
/Journals_MinhPV   -> OK
/Categories_MinhPV -> OK
/Publishers_MinhPV -> OK
```
