# Kontor Constitution

Kontor ist die Zeiterfassung der Galactic Service Company (GSC).

## Core Principles

### I. Fachlogik in Kontor.Core

Alle fachlichen Regeln MÜSSEN in `Kontor.Core` implementiert sein. Oberfläche und
Datenbankzugriff DÜRFEN KEINE Fachlogik enthalten; sie nehmen Eingaben entgegen,
speichern und zeigen an.

*Begründung:* Regeln an genau einer Stelle lassen sich ohne Oberfläche und ohne
Datenbank prüfen und ändern.

### II. Test-First für Fachlogik (NON-NEGOTIABLE)

Jede Regel in `Kontor.Core` MUSS zuerst MSTest-Tests bekommen, danach die
Implementierung. Mock-Bibliotheken DÜRFEN NICHT verwendet werden; Test-Doubles werden
selbst geschrieben.

*Begründung:* Die Tests beschreiben die Regel, bevor es Code gibt, und bleiben ohne
fremde Bibliotheken lesbar.

### III. Buchungen bleiben

Eine gespeicherte Buchung DARF weder geändert noch gelöscht werden.

*Begründung:* Jede erfasste Arbeitszeit bleibt nachvollziehbar; Korrekturen werden
später ein eigenes Feature.

### IV. Englische Bezeichner

Namespaces, Typen, Methoden und Tests MÜSSEN englisch benannt sein, auch wenn der
Fachbegriff deutsch ist.

*Begründung:* Einheitlicher Teamstandard für den gesamten Code.

### V. Feste Paketversionen

NuGet-Pakete MÜSSEN mit fester Version zentral in `Directory.Packages.props` stehen.
Platzhalter wie `10.*` DÜRFEN NICHT verwendet werden. Ausgenommen sind die SDK-Version
in `global.json` und lokale .NET-Tools im Tool-Manifest `.config/dotnet-tools.json`.

*Begründung:* Jeder Build verwendet dieselben Pakete, Aktualisierungen sind bewusste
Änderungen an einer Stelle. SDK und lokale Tools haben ihre eigene Ablage.

## Technische Rahmenbedingungen

Diese Technik gilt für jedes Feature und muss im Plan nicht wiederholt werden:

- Zielplattform: .NET 10
- Oberfläche: Blazor Web App mit Render-Modus `InteractiveServer`
- Persistenz: EF Core mit SQL Server (lokal LocalDB)
- Tests: MSTest

## Governance

- Diese Constitution hat Vorrang vor anderen Konventionen im Repository.
- Änderungen erfolgen über `/speckit-constitution`. Die Version folgt Semantic
  Versioning: MAJOR für entfernte oder neu definierte Prinzipien, MINOR für neue
  Prinzipien oder Abschnitte, PATCH für Klarstellungen.
- Die Einhaltung wird im Constitution Check jedes Plans und durch `/speckit-analyze`
  geprüft.

**Version**: 1.0.0 | **Ratified**: 2026-10-25 | **Last Amended**: 2026-10-25
