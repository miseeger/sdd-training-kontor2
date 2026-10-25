# Kontor

Zeiterfassung der Galactic Service Company (GSC): Mitarbeitende buchen ihre Arbeitszeit auf
Wartungsprojekte. Kontor rechnet jede Buchung mit ihrer Dauer ab, aufgerundet auf die nächste
volle Viertelstunde, und prüft Überschneidungen, Mindestpause und Tageshöchstarbeitszeit, bevor
eine Buchung gespeichert wird.

Entstanden mit Spec-Driven Development und GitHub Spec Kit 1.2.0. Die Grundsätze stehen in
`.specify/memory/constitution.md`, das erste Feature unter `specs/001-zeit-erfassen/`.

## Voraussetzungen

- .NET 10 SDK
- SQL Server LocalDB (`sqllocaldb info` zeigt `MSSQLLocalDB`)

## Bauen, testen, starten

```powershell
dotnet tool restore
dotnet build
dotnet test
dotnet run --project src/Kontor.Web
```

Beim ersten Start legt die App die Datenbank `KontorDb` an. Prüfszenarien:
[`specs/001-zeit-erfassen/quickstart.md`](specs/001-zeit-erfassen/quickstart.md).
