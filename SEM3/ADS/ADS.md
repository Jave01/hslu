# ADS
## Bäume
Sind hirarchische Strukturen.

Terminologie:
- Interner Knoten: Knoten mit min. 1 Kind
- Externer Knoten: Knoten ohne Kinder
- Tiefe eines Knotens: Anzahl Vorgänger
- Höhe eines Knotens: 
	- Externer Knoten: 0
	- Interner Knoten: 1 + maximale Höhe aller Nachfolgerknoten


### Binäre Bäume
- Besitzen höchstens zwei Kinder
- Die Kinder eines Knotens sind ein geordnetes Paar (links, rechts)
- Zusätzliche Methoden:
	- left(p)
	- right(p)
	- sibling(p)


### Pre-Order Traversierung
Es werden zuerst die Parents vor den Kindern besucht.

### Post-Order Traversierung
Es werden zuerst alle Kinder bearbeitet -> Algorithmus geht 


### In-order Traversierung
1. Zuerst zu erstem linkem externen Knoten gehen
2. Linker externer Knoten bearbeiten
3. Parent bearbeiten
4. Rechter Knoten Bearbeiten
5. Repeat
