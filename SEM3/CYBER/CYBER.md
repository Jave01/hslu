# CYBER
## General
- Selbststudium Theorie
- Anwendung im Unterricht und Gruppen
- Testatbedingung sind die 5 Übungs-PDF's
- Prüfung ist 1.5 h schriftlich & open-book

Gute Quelle für Bedrohungen: Enisa
[Awesome Pentest](https://github.com/enaqx/awesome-pentest)

## Hacking ist Kreativität
Find ways the others haven't thought about

## Begriffe
### Exploit
- CPE: Common Platform Enumeration
- CWE: Common Weakness Enumeration
- CVE: Common Vulnerability & Exposure
- CVSS: Common Vulnerability Scoring System

### Mitre
- Non-Profit Abspaltung von MIT.
- Verwaltet CVE
- Organistaion zum Betrieb von Forschungsinstituten

### Buffer overflow
Memory überschreiben

## Vorgehen bei Pentest
- Bereits bekannte Lücken zu suchen und ausnutzen
- Entdecken von fehlenden Sicherheitsmassnahmen

1. Vorbereitung
	1. Genehmigung einholen
	2. Durchführungszeitraum
	3. Was tun bei Störung / Notfalls
	4. Ziele definieren 
	5. Aufwand abschätzen
	6. Projektplanung
2. Informationsbeschaffung
	- Host discovery
	- Portscans
	- Banner Grabbing
	- Schwachstellenscan
3. Bewertung der Informationen
	- Ziele für manuelle Angriffe auswählen
	- Aufwand und Erfolgschancen abwägen
4. Aktive Angriffe
	- Ausnutzung gefundener oder vermuteter Schwachstellen
	- Dokumentation aller Angriffe!
5. Abschlussanalyse
	- Abschlussbericht / -präsentation / -gespräch


## Kill Chain

Reihenfolge:
1. Recon
2. Weaponization
	1. Coupling exploit with backdoor
3. Delivery (E-Mail, Website ...)
4. Exploitation (Execute code on victim machine)
5. Installation - installation malware


Nutzen:
- Audienz im C-Level
- Angriffsbestandteile einordnen
- Gegenmassnahmen einordnen


## Rootkits
Ziel: Persistenter Admin Zugang

### Beispiele
Binary rootkits:
- Sammlungen von modifizierten Systembinärdateien
- Angreifer überschreibt Originaldateien
- Quellcode modifizieren und Binärdatei neu kompilieren

Kernel Rootkits:
- User-Prozess muss System Calls brauchen
- Nutzen Loadable Kernel Modules (LKM's) für Linux oder Gerätetreiber für Windows


Virtual Machine Based Rootkits:
- Idee: Verschiebung des aktuellen Betriebssystems in eine VM
- Das originale Betriebssystem wird als VM ausgeführt
- Installation: ändern der Boot-Sequenz
- Das Zielsystem läuft jetzt als Gast. das Rootkit ist Host

BIOS/UEFI Rootkits
- Veränderung von Windows Dateien in der Boot-Phase

### Gegenmassnahmen
- Anti-Virus
- Offline: Prüfsummen
- Online
	- Verhaltensanalyse
	- Speicheranalyse
	- Kommunikation IDS/IPS
- LKM's verbieten


## Reporting

>[!warning]
>Wenn es nicht aufgeschrieben wurde ist es nicht passiert!

