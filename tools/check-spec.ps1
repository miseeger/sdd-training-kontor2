<#
.SYNOPSIS
  Prüft die Spec-Kit-Artefakte eines Repositories auf Hygiene (drei Regeln).

.DESCRIPTION
  R1  In keiner specs/*/spec.md steht noch ein [NEEDS CLARIFICATION ...]-Marker.
  R2  In keiner specs/*/tasks.md ist ein Task offen ("- [ ]").
  R3  Die Constitution (.specify/memory/constitution.md) ist vorhanden und enthält keinen
      Platzhalter der Vorlage wie [PROJECT_NAME].

  Das ist Hygiene, kein Beweis: Ein gesetzter Haken sagt nicht, dass der Code die Aufgabe erfüllt
  (Handbuch Kap. 4.11, "completion claims are not evidence"). Ob die Spec stimmt und der Code zur
  Spec passt, prüfen Menschen und /speckit-converge.

  Die Regeln stammen nicht aus der Spec-Kit-Doku, sondern sind ein Eigenbau für dieses Training.
  Gesucht wird nur in den Dateien, die gemeint sind: research.md und die Checklisten zitieren den
  Marker, ohne ihn offen zu lassen.

.PARAMETER Root
  Wurzel des Repositories. Standard: das Verzeichnis über tools/.

.EXAMPLE
  ./tools/check-spec.ps1
#>
[CmdletBinding()]
param(
    [string] $Root = (Split-Path -Parent $PSScriptRoot)
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$Root = (Resolve-Path -LiteralPath $Root).Path
$fehler = [System.Collections.Generic.List[string]]::new()
$inPipeline = $env:TF_BUILD -eq 'True'   # Azure Pipelines setzt TF_BUILD

function Get-RelPath([string] $Pfad) {
    [System.IO.Path]::GetRelativePath($Root, $Pfad).Replace('\', '/')
}

function Add-Fehler([string] $Regel, [string] $Pfad, [int] $Zeile, [string] $Text) {
    $rel = Get-RelPath $Pfad
    $meldung = "[$Regel] $Text"
    if ($inPipeline) {
        # Azure Pipelines zeigt das als Fehler mit Datei und Zeile an
        Write-Host "##vso[task.logissue type=error;sourcepath=$rel;linenumber=$Zeile;columnnumber=1]$meldung"
    }
    else {
        Write-Host "FEHLER  ${rel}:$Zeile  $meldung" -ForegroundColor Red
    }
    $fehler.Add("$rel`:$Zeile $meldung")
}

# R1 und R2: je Feature unter specs/
$specs = Join-Path $Root 'specs'
$features = @()
if (Test-Path -LiteralPath $specs) {
    $features = @(Get-ChildItem -LiteralPath $specs -Directory | Sort-Object Name)
}

foreach ($feature in $features) {
    $spec = Join-Path $feature.FullName 'spec.md'
    if (Test-Path -LiteralPath $spec) {
        $zeilen = Get-Content -LiteralPath $spec
        for ($i = 0; $i -lt $zeilen.Count; $i++) {
            if ($zeilen[$i] -match '\[NEEDS CLARIFICATION') {
                Add-Fehler 'R1' $spec ($i + 1) 'Offener Klärungsmarker in der Spec.'
            }
        }
    }

    $tasks = Join-Path $feature.FullName 'tasks.md'
    if (Test-Path -LiteralPath $tasks) {
        $zeilen = Get-Content -LiteralPath $tasks
        for ($i = 0; $i -lt $zeilen.Count; $i++) {
            if ($zeilen[$i] -match '^\s*- \[ \]') {
                $task = $zeilen[$i].Trim()
                Add-Fehler 'R2' $tasks ($i + 1) "Task offen: $task"
            }
        }
    }
}

# R3: Constitution
$constitution = Join-Path $Root '.specify/memory/constitution.md'
if (-not (Test-Path -LiteralPath $constitution)) {
    Add-Fehler 'R3' $constitution 1 'Die Constitution fehlt.'
}
else {
    $zeilen = Get-Content -LiteralPath $constitution
    for ($i = 0; $i -lt $zeilen.Count; $i++) {
        if ($zeilen[$i] -match '\[[A-Z][A-Z0-9_]{3,}\]') {
            Add-Fehler 'R3' $constitution ($i + 1) "Platzhalter der Vorlage in der Constitution: $($Matches[0])"
        }
    }
}

Write-Host ''
Write-Host "Geprüft: $($features.Count) Feature(s) unter specs/ und die Constitution."
if ($fehler.Count -gt 0) {
    Write-Host "Ergebnis: $($fehler.Count) Fehler." -ForegroundColor Red
    exit 1
}
Write-Host 'Ergebnis: grün.' -ForegroundColor Green
exit 0
