# Quickstart: Zeit erfassen

**Feature**: [spec.md](spec.md) | **Date**: 2026-10-25

## Voraussetzungen

- .NET 10 SDK
- SQL Server LocalDB (`sqllocaldb info` zeigt `MSSQLLocalDB`)
- PowerShell 7

## Bauen und testen

```powershell
dotnet tool restore
dotnet build
dotnet test
```

Erwartet: Build ohne Warnungen, alle Tests in `tests/Kontor.Core.Tests` grün.

## Starten

```powershell
dotnet run --project src/Kontor.Web
```

Beim ersten Start in Development legt die App die Datenbank `KontorDb` in LocalDB an
(Migrationen, siehe [research.md](research.md), R6). Die Adresse steht in der Konsolenausgabe.

## Prüfszenarien

| # | Schritt | Erwartet | Bezug |
|---|---|---|---|
| 1 | Juna Sato, 2026-11-02, 08:10–10:17 auf `NSL-ALB-01` | gespeichert mit 08:10–10:17, tatsächlich 2:07, abgerechnet 2:15, Tagessumme 2:15 | US1-1, FR-004, FR-005, FR-012 |
| 2 | Juna Sato, gleicher Tag, 10:00–12:00 auf `KBA-TG3-04` | abgelehnt, Überschneidung mit 08:10–10:17 genannt | US1-2, FR-007, FR-010 |
| 3 | Juna Sato, gleicher Tag, 10:17–12:00 auf `KBA-TG3-04` | gespeichert, abgerechnet 1:45, Tagessumme 4:00 | US1-3 |
| 4 | Tomás Okoye, 2026-11-02, 08:00–10:00 auf `HFL-PER-02` | tatsächlich und abgerechnet 2:00 | US1-4 |
| 5 | Tagesliste ansehen | keine Bearbeiten- oder Löschen-Funktion | US1-5, FR-011 |
| 6 | Rik Halvorsen, 2026-11-03, 07:00–14:00 | abgelehnt, Pausenregel genannt | US2-1, FR-008 |
| 7 | Rik Halvorsen, gleicher Tag, 07:00–11:00 und 11:30–14:00 | beide gespeichert | US2-2 |
| 8 | Rik Halvorsen, gleicher Tag, 14:45–17:15 (9:00 h), dann 17:30–19:00 | zweite abgelehnt, Höchstarbeitszeit genannt | US2-3, FR-009 |
| 9 | Ilka Brandt, Ende vor Beginn (10:00–09:00) | abgelehnt, Regel genannt | FR-006 |

Die Datenmodell-Details stehen in [data-model.md](data-model.md).
