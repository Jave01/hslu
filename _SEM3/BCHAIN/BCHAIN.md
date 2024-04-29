# Abgabe
- Businessplan max. 2 Seiten
- Prototyp
- Projektdokumentation (10 Seiten Implementation + 2 Seiten Businessplan)

Noten:
- 30% Präsentation 
- 70% Projektdokumentation

## Businessplan
- Name der Firma, Gründungsmitglieder, Rollenzuweisung
- Beschreibung der Idee (Was ist innovativ an eurer Lösung und welches Problem/Bedürfnis angehen)
- Begründung Einsatz einer dezentralen Lösung (Do you need a Blockchain)
- Wie könnt ihr Geld mit eurer Idee erwirtschaften?
- Konkurrenz und Absatzmarktanalyse
- SWOT - Analyse
- Länge: max 2 Seiten
- Abgabe: auf Illias hochladen
- Pitching: 4 min pro Gruppe (Auf alle Punkte kurz eingehen)


## Projektarbeit
- Technische Anforderungen an die Lösung (Kosten Mining, Geschwindigkeit)
- Systemarchitektur inkl. Wahl Blockchain-Protokoll (Grafik + detaillierte Erläuterung)
- Aktivitäts- & Sequenzdiagramm (UML - optional)
- Oracles (Authentizität der Daten? Benutzermanagement?)
- Schreiben der Transaktionen in Blöcke (Welcher Consensus-Algorithmus wird genutzt?)
- Beschreibung des/der Smart Contracts.
- Einsatz des Prototypen mit Beispiel demonstrieren.
- Fazit, Lessons learned, Weiterentwicklungsmöglichkeiten sein.


# Introduction
- Zentrales system: es braucht eine **Trusted Third Party**.
- Dezentrales system: alle sind gleichwertig.

## Bitcoin basics
- Asymmetrische Verschlüsselung, public key ist in Bitcoin Adresse integriert
- Jedes Mitglied im Netzwerk muss eine Transaktion bewilligen.
- "Immutable, Distributed Ledger" ist eine dezentrale Datenbank.


## Verschiedene Arten von Blockchain
### Permissioned private Blockchain
- Teilnahme und Zugangsrechte sind eingeschränkt
- Die Identität aller Teilnehmenden ist bekannt
- 1 "vertrauenswürdige" Partei betreibt alle Nodes und erstell Blöcke
- **Fokus:** Kein freier Zugang. Absolute Kontrolle durch 1 Partei
- **Beispiel:** CBDC (Central Bank Digital Currency)
- **Vorteile:** 
	- Sehr schnell
	- Verwaltung
	- Anonymität - Zugangsberechtigung bedingt Einsatz von Benutzerkonten
- **Nachteile:**
	- Abhängigkeit - Teilnehmenden sind machtlos.
	- Sicherheit - Trusted Third Party
	- Zugang - Zugangsberechtigung kann willkürlich erfolgen

### Permissioned (Consortium) private Blockchain
Nicht nur eine Trusted Party, aber eine kleine, fixe Gruppe von Entitäten.
- Teilnahme und Zugangsrechte sind eingeschränkt
- Die Identität aller Teilnehmenden ist bekannt
- Ein Konsortium betreibt das Netzwerk, betreibt Nodes und erstell Blöcke
- **Fokus:** Kein freier Zugang. Zentralisierung der Macht beim Konsortium.
- **Vorteile:**
	- Mix - Mix zwischen Public & Private bietet zusätzliche Vorteile
	- Anonymität - Datensicherheit wird eher durchgesetzt als bei Privat
	- Skalierbarkeit
- **Nachteile:**
	- Kontrolle - vom Konsortium
	- Datenschutz - Konsortium hat absolute Macht über die Benutzerdaten
	- Zugang - Kann willkürlich erfolgen


### Permissionless Blockchain
Jeder Teilnehmer kann theoretisch einen Block erstellen.
- Jeder kann betreten
- Identitäten sind nicht bekannt
- Alle können eine Full Node betreiben und als Miner agieren.
- **Fokus**: Freie und gleiche Rechte / Zugang für alle Teilnehmenden.
- **Vorteile:**
	- Sicherheit - Nicht single point of failure
	- Zugang - Jeder kann betreten
	- Viele Nodes - Hohe Stabilität
- **Nachteile:**
	- Skalierbarkeit
	- Anonymität - alle Transaktionen sichtbar
	- Verwaltung - Schwer zu kontrollieren
	- Business Model - Neue Business Modell müssen entwickelt werden.


## Tokens & Coins
### Begrifflichkeiten
- **Coins**: Digitale Peer-to-Peer Währung (Cash), basierend entweder auf einem eigenen Blockchain Protokoll (Ethereum = Ether) oder abstammend vom Bitcoin Protokoll
- **Token**: Tokens sind digitale Vermögensgegenstände, welche nicht auf einem eigenen Protokoll basieren (Ethereum = ERC20 Token). Tokens werden anhand der Funktionsweise unterschieden: Utility, Payment, Security. Stablecoins sind Tokens, bis jetzt noch ohne Klassifikation.

### Tokens
- Durch standardisierte Tokens können Unternehmen mit DAPPs eigene Währung implementieren.
- Durch diese Entkopplung kann der Wert des Unternehmens / Währung frei am Markt bestimmt werden.


#### Standards
- Beispiel: ERC20 (Ethereum Request for Comment, Nummer ist Github Issue Nummer)
- Standards legen Funktionalitäten fest
- Base Funktionalitäten:
	- TotalSupply
	- balance(owner)
	- transfer
	- ...

Beispiele für verschiedene Tokens:
- Währung (Tether)
- Recht auf Dienstleistung (z.B. Digital Coupon für Konzert)
- Stimm- und Wahlrecht (eVoting token)
- Digitales Asset / NFTs oder Bindung an physische Güẗer -> 1/100 von Mona Lisa kaufen
- Aktien -> sehr selten weil rechtlich schwierig


#### Payment Tokens
#TODO 
KYC?

#### Utility Token
Jeder will seine Tokens als Utility Tokens markieren

- Provide digitally access to an application or service -> skins
- Werden nicht als Möglichkeit angeschaut um Geld auszutauschen.
- Wenn man Skins kaufen will muss sichergestellt werden, dass Skins nicht mehr in Geld umgewandelt werden können -> sonst Payment Token



#### Security Tokens
They confirm a claim





## P2P