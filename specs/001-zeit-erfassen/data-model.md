# Data Model: Zeit erfassen

**Feature**: [spec.md](spec.md) | **Date**: 2026-10-25

## Employee (Mitarbeitende)

| Field | Type | Constraints |
|---|---|---|
| `Id` | `int` | Primärschlüssel |
| `Name` | `string` | required, max. 100 Zeichen |

Fest hinterlegt (Seed): Ilka Brandt, Tomás Okoye, Juna Sato, Rik Halvorsen.

## Project (Projekt)

| Field | Type | Constraints |
|---|---|---|
| `Id` | `int` | Primärschlüssel |
| `Key` | `string` | required, max. 20 Zeichen, eindeutig |
| `Name` | `string` | required, max. 200 Zeichen |

Fest hinterlegt (Seed): `NSL-ALB-01` Antriebswartung Albatros, `KBA-TG3-04`
Schildgenerator-Kalibrierung Tiefgrund III, `HFL-PER-02` Lebenserhaltung prüfen Perihel.

## TimeEntry (Buchung)

| Field | Type | Constraints |
|---|---|---|
| `Id` | `int` | Primärschlüssel |
| `EmployeeId` | `int` | required, Fremdschlüssel auf `Employee` |
| `ProjectId` | `int` | required, Fremdschlüssel auf `Project` |
| `Date` | `DateOnly` | required |
| `Start` | `TimeOnly` | required, wie eingegeben (FR-004) |
| `End` | `TimeOnly` | required, wie eingegeben, nach `Start` (FR-006) |
| `Note` | `string?` | optional, max. 500 Zeichen |
| `ActualDuration` | `TimeSpan` | berechnet, nicht gespeichert: `End - Start` |
| `BilledDuration` | `TimeSpan` | berechnet, nicht gespeichert: `ActualDuration` aufgerundet auf volle 15 Minuten (FR-005) |

**Unveränderlich**: kein Update, kein Delete (Prinzip III, FR-011). Index auf
(`EmployeeId`, `Date`) für das Laden der Tagesbuchungen.

## Regeln (Kontor.Core/Rules)

| Rule | Prüft | Quelle |
|---|---|---|
| `BillingRounding` | tatsächliche Dauer aufgerundet auf das nächste Vielfache von 15 Minuten | FR-005 |
| `TimeRangeRule` | Ende nach Beginn, am selben Tag (kein Ende nach Mitternacht) | FR-006 |
| `OverlapRule` | keine Überschneidung mit Buchungen derselben Person am Tag; Berührung erlaubt | FR-007 |
| `BreakRule` | > 6 h → Pause ≥ 30 min, > 9 h → Pause ≥ 45 min; nur Lücken ≥ 15 min zählen; tatsächliche Dauern | FR-008, FR-013 |
| `MaxDailyHoursRule` | Tagesarbeitszeit ≤ 10 h, tatsächliche Dauern | FR-009, FR-013 |

## BookingResult

| Field | Type | Bedeutung |
|---|---|---|
| `Succeeded` | `bool` | gespeichert ja/nein |
| `ViolatedRule` | `string?` | Name der verletzten Regel bei Ablehnung (FR-010) |
| `Message` | `string?` | Hinweis für die Nutzerin |
| `Entry` | `TimeEntry?` | die gespeicherte Buchung |

## DayOverview (Tagesliste)

| Field | Type | Bedeutung |
|---|---|---|
| `Entries` | Liste von `TimeEntry` | Buchungen des Tages mit Beginn, Ende, `ActualDuration`, `BilledDuration` (FR-012) |
| `Total` | `TimeSpan` | Summe der `BilledDuration` (FR-012, SC-003) |
