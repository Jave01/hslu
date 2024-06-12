# Mobile Information Security

## Terminologie

- **MANET:** mobile ad hoc networks
- **VANET:** vehicular ad hoc networks (sub group of MANET)
- **MIMO:** Multiple Input Multiple Output (Mehrantennentechnik)
- **IMSI:** International Mobile Subscription Identifier -> identify the subscription
- **IMEI:** International Mobile Equipment Identifier -> identify the phone
- **AUC:** Authentication Center


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

- **home network:** permanentes "Zuhause" des Handys (z.B. Swisscom) -> hat Vertrag gespeichert
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

### 2G - GSM

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
	- Home Location Register #HLR (Datenbank mit Benutzer, grundlegende Teilnehmerdaten)
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


### 3G - UMTS

#TODO zu wenig wissen
#TODO Data plane / control plane

Universal mobile technologies standard.

Für Referenz zur GSM Architektur siehe [[MOBINFSEC#Struktur und Aufbau|Struktur und Aufbau]]

![[UMTS_architecture.png]]

#MSC = Mobile Switching Center
#### SNR

- Signal-to-noise ratio
- Je höher SNR desto kleiner die Fehlerrate.
- Bei gleichbleibenden Bedingungen haben schnellere Übertragunsverfahren eine höhere Fehlerrate

#### Adaptive Modulation

- Modulationsverfahren anpassen anhand von Bedingung
- Handy wählt immer Modulationsverfahren, welches die höchstmögliche Übertragungsrate ermöglicht

![[adaptive Modulation.png]]




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


## 4G - Long Term Evolution (LTE)

Why?
- Viel mehr Geräte im Mobilfunknetz
- Industrie sagt 1000-facher Datenverkehr
- Netflix in UHD streamen


How?
- **Massiv mehr Antennen** - kleinere Zellen
- Funkspektrum erhöhen (10 MHz, 20 MHz), vorhandenes Spektrum ( #TODO wie gross?) wurde mit UMTS bereits gut ausgenutzt.
- Störungen und Interferenzen
- Genauer:
	- Neues Frequenzzuteilungsschema
	- Neue multiplexing-Technik: OFDMA
	- MIMO-Technologie, Mehrpunkt-Übertragung und -Empfang
	- Relaying
	- **Small Cells, Home Base Stations**
	- WLAN integrieren, ...



### Der Standard

- Highspeed für Mobiltelefone
- Basierend auf GSM und UMTS
- **LTE** = Long Term Evolution
- **MIMO** = Mehrpunkt-Funk
- Ausbaustufen
	- LTE: 3GPP Release 8
	- LTE Advanced: 3GPP Release > 10
- Erstes LTE Netz: NA 2010


### Geschichte

![[5) - 4G Long-Term Evolution (LTE).pdf#page=9]]

![[5) - 4G Long-Term Evolution (LTE).pdf#page=10]]


### Architektur

![[Netzarchitektur_3G-LTE.canvas|Netzarchitektur_3G-LTE]]

- RNC wurde integriert in die Node B (Basisstation)
- MME übernimmt das Accounting


**Luftschnittstelle**
- Mobile Device (UE - User equipment)
- Base station (enhanced Node B)
**ALL-IP Enhanced Packet Core (EPC)**
- Mobility Management Entity (MME) (oft vereint mit HSS)
- Packet gateway
- Serving gateway



>[!note]
>Route:
>Base station -> Serving Gateway (S-GW) -> Packet Gateway (P-GW) -> Internet


**Elemente in der 4G-LTE Architektur**
- Basisstation
	- Empfängt die Funksignale der Smartphones
	- verwaltet drahtlose Funkressourcen (Zelle)
	- koordiniert Geräteauthentifizierung mit anderen Elementen
	- ähnlich wie WiFi AP, aber:
		- aktive Rolle bei der Benutzermobilität
		- Koordinaten mit fast Basisstationen, um die funknutzung zu optimieren
	- LTE-Jargon: eNode-B
- Home Subscriber Service (HSS)
	- Speichert Informationen über Mobilgeräte
	- Home Network
	- Speichert alle vertraglichen Sachen
	- arbeitet mit Geräteauthentifizierung
- Serving Gateway, S-GW
	- Tunnel zu P-GW und Base station
- PDN Gateway, P-GW (Packet Gateway?)
	- Schnittstelle zum Internet
	- Stellt NAT-Dienste bereit
- Mobile Management Entitiy (MME)
	- Handover zwischen Zellen
	- Authentifizierung
	- Pfadaufbau (Tunneling) von UE zu P-GW


### Advanced Features

- Mehrantennentechnik
	- Bessere Ausnutzung der Funkressourcen
	- Hohe Datenraten
	- Störungsanfälligkeit



## Mobility

>[!note] Vergleich
>- **Keine Mobilität:** Nutzer ist stationär und nutzt immer gleichen Access Point
>
>- **Mitte:** Beim Bewegen wird die Verbindung getrennt und neu aufgebaut
>	- IP Adresse wechselt
>	- Verbindung wird unterbrochen
>
>- **Hohe Mobilität:** (ab GSM) Mobiler Benutzer, der mehrere Zugriffspunkte/Netzwerke durchläuft und gleichzeitig Verbindungen aufrechterhält 

Von oben nach unten:
- Steigender Aufwand -> benötigt Funktionalität
- Steigender Komfort für den Nutzer (kein Verbindungsunterbruch)

Repetition:
- Home network -> Vertrag (z.B. Swisscom)
- Visited network -> jedes andere
- SIM-Karte: globale Identifikationsmerkmale
- Swisscom und Sunrise brauchen Vertrag für Roaming


### Mobility in GSM (2G)

indirect routing to mobile:

![[indirect_routing_to_mobile.png]]

1. Correspondent calls a number -> number identifies user not location
2. Call goes to public switched telephone network
3. Call routed to home network (with MSC)
4. MSC consults #HLR -> gets roaming number of mobile in visited network
5. Home MSC sets up 2nd leg of call to MSC in visited network
6. Mobile Switching Center determines base station
7. #MSC in visited network completes call through base station to mobile


#### Handoff: switch between base stations #BSS

>[!note]
>Handoff goal:
>route call via new base station (without interruption)
>reasons:
>- stronger signal to/from new BSS
>- load balance
>- GSM doesn't mandate why to perform handoff
>
>Handoff is initiated by old #BSS
>
>Handoff between cells is controlled by #MSC 

#TODO Maybe study detailed hand-off between BSS [[Mobility_in_GSM_LTE_UMTS.pdf#page=11|Slide p.11]]




#### Handoff between MSC's

#MSC Handoff
- _anchor MSC:_ first MSC visited during call
	- call remains routed through anchor MSC
- new MSC's add on to end of MSC chain as mobile moves to new MSC
- optional path minimization step to shorten multi-MSC chain



### Mobility in UMTS (3G)
#UMTS 

Handovers:
- Hard: Break Before Make, link between NodeB & UE
- Soft: mobile station is connected to several NodeB's
- Softer: mobile station is connected to several sectors of one NodeB


#### Soft Handover

![[soft_handover.png]]


- Phone aims for hysteresis
- _PilotE_ - "Pilotsignal" -> momentan stärkstes Signal
- Y-Achse: Verhältnis momentan stärkstes Signal zu den anderen Antennen 
- Wenn Verhältnis unter die untere Hysterese-Achse fällt, wird die Antenne in das AS (Active Set) aufgenommen -> kann sich bei Gelegenheit damit verbinden.



### Mobility in LTE (4G)

**5 Mobility Tasks:**
- identifying itself, home network
- control-plane configuration -> MME, home HSS establish control-plane state
- data-plane configuration
- mobile handover


![[mobility_tasks.png]]


>[!tip]
>Tunnel bedeutet etwas über etwas anderes überleiten (overlay)
>**Vorteile:**
>- Daten abgetrennt vom restlichen Netz
>- Einfach umleitbar
>
>Vergleich mit VPN-Tunnel

S-GW to BS tunnel: when mobile changes base stations, simply change endpoint IP address
S-GW to home P-GW tunnel


**Arten von Handovers:**
- Intra-Cell - andere Frequenz oder Zeitschlitz in derselben Zelle
- Inter-Cell - Zu Nachbarzelle
- Inter-BSC - Zu Nachbarszelle, welche anderen BSC aber gleicher MSC hat
- Inter-MSC - Zu Nachbarzelle, welcher anderer BSC und MSC hat
- Inter-PLMN - In zelle aus anderem Mobilfunk
- Inter-System - In Zelle mit anderer Mobilfunktechnik (e.g. GSM -> UMTS)



## What happens when I turn on my phone?

#MS = Mobile station (Phone)
#BTS = Base transceiver station (Zelle / Antenne)
#BSC = Base station controller

1. processors boot up
2. Hardware gets initialized
3. Network selection (ARFCN = Absolute Radio Frequency Channel Number)
4. Phone quick scans all ARFCN, writes down received energy
	1. Create prioritized list
6. Align with carrier frequency -> adjust quartz
7. Get to know the time slots / decode the time-slots
8. Get to know the Distributor
	1. Decode BCCH (Broadcast common control channel)
	2. These information only get sent in a fairly big interval
	3. Decode IMSI-Network (first 5 numbers)
9. Do it for all slots (967) - mostly only the best 10 until valid found




### GSM

Communicates with time-slots

When boots up it tries to communicate with FDD (Frequency Division something)

Prüft 967 Kanäle auf Energie

**Automatische Netzauswahl:**
- Anhand von IMSI (erste 5 Stellen der IMSI) im Heimnetz
	- Für Schweiz-Salt: 22803
- Anschliessend Registrierung beim ersten PLMN


**Location update:**
- Sub-layer in Layer 3
- If location of MS changes
- used to update location/presence information of the network

### UMTS

Communicates with TDMA (Time Division Multiple Access)


## Sicherheitsarchitekturen

- Message Authentication Codes - #MAC

**Besonderheiten:**
- Übertragung über öffentliches Medium (Luft)
- Teilnehmer sind mobil und Verbindungen sind nicht festgelegt
- Mobiler Endnutzer verwendet unterschiedliche Basisstationen und Übertragunsressourcen

**Lösungen:**
- Eindeutige Identifikation eines Teilnehmers
- Eindeutige Identifikation eines Geräts
- Tracking des Nutzers über Location Identifier über Basisstation hinweg
- Mobilfunk-Anker: Heimnetzwerk-Provider


### Angriffsvektoren

- **Strecke zwischen MS und Einspeisung in das Festnetz**
	- Risiken
		- Abhören
		- Unautorisierte Übertragung
	- Sicherheitstechniken
		- SIM-Karte mit Identifikationsnummer und -schlüssel
		- Verschlüsselung der Kommunikation
		- Positionsanonymisierung


### MAC's

#MAC Definition

- Message Authentication Codes
- Werden genutzt für Authentifizierung
- Integrität sicherstellen
- Ähnlich zu Signaturen
- Kryptografische Prüfsumme

Ziel:
- Prüfsumme errechnen mit symmetrischen Schlüssel $m = MAC_k(x)$


### Challenge-Response Verfahren

- Authentifizierungsverfahren

Schritte:
1. Identifikation von Alice
2. Bob erzeugt Zufallszahl und schickt Alice
3. Alice verschlüsselt mit festgelegtem Verschlüsselungsverfahren
4. Alice schickt verschlüsselte Zufallszahl an Bob
5. Bob entschlüsselt und prüft Passwort


**Angriffsvektoren:**
- Zufallszahl über unsicheren Kanal
- Response über unsichere Kanal
- Angreifer kann aus RAND und C den Schlüssel ableiten
- Bob muss Schlüssel von Alice sicher und lange aufbewahren


>[!tip]
>Just use asymetric encryption



### GSM-Sicherheitsmerkmale

TMSI: Temporary IMSI, für die Verschleierung des Nutzers, wird von VLR vergeben.

Authentifizierung und Integritätsalgorithmen: COMP-128
- Authentifikation: A3 -> #AUC
- Verschlüsselung: A5, Satz von symmetrischen Verschlüsselungsverfahren-> #MSC
- Schlüsselgenerierung: A8 -> #AUC 
- Individual Subscriber Authentification Key - $k_i$ - 128 bit, private key -> #AUC 
- Source-Code wurd 1998 reverse-engineert
- Seit 2006 können alle Versionen in Echtzeit gebrochen werden.
- $k_i$ ist auch im Authentication Center gespeichert


Detaillierte Schritte:

0. Pin wird vom Nutzer eingegeben
1. A3 - Authentifizieren / Einbuchen
2. AC erstellt Zufallszahl (Challenge)
3. A3 erstellt mit privatem Schlüssel und Zufallszahl die Response für Authentifikation
4. Sendet Response -> Authentifikation abgeschlossen
5. A8 erstellt mit der Zufallszahl und privatem Schlüssel einen session key
6. A5 verschlüsselt payload $m$ mit dem Session Key


### Sicherheitsmerkmale bei LTE

- Auf SIM:
	- IMSI
	- Pre-Shared Key $k$
- Eingeschränkter Zugriff auf die UICC über API
- Führt kryptographische Vorgänge zur Authentifizierung durch

#EAP - Extensible Authentication Protocol


3 Algorithmen:
- SNOW 3G
- AES
- ZUC

Laut 3GPP ist Verschlüsseln optional


## Überwachung von Telekommunikationsanlagen

Was darf, kann die Polizei

### Rechtliche Grundlagen

**BÜPF** - Bundesgesetz betreffend die Überwachung des Post- und Fernmeldeverkehrs

Wann darf gebraucht werden:
- Im Rahmen eines Strfaverfahrens
- zum Vollzug eines Rechtshilfeersuchens
- im Rahmen der Suche nach vermissten Personen
- Im Rahmen der Fahndung nach Personen, die zu einer Freiheitsstrafe verurteilt wurden oder gegen die eine freiheitsentziehende Massnahme angeordnet wurde


**Pflichten der Anbieter von Fernmeldediensten:**
- Fernmeldediensten (Art. 26)
    - Swisscom
    - Sunrise
    - Salt
- Anbieter abgeleiteter Kommunikationsdienste (Art. 27)
    - Threema
    - Protonmail
- Betreiber von internet Fernmeldenetzen (Art. 28)
    - HSLU (public WLAN)
    - Uni Bern
- Personen, die Zugang zu einem öff. Fernmeldenetz zur Verfügung stellen (Art. 29(
    - McDonalds
- Professionelle Wiederverkäufer von Zugangskarten (Art. 30)
    - Bahnhofskiosk


#### Artikel 26

Pflichten:
- Daten liefern
- Daten 6 Monate aufbewahren
- Notwendige technische Daten liefern
- Überwachung dulden
- Verschlüsselung entfernen
- Bundesrat kann Dienste von geringer Wichtigkeit von gewissen Pflichten entbinden

#### Artikel 27

Bundesrat kann wichtige ienste gewissen Pflichten gem. Art. 26 unterstellen

#### Artikel 27-29

Pflichten:
- KEINE Aufbewahrungspflicht
- Zugang zu Anlagen gewähren
- Notwendige technische Daten liefern
- Verfügbare Randdaten liefern

#### Artikel 30

Benutzeridentifikation des Käufers muss sichergestellt werden.


### Auskünfte

- Einfache Auskunft:
    - Warum einfach? simple Datenbank Einträge, verfügbar innerhalb von Minuten
    - Telefonbuch- & IP-Anfragen
    - Auskünfte über IMEI, Nutzerinformationen, IP-Anschlüsse, Telefonnummern etc.
- Komplexe Auskunft:
    - Ausweiskopien
    - Vertragsdaten


Verwendung:
 - Wem gehört ein gefundenes Gerät / SIM?
 - Welche Geräte hat eine bestimmte Person?
 - Wer ist die Person, die eine bestimmte SIM erworben hat?
 - In welchen Geräten (IMSI) befand sich eine bestimmte SIM (IMEI)


### Rückwirkende Überwachung

Target: Ein Teilnehmer (= SIM)

- Nur Metadaten zu Ein- und Ausgehende Anrufe und SMS
    - Partner
    - Zeitpunkt
    - Dauer
- Data-Verbindungen
- ID, Standort der Antenne und Azimut der Antenne


### Echtzeitüberwachung

- Die Daten aller ein- und ausgehenden Kommunikationen (Anruf, SMS & Data) werden abgefangen.
    - Metadaten
    - Inhalt der Kommunikation
- ID, Standort und Azimut der Antenne
- Muss durch Zwangsmassnahmengericht angeordnet werden!


Verwendung:
- Wird verwendet für Dinge die in Zukunft passieren werden
- Für organisierte Kriminalität
    - Menschenhandel 
    - Terrorismus
    - Schmuggler
- Identifikation des Netzwerkes
- Finden von Drop-Off-Punkten, Lager, Verstecke
- Wird nur gemacht, wenn sehr guter Grund vorhanden ist
    - Hohe rechtliche Hürde
    - Sehr hohe Kosten
    - Auswertung sehr aufwendig
    

### Personensuche

Was ist eine Notsuche?
-> Persone finden, die verschollen ist
-> verschollen: Aufenthalt unbekannt/schwVertragsdatener zu ermitteln oder Annahme für Gefährdung der Gesundheit oder Leben.

Was ist Fahndung?
-> Suche nach verurteilten Personen


Vorgehen:
1. Ist ein bestimmtes Gerät aktiv? (Versand einer leeren SMS)
    1. Ja -> Auf welcher Antenne befindet sich das Gerät zurzeit?
    2. Nein -> Welches war die letzte Antenne, mit der das Gerät verbunden war.
2. Wo befindet sich das Gerät?
    1. Resultat: Antennen-ID -> Gerät befindet sich in Bereich, welcher von Antenne abgedeckt
    2. Abdeckung der Antenne ist ziemlich random.


### Antennensuchlauf

Es wird kein Endgerät überwacht sondern eine Antenne für einen Zeitraum von 2h

**Antennenauswahl:**
- Mit welchen Antennen hat sich Gerät verbunden
- Karte mit Antennen


**Mit welcher Antenne verbindet sich Endgerät:**
- Theorie
    - Variiert je nach Technologie
    - Dominierender Faktor: Signalstärke
    - Hypothese: Gerät verbindet sich mit nächster Antenne
- Praxis
    - Nicht alle Antennen senden gleich stark
    - Umfeld kann Signal schwächen
    - Welche Antennen sind verfügbar?
    - Welche Antenne ist an einem Ort effektiv am stärksten
    - Hypothese: Gerät verbindet sich mit dem stärksten Signal

Bester Ansatz: Messen


## 5G Mobile Networks

- 2018 erster release des Standards
- 2020 erste kommerzielle erhältlichkeit
- 2022 mehr als 2 Samsung Phones mit 5G


Ziel
- 10-fache Erhöhung Spitzenrate
- 10-fache Verringerung der Latenz
- 100-fache Erhöhung der Verkehrskapazität gegenüber 4G
- Zelldurchmesser: 10-100m
- 5G-NR (New Radio): mmWaves, Verbesserungen zur aktuellen Schnittstelle


### Vollständiges 5G netzwerk

Umfasst:
- eMBB (erweiterter mobile Breitbandzugang): Internet für alle über 5G wie Glasfaser
- URLLC (Ultra Reliable Low Latency Communications): Release 16, Verbesserungen vor allem im Kernnetz: Latenz und Verlässlichkeit
- mMTC (Massive Machine Type Communications): Release 16, Ommunikation von Gleingeräten untereinander (IoT)


### Funkübertragung und Funkzugansnetz

5G-NR -> eine Reihe von Standards, erweiterung von LTE
Wurde entwickelt um Verbindung zu schaffen die Glasfaser entspricht.

Anforderungen:
- Mobilität 
- IoT - Muss IoT unterstützen
- Schlankes Design - Signale nur bei Bedarf
- Adaptivität - auf geringere Bandbreite mit kleinerem Stromverbrauch verwenden
- Strikte Vorgaben

>[!todo]
>- [ ] Unterschiede allgemein GSM, UMTS, LTE (Advanced), 5G
>- [ ] Warum ist LTE 3.9
>- [ ] Bilder
>- [ ] Kryptographischen Ablauf
>- [ ] Immer Bilder zeichnen können


>[!notes] Notes für MEP
>Wir bekommen IPad, würfeln um Startthema
>Grundsätzlich nur 1 max 2 Thema
