# C Sharp in Action

## Einführung SW2

Struct vs Objects:
Struct wird auf dem Stack abgelegt, heisst falls ein Struct einer Funktion übergeben wird, wird eine Kopie gemacht und nicht eine Referenz übergeben.

Value types (int, struct, object) are compatible with boxing/unboxing

### Boxing

object obj = 3;
wraps up the value 3 in a heap object.


### Unboxing

int x = (int) obj;

unwraps the value again


### Structs

Fields **must not** be initialized in their declaration

```cs
struct Point {
	int x = 0; // compilation error
}
```

can be allocated with
```cs
Point p; // fields are initialized to '0' (if p is a field of a class!)
Point q = new Point();
```


### Methods

Methods can only be overridden if marked by `virtual`.

Parameters:
- standard/nothing (copy)
- ref (reference)
- out (no value is passed, write in callee needed)




## Delegates and Events

### Delegates

- In C: Funktionspointer
- Methode registrieren und übergeben?
- can have value `null`
- normal objects on heap
- A delegate variable can hold multiple methods at the same time


Syntax delegate type:
```cs
// 1. Declaration of delegate type
delegate void DelName (string sender)

// 2. declaration of a delegate variable 
DelName greetings;

// 3. assigning a method to a delegate variable
void SayHello(string sender){
	Console.WriteLine("Hello from " + sender);
}

greetings = new DelName(SayHello) 
// or just greetings = SayHello

// 4. calling a delegate variable
greetings("John");

```


In .NET library delegates have following signature:
```cs
delegate void SomeEventHandler (object source, MyEventArgs e);
```

- Result:  `void`
- 1. parameter: sender of event (type `object`)
- 2. parameter: Method parameters

### Events

- Special Delegate Field
- Why events instead of delegates?
	- Only the class that declares the event can fire it (better encapsulation)
	- Other classes may change event fields only with `+=` or `-=` (but not with '=')



```cs
delegate void DelName (object source, MyEventArgs e)

class Model {
	public event DelName notify;
	public void Change() {... notify(("Model"));}
}
```


## Sockets

```c#
using System.IO
```


**Drei Operationen:**
- In Stream schreiben
	- Welche Art, bestimmt der Typ des Streams
	- Es werden Bytes geschrieben
- Aus Stream lesen
	- Es werden Bytes gelesen
- Wahlfreier Zugriff auf Dateninformationen in einen Stream
	- Ganzer Stream auszuwerten ist nicht immer nötig


Types
- Base-Streams - direkt aus Strom Daten lesen oder schreiben
- Pass-Through-Streams - ergänzen Base-Stream um spezielle Funktionalitäten


>[!tip]
>Handle streams inside a `using` statement like this:
>```csharp
>using (StreamWriter sw = new StreamWriter("data.txt"), System.Text.Encoding.UTF8){
>	sw.WriteLine("Hello there");
>}
>```
>Then the memory gets properly free'd

>[!note]
>`System.Text.Encoding.UTF8` ensures proper encoding



### Socket Prinzip

- Ist Ende-zu-Ende Kommunikation
- Stellen Abstraktion eines Datenendpunktes dar


**Funktionalitäten:**
- Verbindung zu Prozess aufbauen
- Daten senden
- Daten empfangen
- Verbindung beenden
- Einen Port / Applikation binden
- An einem Port auf Verbindungswunsch hören
- Verbindungswunsch akzeptieren / ablehnen


>[!note]
>Wir bewegen uns nur auf dem logischen Pfad (Applikation zu Applikation)
>Physischer Pfad beinhaltet noch die anderen Layers


Erzeugen:
```csharp
TcpClient the Client;
try {
	theClient = new TcpClient(String host, int port);
}
catch (ArgumentOutOfRangeException) {
	// Port ausserhalb des erlaubten Bereichs
}
catch (SocketException) {
	// Fehler beim Zugriff auf Socket
    Console.WriteLine("No: {0}", somevar);
}
```


Informationen:

```csharp
try {
	var theClient = new TcpClient("localhost", 2);
	Socket s = theClient.Client;
	s.LocalEndpoint;
	s.RemoteEndpoint;
	s.ProtocolType;
}
catch (Exception e){
	Console.WriteLine("sucks");
}
```


Server socket (Deprecated):
```csharp
try {
	TcpListener listen = new TcpListener(port);
	listen.Start();

	while(...){
		var client = listen.AcceptTcpClient();// wait for client to connect
		// Kommunikation mit client
		client.Close();
	}
}
catch (Exception e){
	Console.WriteLine("You suck");
}
```

>[!warning]
>Above code is deprecated see recommended below


Server Socket (Recommended):
```csharp
try {
	IPAddress ip = Dns.GetHostEntry("raspberrypi").AddressList[0];
	TcpListener listen = new TcpListener(ip, 420);
	listen.Start();
	while (true){
		TcpClient client = listen.AcceptTcpclient();
		Console.WriteLine("Verbindung zu {0} aufgebaut", client.Client.RemoteEndPoint);
		TextWriter tw = new StreamWriter(client.GetStream());
		tw.Write(DateTime.Now.ToString());
		tw.Flush();
		client.Close();
	}
}
```

