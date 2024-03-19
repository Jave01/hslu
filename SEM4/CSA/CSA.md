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