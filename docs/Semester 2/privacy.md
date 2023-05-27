# PRIVACY

## Administratives

### Leistungsnachweis

-   30% Gruppenarbeit
    -   10% Executive Summary
    -   10% Präsentation
    -   10% Executive Summary (Recht)
-   70% Prüfung (Form: TBD)

Gruppenarbeit Note ist eine Gruppennote

### Deadlines

-   [x] 03\. März: Gruppenbildung
-   [x] 10\. März: Executive Summaries

## Inhalt

### Big Data

collect massive amounts of (different) data from many different people.

**Examples:**

-   Sensors (smart watch)
-   Web Data, E-Commerce
-   Mobile phones
    -   GPS
    -   Search entries
-   E-mails, Social networks

**Dimensions**:

-   Volume (amount of data)
-   Velocity (speed of generation of the data)
-   Variety (different kind of data)
-   Veracity (quality of data)
-   Value (commercial value of data)

### Privacy

1. **Personal privacy**  
   Schutz einer Person vo unangemessenen Eingriffen die ihr moralisches Empfinden und Privatsphäre verletzen
2. **Territorial privacy**  
   Schutz eines physischen Bereichs um eine Person herum, der ohne Zustimmung der Person nicht verletzt werden darf.
3. **Informational Privacy**  
   Schutz vor missbräuchlichen Verarbeitung personenbezogener Daten sowie den Schutz des rechts auf informationelle Selbstbestimmung.

#### Privacy Control

-   Psudonymität -> seine Identität nicht preisgeben
-   Unbeobachtbarkeit -> andere können nicht beobachten
-   Unlinkability -> Absender und Empfänger können nicht als miteinander kommunizierend identifiziert werden
-   Depersonalisierung -> meist nur wenig verändert (mit 2. er Database umkehrbar)

#### Digitale Identität

Digitale Identitäten repräsentiere die Nutzer in der Informationsgesellschaft. Solche digitalen Identitäten bestehen aus technisch abgebildeten Attributen der Nutzer.

**-diebstahl:** die missbräuchliche Nutzung personenbezogener Daten (der Identität) einer natürlichen Person durch Dritte.

#### Anonymität

Ein Subjekt ist anonym gegenüber einem Angreifer, wenn der Angreifer das Subjekt in einer Menge von Subjekten nicht hinreichend identifizieren kann.

**Arten der Anonymität**

-   Sender und Empfängeranonymität -> Identität
-   Ortsanonymität -> Attribute
    **Anonymisierung**
-   der Prozess, um Anonymität zu erreichen
-   personenbezogene Daten derart zu verändern, dass Einzelangaben nicht mehr oder nur sehr schwierig einer bestimmten oder bestimmbaren natürlichen Person zugeordnet werden.

**Warum Anonymität?**

-   Tracking Vermeiden
-   Vertrauliche persönliche Informationen (Abstimmung, politische Meinung)

**Nicht einfach anonym geblieben:**

-   Netzwerk IP
-   Web App
    -   Cookies
    -   Flash

**Absolute Anonymisierung**  
Diese liegt vor, wenn niemand mehr in der Lage ist, den Personenbezug wiederherzustellen. Dies kann bspw. durch die Löschung von Identifikationsmerkmalen in einer Datenbank erfolgen.

### Tracking

#### Cookies

-   Werden unter den Domainnamen des Webservers abgespeichert und bei jeder neuen Abfrage an diesen Webserver zurückgeschickt.
-   Zweck: HTTP selbst ist zustandslos

**1st Party cookies**

-   von der Website gesetzt auf der ein User gerade surft
-   werden von Browsern nicht

**3rd Party Cookies**

-   Durch einen Dritten gesetzt, also nicht durch die eigentliche Website, auf der man sich befindet.
-   Häufig von Advertisern gespeichert, die Werbung auf

**Cookie syncing**

-   Firmenübergreifende Daten, gespeichert mit Cookies

#### Fingerprint

**Browser-Fingerprinting**

-   Reguläre Web-Interatkionen werden benutzt um Informationen über dich zu sammeln und dich zu identifizieren

**Vermeiden**

-   Javascript deaktivieren
-   Cookies deaktivieren
-

#### Mobile Tracking

!!! warning "informative website 'do not track'"

    [website](https://donottrack-doc.com/en/intro)

#### Location Tracking

TPEG-Zugriff (Transpoort Protocol Experts Group)  
ist eine Serie von Datenprotokollen für die Übertragung von Verkehrs- und Reiseinformationen.

#### Gesichtserkennung

1. Gesichtsfindung
2. Referenzpunktdetektor (Augen, Nase, Mund, etc.)
3. Gesichtserkennung
    - Identifikation
    - Verifikation
    - Analyse

### RFID

| Short | meaning                        |
| ----- | ------------------------------ |
| RFID  | radio frequency identification |
| NFC   | near field contact             |

#### E-Passport

Für das Auslesen von Fingerabdrücken / einfordern von sensiblen Daten wird ein Auslesezertifikat benötigt.

#### Access Control

Basic Access Control (BAC) ermöglich den Zugriff auf grundlegende Passdaten, während Extended Access Control (EAC) verschlüsselte Kommunikation und Zugriff auf biometrische Daten ermöglicht.

#### Crowds

Webanfragen hinter anderen Crowds Diensbenutzer verbergen. Jeder Benutzer hat eine Applikation installiert (Jondos). Der Traffic wird über zufällige "Jondos" weitergeleitet.

#### Mix

Nachrichten werden nicht direkt an den Empfänger gesendet, sondern werden über zufällige Mixes gesendet.  
**Grundfunktionen**:

1. Löschen von Duplikaten
2. Sammeln von Nachrichten
3. Umkodierung der Nachrichten
4. Umsortieren der Nachrichten

### Anonymisierung

#### Generalisieren

**Beispiel PLZ:**  
4012 4011 4019 -> 401\*

#### k-Anonymität

k-Anonymität tritt immer dann auf wenn ein Eintrag k-mal in einer Liste auftaucht. Beispiel PLZ: für 3-k tritt der Eintrag 401\* drei mal auf.

### Datenschutz

!!!note "Lernziele"

    1. Ordnen das Recht konkreten digitaleen Sachverhalten zu
    2. Definieren die Persönlichkeit und entscheiden, ob sie verletzt wurde
    3. Wenden die Grundzügee im Datenschutzgesetz an
    4. Unterscheiden die Datenbearbeitung von Behörden und Privaten
    5. Rechtfertigen Eingriffe in die Persönlichkeit.
    6. Erkennen die Risiken und nennen die Folgen von unrechtmässigen Eingriffen in die Persönlichkeit.
    7. Wenden die Erkenntnisse der Vorlesung auf Gruppenarbeiten an

#### Person

Was macht eine Person aus?

-   Natürliche Person: Person aus Fleisch und Blut
-   Juristische Person: Person die durch Gesetz entsteht (Firma, Verein, Staat)

#### Beiteiligte

1. Betroffene Person - eine identifizierbare lebende Persone deren Daten verarbeitet werden
2. Datenschutzberater / Datenschutzbeauftragter - wacht über und leitet die Datenbearbeitung in einem Unternehmen
3. Verantwortlicher / Controller - bestimmt Zweck und Mittel der Datenverarbeitung
4. Auftragsbearbeiter / Prozessor - verarbeitet Daten im Auftrag des Verantwortlichen
5. Empfänger - erhält Daten vom Verantwortlichen (maybe)
6. Vertreter - vertritt den Verantwortlichen im Ausland
7. Dritte - Jede Partei, an die personenbezogene Daten weitergegeben werden

#### Was schützt das DSG?

-   Persönlichkeit
-   Privatsphäre
-   Persönlichkeitsrechte

#### Datenbearbeitung

Jede Bearbeitung von Personendaten

-   Beschaffung
-   Speicherung
-   Verwendung
-   Veränderung
-   Bekanntgabe
-   Archivierung

#### Wann ist eine Datenbearbeitung erlaubt

-   Einwilligung der betroffenen Person
-   Bearbeitung der Daten ist rechtmässig
    -   Bearbeeitung durch Private
    -   Bearbeitung durch Behörden
    -   Bearbeitung durch Organe
-   Ist die von mir vorgesehene Bearbeitung verhältnismässig
-   Sind die Daten korrekt

### Datenexport - Geheimnisschutz

---

<p style="color: lightgreen;">Hello</p>
