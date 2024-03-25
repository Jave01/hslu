# Mobile Information Security

## Terminologie

- **MANET:** mobile ad hoc networks
- **VANET:** vehicular ad hoc networks (sub group of MANET)


## History

- 1895: Erste Funkübertragung 
- 1907: Erste Kommerzielle transatlantische Verbindungen
- 1926: Erstes Telefon im Zug
- 1979: 1G
- 1991: 2G (erstmals digital)
- 1992: Einführung von Global Systems for Mobile communications (GSM)
- 2000: 3G (max 5.76 Mbit/s)
- 2009: Long Term Evolution (LTE - almost 4G)
- LTE Advanced -> 4G
- 2018: 5G - theoretisch bis zu 20 Gbit/s


Standardisierungs-Organisation: 3GPP (3rd Generation Standard Partnership Project)


Geordnet mit Abkürzungen und "Haupt"-Standard


## Aufbau eines Wireless Networks

1. Drahtlose Geräte
2. Basisstation
3. Drahtlose Verbindung
4. Netzwerkinfrastruktur: verbindendes Kernnetz
	1. Kernnetz / core network (Mobilfunk)
	2. Backbone (kabelgebundenes Internet)
	3. Backbone (WiFi)


**ad-hoc Modus:** 
- keine Basisstationen
- Knoten bilden eigenes Netzwerk (Mesh)




**Challenges:**
- Mehrbenutzerbetrieb
- Hohe Datenrate
- Hohe Abdeckung
- Spektrumbeschränkungen
- Störanfälligkeit gegenüber anderen



### Herausforderungen einer drahtlosen Kommunikation

- wireless
	- Mehrbenutzerbetrieb
	- Hohe Datenrate
	- **Hohe Abdeckung:** Begrenzte Abdeckung, abhängig von Sende- und Empfangsstärke
	- **Spektrumbeschränkungen:** Begrenzte Anzahl Frequenzen
	- **Störanfälligkeit gegenüber Anderen:** (Senden stört andere Teilnehmer)
- mobility
	- Geräte wechseln Standort
	- Energieeinschränkungen
	- Geräte befinden sich an einem unbekannten Standort



### Beispiele

- drahtlose
	- Mobilfunk
	- WLANs
	- Sensornetzwerke
	- Satellitenkommunikationsnetzwerke
	- Richtfunk
- Wireless PAN (Personal Area Network)
- ...


### Zahlen & Fakten

- 2012: Apple verkauft 4 Smartphones/s 340'000/tag
- 2016: mehr Mobilfunkverkehr als restlicher Verkehr
- 2021: 71% Mobilfunk


## Grundbegriffe und -konzepte

### Zelluläres Konzept

McDonald, AT&T, 1978

Basisstationen in Sektoren Segmentierung in Zellen

Vorteile:
- Vollständige Abdeckung
- Räumliche Wiederverwendung verfügbarer Frequenzen
- Einführung von Regionen zur Wiederverwendung von Frequenzen
- Handover oder Handoff: Übergabe der Verbindung über die Zellengrenze
- Standortbezogene Dienste

Faktoren für Funkzellengrösse
- Meteorologisch und geografische Gegebenheiten
- Siedlungsstruktur
- Vegetation

Faktoren für Zellgrösse
- Erlaubte Sendeleistung
- verwendeter Mobilfunkstandard
- Aufbauhöhe und Typ


Dreifarbenproblem:
- Graphentheorie, keine Nachbaren haben die gleiche Farbe


## Terminologie

- **home network:** permanentes "Zuhause" des Handys (z.B. Swisscom)
- **Heim-Adresse:** Adresse unter der das Handy erreichbar ist (z.B. SIM-ID)
- **Home-agent:** Führt Mobilitätsfunktionen im Namen des Handys durch, wenn sich Handy entfern
- _wechselt Netzwerk_
- **visited network:** fremdes Netzwerk, in dem sich das Handy befindet
- **care-of-address:** Adresse im besuchten Netzwerk
- **Heim-Adresse:** Zugehörigkeit bleibt bestehen
- **Korrespondent:** möchte mit Handy kommunizieren
- **Foreign agent:** gleich wie _Home-agent_ aber im besuchten Netzwerk


**Ansätze:**
- Netzwerk:** 
	- "Das Protokoll soll das regeln"
	- globaler Table mit routing von IP zu Location
	- keine Änderungen an Endsystemen
	- nicht skalierbar
- **End-System
	- "functionality at the edge"
	- **Indirektes Routing:** Kommunikation vom Kommunikationspartner zum Mobiltelefon geht über das Heimnetzwerk und dessen Heim-Adresse und wird dann ins _visited network_ weitergeleitet
	- **Direktes Routing:** Kommunikationspartner erhält Adresse des Handys im _visited network_ und sendet direkt ans Handy


**Schritte bei \[ ]:**
1. Mobilgerät meldet sich bei Mobilitätsmanager an (foreign agent)
2. Mobilitätsmanager registriert den Standort des Mobiltelefons beim Heimat-HSS



**Indirektes routing:**
1. Kommunikationspartner verwendet die Heimatadresse als Datagramm-Zieladresse
2. Home-Gateway leitet an _visited network_ gateway weiter.
3. gateway kommuniziert mit Mobiltelefon
4. _Mobiltelefon sendet zurück an visited network gateway_
5. Gateway leitet entweder an home network gateway oder direkt zu Korrespondent weiter

Vorteile:
- Falls Bewegung zwischen Netzwerken: transparent für Kommunikationspartner
- Laufende Verbindungen zwischen Kommunikationspartner und Mobiltelefon können aufrechterhalten werden

Nachteile:
- Ineffizient


**Direktes routing:**
1. Kommunikationspartner kontaktiert home-HSS
2. Erhält visited network des Mobiltelefons
3. Datagramm wird an Adresse von _visited network_ gesendet
4. Visited network router leitet an handy weiter


## Charakteristika einer drahtlosen Verbindung

Unterschiede zu kabelgebunden:
- Pfadverlust
- Interferenz
- Mehrwegausbreitung (unterschiedliche Ankunftszeiten)


Hidden nodes:
- A, B hören sich
- B, C hören sich
- A, C hören sich _nicht_, weshalb A,C sich ihrer Interferenz nicht bewusst sind


## Mobilfunkgenerationen

### GSM (2G)

#### Allgemeine Eigenschaften

- Erste Version: 1991: _volldigitale_ Mobilfunknetze
- _Groupe Spéciale Mobile_
- Unbenannt in: Global System for Mobile communication
- Mit GPRS-Erweiterung (2000 - 2.5G) mobiler Internetzugang
- 3 Netze
	- Primär: P-GSM-900
		- uplink: 890-915 MHz
		- downlink: 935 - 960 MHz
	- GSM-1800 (Digital Cellular System - GSM)
	- GSM-1900 (Personal Communications Service - PCS - USA)


#### Unterschiede zur Festnetz-Telefonie

- Authentifizierung
- Übertragung über die Luft
- Erweiterte Mobilitätsverwaltung mit HLR, VLR, Location Update
- In C Netz Bezirk-Vorwahl nicht mehr notwendig & unterbrechungsfreie Wechsel zwischen Bezirken
- Nutzung von Fremdnetzen (Roaming)
- Im Freien waren bei Sichtkontakt teilweise bis zu 35 km erreichbar.


#### Struktur und Aufbau

- **Public Switched Telephone Network** ISDN/PSTN
	- connection to Internet
	- ... oder Heimtelefon (data network)
- --- _Signaling systems_ ----
- **GMSC**
	- Authentication Center (AUC)
	- Home Location Register (Datenbank mit Benutzer, grundlegende Teilnehmerdaten)
	- Visitor Location Register (Datenbank, Informationen über TN die vorübergehen MSC besuchen)
- **Mobile Switching Center**
	- connects to BSC
- --- _A-Interface_ ----
- **Base Stations Control** (aka Basisstation)
	- Gets decoded data form Base Transceiver Station
	- Handles Handovers
- --- _A bis-Interface_ ----
- **Base Transceiver Station** (multiple - connected to Base Station Control)
	- Handover happens between them
	- Antenne
	- Connect to Mobile Station
- --- _GSM Radio Air Interface_ ----
- **Mobile Station** (mobile phone)

![[GSM_architecture.png]]

#### General Packet Radio Service 

- Erfinder: Bernhard Walke
- Erweiterung von GSM
- Erstes mal TCP/IP
- Gemeinsame Nutzung eines Kanals durch mehrere Nutzer
- Höhere Datenraten durch parallele Nutzung mehrerer Kanäle
- GSM mit zusätzlichen Funktionen


### UMTS

Universal mobile technologies standard.

Für Referenz zur GSM Architektur siehe [[MOBINFSEC#Struktur und Aufbau|Struktur und Aufbau]]

![[UMTS_architecture.png]]


#### SNR

- Signal-to-noise ratio
- Je höher SNR desto kleiner die Fehlerrate.
- Bei gleichbleibenden Bedingungen haben schnellere Übertragunsverfahren eine höhere Fehlerrate

#### Adaptive Modulation

- Modulationsverfahren anpassen anhand von Bedingung
- Handy wählt immer Modulationsverfahren, welches die höchstmögliche Übertragungsrate ermöglicht

![[VL5 - 1 - Einfache Aspekte der Funkuebertragung und die UMTS Mobilfunkarchitektur - Grundbegriffe und nkonzepte - Einheit 3 - Adaptive Modulation, Mehrfachzugriff und Power Control.pdf#page=8]]




#### Zugriffsverfahren

- Regelt Zugriff auf ein Medium für verschiedene Teilnehmer
- Wichtig, wenn auf einem Medium mehrere Teilnehmer gleichzeitig aktiv sind
- Jede Mobilfunkgeneration definiert eigenes Zugriffsverfahren

**Verfahren:**
- Raummultiplexverfahren (gerichtete Antennen)
- Frequenzmultiplexverfahren
- Zeitmultiplexverfahren
- Codemultiplexverfahren

#### Frequency Division Multiple Access (FDMA)

- getrennte, nicht überlappende Frequenzbänder
- Exklusive Zuteilung einer Frequenz für die gesamte Dauer einer Verbindung
- Frequenzbereich kann auch noch zeitlich unterteilt werden


#### FDMA + Time Division Multiple Access (TDMA)

- In Zeitschlitze unterteiltes Frequenzband
- Kombination aus Zeit- und Frequenzmultiplex
- Wird in #GSM -Systemen verwendet - kombiniert mit Frequenzsprungverfahren


**Frequency hopping:**
- Wechsel des Frequenzbandes in jedem Zeitrahmen gemäss Sprungsequenz
- Bessere Robustheit gegen¨über frequenzselektivem Fading und Interferenzen
- Technisch aufwändig


#### Space Division Multiple Access (SDMA)

- Trennendes Medium ist der Raum, wird durch gerichtete Antennen realisiert
- Steuerung intelligenter Antennen erfolgt elektronisch, nicht mechanisch
Kombination mit FDMA/TDMA oder CDMA



#### Code Division Multiple Access (CDMA)

- Verwendet in #UMTS
- Unterscheidet Signale anhand von Codes
- Verwendet orthogonale Codes
- Empfänger filter entsprechendes Signal im Coderaum
- Signale von anderen Stationen werden als Rauschen wahrgenommen



#### Power Control

- Minimierung von Interferenzen durch power control
- alle Signale gleich stark empfangen->
	- Handys weit weg senden stark
	- Handys ganz nah senden schwach
- Power-Down Signal ist dominanter als Power-Up



#### Cell Breathing 

- UMTS, 3. Generation
- Grösse der Zellen variiert je nach Verkehrslast
- Mehr Datenverkehr bedeutet mehr Störungen
- In Verbindung mit off loading: _active cell breathing_
	- Zell-Grösse wird künstlich reduziert. Stark belastete Zellen werden kleiner, während benachbarte Zellen ihren Versorgungsbereich vergrössern, um dies auszugleichen
	- überlastete Zellen lagern Teilnehmerverkehr auf benachbarte Zellen aus.



### Evolution der Mobilfunkarchitektur

Unterschied zu Heimnetze:

![[VL6 - Evolution der Mobilfunkarchitektur.pdf#page=5]]


