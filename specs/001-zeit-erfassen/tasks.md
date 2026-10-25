---

description: "Task list for feature 001 Zeit erfassen"
---

# Tasks: Zeit erfassen

**Input**: Design documents from `/specs/001-zeit-erfassen/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, quickstart.md

**Tests**: Enthalten. Die Constitution verlangt Test-First für jede Regel in `Kontor.Core` (Prinzip II, NON-NEGOTIABLE); Test-Doubles werden selbst geschrieben.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2)
- Include exact file paths in descriptions

## Path Conventions

- Projekte unter `src/`, Tests unter `tests/`, Lösungs- und Build-Dateien im Repository-Wurzelverzeichnis (plan.md, Project Structure)

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [X] T001 `global.json` mit fester .NET-10-SDK-Version anlegen
- [X] T002 `Kontor.slnx`, Ordner `src/` und `tests/` und `.gitignore` für .NET anlegen
- [X] T003 [P] `Directory.Build.props` anlegen: `net10.0`, `Nullable` enable, `ImplicitUsings` enable, `TreatWarningsAsErrors` true
- [X] T004 [P] `Directory.Packages.props` anlegen: `ManagePackageVersionsCentrally` und `CentralPackageTransitivePinningEnabled` true; `Microsoft.EntityFrameworkCore.SqlServer` 10.0.13, `Microsoft.EntityFrameworkCore.Design` 10.0.13, `MSTest` 4.5.1, `System.Security.Cryptography.Pkcs` 10.0.13; keine Platzhalterversionen (research.md, R8)
- [X] T005 [P] `nuget.config` mit `<clear />` und nur nuget.org; Tool-Manifest `.config/dotnet-tools.json` mit `dotnet-ef` 10.0.13
- [X] T006 Class Library `src/Kontor.Core/Kontor.Core.csproj` ohne Paket- und Projektreferenzen
- [X] T007 Class Library `src/Kontor.Infrastructure/Kontor.Infrastructure.csproj` mit Referenz auf `Kontor.Core` und den EF-Core-Paketen
- [X] T008 Blazor Web App `src/Kontor.Web/Kontor.Web.csproj` mit Render-Modus `InteractiveServer` global, Referenzen auf `Kontor.Core` und `Kontor.Infrastructure`
- [X] T009 Testprojekt `tests/Kontor.Core.Tests/Kontor.Core.Tests.csproj` mit `PackageReference` auf `MSTest` und Referenz auf `Kontor.Core`; alle Projekte in `Kontor.slnx` aufnehmen

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T010 [P] Entität `Employee` in `src/Kontor.Core/Domain/Employee.cs`: `Id`, `Name` (required, max. 100 Zeichen)
- [X] T011 [P] Entität `Project` in `src/Kontor.Core/Domain/Project.cs`: `Id`, `Key` (required, max. 20 Zeichen, eindeutig), `Name` (required, max. 200 Zeichen)
- [X] T012 [P] Entität `TimeEntry` in `src/Kontor.Core/Domain/TimeEntry.cs`: `Id`, `EmployeeId` (required), `ProjectId` (required), `Date` (`DateOnly`, required), `Start` und `End` (`TimeOnly`, required, wie eingegeben), `Note` (optional, max. 500 Zeichen), berechnet `ActualDuration` (`End - Start`, nicht gespeichert); keine öffentlichen Setter (Prinzip III). `BilledDuration` kommt erst in T023, nach ihren Tests (Prinzip II)
- [X] T013 [P] Schnittstellen `ITimeEntryRepository` (nur `AddAsync` und `GetForEmployeeAndDateAsync`), `IEmployeeRepository`, `IProjectRepository` in `src/Kontor.Core/Abstractions/`
- [X] T014 `KontorDbContext` und Entity-Konfigurationen mit den Constraints aus data-model.md, Index auf (`EmployeeId`, `Date`), Seed per `HasData` (Ilka Brandt, Tomás Okoye, Juna Sato, Rik Halvorsen; `NSL-ALB-01` „Antriebswartung Albatros“, `KBA-TG3-04` „Schildgenerator-Kalibrierung Tiefgrund III“, `HFL-PER-02` „Lebenserhaltung prüfen Perihel“) in `src/Kontor.Infrastructure/KontorDbContext.cs` und `src/Kontor.Infrastructure/Configurations/`
- [X] T015 Repositories (nur Hinzufügen und Lesen, je Operation ein Kontext über `IDbContextFactory`) in `src/Kontor.Infrastructure/Repositories/` und Registrierung `AddKontorInfrastructure` in `src/Kontor.Infrastructure/DependencyInjection.cs` (research.md, R9)
- [X] T016 `src/Kontor.Web/Program.cs`: Infrastruktur registrieren, Connection String `KontorDb` für LocalDB in `src/Kontor.Web/appsettings.Development.json`, ausstehende Migrationen in Development beim Start anwenden
- [X] T017 Erste Migration `InitialCreate` mit dem lokalen Tool erzeugen (`dotnet tool restore`, `dotnet ef migrations add InitialCreate`) in `src/Kontor.Infrastructure/Migrations/`
- [X] T018 Test-Double `InMemoryTimeEntryRepository` in `tests/Kontor.Core.Tests/Fakes/InMemoryTimeEntryRepository.cs`

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 - Arbeitszeit auf ein Projekt buchen (Priority: P1) 🎯 MVP

**Goal**: Eine Person bucht Arbeitszeit auf ein Projekt; Kontor speichert Beginn und Ende wie eingegeben, berechnet die Abrechnungsdauer, verhindert Überschneidungen und zeigt die Tagesliste mit Tagessumme.

**Independent Test**: Juna Sato bucht 08:10–10:17 auf `NSL-ALB-01` → tatsächlich 2:07, abgerechnet 2:15, Tagessumme 2:15; 10:00–12:00 am selben Tag → abgelehnt wegen Überschneidung.

### Tests for User Story 1 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [X] T019 [P] [US1] `BillingRoundingTests` in `tests/Kontor.Core.Tests/Rules/BillingRoundingTests.cs`: 2:07 → 2:15, 2:00 bleibt 2:00, 0:05 → 0:15, 1:43 → 1:45, 0:01 → 0:15 (FR-005)
- [X] T020 [P] [US1] `TimeRangeRuleTests` in `tests/Kontor.Core.Tests/Rules/TimeRangeRuleTests.cs`: Ende vor oder gleich Beginn abgelehnt, über Mitternacht (22:00–00:30) abgelehnt, 08:05–08:10 erlaubt (FR-006)
- [X] T021 [P] [US1] `OverlapRuleTests` in `tests/Kontor.Core.Tests/Rules/OverlapRuleTests.cs`: 10:00–12:00 gegen 08:10–10:17 abgelehnt, Berührung 10:17–12:00 erlaubt, Buchungen anderer Personen zählen nicht (FR-007)
- [X] T022 [US1] `TimeEntryServiceTests` (User Story 1) in `tests/Kontor.Core.Tests/Services/TimeEntryServiceTests.cs`: Akzeptanzszenarien 1–4 von User Story 1; Tagessumme = Summe der Abrechnungsdauern (FR-012, SC-003); Notiz über 500 Zeichen abgelehnt; bei Ablehnung ist die verletzte Regel benannt (FR-010); nutzt T018

### Implementation for User Story 1

- [X] T023 [US1] `BillingRounding` in `src/Kontor.Core/Rules/BillingRounding.cs` und `TimeEntry.BilledDuration` darüber berechnen in `src/Kontor.Core/Domain/TimeEntry.cs` (nicht gespeichert; macht T019 grün)
- [X] T024 [P] [US1] `TimeRangeRule` in `src/Kontor.Core/Rules/TimeRangeRule.cs` (macht T020 grün)
- [X] T025 [P] [US1] `OverlapRule` in `src/Kontor.Core/Rules/OverlapRule.cs` (macht T021 grün)
- [X] T026 [US1] `BookingRequest`, `BookingResult`, `DayOverview` und `TimeEntryService` in `src/Kontor.Core/Services/`: Notiz prüfen, `TimeRangeRule` und `OverlapRule`, speichern ohne Beginn und Ende zu verändern; Tagesliste und Tagessumme liefern (macht T022 grün; depends on T023–T025)
- [X] T027 [US1] `TimeEntryService` in `src/Kontor.Web/Program.cs` registrieren
- [X] T028 [US1] Seite `src/Kontor.Web/Components/Pages/Booking.razor` als Startseite: Person aus Liste wählen, Datum, Beginn, Ende, Projekt, Notiz (optional, max. 500 Zeichen), Speichern; Hinweis mit verletzter Regel bei Ablehnung; Tagesliste mit Beginn, Ende, tatsächlicher und abgerechneter Dauer je Buchung und Tagessumme; keine Bearbeiten- oder Löschen-Funktion (FR-001 bis FR-004, FR-010 bis FR-012)
- [X] T029 [US1] Navigation auf die Buchungsseite reduzieren, Beispielseiten der Vorlage (Counter, Weather) entfernen in `src/Kontor.Web/Components/`

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Arbeitszeitregeln bei der Erfassung einhalten (Priority: P2)

**Goal**: Kontor lehnt Buchungen ab, die die Mindestpause oder die Tageshöchstarbeitszeit verletzen würden, und nennt die Regel.

**Independent Test**: Rik Halvorsen bucht 07:00–14:00 → abgelehnt (Pause); 07:00–11:00 und 11:30–14:00 → beide gespeichert; Tag mit 9:00 Stunden plus 1:30 → abgelehnt (Höchstarbeitszeit).

### Tests for User Story 2 ⚠️

- [X] T030 [P] [US2] `BreakRuleTests` in `tests/Kontor.Core.Tests/Rules/BreakRuleTests.cs`: mehr als 6 h mit weniger als 30 min Pause abgelehnt, 07:00–11:00 und 11:30–14:00 erlaubt, Lücke unter 15 min zählt nicht, mehr als 9 h verlangt 45 min; tatsächliche Dauern (FR-008, FR-013)
- [X] T031 [P] [US2] `MaxDailyHoursRuleTests` in `tests/Kontor.Core.Tests/Rules/MaxDailyHoursRuleTests.cs`: mehr als 10 h abgelehnt, genau 10 h erlaubt; tatsächlich 10:00 h aus drei Buchungen (3:23, 3:23, 3:14) erlaubt, obwohl die Abrechnungsdauern 10:15 h ergeben (FR-009, FR-013)
- [X] T032 [US2] `TimeEntryServiceTests` (User Story 2) in `tests/Kontor.Core.Tests/Services/TimeEntryServiceTests.cs`: Akzeptanzszenarien 1–3 von User Story 2; abgelehnt wird nur die neue Buchung

### Implementation for User Story 2

- [ ] T033 [P] [US2] `BreakRule` in `src/Kontor.Core/Rules/BreakRule.cs` (macht T030 grün)
- [ ] T034 [P] [US2] `MaxDailyHoursRule` in `src/Kontor.Core/Rules/MaxDailyHoursRule.cs` (macht T031 grün)
- [ ] T035 [US2] `BreakRule` und `MaxDailyHoursRule` nach der `OverlapRule` in `src/Kontor.Core/Services/TimeEntryService.cs` einbinden (macht T032 grün)

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T036 [P] `README.md` im Wurzelverzeichnis: Zweck, Voraussetzungen, Verweis auf `specs/001-zeit-erfassen/quickstart.md`
- [ ] T037 Prüfszenarien aus `specs/001-zeit-erfassen/quickstart.md` durchgehen

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: keine Abhängigkeiten
- **Foundational (Phase 2)**: hängt von Setup ab, blockiert alle User Stories
- **User Story 1 (Phase 3)**: hängt von Foundational ab
- **User Story 2 (Phase 4)**: hängt von Foundational ab; T035 erweitert den `TimeEntryService` aus T026
- **Polish (Phase 5)**: nach den gewünschten User Stories

### Within Each User Story

- Tests zuerst; sie müssen vor der Implementierung fehlschlagen
- Regeln vor Service, Service vor Seite

### Parallel Opportunities

- Setup: T003, T004, T005
- Foundational: T010, T011, T012, T013
- User Story 1: Tests T019, T020, T021; danach Regeln T024, T025 parallel zu T023
- User Story 2: Tests T030, T031; danach Regeln T033, T034

---

## Parallel Example: User Story 1

```text
Task: "BillingRoundingTests in tests/Kontor.Core.Tests/Rules/BillingRoundingTests.cs"
Task: "TimeRangeRuleTests in tests/Kontor.Core.Tests/Rules/TimeRangeRuleTests.cs"
Task: "OverlapRuleTests in tests/Kontor.Core.Tests/Rules/OverlapRuleTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Phase 1: Setup
2. Phase 2: Foundational
3. Phase 3: User Story 1
4. **STOP and VALIDATE**: User Story 1 prüfen (`dotnet test`, App starten, buchen)

### Incremental Delivery

1. Setup + Foundational → Grundlage steht
2. User Story 1 → prüfen → vorzeigbar (MVP)
3. User Story 2 → prüfen → Arbeitszeitregeln aktiv

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Verify tests fail before implementing
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
