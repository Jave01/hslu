# ADS

## Generelle Infos

### Testate

-   SW10 & SW12: Testatabgaben
-   Programme in Python oder Java
-   Ein Testat muss bestanden sein für die Prüfungszulassung

### Prüfungen

-   Open-Book
-   Keine elektronischen Hilfsmittel

## Pseudo-Code

Beispiel:
```
Algorithm arrayMax(A, n)
	currentMax <- A[0]
	for i <- 1 to n-1 do
		if A[i] > currentMax then
			currentMax <- A[i]
		{ increment counter i }
	return currentMax
```

geschweifte Klammern markieren { Kommentare }.

Anzahl Operationen von for-Schleifen ist 1.

## Analyse von Algorithmen

Ein Algorithmus ist eine _step-by-step_ Anweisung zum Lösen eines Problems mit begrenztem Zeitaufwand.

Relevante _primitive_ Operationen für die Analyse:

-   Evaluation eines Ausdrucks
-   Zuweisung eines Wertes an eine Variable
-   Indexieren eines Arrays
-   Aufruf einer Methode
-   Verlassen einer Methode

## Arithmetische Progression

Die Laufzeit von _prefixAveragtes10_ ist:
$$\mathcal{O}(1 + 2 + 3 + \dots + n) \Rightarrow \sum_{i=1}^{n}i \Rightarrow \mathcal{O}(n^2)$$

## Selection-Sort

1. Daten in eine unsortierte Sequenz verschieben (Input Datensequenz leeren)
2. Kleinstes Element aus unsortierter Sequenz aussuchen
3. Element ans Ende der originalen Liste speichern

Laufzeit: $\mathcal{O}(n)$


## Insertion-Sort

1. Erstes Element nehmen
2. In eine zweite Sequenz speichern (sortiert)
3. repeat until empty
4. sortierte Sequenz in Originalliste verschieben

**Beispiel:**
Input: (7,4,8,2,5,3,9)
Phase 1: Insert

| Originalliste | sortierte Sequenz |
| ------------- | ----------------- |
| (4,8,2,5,3,9) | (7)               |
| (8,2,5,3,9)   | (4,7)             |
| (2,5,3,9)     | (4,7,8)           |
| ...           | ...               |
| ()              | (2,3,4,5,7,8,9)                  |

In-Place Insertion-Sort

Funktioniert wie insertion sort, aber ohne zusätzliche Liste.
Linker Teil der Liste ist der sortierte Teil, der rechte ist unsortiert

## Lists

### Array-lists

Arrays, aber es werden Elemente kreiert / gelöscht wenn sie gebraucht / nicht mehr gebraucht werden.
![[Array-list.png]]

### Linked-Lists

Einfach verkettete Liste. Jedes Element besitzt ein Link zum nächsten, das Letze enthält `null`.
![[Linked-lists.png]]

Meistens wird jedoch ein Header und Trailer Element benutzt, welches auch ein normales Element ist, jedoch ohne Inhalt und nur dazu dient um den Anfang und das Ende zu markieren.

### Doubly-Linked-Lists

-   Jeder Knoten speichert die Verbindung zum Vorgänger und Nachgänger
-   Header und Trailer sind oft spezielle Knoten sogenannte _Sentinels_ -> .next() gibt niemals null pointer exception.

## Queue / Deque

Eine Queue ist FIFO und eine Deque (Double-Ended-Queue) ist das gleiche Prinzip aber

## Iterators

Iteratoren können in python mit `iter(iterable)` erstellt werden und Elemente können mit `next(my_iter)` respektive `my_iter.__next__()` abgerufen werden.
Am Ende wird eine `raise StopIteration` Exception geworfen.

## Abstrakte Datentypen (ADTs)

Wie eine Klasse aber abstrahierter
Spezifiziert:

-   Datenfelder / Attribute
-   Operationen / Methoden
-   Ausnahmen und Fehler der Methoden

### Stack

Der Stack ADT speichert beliebige Objekte. Zugriff erfolgt gemässe LIFO.

Hauptoperationen:

-   `push(Object)` - Element hinzufügen
-   `pop()` - Element entfernen und zurückgeben

Weitere Operationen:

-   `top()` - liefert letztes Element ohne zu entfernen
-   `size()` - Anzahl Elemente
-   `isEmpty()` - bool

Anwendungen:

-   Web Browser
-   "Undo"-Operationen im Editor
-   Rekursion

### Queue

Speichert beliebige Objekte. Einfügen und Entfernen ist FIFO.

Hauptoperationen:

-   `enqueue(Object)` - einfügen von Objekt am Ende
-   `dequeue()` - entfernen und zurückgeben des vordersten Elements

Weitere Operationen:

-   `first()` - liefert das erste Element ohne zu entfernen
-   `size()` - Anzahl Elemente
-   `isEmpty()` - bool

Anwendungen:

-   Wartelisten, Schalter, Zoll, Skilift
-   Zugriff auf gemeinsame Ressourcen (Drucker)
-   Multithreading

### Deque

Double-Ended-Queue. Einfügen und Entfernen ist FIFO. Das Einfügen und Entfernen erfolgt aber am Anfang oder Ende der Queue.

Hauptoperationen

-   `addFirst(Element)` - einfügen am Anfang
-   `addLast(Element)` - einfügen am Ende

Weitere Operationen

-   `removeFirst()` - erstes Element entfernen und zurückgeben
-   `removeLast()` - letztes Element entfernen und zurückgeben
-   `first()` - liefert das erste Element ohne entfernen.
-   `last()` - liefert das letzte Element ohne entfernen.

Auslesen aus einer leeren Queue sollte null zurückgeben.

## Bäume

Sind hirarchische Strukturen.

Terminologie:

-   Interner Knoten: Knoten mit min. 1 Kind
-   Externer Knoten: Knoten ohne Kinder
-   Tiefe eines Knotens: Anzahl Vorgänger
-   Höhe eines Knotens:
    -   Externer Knoten: 0
    -   Interner Knoten: 1 + maximale Höhe aller Nachfolgerknoten

### Basic Operationen
- `set_root` - set root at root index (prob 1)
- `Position root` - return root node
- `int num_children` - return number of children
- `Position parent` - return parent node
- `PositionList children` - return all children positions
- `is_internal(p)`
- `is_external(p)`
- `is_root(p)`
- `size()`
- `is_empty()`
- `iterator()`

### Binäre Bäume

-   Besitzen höchstens zwei Kinder
-   Die Kinder eines Knotens sind ein geordnetes Paar (links, rechts)
-   Zusätzliche Methoden:
    -   `left(p)`
    -   `right(p)`
    -   `sibling(p)`
- echte Binärbäume: exakt 2 Kinder
- Array-basierte Speicherverfahren ist: index(child) = 2\*index(parent)


### Pre-Order Traversierung

Es werden zuerst die Parents vor den Kindern besucht.
![[pre-order-traversierung.png]]

Beispiel: Drucken eines strukturierten Dokumentes.

### Post-Order Traversierung

Es werden zuerst alle Kinder bearbeitet
![[post-order-traversierung.png]]

Beispiel: Angabe des benutzten Speichers und seinen Unterverzeichnissen.

### In-order Traversierung

Knoten wird _nach_ seinem linken Subtree und _vor_ seinem rechten Subtree besucht.

Beispiel: Darstellung von binären Bäumen.

## Heap

Ein Heap (Haufen, Halde) ist ein Binärbaum, welcher in seinen Knoten Schlüssel speichert, welche die folgende Eigenschaften haben:

Für jeden Knoten, der nicht root ist gilt:
$$key(v) \geq key(parent(v))$$
**Eigenschaften:**
Sei $h$ die Höhe des Heaps
- Für alle Ebenen von $i=0$ bis $h-1$ sind $2^i$ Knoten auf der Ebene vorhanden (volle Levels)
- Auf der Tiefe $h$ befinden sich die Knoten links (es wird von links aufgefüllt).
- Der letzte Knoten ist der rechteste auf der Tiefe $h$.
- Es gibt maximal 1 Knoten welcher nur ein Kind hat und dieses muss ein linkes sein.
- Baum hat eine Höhe von $\mathcal{O}(\log(n))$

## Priority Queue
Speichert eine Collection von Entries
Entry: Key/Value-Paar. (Key ist meistens eine Nr. -> daher Priority)


Wichtigste Methoden
- `insert(k,v)` - fügt eine neue Entry in die Queue ein
- `removeMin()` - Kernoperation, gibt das kleinste Angebot zurück

Zusätzliche Methoden
- `min()`
- `size()`
- `isEmpty()`

Anwendungen: Auktionen, Börse.

### Sequenz-basierte Priority Queue

Implementierung mit einer unsortierten Liste:
- `insert()` - $\mathcal{O}(1)$ 
	- Wird am Anfang oder Ende eingefügt
- `removeMin()` & `min()` - $\mathcal{O}(n)$
	- ganze Liste muss traversiert werden


### Heap-basierte Priority Queue

**Eigenschaften:**
- Baumstruktur, kleinstes Element ist root
- `key(v) >= key(parent(v))`
- Laufzeit (Heap-Sort): $\mathcal{O}(n\space log \space n)$
- `insert()` - $\mathcal{O}(n)$
- `min()` - $\mathcal{O}(1)$


**Insert:**
1. `insert()` - fügt das Element am Ende des Baumes ein
2.  `upheap()` - schiebt das eingefügt Element so lange nach oben bis Ordnung wiederhergestellt ist.

#### Upheap

Stellt Ordnung nach Einfügen eines neuen Schlüssels wieder her.
Neues Element wird so lange nach oben "verschoben" bis das Parent Element kleiner ist, oder root erreicht wurde.


#### Downheap

Nach `removeMin()` wird das letzte Element nach root geschoben und dieses dann immer mit dem jeweiligen kleineren Kind vertauscht wird bis alle Kind-Schlüssel grösser sind.

### Heap-Sort

1. alle Elemente mit `insert()` einfügen
2. alle Elemente mit `removeMin()` auslesen
3. ausgelesene Sequenz ist sortiert.


**Remove:**
1. `removeMin()` - entfernt kleinstes (root) Element
2. Das letzte Element (eines der grössten) wird in root eingesetzt
3. `downheap()` - schiebt das jetzige root Elemente so lange nach unten bis Ordnung wiederhergestellt ist.

### Adaptable Priority Queue
Keys und Values können nachträglich angepasst werden (und Baum wird neu sortiert).

Methoden:
- `insert(k, v)` - gibt neu die Entry zurück
- `remove(e)` - entfernt eine Entry und gibt diese zurück
- `replaceKey(e,k)` - Schlüssel von Entry `e` wird durch k ersetzt und der alte key wird zurückgegeben
- `replaceValue(e,v)` - Wert von Entry `e` wird durch `v` ersetzt, alter Wert wird zurückgegeben


## Map
- Durchsuchbare Collection mit Key-Value entries.
- Erlaubt keine identischen Keys (Priority Queue schon)
- Anwendungen:
	- Einfache Datenbanken
	- Personalverzeichnis
	- Ortschaftsnamen (Key=PLZ, Value=Name)

Basic Operationen:
- `get(k)`
- `put(k, v)` - entry existiert noch nicht -> return null, andernfalls return old value
- `remove(k)`
- `size()`
- `keySet()`
- `values()`
- `entrySet()`


## Multimap
Erlaubt mehrere Keys.
`remove(k,v)` - da ein key auf mehrere Elemente zeigen kann muss angegeben werden, welcher gelöscht werden soll.

## Set & Multiset

Collection ohne Duplikate.

Multiset: erlaubt Duplikate


## Hash-Tabelle

Möglichkeiten bei Kollisionen: 
- **seperate-chaining**: An der Adresse eine Liste speichern, statt nur 1 Wert
- **Open-Addressing:** Neuer nicht kollidierender Index suchen
	- **Linear Probing**: nächste freie Stelle suchen.
		- alternierend vorwärts/rückwärts
		- quadratische Sondierung (+1 Schritt, +4, +9...)
	- **Double Hashing:** zweite Hash Funktion anwenden.
- **Double Hashing** - bei Kollision erneut hashen.


## Binary Search

- bei jedem Schritt wird die Anzahl der Knoten halbiert
- terminiert nach $\mathcal{O}(\log n)$


### Binary-Search-Tree

* Ein binärer Baum mit Key-Value Paaren, welche in _internen Knoten_ speichert.
* Externe Knoten enthalten keine Werte, sind nur Platzhalter anstelle von `null`.
* Knoten sind so angeordnet, dass In-Order Traversierung die Knoten in aufsteigender Reihenfolge besucht.

#### Suche

- `TreeSearch(key, Node)` vergleicht den übergebenen Key mit dem Key des übergebenen Knoten. 
- Wenn die Keys gleich sind wird der übergebene Knoten als Resultat zurückgegeben.
- Wenn die Keys ungleich sind wird rekursiv im entsprechenden Sub-Baum weitergesucht.
- Wenn der übergebene Knoten ein _externer Knoten_ ist, wird dieser als Resultat zurückgegeben.


#### Einfügen

- `insert(key)` sucht zuerst den Key mit `TreeSearch()`
- Ist ***Key*** noch nicht im Baum vorhanden und ***w*** ist das Blatt, welches mit der Suche gefunden wird. Wir fügen ***k*** beim Knoten ***w*** ein und expandieren ***w*** in einen _internen Knoten_ ![[binary_tree_insert.png]]


#### Löschen

- Bei `remove(key)` wird zuerst nach dem Key gesucht.
- Dann muss zwischen 3 Fällen unterschieden werden
	1. Node hat 2 externe Knoten
	2. Node hat 1 externer Knoten
	3. Node hat keine externe Knoten


**Fall 1:**
Gesuchte Node wird mit ihrem rechten Kind ersetzt.

**Fall 2:**
Das Child wird and die Stelle vom Parent verschoben.

**Fall 3:**
Finde Knoten ***w*** welcher in ***Inorder-Folge*** der Vorgänger von der gesuchten Node ***v*** ist und kopiere den Key von ***w*** nach ***v***. Lösche Knoten ***w***, indem dessen externer Knoten mit `removeExternal(z)` gelöscht wird.


### Performance

Ein binärer Baum mit ***n*** entries und der Höhe ***h***:
- benötigter Speicher ist $\mathcal{O}(n)$
- `find`, `insert` und `remove` sind $\mathcal{O}(h)$

Die Höhe ***h*** ist:
- $\mathcal{O}(n)$ im schlechtesten Fall
- $\mathcal{O}(\log n)$ im besten Fall


## AVL-Tree

Erweiterung von Binary-Search-Tree, für eine Garantie, dass die Höhe $\mathcal{O}(\log n)$ ist. Somit gilt der Baum als balanciert.

Der Höhenunterschied der beiden Teilbäume aller Knoten darf nicht grösser als 1 sein.

Umstrukturierung **Single**-Rotation:
1. Knoten $x$ finden, bei dem die Regel verletzt ist.
2. Child-Knoten $y$ von grösserem Baum finden
3. Child-Knoten $y$ mit $x$ tauschen und $x$ beim kleineren Sub-Baum einfügen


Umstrukturierung **Doppel**-Rotation:
1. Einfach 2 mal Single brauchen.


## Splay-Tree

- Rotationen nach _jeder_ Operation (auch nach suchen)
- Operation ***splay:*** siehe SW 07 s.19


![[splay_tree_regelwerk.png]]



## Merge-Sort

Divide-and-Conquer

Divide: Input Daten S in zwei getrennte Teilmengen $S_1$ und $S_2$ aufteilen
Recur: (Wiederholen) die Teilprobleme von $S_1$ und $S_2$ lösen. Rekursiv $S_1$ & $S_2$ sortieren.
Conquer: Zusammenfügen der Lösungen $S_1$ und $S_2$ in $S$.  Immer kleinstes (vorderstes) Element einfügen.

Laufzeit: $\mathcal{O}(n \log n)$

Kurz: rekursiv die Sequenz aufsplitten und dann immer das kleinste Element der beiden Teillisten in eine zusammengefügte Liste hinzufügen.
## Quick-Sort

1. Auswahl eines beliebigen pivot Elementes $x$. 
2. Aufteilung von $S$ in kleinere Elemente ($L$), grössere Elemente ($G$) und gleiche Elemente ($E$), basierend auf dem Pivot-Element.
3. Einzelne Teilgruppen nach gleichem Prinzip lösen (Recur)
4. Zusammenfügen


Laufzeit:
Best-case: $\mathcal{O}(n \log n)$
Worst-case: $\mathcal{O}(n^2)$


## Zusammenfassung Sortier-Algorithmen

| Algorithmus | Zeit | Notizen |
| ---- | ---- | ---- |
| selection-sort | $\mathcal{O}(n²)$ | - in-place<br>- slow (good for small inputs) |
| insertions-sort | $\mathcal{O}(n²)$ | - in-place<br>- slow (good for small inputs) |
| merge-sort | $\mathcal{O}(n \space \log n)$ | - sequential data access, stable<br>- fast |
| quick-sort | $\mathcal{O}(n \space \log n)$ <br>expected | - in-place, randomised<br>- fastest |


## Entscheidungsbaum

Wie viele Möglichkeiten gibt es, die Zahlen zu vertauschen (Permutationen / $n!$ ).
Höhe des binären Baumes ist dann $\log (n!)$.
<p style="color: orange">Jeder Vergleichs-basierter Sortier Algorithmus hat eine minimale Laufzeit von log(n!)</p>
Vergleichs-basierte Sortierung hat eine untere Grenze der Laufzeit von $\mathcal{O}(n \space \log (n))$

## Bucket-Sort

Grundsätzlich ein Array, bei welchem die Keys gleichgesetzt mit dem Index in einem Array ist. Grundsätzlich wird an jedem Index ein weiteres Array gespeichert $\rightarrow$ Falls es mehrere gleiche Werte gibt. 
Beispiel:
`[7, 1, 3, 7, 3, 7]` $\rightarrow$ `[[], [1], [], [3, 3], [], [], [], [7, 7, 7], [], []]`
(Das zweite Array hat eine vorgegebene Länge $n$).


Vorgang:
- Sei $S$ eine Sequenz von $n$ (Key, Value) Entries mit Keys im Bereich von $[0, \mathbf{N} - 1]$
- Bucket-Sort benutzt die Keys als Index in einen Hilfs-Array $B$ von Sequenzen (Buckets)
	- Phase1: Sequenz $S$ leeren durch verschieben jedes Entry $(k, o)$ in sein Bucket $B[k]$
	- Phase2: Für $i = 0 \dots N-1$, verschiebe die Entries des Buckets $B[i]$ and das Ende der Sequenz $S$

Analyse:
- Phase 1 benötigt $\mathcal{O}(n)$
- Phase 2 benötigt $\mathcal{O}(N + n)$
- Bucket-Sort benötigt $\mathcal{O}(n + N)$



## Lexikographische Ordnung

n-tuples sind einfach tuples, bei welchem jeder Index eine weitere Dimension repräsentiert.
Heisst die Koordinaten im 3-dimensionalen Raum ist ein 3-tuple.
Ist eine Lexikographische Sortierung, das heisst:
Sortiert n-tuples in einer Sequenz $S$. Dabei wird nacheinander zuerst das hinterste Element der Tuples verglichen und dann eines weiter vorne bis zum vordersten.
`stableSort(S, Ci)` ist ein Sortieralgorithmus, welcher nach der Funktion $C_i$ sortiert.

![[Lexikographische Sortierung (Radix-Sort).png]]

Performance: $\mathcal{O}(d * T(n)$ , wobei $T(n)$ die Laufzeit von `stableSort()` ist.

## Radix-Sort

Ist eine Spezialisierung eines lexikographischen Sortier-Algorithmus, welcher [[ADS#Bucket-Sort|Bucket-Sort]] als Sortier-Algorithmus für die einzelnen Dimensionen verwendet. 

Performance: $\mathcal{O}(d(n + N)$, wobei $N$ die grösste Zahl und $i$ die Anzahl Dimensionen ist.

## Graphen

Terminologie:

| Begriff | Definition |
| ---- | ---- |
| Einfacher Pfad | direkter Pfad von A nach B |
| Nicht einfacher Pfad | Pfad mit Loops / Node kommt mehrmals vor |
| Zyklus | zirkuläre Sequenz (Anfang und Ende ist gleich, kann auch einfach odr nicht einfach sein) |
| $n$ | Anzahl Vertizes |
| $m$ | Anzahl Kanten |
| $deg(v)$ | Grad von Vertex $v$ |
| $E$ | ist eine Collection von Vertizes-Paaren, Kanten (engl. Edge) |
| $V$ | ist ein Set von Vertizes (Knoten) |
| inzident (enden) | falls $a$ und $b$ an Knoten $V$ enden sind $a, b$ inzident an $V$ |
| Adjazente | (benachbarte Vertizes / Knoten) |
| Parallele Kanten | fall beide Kanten den gleichen Ursprungs- & Endknoten haben. |
| Spanning Tree | Ist ein aufspannender (alle Knoten sind verbunden) Baum, welche alle Knoten vom ursprünglichen Graphen beinhaltet |



Eigenschaften:

$\sum{}_v deg(v) = 2m$
Beweis: jede Kante wird zweimal gezählt


### Graph ADT

Zugriffsmethoden
- `endVertices(e)`: an Array of the two end-vertices of $e$
- `opposite(e)`
- `areAdjacent(v, w)`
- `replace(v, x)`
- `replace(e, x)`

Update-Methoden
- `insertVertex(o)`: insert a vertex storing element o.
- `insertEdge(v, w, o)`: insert an edge between $v$ and $w$, storing Element $o$.
- `removeVertex(v)`
- `removeEdge(e)`

Iterator-Methoden
- `incidentEdges(v)`: all edges incident to $v$.
- `vertices()`: all vertices in the graph.
- `edges()`: all edges in the graph.


### Adjazenz-Liste 

- Kanten-Listen Struktur
- Inzidenz-Sequenz für jeden Vertex
	- Sequenz der Positionen auf Kantenobjekte der inzidenten Kanten
- Erweiterte Kanten-Objekte
	- Referenziert auf die assoziierten Positionen in der Inzidenzsequenz der Endvertizes


### Adjazenz-Matrix

- Kanten-Listen Struktur
- erweiterte Vertex-Objekte
	- Integer Key (Index) assoziiert mit Vertex
- 2D-Array Adjazenz-Array
	- Referenziert auf die Kantenobjekte für adjazente (benachbarte) Vertizes
	- **null** für nichtadjazente (nicht-benachbarte) Vertizes


## Depth-First-Search (DFS)

- Depth-First Search (DFS) ist eine allgemeine Technik für die Traversierung eines Graphen
- Eine DFS Traversierung eines Graphen G 
	- besucht alle Vertizes und Kanten von G
	- bestimmt, ob G verbunden ist
	- berechnet / bestimmt die verbundenen Komponenten von G
	- berechnet einen aufspannenden Wald von G
- DFS auf einem Graphen mit $n$ Vertizes und $m$ Kanten benötigt $\mathcal{O}(n+m)$ Zeit.
- DFS kann man auch erweitern, um andere Graphenprobleme zu lösen:
	- Finden und Ausgeben eines Pfades zwischen zwei gegeben Vertizes
	- Finden von Zyklen
- DFS entspricht in etwa der Euler-Tour bei binären Bäumen.

Benutzt einen Mechanismus um "Labels" auf Kanten und Vertizes zu setzen.

>[!note]
>Benötigt $\mathcal{O}(n+m)$ Zeit auf Graph mit $n$ Vertizes und $m$ Kanten.

Alle mit DISCOVERY markierten Kannten bilden den aufspannenden Baum.

![[beispiel_DFS.png]]


### Breadth-First-Search / Breitensuche (BFS)

- Ist eine allgemeine Technik für die Traversierung eines Graphen.
- Eine BFS Traversierung eines Graphen $G$
	- besucht alle Vertizes und Kanten von $G$
	- bestimmt ob $G$ verbunden ist
	- bestimmt die verbundenen Komponenten von $G$
	- berechnet einen aufspannenden Wald von $G$
- BFS auf einem Graphen mit $n$ Vertizes und $m$ Kanten benötigt $\mathcal{O}(n + m)$ Zeit
- Kann auch verwendet werden um:
	- Finden und Ausgeben eines Pfades mit einer minimalen Anzahl Kanten zwischen zwei gegebenen Vertizes
	- Finden von Zyklen


![[bfs_pseudo_code.png]]

![[bfs_beispiel.png]]


### DFS vs BFS

| Applikationen                                             | DFS | BFS |
| --------------------------------------------------------- | --- | --- |
| Aufspannender Wald, Verbundene Komponenten, Pfade, Zyklen | ✔️  | ✔️  |
| Kürzester Pfad                                                          |     | ✔️  |
| Biconnected Komponenten                                                          | ✔️    |     |

## Gerichtete Graphen

- Jede Kante hat eine Richtung
- Digraph: jede Kante geht nur in eine Richtung
- Strong connectivity: Jeder Vertex kann alle anderen Vertizes erreichen
- Wenn G einfach ist: $m \leq n(n-1)$
- Wenn In- und Out- Kantenlisten in separaten Listen sind, ist Laufzeit von Grösse der Listen abhängig
- Vier Typen von Kanten
	- Baumkanten (discovery) - Kante des Waldes
	- Rückkanten (back) - Verbindung zum Vorgänger
	- Vorwärtskanten (forward) - Verbindung zu einem Nachfolger
	- Kreuzungskanten (cross) - Alle übrigen Kanten


Digraph Anwendungen:
- Task-Scheduling (Task a muss terminieren bevor Task b anfangen darf)


### Gerichtete Tiefensuche

DFS und BFS können auf gerichtete Kanten angewendet werden. Dabei gibt es aber 4 verschiedene Arten von Kanten:
- discovery
- back
- forward
- cross



### Gewichtete Graphen

- Jede Kante hat einen assoziierten nummerischen Wert (das Gewicht)
- Die Länge eines Pfades ist die Summe der Gewichte seiner Kanten


### Dijkstra

Arbeitet mir "Wolken". Nacheinander Strecken zu Vertizes berechnen und diese dann in die "Wolke" aufnehmen.

- Die Distanz eines Vertex $v$ zu einem Vertex $s$ ist die Länge des kürzesten Pfades zwischen $s$ und $v$
- Dijkstra's Algorithmus berechnet die Distanzen zu allen Vertizes von einem Start-Vertex $s$ aus
- Annahmen:
	- Der Graph ist verbunden
	- Die Kanten sind ungerichtet
	- Die Kantengewichte sind nicht negativ
- Mit jedem Vertex $v$ speichern wir eine Eigenschaft $d(v)$ welche die Distanz von $v$ zu $s$ im Untergraph (bestehend aus der Wolke und den Nachbar-Vertizes) angibt
- Bei jedem Schritt:
	- Wir fügen der Wolke den Vertex $u$ hinzu, welcher ausserhalb der Wolke ist und die kleinste Distanz $d(u)$ aufweist
	- Wir aktualisieren die Distanzen von allen Nachbar-Vertizes von $u$


### Dijkstra ADT

- Eine [[ADS#Adaptable Priority Queue|Adaptierbare Priority Queue]] speichert die Vertizes ausserhalb der Wolke:
	- Key: Distanz
	- Element: Vertex
- Locator-basierte Methoden:
	- `insert(k,e)` - gibt einen Locator zurück
	- `replaceKey(l,k)` - ändert den Schlüssel eines Eintrags
- Wir speichern zwei Eigenschaften mit jedem Vertex:
	- Distanz $d(v)$
	- Locator der Priority Queue


