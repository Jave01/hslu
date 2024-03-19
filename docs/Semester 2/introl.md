# INTROL

## Administration

Theoriefragen zu den Laborübungen vorgängig lesen und auf Ilias beantworten. Ist kein Test aber man muss 90% erfüllen um den Test zu bestehen.  
Abgabe der Laborberichte anschliessend als PDF im Abgabeordner auf Ilias innerhalb einer Woche.

### Testatbedingungen

-   6 Laborberichte
-   Testatabgabe "Open your Mind" (Präsentationen der Ergebnisse in der letzten Semesterwoche)

### MEP

-   Elektronische Schlussprüfung

### Labor

-   Einer- oder Zweiergruppen
-   6 Laborberichte während Laborzeit ausfüllen und danach hochladen.

### Literatur

-   [Computernetzwerke](https://www.orellfuessli.ch/suche?sq=978-3-86894-137-1)
-   [Wireshark](https://www.orellfuessli.ch/suche?sq=978-3-+95845-683-9)

## Inhalt

### Kreatives Denken

-   [Manager Magazin](http://www.manager-magazin.de/lifestyle/artikel/sieben-methoden-zum-innovativen-denken-a-1072513.html)
-   [Adversial Thinking](https://www.smokescreen.io/adversarial-thinking-improving-cybersecurity-with-ants-and-barcodes/)
-   [Quergedacht](https://www.orellfuessli.ch/suche?sq=978-3-8423-5956-7)

#### Modes of Thinking

Convergent thinking:

-   Schuldenken
-   Using logic
-   also called:

    -   Linear thinking
    -   Analytical thinking
    -   Vertical thinking
    -   Critical thinking

Divergent thinking

-   Using imagination
-   Be creative
-   also called:

    -   Creative thinking
    -   Horizontal thinking

Lateral thinking

-   Both convergent and divergent
-   Think "outside the box"

[Ted Talk](https://www.youtube.com/watch?v=gyM6rx69iqg)

#### Kopfstandtechnik

Das Problem auf den Kopf zu stellen um den worst-case zu beschreiben.  
z.B.: Was müssen wir tun um das Projekt zu scheitern.

#### Crazy8

Jedes Teammitglied in 8 Minuten, auf 8 Feldern, 8 Lösungsansätze aufzuschreiben.

#### Zufalltechnik

Aus einem Lexikon oder ein Warenhaus Katalog ein zufälliges Wort oder Bild nehmen und anhand von dieser Sache lösen.

#### Provokationstechnik

Eine der wichtigsten Methoden. Die Lösung provokativ anders anzuschauen.

1. Verfälschung

    1. Das Basketballfeld ist schief

2. Umkehrung

    1. Der Hund geht mit dem Mensch gassi.
    1. Der Schüler lehrt den Professor

3. Idealfall

    1. Es gibt keinen Fehler
    1. Die Batterie ist nie lehr

4. Übertreibung

    1. Das Auto ist 20 Meter lang.
    1. Der Tag hat 50 Stunden.

5. Aufhebung von Annahmen

    1. Das Handy braucht keinen Strom
    1. Die Schüler brauchen keinen Lehrer.

### Netzwerke

Commands:

    nslookup 8.8.8.8 // lookup for public google DNS server

#### Angriffsflächen des Internets

-   **Sniffing** - abhören von Informationen
-   **Manipulation** von Informationen
-   **Maskerading** - Verstecken hinter falscher Identität
-   **Replay** - wiedereinspielung von Informationen
-   **Umleiten** - um Daten besser abzuhören
-   **DoS** - Kommunikationssystem stören
-   **Hijacking** - Kommunikationsverbindung übernehmen

#### Analysatoren

-> Just use Wireshark

## Wireshark

### TCP

Seq number basiert auf irgendeiner Startnummer, welche als 0 (initiator) behandelt wird. Die nächste seq Nummer ist jeweils die jetzige Nummer + die Länge des Packets.

### Features

#### Statistics

-   Flow Graph -> Visualisiert die Kommunikation zwischen 2 Endpunkten (z.B. TCP)
-   I/O Graph -> wann wie viele Packete kommuniziert wurden
-   Verbindungen -> paralelle Verbindungen visualisiert

### IPv6

IP: Ermöglicht die Kommunikation zwischen verschiedenen Netzwerken / LAN's  
Vorteile von IPv6:

1. Effizienteres Routing ohne Fragmentierung von Paketen
2. Eingebaute QoS
3. Eliminierung von NAT zur Erweiterung des Adressraums von 32 auf 128 Bit
4. Eingebaute Sicherheit auf Netzwerkschicht (IPsec)
5. Zustandslose Adressen-Autokonfiguration für einfachere Netzwerkverwaltung
6. Verbesserte Header-Struktur mit weniger Verarbeitungsaufwand

Erfinder von IP: Vinton G. Cerf zusammen mit Robert Kahn (Anfang 1974 spezifizierung, Ende 1974 RFC), ARPANET (erste Umsetzung 1983)

| IPv | One to many | Sammlung von Computern mit demselben Prefix (nähster Server wird gewählt) | Header  |
| --- | ----------- | ------------------------------------------------------------------------- | ------- |
| 6   | Multycast   | Anycast                                                                   | 128 Bit |
| 4   | Broadcast   | Auf IP Layer nicht möglich -> DNS                                         | 32      |

Subnet and interface are always 64 bits long.  
Für Subnet: 46 bit global routing prefix & 16 bit subnet.  
Global routing prefix: 0-12bit RIR (registrare), 0-32 bit (ISP), 0-48 bit (site).

Subnet(64bit) --> RPR
Subnet(64bit) --> ISP
Subnet(64bit) --> personal<span> <\span>subnet

-   Registrare: ke ahnig
-   ISP: Swisscom
-   Site: site/allgemeines Subnet von Swisscom
-   Subnet: dein eigenes Subnet

| Name                 | IPv6          | IPv4                |
| -------------------- | ------------- | ------------------- |
| Loopback             | ::1           | Localhost 127.0.0.1 |
| **RIPE**             | 2001:0600:... |                     |
| **ARIN**             | 2001:04xx:... |                     |
| **APNIC**            | 2001:02xx:... |                     |
| Link-Local           | fe80::...     |                     |
| Site-Local           | FEc0::...     |                     |
| Multicast            | FF::...       | Broadcast           |
| all-router Multicast | FF02::...     |                     |

Link-Local funktioniert nur im lokalen LAN (Alternative zu DHCP). For generating Link-Local addresses, SLAAC can be used (like DHCP in IPv4). ARP not required, MAC address is part of SLAAC protocol.

#### Dynamische Addresszuweisung

kann auf 3 Arten erreicht werden

1. SLAAC
2. stateless DHCPv6
3. statefull DHCPv6

Wenn SLAAC zur Zuweisung
der IPv6-Hostadressen und DHCPv6 zur Zuweisung anderer Netzwerkparameter verwendet wird,
lautet die Bezeichnung Stateless DHCPv6. Bei Stateful DHCPv6 weist der DHCP-Server alle Infor-
mationen zu, einschliesslich der Host-IPv6-Adresse. Die Regelung, wie Hosts ihre dynamischen
IPv6-Adressierungsinformationen erhalten, ist abhängig von den Flag-Einstellungen, die in den
Router-Advertising (RA)-Meldungen enthalten sind.

##### Packets

Router Advertisement:

-   Managed: get IP from DHCP
-   Other: get add. information from DHCP
-   A-flag: use SLAAC

If Managed is 1 all others are obsolete.

##### SLAAC

Kein DHCP Server erforderlich

1. Generate address
2. Is address used? yes -> go to 1, no -> use address

Hat Sicherheitslücken -> ev. DHCPv6 brauchen.

#### IPv6 over IPv4

Kommt zum Einsatz falls ein Netzwerk nur IPv4 unterstüzt. Dabei wirde eine IPv6 Kommunikation über IPv4 geführt (tunneling, IPv6 inside IPv4).

### Strategie

Strategien oder Vorgehensweisen können auf Mitre Att&ck gefunden werden.

#### Begriffsdefinition

-   Ursprünglich: Kunst der Heeresführung
-   Eine Vorteilsposition gegenüber Gegnern oder Konkurrenten
-   Langfrisitge Ausrichtung auf ein Ziel

Strategie vs Taktik:

-   Strategie: Was man langfristig erreichen will
-   Taktik: Wie man ans Ziel kommt

### Taktiken

In der Wirtschaft:

-   Initiative, first move
-   Kooperation
-   Reservenbildung
-   Beweglichkeit, anpassungsfähig
-   Informationsüberlegenheit
-   Überraschung/Täuschung

### Man in the middle

-   **Physical Layer:** Angreifer emuliert Access Point in Netzwerk
-   **Network Layer:** ARP Poisoning
-   **Anwendungs Layer:** DNS spoofing

#### IP forwarding

enable ip forwarding: sysctl net.ipv4.ip_forward=1

### Firewall

-   Limit traffic
-   Block traffic
-   Allow traffic

Ist auf Layer 3 und höher.

#### Packetfilter

-   Wedren oft mit Access Lists umgesetzt. Diese werden auf den Routern konfiguriert.
-   Wenn nicht direkt im Router, dann do nah am Internet wie möglich, technisch gesehen meist vor dem Router.
-   Schaut Packete auf Netzwerkebene an.

Beispieldefinition einer ACL

-   Ziele
    -   Zigriff vom Internet ins interne Netz verboten
    -   Zugriff vom internen Netz ins Internet erlaubt

Darf nicht direkt so umgesetzt werden, da sonst kein HTML zurückkommt. Zum Beispiel mit TCP-Flags realisieren (ACK-Flagged Nachrichten erlauben).

1. Schwierig zu konfigurieren
2. Probleme mit gewissen Protokollen (z.B. FTP)
3. Jedes Paket wird einzeln kontrolliert (keine Zusammenhänge zu verhergehenden Paketen)
4. Probleme mit Protokollen mit dynamischen Port-Nummern
5. Probleme mit bestimmten Angriffsarten
    - Pakete mit Source Port 20 (TCP) oder 53 (TCP/UDP)
      ...

#### Statefull

Auf Layer 3

-   Verbindungsorientiert
    Schaut sich den gesamten Verbindungsaufbau an und entscheidet dann ob erlaubt oder nicht (wenn der Verbindungsaufbau von innen kommt, lässt sie auch den inkommenden traffic rein).

-   Packetfilter mit "Intelligenz"
-   Zusammenhänge zwischen Paketen werden berücksichtigt ("Stateful" im Vergleich zu "Stateless", wie bei Paketfilter)

#### Application Layer Gateways

Geht bis auf Application Layer hoch nicht nur Layer 3. Werden auch Proxy genannt.  
Proxy $\equiv$ Stellvertreter.

-   Einfacher zu konfigurieren
-   Keine direkte Netzverbindung vom Internet ins interne Netz
-   Relativ sicher im Vergleich zu Packet Filter.  
    Aber: Proxy muss sicher sein, sonst ist alles unsicher.
-   Brauch viel Rechenleistung, ansonsten sehr langsam.

#### Schutz des Perimeters

Perimeter = Übergang zwischen Internet und Intranet. Rein funktional wird dafür nur ein Router benötigt. Meistens ist dies aber nicht ausreichend und es wird zusätzlich eine Firewall verwendet.

#### Regelaufbau

-   Zuunterst: Default Regel (alles blockieren)
-   Alle Regeln darüber: Erlauben was benötigt wird

#### Web Application Firewalls ("WAF")

-   Schaltest du direkt vor einen Webserver. Ergo schützt deinen Webserver. Kann den Verkehr analysieren, filtern und verändern.
-   Schutz gegen SQL-Injection, XSS etc.
-   Wird zusätzlich zum "normalen" Firewalling verwendet.
-   Je nach Produkt
    -   Kann als Bridge, Router, Reverse Proxy oder auf dem Server selber installiert werden
    -   Kann als Blacklist oder Whitelist konfiguriert werden
    -   Validierung von Cookies, Session State, User, usw.
-   Pro-aktiver Schutz gegen neue (ev. noch nicht entdeckte) Angriffe möglich.

## Vorbereitung für die Prüfung

-   Alle Hüte anschauen
-   Sun Chi
