# Research: Zeit erfassen

**Feature**: [spec.md](spec.md) | **Plan**: [plan.md](plan.md) | **Date**: 2026-10-25

Der Technical Context enthält keine `NEEDS CLARIFICATION`: Plattform, Oberfläche, Persistenz
und Test-Framework kommen aus der Constitution. Die folgenden Entscheidungen klären
Abhängigkeiten und Muster.

## R1 — Datentypen für Datum und Uhrzeit

- **Decision**: `DateOnly` für das Datum, `TimeOnly` für Beginn und Ende.
- **Rationale**: Eine Buchung liegt innerhalb eines Tages (FR-006), Zeitzonen spielen keine
  Rolle (Assumptions). EF Core 10 bildet `DateOnly` und `TimeOnly` auf `date` und `time` ab.
- **Alternatives considered**: `DateTime` für Beginn und Ende (Datum doppelt ausdrückbar,
  Mitternacht schwerer zu prüfen); `DateTimeOffset` (Zeitzonen nicht gefordert).

## R2 — Abrechnungsdauer

- **Decision**: Beginn und Ende werden unverändert gespeichert. Die tatsächliche Dauer
  (`End - Start`) und die Abrechnungsdauer sind berechnete Werte von `TimeEntry`, keine
  Spalten. `BillingRounding` rundet die tatsächliche Dauer auf das nächste Vielfache von
  15 Minuten auf; ein Vielfaches bleibt unverändert (FR-005).
- **Rationale**: FR-004, FR-005. Berechnete Werte können nicht von Beginn und Ende abweichen.
- **Alternatives considered**: Abrechnungsdauer als Spalte (zweite Wahrheit neben Beginn und
  Ende); Beginn und Ende runden (widerspricht FR-004).

## R3 — Wo die Regeln geprüft werden

- **Decision**: `TimeEntryService` in `Kontor.Core` lädt die Buchungen der Person für den Tag
  über `ITimeEntryRepository` und prüft die neue Buchung nacheinander mit `TimeRangeRule`,
  `OverlapRule`, `BreakRule` und `MaxDailyHoursRule`. Ergebnis ist ein `BookingResult`:
  gespeichert, oder abgelehnt mit der verletzten Regel (FR-010).
- **Rationale**: Prinzip I und II: Die Regeln sind reine Funktionen über Zeitspannen und ohne
  Datenbank testbar.
- **Alternatives considered**: Prüfung in der Blazor-Komponente oder per Datenbank-Constraint
  (verstößt gegen Prinzip I).

## R4 — Pause und Tagesarbeitszeit

- **Decision**: Tagesarbeitszeit = Summe der **tatsächlichen** Dauern aller Buchungen des Tages
  einschließlich der neuen (FR-013). Pause = Summe der Lücken zwischen zeitlich
  aufeinanderfolgenden Buchungen, nur Lücken ab 15 Minuten zählen. Mehr als 6 Stunden
  verlangen mindestens 30 Minuten Pause, mehr als 9 Stunden mindestens 45 Minuten (FR-008);
  mehr als 10 Stunden sind nie erlaubt (FR-009).
- **Rationale**: FR-008, FR-009, FR-013 und Clarifications vom 2026-10-25. Die
  Abrechnungsdauer dient nur der Abrechnung.
- **Alternatives considered**: Pause als eigene Buchungsart (nicht gefordert).

## R5 — Unveränderlichkeit

- **Decision**: `ITimeEntryRepository` bietet nur `AddAsync` und Lesemethoden; `TimeEntry` hat
  keine öffentlichen Setter; die Oberfläche hat keine Bearbeiten- oder Löschen-Funktion.
- **Rationale**: Prinzip III, FR-011.
- **Alternatives considered**: Soft Delete (wäre eine Löschung; Korrekturen kommen später als
  eigenes Feature).

## R6 — Datenbankschema und Migrationen

- **Decision**: EF-Core-Migrationen in `Kontor.Infrastructure`, erzeugt mit `dotnet-ef` als
  lokalem Tool (`.config/dotnet-tools.json`, `dotnet tool restore`). In der Umgebung
  Development wendet `Kontor.Web` ausstehende Migrationen beim Start an.
- **Rationale**: Planungsauftrag; niemand braucht eine globale Installation.
- **Alternatives considered**: `EnsureCreated` (verträgt keine späteren Schemaänderungen).

## R7 — Stammdaten

- **Decision**: Seed über `HasData` in den Entity-Konfigurationen: Mitarbeitende Ilka Brandt,
  Tomás Okoye, Juna Sato, Rik Halvorsen; Projekte `NSL-ALB-01` „Antriebswartung Albatros“,
  `KBA-TG3-04` „Schildgenerator-Kalibrierung Tiefgrund III“, `HFL-PER-02` „Lebenserhaltung
  prüfen Perihel“.
- **Rationale**: Laut Spec fest hinterlegt (FR-002, FR-003); über Migrationen reproduzierbar.
- **Alternatives considered**: Seed im Startcode (läuft bei jedem Start).

## R8 — Paketversionen

- **Decision**: `Directory.Packages.props` mit `ManagePackageVersionsCentrally` und festen
  Versionen: EF Core 10.0.13, `MSTest` 4.5.1. `CentralPackageTransitivePinningEnabled` mit
  fester Version für `System.Security.Cryptography.Pkcs` 10.0.13, weil EF Core SqlServer eine
  ältere Fassung mit bekannten Sicherheitslücken nachzieht (`NU1902`, `NU1903`). `global.json`
  legt die SDK-Version fest, das Tool-Manifest `dotnet-ef` 10.0.13. Dazu `nuget.config` mit
  `<clear />` und nur nuget.org, weil Central Package Management bei mehreren Paketquellen
  `NU1507` meldet.
- **Rationale**: Prinzip V; mit `TreatWarningsAsErrors` würden die Warnungen den Build brechen.
- **Alternatives considered**: Warnungen unterdrücken (verdeckt eine echte Sicherheitslücke);
  Platzhalterversionen (verstößt gegen Prinzip V).

## R9 — Blazor und DbContext

- **Decision**: Blazor Web App mit `InteractiveServer` global. Die Person wird auf der Seite
  aus einer Auswahlliste gewählt (FR-002). `AddDbContextFactory<KontorDbContext>`: Die
  Repositories erzeugen pro Operation einen eigenen Kontext.
- **Rationale**: Render-Modus aus der Constitution. Eine Blazor-Server-Verbindung lebt lange;
  ein gemeinsamer `DbContext` führt bei parallelen Aufrufen zu „A second operation was started
  on this context instance“.
- **Alternatives considered**: `AddDbContext` mit Scoped-Lebensdauer (siehe oben).
