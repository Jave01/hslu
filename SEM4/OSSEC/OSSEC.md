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