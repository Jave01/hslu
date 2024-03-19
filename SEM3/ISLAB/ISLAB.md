- Immer selbstständige Theorievorbereitung, dann Labor.

# Aufteilung
- 35 h Unterricht
- 15 h Termpaper
- 25 h Fertigstellung von Übungen
- 15 h Prüfungsvorbereitung

Bewertung:
- 49% Termpaper & Präsentation
- 51% mündliche Prüfung zum Modulinhalt, 15 min

# Generell Termpaper
- Termpaper Präsentation Ende Semester, 5 min?, nur essentielle Punkte.
- ChatGPT legal, aber muss als Quelle angegeben werden.
- Quellen Format: APA 7th edition.
- HSLU WIPRO Struktur muss eingehalten werden. -> einzelne Kapitelstruktur ("Aufbau_Bericht_HSLU_Projektschine.pdf" in Illias)
	- Problem, Fragestellung -> 3 bis 4 Bulletpoints, Ja/Nein Fragen
	- Stand der Forschung / Technik (Erwartung ein Informatiker liest es)
	- Ideen und Konzepte: Wie kommen wird von Zustand A nach B (Computersimulation, Laborversuch)
	- Methoden (vielleicht Experiment, Literaturrecherche, Interview (nicht empfohlen))
	- Realisierung, Hauptteil
	- Evaluation und Validation, Fragestellung von 1. erfüllt.
	- Ausblick, was kann man tun, wenn man daran weiter forscht.
	- Anhänge
	- Abkürzungs-, Abbildungs-, Tabellen-, Formel-Verzeichnis
	- Literaturverzeichnis
- Gute Abbildungen, kann auch Skizze sein
- Gute Tabellen
- Scope: max 35'000 Zeichen Inhalt mit Leerzeichen ohne Verzeichnisse


# Thema
## Prioritäten
1. Threat Intelligence
2. Malware Analyse
3. NFC Relay Attacks
4. NFC Security
5. Hardware Attacks
6. Vault 7 - CIA Leaks


## Malwares
- GootLoader  [(NJCCIC) page on GootLooader](https://www.cyber.nj.gov/alerts-advisories/gootloader-malware-platform-uses-sophisticated-techniques-to-deliver-malware) and [BlackBerry’s Blog on GootLoader](https://blogs.blackberry.com/en/2022/07/gootloader-from-seo-poisoning-to-multi-stage-downloader).
- GOZ




## Access Management Linux

Berechtigung besteht aus zwei Komponenten:
- Ressource, auf welche zugegriffen wird
- die zu berechtigenden Operationen, welche für die Ressource freigegeben werden


Directory:
- read: Dateien mit ls auflisten
- write: hinzufügen, umbenennen, löschen
- execute: change directory


UID & GID herausfinden: `getent passwd`.



## Access Management Windows

Active Directory: Eine Möglichkeit Benutzer und Gruppen verschiedene Recte zuzuteilen. Erhöht die Sicherheit.


Standardmässige Struktur:
- Organisationseinheiten (OU's) - Sammlungen, welche mehrere Objekte beinhalten (User, Gruppen, Computer), Objekte werden nach Kategorien gesammelt.
- Container: beinhaltet eine oder mehrere Active Directory Domains, diese beinhaltet eine hierarchische Struktur an Objekten, Security Services, Regeln und DNS Domain Namen.
- OU's haben keine Regeln, sind nur zur Aufteilung.


## Docker Introduction

- Docker Images: enthalten die Umgebung mit Betriebssystem, Libraries und so weiter, plus die Applikation die danach auf dem Container laufen soll. Mit Ihnen können mithilfe des Docker Daemons Container gestartet werden.
- Vorteile:
	- Scalable, 
	- Portable, sofern das Endgerät eine Docker Engine hat lauft mein Container dort
	- Deployment vereinfacht.
- Unterschiede zu VM's:
	- Virtuelle Maschine hat immer ein GastOS
	- VM stellt ein komplettes OS zur Verfügung -> man interagiert direkt damit. Bei Docker kommuniziert man mit dem Daemon, der die Images verwaltet.
- Images isolieren:
	- Binaries / Libraries
	- Applikationen
	- OS


Syntax:
![[Pasted image 20240117102356.png]]


## Docker Security

- Control Groups: Bieten eine Möglichkeiten Systemressourcen zu unterteilen
- User Namespace: ermöglicht es User und Gruppen von Containern und dem System zu trennen. Doppelbelegungen möglich


Docker Security Bench: script zum Überprüfen der Sicherheit, anhand von best practices.

Wenn man der Docker Gruppe angehört, hat man automatisch admin Rechte.

Wenn ein kritisches Script in ein allgemein verfügbarer Ordner kopiert wird.


## Network Security Monitoring (NSM)

- NIDS: Kontrolliert Netzwerkverkehr auf Schadsoftware
- Host Intrusion Detection: Snapshot von allen Dateien auf Host, vergleicht ob zwischen zwei Snapshots kritische Dateien verändert wurden.


Suricata: Intrusion Detection System, mit den allgemeinen rules-syntax.
Kibana: Front-end, welches die Logs analysiert.

nftables: Kann gebraucht werden um Packete zu droppen.


## Public-Key Infrastructure

