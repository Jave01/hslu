# KRYPTO

## MEP

- open book
- schriftlich
- 90 Minuten
- ein Rechner erlaubt

>[!tip]
>XOR Tabelle von Woche 1 Seite 21 Ausdrucken



## Terminologie

- **Kryptographie:** entwerfen von Kryptoalgorithmen
- **Kryptoanalyse:** brechen von Kryptoalgorithmen
- **Kryptos:** "verstecken"
- **Graphie:** "schreiben"
- **Perfekte Sicherheit:** unendliche viele Ressourcen sind equivalent zu raten.
- **Unkeyed Kryptographie:** Hashfunktionen
	- Hashfunktionen
	- **Mit** trapdoor: Ist Geheimniss bekannt, kann Umkehrfunktion berechnet werden
		- RSA
	- **Ohne** Trapdoor: Selbst wenn Geheimnis bekannt, kann Umkehrfunktion nicht berechnet werden
		- Diffie-Hellman
- **Symmetrische Kryptographie**
	- Beide Partner besitzen den geheimen Schlüssel
	- $\mathcal{O}(n²)$
- **Asymetrische Kryptographie**
	- $\mathcal{O}(n)$




## Präsenz 1

### Angriffsarten

- Abhören (Confidentiality)
- Verändern (Integrity)
- Erfundene Meldung einspielen (Insertion)
- Meldung abfangen und einspielen (Replay)
- Löschen von Meldungen (Delete)
- Sich für jemanden anders ausgeben (Masquerade)
- Abstreiten die Meldung geschickt zu haben (Non repudiation of origin)
- Abstreiten die Meldung erhalten zu haben (Non repudiation of receipt)



### Angreifer

Outsider werden noch unterschieden zwischen passiv und aktiv

**passiv:**
- abhören
- verfälschen
- greift nicht aktiv ein

**aktiv:**
- Meldungen verfälschen
- Protokolle verfälschen


Insider: selbsterklärend


### Sicherheitsanforderungen

- Vertraulichkeit
	- sym. Lösung: Message Authentication Code MAC
	- asym. Lösung: digitale Signatur
- Geheimhaltung
	- Verschlüsselung


### Authentizierwert anhängen

$K_G$ = Key Generate
$K_V =$ Key Verify 

$K_G = K_v$, dann heisst der Authentizierwert _MAC_
$K_G \neq K_V$ dann is es eine _digitale Signatur_



### Entropie = mittlerer Informationsgehalt

2 Schlüssel mit 28 bit:
$2 * 28$ bit = $56$ bit, resp. $2 * 7 = 14$ hex

- Der Schlüsselraum beträgt theoretisch $2^{28}$ Schlüssel
- Werden die Schlüssel basierend auf dem ersten bit generiert, existieren noch 2 Schlüssel


1) Informationsgehalt eines Zeichens $x_i$ wird definiert mit:

$$H(x_i) = -log_2\left(P(x_i)\right) = log_2\left(\frac{1}{P(x_i)}\right)$$

2) Die Entropie oder mittlere Informationsgehalt der Nachrichtenquelle wird definiert:

$$H(X) = -\sum_{i=1}^{n}P(x_i) \cdot log_2(P(x_i)) = \sum_{i=1}^{n}P(x_i) \cdot H(x_i)$$


Übung:
#TODO 
p. 21

| $x_i$ | A   | B   | C    |
| ----- | --- | --- | --- |
| $P(x_i)$      | 1/2    | 1/4    | 1/4    |
| $H(x_i)$      | 1    | 2    | 2    |

$H(X)$ = 


![[cryptology_classification.png]]

## Symmetrische Kryptographie

AES: Entschlüsselung dauert doppelt so lange wie Verschlüsselung

One-Time-Pad ist stream Chiffre

### ECB vs CBC

Electronic Codebook

Code block chaining: vorheriger Block wird Schlüssel von nächstem Block


### Mix modus

Das Beste aus beiden Welten: IV wird einfach inkrementiert von Block zu Block.


## Einwegfunktionen, Hashes & MACs

Kryptograph. sichere Hashfunktion $\Rightarrow$  Einwegfunktion
Einwegfunktion $\neq$ sichere Hashfunktion

**Definition Einwegfunktion:**
In eine Richtung sehr leicht zu berechnen, in die andere Richtung technisch sehr schwer bis unmöglich zu berechnen

**Definition Hashfunktion:**
Funktion, die Elemente von einer grossen Menge in eine kleine abbildet.


**Hashfunktionen:**
- Typ 1: unsicher ohne Schlüssel (Prüfziffern)
- Typ 2: sicher mit Schlüssel (Message Auth. Code - MAC, should be *integrity* code)
- Typ 3: sicher ohne Schlüssel (Manipulation Detection Code - MDC)


### Sicherheitseigenschaften

1) **Urbildresistenz (oder Einwegeigenschaft):** Für ein gegebenes $y \in \{0, 1\}^n$ ist es praktisch nicht möglich einen Wert $x \in \{0, 1\}$ zu finden mit $h(x) = y$.
2) **Schwache Kollisionsresistenz** (oder zweite Urbildresistenz oder 2nd-Preimage Eigenschaft) Für ein gegebenes $x \in \{0, 1\}^*$ ist es praktisch nicht möglich, einen Wert $x' \in 0, 1\}^*$ und $x′ \neq x$ zu finden mit $h(x) = h(x)′$.
3) **Starke Kollisionsresistenz** (oder Kollisionsresistenz) Es ist praktisch nicht möglich zwei Werte $x, x′ \in \{0, 1\}^*$ und $x′ \neq x$ zu finden, dass $h(x) = ℎ(x')$.



### CBC-MAC oder HMAC

Gewährt:
- Integrität
- Authentizität



![[hmac.png]]


## Public-Key

### RSA: e-te Wurzle mod N berechnen

- Das Potenzieren $y = f(x) \equiv x^{e} \mod N$ ist rechenintensiv aber einfach
- Das Inverse $x \equiv f^{-1}(x) \equiv y^{1/e} \equiv \sqrt[\leftroot{-2}\uproot{2}e]{y} \mod N$ ist extrem schwierig.
- Dabei ist das Berechnen von der e-ten Wurzel mod N nur das Eine und N = p * q das andere Problem. Ist das eine bekannt kann der Algorithmus gebrochen werden.



## Elliptische Kurven

- EC = Elliptic Curve
- ECC = Elliptic Curve Cryptography


- Was: mathematische Objekte, die man als Public-Key Kr. verwenden kann
- Warum: 
	- wenige und schnelle Operationen (statt viele langsame wie RSA)
	- wenig Speicherplatz
	- Es gibt Standards
	- Patent-freie Algorithmen


**Allgemeine Form:**

$$y² = x³ + ax + b$$

>[!important] Wichtig
>NICHT von der form $y = f(x)$

Nichtsingularitätsbedingung:
$$4a³ + 27b² \neq 0$$
Diese muss erfüllt sein, ansonsten gibt es mehrere Wurzeln (Ergebnisse)

### Neutrales und Inverses Element

Inverses Element resp. Spiegelung an der x-Achse:
$$P'(x,-y) = -P(x,y)$$
Punktaddition:
$$P' + P = P + P' = \mathcal{O}$$

Das neutrale Element ist der Punkt im unendlichen, also
$$\mathcal{O}(x,\infty)$$

Dann einfach noch die einzelnen Koordinaten mit $p$ modulo rechnen.

### Allgemeine Form & Zusammenfassung:

- E = Zusammenfassung aller Punkte, welche diese Bedingung erfüllen
- P muss zwischen 256 und 512 Bit sein
- A & B können beliebig sein, müssen aber die beiden Gesetze erfüllen

Bereich der Punkte: 
$$p+1 - 2 \sqrt{p} \leq |E| \leq p + 1 + 2 \sqrt{p}$$


### Formel zur Addition von 2 Punkten

Zuerst Steigung berechnen
$$s \equiv \frac{y_2 - y_1}{x_2 - x_1} \mod p$$
dann:
$x_3 \equiv s² -x_1 -x_2 \mod p$
$y_3 \equiv s(x_1 - x_3) - y_1 \mod p$


Falls man P + P rechnen muss, resp wenn $x_1 = x_2;y_1 = y_2$ dann ändert sich die Steigungsformel zu:
$$s \equiv \frac{3 \cdot x_1² + a}{2 \cdot y_1} \mod p$$
### Double and add Algorithmus

Analog kann zum Square and Multiply Algorithmus effizient gerechnet werden.

Beispiel: 28 * P

- 28 = 16 + 8 + 4 = 2⁴ + 2³ + 2² = 11100
- Mit der ersten 1 macht man nichts
- wegen 1: p -> 2p -> 2p + p = 3p | double and add
- wegen 1: 3p -> 6p -> 6p + p = 7p | double and add
- wegen 0: 7p -> 14p
- wegen 0: 14p -> 28p