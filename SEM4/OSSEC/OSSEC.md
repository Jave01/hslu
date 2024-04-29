# OSSEC

## MEP

- Schriftliche Prüfung (100%)
- Open book


## Was ist ein Computer

- **Von-Neumann-Architektur:** Modell, Computer enthält Computerprogrammbefehle als auch Daten.
- Trennung von Berechnung und Steuerung
- Flexibilität durch Softwaresteuerung.
- **CPU:**
	- ALU
	- Cache
	- Control Unit: liest Programmbefehle aus


![[Funktionsweise_computer.canvas|Funktionsweise_computer]]


### Boot Ablauf

1. **POST:** Power On Self-Test
2. Initialisierung der Hardware
3. Optional
	1. BIOS/UEFI Passwort
	2. Startbildschirm
	3. UEFI Konfigurationsmenu
4. Aufrufen von Erweiterungen 
5. Urlader: Feststellen, von welchem Datenträger gebootet werden soll




## Secure Boot & Hauptverschlüsselung

Boot-Ablauf:
1. Hardware
	1. Urlader 
		- lädt Programm von Bootsektor der CD oder USB Stick, welches den **Lader** nachlädt und im Bootsektor der Festplatte speicher
	2. Lader 
3. Betriebssystem
4. Anwenderprogramme



### Boot Sektor Virus

- Wird vor Betriebssystem geladen 
- Schutzmassnahme: Secure Boot



### Secure Boot

- Software-Komponenten werden verifiziert
- Verwendet Kryptografische Signaturen, die in UEFI Datenbank hinterlegt werden
- TPM


### Angriffsvektoren auf Hauptspeicher

- Damit Passwörter nicht jedes Mal eingegeben werden.


### Cold-boot Angriff

[[OSSEC 02 - Secure Boot aamp; Hauptspeicherverschluesselung.pdf]] p.50

RAM Auslesen mit Zugriff auf Hardware.


### Motivation zur Hauptspeicherverschlüsselung

- Integrität
- Vertraulichkeit
- Unbefugter Zugriff verhindern
- Vertrauenswürdigkeit


### TPM

Drei Operationen:

- **Binding:** Code und Daten verschlüsseln
- **Sealing:** Bezieht Konfiguration, nur wenn gleich, werden Daten entschlüsselt (BitLocker)
- **Remote Attestation:** Remote Partei kann Konfiguration überprüfen



Nachteile:
- Sämtliche Operation haben einen einzigen Vertrauensanker
- Für eine Messung der Vertrauenswürdigkeit per TPM muss die gesamte Konfiguration eines Systems betrachte werden:
	• über Measured Launch hinaus
	• Hashes für sämtliche Komponenten des Betriebssystems
	• Hashes für alle Treiber und jede geladene Software


### AMD: Secure Memory Encryption

Bei Memory Read entschlüsselt (C-bit)
Daten werden entschlüsselt bevor der CPU gefüttert.

Memory wird in sichere und unsichere "Welt" unterteilt.

### AMD: SEV

• Für Serverprozessor
• Zielt auf nicht vertrauenswürdige Cloud-Provider, die sensible Kundendaten aus dem Server-RAM auslesen könnten
• Kann beliebig grosse Speicherbereiche einrichten


### AMD TrustZone

Selbst höher privilegierte Software, einschliesslich Betriebssystem, kann nicht auf TrustZone zugreifen.

Verbindung findet über Monitor-Call statt.



## Authentisierung und Autorisierung

- Nachweis einer Person, dass sie die Person ist, die sie vorgibt zu sein
- Auch anwendbar für Objekte, Tiere, Dienste usw.


**Authentisierung:**
- Erfolg durch Vorlegen eines Nachweises, der die Identität bestätigen soll
	- geheime Informationen
	- Identifizierungsgegenstand
	- Identifizierungsobjekt
- Starke Authentisierung: Kombination dieser Verfahren


**Authentifizierung:**
- Prüfung (Verifikation)
- Überprüfen auf Echtheit
- Findet nach Authentisierung statt


**Autorisierung:**
- Einräumung von speziellen Rechten
- Prüfung der Rechte und Konsequenz
- Erfolgreiche Identifikation heisst nicht automatisch erlaubte Nutzung der Zugriff auf bereitgestellte Dienste, Leistungen oder Ressourcen



### Nachvollziehbarkeit & Claims

**Nachvollziehbarkeit**
- Definiert das Mass wie viel aufgezeichnet wird

**Claim**
- Eigenschaft einer Identität, die für Zugriff entscheidend ist


### Identity Management

- Erstellung, Speicherung, Synchronisation & Löschung von Identitäten
- organisatorischer Stuff ist schwieriger als technisch
- Tools:
	- LDAP
	- Meta-directory


### Access Management

- Regelt Zugriff eines Subjekts auf ein Objekt
- Beinhaltet Authentisierung und Autorisierung
- Zugriff auf Ressourcen muss gesteuert werden
- Protokolle
	- SAML
	- Kerberos


### Identity and Access Management

- **Governance**
	- Definition Policy
	- Festlegung Organistation, Domänen, Akteuere & Prozesse
	- Treffen von Entscheidungen in Bezug auf Strategie
- **Risikomanagement**
	- Analysieren und Behandeln der Risiken im Zusammenhang mit IAM
- **Compliance**
	- Sicherstellen der Einhaltung der regulatorischen Rahmenbedingungen
	- Prüfen der Einhaltungen



### Verwaltung von Identitäten

- Zu klären:
	- Wer-wem-welche
	- Umgang mit "Externen"
	- Umgang mit Personalwechsel
	- Abgabe von Schlüsseln
	- Sperren von Konten
	- Vergessene Zugangscodes
- Primär ein Problem der richtigen Prozesse


## Kapselung

>[!note]
>Java might not run on a System, just because it runs in the JVM -> libraries might be platform specific


### Docker image

- So schlank wie möglich
- Systembibliothek verläuft konservativ -> Abhängigkeiten werden an Container ausgelagert
- Eine Anwendung kann in derselben Umgebung übersetzt werden,, in der sie später laufen soll



### Übung

**Härtung**
- Container Host: Angriffsvektoren minimieren, Zugriffskontrolle
- Runtime: Status Baselines, (normal, sicher) Fokus auf Absichern der Apps
- Registry: Server absichern, Zugriffsrichtlinien etablieren, Image-Scanner einsetzen
- Images: Minimum an Code, Single Purpose
- Orchestrator: aus offizieller Quelle
- Persistent Storage: Schreibzugriff auf Shared Storage nötig?



### Vorteile

- Nutzer müssen nur wenige Kommandos für Installation kennen
- Installation ist minimalinvasiv
- Abhängigkeiten der Applikation vom Zielsystem sind auf Systemaufrufe reduziert
- Anwendung bringt ihre eigenen Bibliotheken mit
- Gute Performance


### Nachteile

- GUI-Ausgaben schwierig
- Geschwindigkeit nicht gleich wie nativ
- Permanente Speicherung - alle Daten zerstört nach shutdown


### Some stuff for Docker

- Kubernetes
	- Container Verwaltung
	- containerisierte Anwendung auf einzelne Knoten verteilen
	- JSON config files
	- besteht aus Diensten die auf die Hosts verteilt sind