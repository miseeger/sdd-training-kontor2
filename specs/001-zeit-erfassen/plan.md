# Implementation Plan: Zeit erfassen

**Branch**: `001-zeit-erfassen` | **Date**: 2026-10-25 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/001-zeit-erfassen/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Mitarbeitende wählen sich aus einer Liste aus und buchen Arbeitszeit mit Datum, Beginn, Ende,
Projekt und optionaler Notiz. Kontor speichert Beginn und Ende wie eingegeben, berechnet die
tatsächliche Dauer und die Abrechnungsdauer (aufgerundet auf die nächste volle Viertelstunde)
und prüft vor dem Speichern Ende nach Beginn, Überschneidung, Mindestpause und
Tageshöchstarbeitszeit mit den tatsächlichen Zeiten. Gespeicherte Buchungen sind
unveränderlich. Die Tagesliste zeigt beide Dauern und die Tagessumme der Abrechnungsdauern.

Technisch: Alle Regeln liegen als reine, ohne Datenbank testbare Klassen in `Kontor.Core`;
`Kontor.Infrastructure` speichert mit EF Core in SQL Server (LocalDB); `Kontor.Web` ist eine
Blazor Web App mit einer Seite zum Erfassen und der Tagesliste.

## Technical Context

Plattform, Oberfläche, Persistenz und Test-Framework stehen in den **Technischen
Rahmenbedingungen der Constitution** und gelten für jedes Feature; der Plan übernimmt sie.

**Language/Version**: C# 14 / .NET 10 (Constitution, Technische Rahmenbedingungen)

**Primary Dependencies**: ASP.NET Core Blazor Web App mit Render-Modus `InteractiveServer`
(Constitution); EF Core 10 (`Microsoft.EntityFrameworkCore.SqlServer`,
`Microsoft.EntityFrameworkCore.Design`); `dotnet-ef` als lokales Tool

**Storage**: SQL Server LocalDB (Constitution), Datenbank `KontorDb`, Connection String in
`appsettings.Development.json`; Schema über EF-Core-Migrationen, in Development beim Start
angewendet

**Testing**: MSTest (Constitution) über das Paket `MSTest`, ohne Mock-Bibliotheken (Prinzip II)

**Target Platform**: Windows-Entwicklungsrechner mit LocalDB, Browser

**Project Type**: Webanwendung (Blazor Server) mit Class Libraries für Fachlogik und Persistenz

**Performance Goals**: Erfassen einer Buchung in unter 30 Sekunden (SC-001); Prüfen und
Speichern ohne spürbare Wartezeit

**Constraints**: Alle Fachregeln ohne Datenbank testbar (Prinzip I, II); keine Änderung oder
Löschung gespeicherter Buchungen (Prinzip III); feste, zentrale Paketversionen (Prinzip V);
keine Web-API und keine Kommandozeile in dieser Phase

**Scale/Scope**: rund 40 Mitarbeitende, eine Seite, drei fest hinterlegte Projekte

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Prinzip | Vor Phase 0 | Nach Phase 1 |
|---|---|---|
| I. Fachlogik in Kontor.Core | ✅ Abrechnungsdauer, Ende nach Beginn, Überschneidung, Pause und Höchstarbeitszeit liegen in `Kontor.Core`; Web und Infrastructure rufen nur auf | ✅ Regeln in `Kontor.Core/Rules`, `TimeEntryService` prüft und speichert über eine Schnittstelle; Web und Infrastructure ohne Regelcode (data-model.md) |
| II. Test-First für Fachlogik | ✅ Jede Regel bekommt MSTest-Tests vor der Implementierung | ✅ `tests/Kontor.Core.Tests`; Test-Double `InMemoryTimeEntryRepository` selbst geschrieben, keine Mock-Bibliothek |
| III. Buchungen bleiben | ✅ Spec verbietet Ändern und Löschen (FR-011) | ✅ Repository bietet nur Hinzufügen und Lesen; `TimeEntry` ohne öffentliche Setter; keine Bearbeiten- oder Löschen-Funktion in der Oberfläche |
| IV. Englische Bezeichner | ✅ | ✅ `TimeEntry`, `Employee`, `Project`, `BillingRounding`, `BreakRule` usw. |
| V. Feste Paketversionen | ✅ NuGet-Pakete mit fester Version in `Directory.Packages.props`; SDK in `global.json`; `dotnet-ef` im Tool-Manifest | ✅ alle NuGet-Pakete einschließlich `MSTest` fest und zentral; `global.json` und `.config/dotnet-tools.json` mit festen Versionen; keine Platzhalter |

Ergebnis: bestanden, keine Abweichung.

## Project Structure

### Documentation (this feature)

```text
specs/001-zeit-erfassen/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

Kein `contracts/`: Kontor bietet in dieser Phase keine Schnittstelle für andere Systeme an
(keine Web-API, keine Kommandozeile).

### Source Code (repository root)

```text
Kontor.slnx
global.json                         # SDK-Version
Directory.Build.props               # net10.0, Nullable, ImplicitUsings, TreatWarningsAsErrors
Directory.Packages.props            # zentrale, feste Paketversionen
nuget.config                        # nur nuget.org
.config/dotnet-tools.json           # dotnet-ef als lokales Tool

src/
├── Kontor.Core/                    # Class Library, keine Abhängigkeit zu EF Core oder Blazor
│   ├── Domain/                     # Employee, Project, TimeEntry
│   ├── Rules/                      # BillingRounding, TimeRangeRule, OverlapRule, BreakRule, MaxDailyHoursRule
│   ├── Abstractions/               # ITimeEntryRepository, IEmployeeRepository, IProjectRepository
│   └── Services/                   # TimeEntryService, BookingRequest, BookingResult, DayOverview
├── Kontor.Infrastructure/          # Class Library, EF Core, DbContext, Migrationen
│   ├── KontorDbContext.cs
│   ├── Configurations/             # Entity-Konfigurationen mit Seed
│   ├── Repositories/
│   ├── Migrations/
│   └── DependencyInjection.cs
└── Kontor.Web/                     # Blazor Web App, InteractiveServer
    ├── Components/Pages/Booking.razor
    ├── Program.cs
    └── appsettings.Development.json

tests/
└── Kontor.Core.Tests/              # MSTest, ohne Datenbank
    ├── Rules/
    ├── Services/
    └── Fakes/                      # InMemoryTimeEntryRepository
```

**Structure Decision**: Drei Projekte unter `src/` und ein Testprojekt unter `tests/`, wie im
Planungsauftrag vorgegeben. `Kontor.Web` referenziert `Kontor.Core` und `Kontor.Infrastructure`
(nur für die Registrierung im DI-Container); `Kontor.Infrastructure` referenziert
`Kontor.Core`; `Kontor.Core` referenziert nichts.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

Keine Abweichungen.
