# Feature Specification: Zeit erfassen

**Feature Branch**: `001-zeit-erfassen`

**Created**: 2026-10-25

**Status**: Draft

**Input**: User description: "Kontor ist die neue Zeiterfassung der Galactic Service Company (GSC), eines Wartungsdienstleisters mit rund 40 Technikerinnen und Technikern. Heute führen alle ihre Stunden in Tabellen, und die Produktverantwortliche tippt am Monatsende alles ab. Als Technikerin möchte ich meine Arbeitszeit auf das Projekt buchen, an dem ich gearbeitet habe, damit am Monatsende niemand meine Stunden abtippen muss. Eine Buchung hat Datum, Beginn, Ende, Projekt und eine optionale Notiz. Nach dem Speichern sehe ich meine Buchungen des Tages und die Tagessumme. Fachliche Regeln: Abgerechnet wird die Dauer einer Buchung, aufgerundet auf die nächste volle Service-Viertelstunde (15 Minuten). Beginn und Ende werden nicht gerundet. Beispiel: 08:10 bis 10:17 dauert 2:07 Stunden und wird mit 2:15 Stunden abgerechnet. Die Tagesliste zeigt je Buchung die tatsächliche und die abgerechnete Dauer, die Tagessumme zählt die abgerechneten Dauern. Buchungen derselben Person dürfen sich zeitlich nicht überlappen. Es gilt eine Mindestpause nach gesetzlicher Vorgabe. Es gilt eine Tageshöchstarbeitszeit. Eine gespeicherte Buchung wird weder geändert noch gelöscht. Nicht Teil dieses Features: Wochenübersicht, Monatsabschluss und Korrektur von Buchungen. In dieser Phase gibt es keine Anmeldung; Mitarbeitende und Projekte sind fest hinterlegt."

## Clarifications

### Session 2026-10-25

- Q: Welche Lücken zwischen zwei Buchungen desselben Tages zählen als Pause? → A: Nur Lücken von mindestens 15 Minuten.
- Q: Rechnen Mindestpause und Tageshöchstarbeitszeit mit der tatsächlichen oder mit der abgerechneten Dauer? → A: Mit der tatsächlichen Dauer.
- Q: Wie behandelt Kontor eine Buchung über Mitternacht? → A: Kontor lehnt sie ab; wer über Mitternacht arbeitet, bucht zwei Einträge, einen pro Tag.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Arbeitszeit auf ein Projekt buchen (Priority: P1)

Eine Technikerin wählt sich aus der Liste der Mitarbeitenden aus und bucht für einen Tag
Beginn und Ende ihrer Arbeit auf ein Projekt, optional mit einer Notiz. Kontor speichert
Beginn und Ende so, wie sie eingegeben wurden, berechnet die abgerechnete Dauer, prüft, dass
sich die Buchung nicht mit einer anderen Buchung derselben Person überschneidet, und zeigt
nach dem Speichern alle Buchungen dieses Tages mit tatsächlicher und abgerechneter Dauer und
der Tagessumme.

**Why this priority**: Ohne Buchen hat Kontor keinen Wert. Diese Story allein ersetzt
bereits die Tabelle: Die Stunden liegen erfasst vor und müssen nicht mehr abgetippt werden.

**Independent Test**: Eine Person auswählen, eine Buchung anlegen und prüfen, dass sie mit
den eingegebenen Uhrzeiten, der richtigen abgerechneten Dauer und der richtigen Tagessumme in
der Tagesliste erscheint; eine überlappende zweite Buchung anlegen und prüfen, dass sie
abgelehnt wird.

**Acceptance Scenarios**:

1. **Given** Juna Sato hat am 2026-11-02 noch keine Buchung, **When** sie 08:10–10:17 auf
   Projekt `NSL-ALB-01` bucht, **Then** wird die Buchung mit 08:10–10:17 gespeichert, die
   Tagesliste zeigt die tatsächliche Dauer 2:07 und die abgerechnete Dauer 2:15, und die
   Tagessumme zeigt 2:15 Stunden.
2. **Given** Juna Sato hat am 2026-11-02 die Buchung 08:10–10:17, **When** sie
   10:00–12:00 auf Projekt `KBA-TG3-04` bucht, **Then** wird die Buchung abgelehnt mit dem
   Hinweis, dass sie sich mit 08:10–10:17 überschneidet.
3. **Given** Juna Sato hat am 2026-11-02 die Buchung 08:10–10:17, **When** sie
   10:17–12:00 auf Projekt `KBA-TG3-04` bucht, **Then** wird die Buchung gespeichert
   (Ende und Beginn berühren sich, sie überschneiden sich nicht), ihre abgerechnete Dauer
   ist 1:45, und die Tagessumme zeigt 4:00 Stunden.
4. **Given** Tomás Okoye hat am 2026-11-02 noch keine Buchung, **When** er 08:00–10:00 auf
   Projekt `HFL-PER-02` bucht, **Then** sind tatsächliche und abgerechnete Dauer beide 2:00
   (eine volle Viertelstunde wird nicht weiter aufgerundet).
5. **Given** eine gespeicherte Buchung, **When** Juna Sato die Tagesliste öffnet,
   **Then** gibt es keine Möglichkeit, die Buchung zu ändern oder zu löschen.

---

### User Story 2 - Arbeitszeitregeln bei der Erfassung einhalten (Priority: P2)

Beim Speichern prüft Kontor zusätzlich die gesetzlichen Arbeitszeitregeln für den ganzen
Tag der Person: die Mindestpause und die Tageshöchstarbeitszeit. Eine Buchung, die eine
Regel verletzen würde, wird nicht gespeichert, und Kontor sagt, welche Regel verletzt ist.

**Why this priority**: Die Regeln schützen die Mitarbeitenden und die GSC. Story 1 ist auch
ohne sie nutzbar, aber erst mit ihnen sind die erfassten Zeiten verlässlich.

**Independent Test**: Für eine Person nacheinander Buchungen anlegen, bis eine Pause fehlt
oder die Höchstarbeitszeit überschritten würde, und prüfen, dass genau diese Buchung mit
Nennung der Regel abgelehnt wird.

**Acceptance Scenarios**:

1. **Given** Rik Halvorsen hat am 2026-11-03 keine Buchung, **When** er 07:00–14:00 auf
   Projekt `HFL-PER-02` bucht, **Then** wird die Buchung abgelehnt, weil bei mehr als
   6 Stunden Arbeitszeit eine Pause von mindestens 30 Minuten fehlt.
2. **Given** Rik Halvorsen hat am 2026-11-03 die Buchung 07:00–11:00, **When** er
   11:30–14:00 bucht, **Then** wird die Buchung gespeichert (6:30 Stunden Arbeitszeit,
   30 Minuten Pause).
3. **Given** Rik Halvorsen hat am 2026-11-03 Buchungen über insgesamt 9:00 Stunden mit
   ausreichender Pause, **When** er eine weitere Buchung über 1:30 Stunden anlegt,
   **Then** wird sie abgelehnt, weil die Tageshöchstarbeitszeit von 10 Stunden
   überschritten würde.

---

### Edge Cases

- Was passiert, wenn das Ende vor oder gleich dem Beginn liegt? → Die Buchung wird
  abgelehnt.
- Was passiert bei einer sehr kurzen Buchung (z. B. 08:05–08:10)? → Sie wird gespeichert;
  die tatsächliche Dauer ist 0:05, die abgerechnete Dauer 0:15.
- Was passiert, wenn sich zwei Buchungen nur an einer Grenze berühren (Ende = Beginn)?
  → Das ist keine Überschneidung.
- Was passiert bei einer Buchung über Mitternacht? → Sie wird abgelehnt; wer über
  Mitternacht arbeitet, bucht zwei Einträge, einen pro Tag.
- Was passiert, wenn zwischen zwei Buchungen nur 10 Minuten liegen? → Die Lücke zählt
  nicht als Pause.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Mitarbeitende MÜSSEN eine Buchung mit Datum, Beginn, Ende, Projekt und
  optionaler Notiz erfassen können.
- **FR-002**: Das System MUSS die Person, für die gebucht wird, aus einer fest hinterlegten
  Liste der Mitarbeitenden auswählen lassen; eine Anmeldung gibt es nicht.
- **FR-003**: Das System MUSS als Projekt nur fest hinterlegte Projekte zur Auswahl anbieten.
- **FR-004**: Das System MUSS Beginn und Ende einer Buchung so speichern und anzeigen, wie
  sie eingegeben wurden; Beginn und Ende werden nicht gerundet.
- **FR-005**: Das System MUSS je Buchung die abgerechnete Dauer bestimmen: die tatsächliche
  Dauer, aufgerundet auf die nächste volle Service-Viertelstunde (15 Minuten). Eine Dauer,
  die bereits ein Vielfaches von 15 Minuten ist, bleibt unverändert.
- **FR-006**: Das System MUSS eine Buchung ablehnen, deren Ende nicht nach dem Beginn liegt.
  Beginn und Ende liegen am selben Tag; eine Buchung über Mitternacht wird abgelehnt.
- **FR-007**: Das System MUSS eine Buchung ablehnen, die sich zeitlich mit einer bestehenden
  Buchung derselben Person überschneidet. Berühren sich zwei Buchungen nur (Ende = Beginn),
  ist das keine Überschneidung.
- **FR-008**: Das System MUSS eine Buchung ablehnen, wenn die Tagesarbeitszeit der Person
  danach mehr als 6 Stunden beträgt und die Pause weniger als 30 Minuten, oder mehr als
  9 Stunden und die Pause weniger als 45 Minuten. Als Pause zählen nur Lücken zwischen zwei
  Buchungen desselben Tages von mindestens 15 Minuten.
- **FR-009**: Das System MUSS eine Buchung ablehnen, wenn die Tagesarbeitszeit der Person
  danach mehr als 10 Stunden beträgt.
- **FR-013**: Tagesarbeitszeit und Pausen für FR-008 und FR-009 MÜSSEN aus den tatsächlichen
  Dauern berechnet werden, nicht aus den abgerechneten.
- **FR-010**: Bei jeder Ablehnung MUSS das System die verletzte Regel benennen.
- **FR-011**: Gespeicherte Buchungen DÜRFEN weder geändert noch gelöscht werden können.
- **FR-012**: Nach dem Speichern MUSS das System alle Buchungen der Person für diesen Tag
  mit Beginn, Ende, tatsächlicher und abgerechneter Dauer anzeigen, dazu die Tagessumme als
  Summe der abgerechneten Dauern.

### Key Entities *(include if feature involves data)*

- **Mitarbeitende**: eine Person, die Arbeitszeit bucht; Name. In dieser Phase fest
  hinterlegt.
- **Projekt**: ein Wartungsauftrag, auf den gebucht wird; Projektschlüssel und Bezeichnung.
  In dieser Phase fest hinterlegt.
- **Buchung**: gehört zu genau einer Person und einem Projekt; Datum, Beginn, Ende (wie
  eingegeben), optionale Notiz. Tatsächliche und abgerechnete Dauer ergeben sich daraus.
  Nach dem Speichern unveränderlich.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Eine Technikerin erfasst eine Buchung in unter 30 Sekunden.
- **SC-002**: 100 % der Buchungen, die eine Regel verletzen (Überschneidung, Pause,
  Höchstarbeitszeit), werden bei der Erfassung abgelehnt.
- **SC-003**: Die angezeigte Tagessumme entspricht in 100 % der Fälle der Summe der
  abgerechneten Dauern des Tages.
- **SC-004**: Keine gespeicherte Buchung lässt sich nachträglich ändern oder löschen.

## Assumptions

- Die gesetzlichen Vorgaben sind die des deutschen Arbeitszeitgesetzes: mehr als 6 Stunden
  Arbeit verlangen mindestens 30 Minuten Pause, mehr als 9 Stunden mindestens 45 Minuten;
  die Tageshöchstarbeitszeit beträgt 10 Stunden.
- Alle Mitarbeitenden arbeiten in derselben Ortszeit; Zeitzonen spielen keine Rolle.
- Eine Notiz ist freier Text mit höchstens 500 Zeichen.
- Mitarbeitende und Projekte werden in dieser Phase nicht über die Oberfläche gepflegt.
- Korrekturen falscher Buchungen sind nicht Teil dieses Features; bis dahin bleibt eine
  gespeicherte Buchung, wie sie ist.
