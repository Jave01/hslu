# Rust

# General
---
## Good to know
---
- `str.trim()` - removes duplicate white spaces
- `format!("{}{}", str1, str2)` - combining strings can also be done with `.to_owned()` and `+`.


## Initialising project
---
```bash
cargo new <project-name>
cargo build
cargo run # build and run
```


## Naming conventions
---

|Item | Convention |
| --- | --- |
| Variables | snake_case |
| Functions | snake_case |
| Modules | snake_case |
| Macros | snake_case! |
| Types | PascalCase |
| Enums | PascalCase |
| Constants | SCREAMING_SNAKE_CASE |
| Statics | SCREAMING_SNAKE_CASE |


## Libraries
---
Libraries are also crates like binary crates (files that can be compiled into a binary) in Rust. They can be imported with `use` and are separated into modules.

```rust
use std::io; // (1) 
use rand::mem; // (2)
```

1. `std` is the standard library and is among other things used for io stuff (`std::io`).
2. `mem` can be used to get the size of a variable (`mem::size_of_val(&x)`).

# Types
---
#TODO Concatenate with the types below.

## Enums
---
General syntax without any value assigned:

```rust
enum IpAddrKind {
    V4,
    V6,
}

let var1 = IpAddrKind::V4;
```

If you want to assign values to the fields, you can do when constructing the object for example storing the actual ip address:

```rust
enum IpAddr {
		V4(u8, u8, u8, u8),
    String),
}

let home = IpAddr::V4(127, 0, 0, 1);

loopback = IpAddr::V6(String::from("::1"));
```

## Implement functions
---
### Enum
---
In types like structs and Enums you can define functions (like methods) to do some stuff. You can do this with the `impl` keyword, although you need to check for the different types if you don’t want a general function (e.g. with the `match` statement):

```rust
enum Message {
		Help,
		Attack,
}

impl Message {
	fn shout(&self){
		match self {
			Message::Help => println!("Help I'm lost!"),
			Message::Attack => println!("We must attack now!")
		}
	}
	fn general_function(&self){
		println!("I'm always the same");
	}
}

let msg1 = Message::Help;
let msg2 = Message::Attack;
msg1.shout();
msg2.shout();

msg1.general_function();
msg2.general_function(); 

/* output:
Help I'm lost!
We must attack now!
I'm always the same
I'm always the same
*/
```


# Modules
---
## Public and private
---
A package can contain multiple binary crates and optionally one library crate.

- **Start from the crate root**: When compiling a crate, the compiler first looks in the crate root file (usually *src/lib.rs* for a library crate or *src/main.rs* for a binary crate) for code to compile.
- **Declaring modules**: In the crate root file, you can declare new modules; say, you declare a "garden" module with `mod garden;`. The compiler will look for the module’s code in these places:
    - Inline, within curly brackets that replace the semicolon following `mod garden`
    - In the file *src/garden.rs*
    - In the file *src/garden/mod.rs*
- **Declaring submodules**: In any file other than the crate root, you can declare submodules. For example, you might declare `mod vegetables;` in *src/garden.rs*. The compiler will look for the submodule’s code within the directory named for the parent module in these places:
    - Inline, directly following `mod vegetables`, within curly brackets instead
    of the semicolon
    - In the file *src/garden/vegetables.rs*
    - In the file *src/garden/vegetables/mod.rs*
- **Paths to code in modules**: Once a module is part of your crate, you can refer to code in that module from anywhere else in that same crate, as long as the privacy rules allow, using the path to the code. For example, an `Asparagus` type in the garden vegetables module would be found at `crate::garden::vegetables::Asparagus`.
- **Private vs public**: Code within a module is private from its parent modules by default. To make a module public, declare it with `pub mod` instead of `mod`. To make items within a public module public as well, use `pub` before their declarations.
- **The `use` keyword**: Within a scope, the `use` keyword creates shortcuts to items to reduce repetition of long paths. In any scope that can refer to `crate::garden::vegetables::Asparagus`, you can create a shortcut with `use crate::garden::vegetables::Asparagus;` and from then on you only need to write `Asparagus` to make use of that type in the scope.

Inside modules you can use super for referring to functions and variables:

```rust
fn deliver_order() {}

mod back_of_house {
    fn fix_incorrect_order() {
        cook_order();
        super::deliver_order();
    }

    fn cook_order() {}
}
```

Here is another example of using `pub` on struct, attributes and functions:

```rust
mod back_of_house {
    pub struct Breakfast {
        pub toast: String,
        seasonal_fruit: String,
    }

    impl Breakfast {
        pub fn summer(toast: &str) -> Breakfast {
            Breakfast {
                toast: String::from(toast),
                seasonal_fruit: String::from("peaches"),
            }
        }
    }
}

pub fn eat_at_restaurant() {
    // Order a breakfast in the summer with Rye toast
    let mut meal = back_of_house::Breakfast::summer("Rye");
    // Change our mind about what bread we'd like
    meal.toast = String::from("Wheat");
    println!("I'd like {} toast please", meal.toast);

    // The next line won't compile if we uncomment it; we're not allowed
    // to see or modify the seasonal fruit that comes with the meal
    // meal.seasonal_fruit = String::from("blueberries");
}
```

>[!note]
>If you use `pub` on an `enum` all of its attributes will be public.


## Multiple files
---
For detailed information see [here](https://doc.rust-lang.org/book/ch07-05-separating-modules-into-different-files.html).

In short - if you want to include modules from different files you have to name the file like the module and the submodules you have to put into a folder named like the parent module. So if you want to split this code (e.g. `src/lib.rs`):
```rust
mod front_of_house{
	pub mod hosting {
	    pub fn add_to_waitlist() {}
	}
}

pub use crate::front_of_house::hosting;

pub fn eat_at_restaurant() {
    hosting::add_to_waitlist();
}
```

into multiple files you would have to create a file called `src/front_of_house.rs` and a file called `src/front_of_house/hosting.rs`.

`src/lib.rs`:
```rust
mod front_of_house;

pub use crate::front_of_house::hosting;

pub fn eat_at_restaurant() {
    hosting::add_to_waitlist();
}
```

`src/front_of_house.rs`:
```rust
pub mod hosting
```

`src/front_of_house/hosting.rs`:
```rust
pub fn add_to_waitlist() {}
```


## Use
---
Consider this code:
```rust
mod front_of_house {
    pub mod hosting {
        pub fn add_to_waitlist() {}
    }
}
```

if we want to use the `add_to_waitlist()` function we would have to specify the full path like this:

```rust
pub fn eat_at_restaurant() {
    // Absolute path
    crate::front_of_house::hosting::add_to_waitlist();

    // Relative path
    front_of_house::hosting::add_to_waitlist();
}
```


But fortunately we can shorten this with the `use` keyword like this:

```rust
mod front_of_house {
    pub mod hosting {
        pub fn add_to_waitlist() {}
    }
}

use crate::front_of_house::hosting;

pub fn eat_at_restaurant() {
    hosting::add_to_waitlist();
}
```

If you want to import multiple things from the same module you can use this syntax:
```rust
use std::time::{SystemTime, UNIX_EPOCH};
```



# Variables & Types
---
## General
---
Variables can be defined with `let` and are immutable by default. If not directly initialized, the type must be specified.

>[!tip] Tip
>If you want to use a variable as mutable, you have to declare it with `mut`


**Primitive data types**: 
- integers
- floats
- booleans
- characters
- strings
- tuples
- arrays.

Examples to all of them:
```rust
/* Integers */ 
let x = 5; 
let y: i32 = 5; // standard integer is signed 32 bit

/* Floats */ 
let x = 2.0; 
let y: f32 = 3.0;

/* Lists */ 
let l1 = [1, 2, 3]; 
let l2: [i32; 3] = [1, 2, 3]; // with type and size specified [type; size] let l3 = [0; 5]; // [0, 0, 0, 0, 0] initialized with "list comprehension"

/* Tuples */ 
let t1 = (1, 2, 3); 
let t2: (i32, f64, u8) = (1, 2.0, 3); // with type specified 
let (x, y, z) = t2; // destructuring 
let x = t2.0; // access by index

/* Booleans */ 
let b1 = true; 
let b2: bool = false;

/* Characters */ 
let c1 = ‘a’; 
let c2: char = ‘b’; 
let c3 = ‘😻’; // unicode

/* Strings */ 
let s1 = "Hello"; // string literal, immutable, stack allocated 
let s2: &str = "World"; // string slice 
let s3 = String::from("Hello"); // heap allocated string
```

```rust 
/* Integers */ 
i8, i16, i32, i64, i128, isize //
u8, u16, u32, u64, u128, usize

/* Floats */ 
f32, f64 
```

>[!info]
>`isize` and `usize` depend on the architecture of the computer the program is running on. On a 64 bit system, they are 64 bits wide, on a 32-bit system, they are 32 bits wide. The `usize` type is typically used to index into collections / pointers.

>[!tip]
If you want to use the same variable name again, you have to use `let` again. This is called shadowing. It is useful if you want to change the type of a variable.


## Casting
---
If you want to cast one variable type to another (for example float to int) you can use `as`.
```rust
// Suppress all warnings from casts which overflow.
#![allow(overflowing_literals)]

fn main() {
    let decimal = 65.4321_f32;

    // Error! No implicit conversion
    let integer: u8 = decimal;
    // FIXME ^ Comment out this line

    // Explicit conversion
    let integer = decimal as u8;
    let character = integer as char;

    // Error! There are limitations in conversion rules.
    // A float cannot be directly converted to a char.
    let character = decimal as char;
    // FIXME ^ Comment out this line

    println!("Casting: {} -> {} -> {}", decimal, integer, character);

    // when casting any value to an unsigned type, T,
    // T::MAX + 1 is added or subtracted until the value
    // fits into the new type

    // 1000 already fits in a u16
    println!("1000 as a u16 is: {}", 1000 as u16);

    // 1000 - 256 - 256 - 256 = 232
    // Under the hood, the first 8 least significant bits (LSB) are kept,
    // while the rest towards the most significant bit (MSB) get truncated.
    println!("1000 as a u8 is : {}", 1000 as u8);
    // -1 + 256 = 255
    println!("  -1 as a u8 is : {}", (-1i8) as u8);

    // For positive numbers, this is the same as the modulus
    println!("1000 mod 256 is : {}", 1000 % 256);

    // When casting to a signed type, the (bitwise) result is the same as
    // first casting to the corresponding unsigned type. If the most significant
    // bit of that value is 1, then the value is negative.

    // Unless it already fits, of course.
    println!(" 128 as a i16 is: {}", 128 as i16);

    // 128 as u8 -> 128, whose value in 8-bit two's complement representation is:
    println!(" 128 as a i8 is : {}", 128 as i8);

    // repeating the example above
    // 1000 as u8 -> 232
    println!("1000 as a u8 is : {}", 1000 as u8);
    // and the value of 232 in 8-bit two's complement representation is -24
    println!(" 232 as a i8 is : {}", 232 as i8);

    // Since Rust 1.45, the `as` keyword performs a *saturating cast*
    // when casting from float to int. If the floating point value exceeds
    // the upper bound or is less than the lower bound, the returned value
    // will be equal to the bound crossed.

    // 300.0 as u8 is 255
    println!(" 300.0 as u8 is : {}", 300.0_f32 as u8);
    // -100.0 as u8 is 0
    println!("-100.0 as u8 is : {}", -100.0_f32 as u8);
    // nan as u8 is 0
    println!("   nan as u8 is : {}", f32::NAN as u8);

    // This behavior incurs a small runtime cost and can be avoided
    // with unsafe methods, however the results might overflow and
    // return **unsound values**. Use these methods wisely:
    unsafe {
        // 300.0 as u8 is 44
        println!(" 300.0 as u8 is : {}", 300.0_f32.to_int_unchecked::<u8>());
        // -100.0 as u8 is 156
        println!("-100.0 as u8 is : {}", (-100.0_f32).to_int_unchecked::<u8>());
        // nan as u8 is 0
        println!("   nan as u8 is : {}", f32::NAN.to_int_unchecked::<u8>());
    }
}
```


## Arrays
---
Need to be printed with `{:?}`.

```rust
println!("{:?}", [1, 2, 3]);
```

Slicing can be done with `[start..end]` or `[start..]` or `[..end]`.

```rust 

let a = [1, 2, 3, 4, 5]; let slice = &a[1..3]; // [2, 3]

println!("{:?}", &a[..3]); // [1, 2, 3]

for i in 1..=3 { println!("{}", i); // 1, 2, 3 } 
```

## Strings
---
heap: `String::from("Hello")`

stack (`&str`): `"Hello"`

```rust 
let s = String::from("Hello World"); 
let slice = &s[1..3]; // "el"
```

Because strings are UTF-8 encoded, you can’t index them. You can only slice them. One possible way to get the chars is to iterate over the string:

```rust 
for c in "my string".chars() {
	println!("{}", c); 
}
```


## Tuples
---
Tuples are fixed length and can contain different types.

The individual values can be accessed with `tup.0`, `tup.1`, `tup.2` etc. or with destructuring:

```rust 
let tup: (i32, char, bool) = (1, "hi", false); 
println!("{:?}", tup); // (1, "hi", false)

let (x, y, z) = tup; 
println!("x = {}, y = {}, z = {}", x, y, z); // x = 1, y = hi, z = false 
```


## Structs
---
```rust 
#[derive(Debug)] 
struct User {     
	username: String,     
	password: String,     
	email: String,     
	is_admin: bool,     
	id: u64, 
}
```

Structs can not be printed by default. To print them, you have to derive the `Debug` trait with `#[derive(Debug)]`.

An object can be created like this:

```rust 
let user = User { 
	username: String::from("BigBadBoy"), 
	password: String::from("password"), 
	email: String::from("email@provider.tv"), 
	is_admin: false, id: 1, 
};

println!("{:?}", user); // User { username: "user", password: "password", email: "email@provider", is_admin: false, id: 1 } 
println!("{}", user.username); // BigBadBoy 
```

If you want to instantiate a struct within a function, you don’t have to manually specify the attributes, you can use the shorthand syntax, as long as the variable names are the same as the attribute names:

```rust 
fn build_user(username: String, email: String) -> User {     
	User {  
		username, // shorthand syntax         
		email, // shorthand syntax         
		is_admin: false,         
		id: 1,         
		password: String::from("password"),     
	} 
}
```

For minimising classes, you can give structs functions:

```rust 
impl User { 
	fn print(&self) { 
		println!("{} ({})", self.username, self.email); 
	} 
}

user.print(); // BigBadBoy (email@provider.tv) 
```

For more info about functions see [Functions](about:blank#functions).


## Enums
---
By default all value are integers.

```rust 
enum PermissionLevel { 
	User, 
	Moderator, 
	Admin, 
}

let user = PermissionLevel::User; 
```

`Enums` can have functions too

```rust 
impl PermissionLevel { 
	fn print(&self) { 
		match self { 
			PermissionLevel::User => println!("I am a User"),
			PermissionLevel::Moderator => println!("I am a Moderator"),
			PermissionLevel::Admin => println!("I am a Admin"), 
		} 
	} 
}

user.print(); // I am a User 
```

Enums can also contain other values than integers:

```rust 
enum PermissionLevel { 
	Guest, 
	User(String), 
	Moderator(String), 
	Admin(String), 
}

let user = PermissionLevel::User(String::from("BigBadBoy")); 
```

Some more examples:

```rust
enum IpAddrKind {
    V4,
    V6,
}

let var1 = IpAddrKind::V4;
```

If you want to assign values to the fields, you can do when constructing the object for example storing the actual ip address:

```rust
enum IpAddr {
		V4(u8, u8, u8, u8),
    String),
}

let home = IpAddr::V4(127, 0, 0, 1);

loopback = IpAddr::V6(String::from("::1"));
```

## Option
---
An option always consists of either `Some` or `None`. It is used to prevent null pointer exceptions. They can then be handled with a match statement.

```rust 
let x: Option = Some(5); 
let y: i32 = 6;

match x { 
	Some(val) => println!("added: {}", val + y), // 11 
	None => println!("only y: {}", y), 
} 
```

more about `match` in chapter [Flow control - match](#match)


# Collections
---
## Vectors
---
Are like arrays but not with a fixed size. They are not directly lists, because the values are always stored next to each other but they are variable in size. If the size of all the stored items exceeds the allocated memory, new memory will be allocated and the values copied to the new destination.
Creating a simple empty vector looks like this where the type will be `Vec<T>`:
```rust
let mut v: Vec<i32> = Vec::new();
```
there is the `vec!` macro which allows you to create a Vector based on a collection of items:
```rust
let v = vec![1, 2, 3];
```

To append a value to a vector use `push()`.
```rust
let mut v = Vec::new();

v.push(5);
v.push(6);
v.push(7);
v.push(8);
```

A Vector can only hold one type of value in it. If you want to store different variable types in one Vector you can use an enum with different attributes.
```rust
enum SpreadsheetCell {
	Int(i32),
    Float(f64),
    Text(String),
}

let row = vec![
	SpreadsheetCell::Int(3),
    SpreadsheetCell::Text(String::from("blue")),
    SpreadsheetCell::Float(10.12),
];
```

>[!note]
>If you want to read a value from a vector you can either use an index or the `get()` function whereas `get()` will return `Option<T>`.


## Hash Maps
---
Allow you to store key-value pairs (dictionary). The type `HashMap<K, V>` is implemented in `std::collections::HashMap`. A new hash map can (as always) be created with the `new()` function. Values can be inserted with `insert(K, V)`.
```rust
use std::collections::HashMap;

let mut scores = HashMap::new();

scores.insert(String::from("Blue"), 10);
scores.insert(String::from("Yellow"), 50);
```

Like vectors, hash maps are homogeneous. Which means that all keys and must have the same type and all values must have the same type.

>[!tip]
>If you want to handle the `Option` from `get()` in one line that could look like this:
>```rust
>let score = scores.get(&team_name).copied().unwrap_or(0);
>```
>where `copied()` converts the `Option<&i32>` to `Option<i32>` and `unwrap_or(0)` either returns the value or in case of `None` it returns 0.

## Hash Maps with already existing values
---
If the key already exists in the Hash Map and you have a new value for it you can either
- overwrite the existing value
- discard the new value
- change the value based on the previous value

For overwriting just use `.insert(key, val)` like with a new key.

If you want to discard the new value if a key-value pair already exists with the given key, you can use the `entry()` method. This looks like this:
```rust
use std::collections::HashMap;

let mut scores = HashMap::new();
scores.insert(String::from("Blue"), 10);

scores.entry(String::from("Yellow")).or_insert(50);
scores.entry(String::from("Blue")).or_insert(50);

println!("{:?}", scores);
```

An example for the third approach is when you want to count the words in a text. Then you can also use the `entry()` method with the `or_insert()` because this return the pointer to the value.
```rust
use std::collections::HashMap;

let text = "hello world wonderful world";

let mut map = HashMap::new();

for word in text.split_whitespace() {
	let count = map.entry(word).or_insert(0);
    *count += 1;
}

println!("{:?}", map); // will print {"world": 2, "hello": 1, "wonderful": 1}
```


# Flow control
---
## Infinite loop
---
There is a built in infinite loop:

```rust
let counter = 0; 
loop {
	println!("Hello");     
	counter += 1;     
	if counter == 5 {
		break;     
	} 
}
```

If there are nested loops you can "name" them for controlling how far you want to break:

```rust 
'outer: loop {     
	println!("Entered the outer loop");     
	'inner: loop {         
		println!("Entered the inner loop");         
		break 'outer; // breaks the outer loop     
	}     
	println!("This point will never be reached"); 
}
```


## While loop
---
while loops are pretty similar to other languages:

```rust 
let mut number = 3; 
while number != 0 {
	println!("{}!", number);     
	number -= 1; 
}
```


## For loop
---
For loops can be used to iterate over a range, items in a list or something that implements the `Iterator` trait:

```rust 
let list1 = [1, 2, 3, 4, 5]; 
for element in list1 { 
	println!("{}", element); // 1, 2, 3, 4, 5 
}

// same output but different syntax 
for i in 1..=5 { 
	println!("{}", i); // 1, 2, 3, 4, 5 } 
```

If you want to iterate over a list and also want to know the index of the current element, you can use `enumerate()`:

```rust 
let list1 = ['a', 'b', 'c', 'd', 'e']; 
for (i, element) in list1.iter().enumerate() { 
	println!("{}: {}", i, element); // 0: a, 1: b, 2: c, 3: d, 4: e 
}
```

for reversing, the `rev()` function can be used on the iterator e.g. `[0..5].rev()`


## Match
---
`match` is similar to `switch` in other languages, where multiple values with the same output can be chained together with `|`:

```rust 
let number = 3;

match number { 
	1 => println!("one"), 
	2 => println!("two"), 
	3 | 5 | 7 => println!("prime for x > 2 and x < 10"), _ => println!("something else"), 
} 
```

```rust 
let mut guess; io::stdin()
				.read_line(&mut guess)
				.expect("Failed to read line");

let var = "adslkfjsadf"

match guess.trim().parse::() { 
	Ok(42) => println!("You guessed 42"), 
	Ok(*) => println!("You guessed something else"), 
	Err(*) => println!("You didn’t enter a number"), 
} 
```

match can also be used to destructure structs:

```rust 
struct Point { x: i32, y: i32, }

let p = Point { x: 0, y: 7 };

match p { 
	Point { x, y: 0 } => println!("On the x axis at {}", x), 
	Point { x: 0, y } => println!("On the y axis at {}", y), 
	Point { x, y } => println!("On neither axis: ({}, {})", x, y), 
} 
```

they can also have a return value (e.g. for error handling):

```rust
let x = 5;

let error = match x { 
	1 => "Error 1!", 
	2 => "Error 2!", 
	3 => "Error 3!", 
	4 => "Error 4!", 
	5 => "Error 5!", 
	_ => "Error 0!", 
}; 
```

Indirectly accessing a variable makes it impossible to branch and use that variable without re-binding. `match` provides the `@` sigil for binding values to names:
```rust
// A function `age` which returns a `u32`.
fn age() -> u32 {
    15
}

fn main() {
    println!("Tell me what type of person you are");

    match age() {
        0             => println!("I haven't celebrated my first birthday yet"),
        // Could `match` 1 ..= 12 directly but then what age
        // would the child be? Instead, bind to `n` for the
        // sequence of 1 ..= 12. Now the age can be reported.
        n @ 1  ..= 12 => println!("I'm a child of age {:?}", n),
        n @ 13 ..= 19 => println!("I'm a teen of age {:?}", n),
        // Nothing bound. Return the result.
        n             => println!("I'm an old person of age {:?}", n),
    }
}
```

You can also use binding to "destructure" `enum` variants, such as `Option`:
```rust
fn some_number() -> Option<u32> {
    Some(42)
}

fn main() {
    match some_number() {
        // Got `Some` variant, match if its value, bound to `n`,
        // is equal to 42.
        Some(n @ 42) => println!("The Answer: {}!", n),
        // Match any other number.
        Some(n)      => println!("Not interesting... {}", n),
        // Match anything else (`None` variant).
        _            => (),
    }
}
```


# Option
---
## The type definition
---
Options are great if you want to handle errors in partially defined functions for example. Options always hold `Some(val)` with a value or `None`. For example
```rust
fn divide(numerator: f64, denominater: f64) -> Option<f64>{
	if denominator == 0.0 {
		None
	} else {
		Some(numerator / denominator)
	}
}
```

The `?` operator can help clean up the `match`-boilerplate code. For example this code:
```rust
fn add_last_numbers(stack: &mut Vec<i32>) -> Option<i32> {
    let a = stack.pop();
    let b = stack.pop();

    match (a, b) {
        (Some(x), Some(y)) => Some(x + y),
        _ => None,
    }
}
```
can be replaced by this code:
```rust
fn add_last_numbers(stack: &mut Vec<i32>) -> Option<i32> {
    Some(stack.pop()? + stack.pop()?)
}
```

>[!note]
>More detailed info about the type `Option` in general can be found [here](https://doc.rust-lang.org/std/option/).

>[!tip]
>For errors there is also the built in type `Result<T, E>` which can either be `Ok()` or `Err()`. `T` and `E` are the two variable types that can be returned from either `Err()` or `Ok()`. It is also very common that the `main` function returns `Result`.



## Destructuring
---
### if let
---
Destructuring can be done with a `match` statement. Although in some cases it is cleaner to use the `if let` statement. For example the following code uses a lot of wasted space:
```rust
// Make `optional` of type `Option<i32>`
let optional = Some(7);

match optional {
    Some(i) => {
        println!("This is a really long string and `{:?}`", i);
        // ^ Needed 2 indentations just so we could destructure
        // `i` from the option.
    },
    _ => {},
    // ^ Required because `match` is exhaustive. Doesn't it seem
    // like wasted space?
};


```
With `if let` the code can be shortened and in addition allows various failure options to be specified:
```rust
fn main() {
    // All have type `Option<i32>`
    let number = Some(7);
    let letter: Option<i32> = None;
    let emoticon: Option<i32> = None;

    // The `if let` construct reads: "if `let` destructures `number` into
    // `Some(i)`, evaluate the block (`{}`).
    if let Some(i) = number {
        println!("Matched {:?}!", i);
    }

    // If you need to specify a failure, use an else:
    if let Some(i) = letter {
        println!("Matched {:?}!", i);
    } else {
        // Destructure failed. Change to the failure case.
        println!("Didn't match a number. Let's go with a letter!");
    }

    // Provide an altered failing condition.
    let i_like_letters = false;

    if let Some(i) = emoticon {
        println!("Matched {:?}!", i);
    // Destructure failed. Evaluate an `else if` condition to see if the
    // alternate failure branch should be taken:
    } else if i_like_letters {
        println!("Didn't match a number. Let's go with a letter!");
    } else {
        // The condition evaluated false. This branch is the default:
        println!("I don't like letters. Let's go with an emoticon :)!");
    }
}
```

in the same way `if let` can be used to match any enum value:
```rust
// Our example enum
enum Foo {
    Bar,
    Baz,
    Qux(u32)
}

fn main() {
    // Create example variables
    let a = Foo::Bar;
    let b = Foo::Baz;
    let c = Foo::Qux(100);
    
    // Variable a matches Foo::Bar
    if let Foo::Bar = a {
        println!("a is foobar");
    }
    
    // Variable b does not match Foo::Bar
    // So this will print nothing
    if let Foo::Bar = b {
        println!("b is foobar");
    }
    
    // Variable c matches Foo::Qux which has a value
    // Similar to Some() in the previous example
    if let Foo::Qux(value) = c {
        println!("c is {}", value);
    }

    // Binding also works with `if let`
    if let Foo::Qux(value @ 100) = c {
        println!("c is one hundred");
    }
}
```


### while let
---
Similar to `if let` a `while let` can be used to clean up Option with `match` statement but in a loop. Consider the following code:
```rust
// Make `optional` of type `Option<i32>`
let mut optional = Some(0);

// Repeatedly try this test.
loop {
    match optional {
        // If `optional` destructures, evaluate the block.
        Some(i) => {
            if i > 9 {
                println!("Greater than 9, quit!");
                optional = None;
            } else {
                println!("`i` is `{:?}`. Try again.", i);
                optional = Some(i + 1);
            }
            // ^ Requires 3 indentations!
        },
        // Quit the loop when the destructure fails:
        _ => { break; }
        // ^ Why should this be required? There must be a better way!
    }
}
```

Using `while let` makes the code much cleaner:
```rust
fn main() {
    // Make `optional` of type `Option<i32>`
    let mut optional = Some(0);

    // This reads: "while `let` destructures `optional` into
    // `Some(i)`, evaluate the block (`{}`). Else `break`.
    while let Some(i) = optional {
        if i > 9 {
            println!("Greater than 9, quit!");
            optional = None;
        } else {
            println!("`i` is `{:?}`. Try again.", i);
            optional = Some(i + 1);
        }
        // ^ Less rightward drift and doesn't require
        // explicitly handling the failing case.
    }
    // ^ `if let` had additional optional `else`/`else if`
    // clauses. `while let` does not have these.
}
```

>[!tip]
>Remember that `if let` and `while let` can be stacked into each other.
>For example:
>`while let Some(Some(variable)) = variable2`


# Console
---
## Simple output
---
Simple output to stdout can be done with: `println!()` or `print!()`.

Formatting can be done with `{}`.

```rust 
let x = 5; let y = 10; println!("x = {} and y = {}", x, y); // (1) 
```

1. Simply printing variables can also be done f-string like: `println!("x = {x} and y = {y}");`.

Printing to `stderr` can be done with `eprintln!()` respectively `eprint!()`.


## Pretty printing
---
Groups of variables (e.g. Arrays, Tuples) can either be printed with `{:?}` or `{:#?}`. The difference is that `{:?}` prints the variables on one line, while `{:#?}` prints them on multiple lines.

```rust 
let a = [1, 2, 3]; 

println!("{:?}", a); // console output: [1, 2, 3] 
```

```rust 
let a = [1, 2, 3]; 
println!("{:#?}", a);

// console output: 
[ 
	1, 
	2, 
	3,
] 
```


## Input
---
Input can be read from `stdin` with the `std::io` library.

it can be as simple as `io::stdin().read_line(&mut input)` if you don’t care about the error case. `&mut` is used to pass a mutable reference to the variable.

If you want to display an error message, you can use `expect()` as in:

```rust
use std::io;

let mut input = String::new(); // (1) 
io::stdin().read_line(&mut input).expect("Failed to read line"); // (2) 
```

1. `String::new()` creates a new empty string on the heap.
2. `read_line()` returns a `Result` type. If the operation was successful, it returns `Ok`. If not, it returns `Err`. `expect()` is used to handle the error case. It prints the message and exits the program.

However proper handling would be done with a `match` statement:

```rust 
let mut input = String::new(); 
match io::stdin().read_line(&mut input) {     
	Ok(n) => {
		println!("{n} bytes read");         
		println!("{input}");     
	},     
	Err(error) => println!("error: {error}"), // (1) 
}
```

1. `eprintln!()` could be used here as well.


# Functions
---
Functions are defined with the `fn` keyword. Functions can be defined anywhere in the file. However, they have to be defined before they are used.

For functions with a return value, the last expression is returned. Altough the `return` statement can also be used. The return type can be specified with `->`. The type of the arguments is specified the same way like variables, with `: <type>`.

```rust 
fn main() { 
	println!("{}", add(1, 2)); 
}

fn add(x: i32, y: i32) -> i32 { x + y }
```


# Generics
---
## General
---
You can use generics if you don't want to limit something to one datatype for making the code more flexible or minimise code duplication.

## Generics in functions
---
If you want your function to accept multiple argument types you have to define the function body with the generics syntax for example a function which finds the largest `<T>` in a list:
```rust
fn largest<T>(list: &[T]) -> &T {
```

however not every type `T` can be compared with each other, so a statement in the function like `var1 > var2` might throw a compiler error. For this you can use traits. Which for the above example might look like this:
```rust
fn largest<T: std::cmp::PartialOrd>(list: &[T]) -> &T {
```
Now this function will only accept types which support comparisons. More about traits [here](#Traits).


## Generics in Structs and Enums
---
A basic example would be:
```rust
struct Point<T> {
    x: T,
    y: T,
}

fn main() {
    let integer = Point { x: 5, y: 10 };
    let float = Point { x: 1.0, y: 4.0 };
}
```

In this example both attributes have the same type - either integer or float. But we can also define structs with different types like this:

```rust
struct Point<T, U> {
    x: T,
    y: U,
}

fn main() {
    let both_integer = Point { x: 5, y: 10 };
    let both_float = Point { x: 1.0, y: 4.0 };
    let integer_and_float = Point { x: 5, y: 4.0 };
}
```

If we implement functions in Structs and Enums with generic types they might look like this then:

```rust
struct Point<T> {
    x: T,
    y: T,
}

impl<T> Point<T> {
    fn x(&self) -> &T {
        &self.x
    }
}

fn main() {
    let p = Point { x: 5, y: 10 };

    println!("p.x = {}", p.x());
}
```

But functions can also be implemented for one type only:

```rust
impl Point<f32> {
    fn distance_from_origin(&self) -> f32 {
        (self.x.powi(2) + self.y.powi(2)).sqrt()
    }
}

```

Or if you want to mix them up:

```rust
struct Point<X1, Y1> {
    x: X1,
    y: Y1,
}

impl<X1, Y1> Point<X1, Y1> {
    fn mixup<X2, Y2>(self, other: Point<X2, Y2>) -> Point<X1, Y2> {
        Point {
            x: self.x,
            y: other.y,
        }
    }
}

fn main() {
    let p1 = Point { x: 5, y: 10.4 };
    let p2 = Point { x: "Hello", y: 'c' };

    let p3 = p1.mixup(p2);

    println!("p3.x = {}, p3.y = {}", p3.x, p3.y);
}
```


# Traits
---
Define a shared behaviour, somewhat similar to interfaces.
## Base Syntax
---
Define a trait:
```rust
pub trait Summary {
    fn summarize(&self) -> String;
    //fn foo(arg) -> val;
    //fn bar(arg) -> val;
}
```

Every type who implements the `Summary` trait has to give its own implementation of the `summarize` method. Traits can have multiple functions, each must be declared on a line, separated by a semicolon.
The Syntax for implementing the functions is somewhat similar like implementing functions for structs and enums.

```rust
pub struct NewsArticle {
    pub headline: String,
    pub location: String,
    pub author: String,
    pub content: String,
}

impl Summary for NewsArticle {
    fn summarize(&self) -> String {
        format!("{}, by {} ({})", self.headline, self.author, self.location)
    }
}

pub struct Tweet {
    pub username: String,
    pub content: String,
    pub reply: bool,
    pub retweet: bool,
}

impl Summary for Tweet {
    fn summarize(&self) -> String {
        format!("{}: {}", self.username, self.content)
    }
}
```

You can also give default implementations:

```rust
pub trait Summary {
    fn summarize(&self) -> String {
        String::from("(Read more...)")
    }
}

```

The you only have to define an empty `impl` block for the desired type for which you want to use the default implementation: 
```rust
impl Summary for NewsArticle {}
```


## Traits as Arguments
---
If you want to declare a parameter type by a trait you can do this in two different ways, either:
```rust
pub fn notify(item: &impl Summary) {
    println!("Breaking news! {}", item.summarize());
}
// or
pub fn notify<T: Summary>(item: &T) {
    println!("Breaking news! {}", item.summarize());
}
```
The second one can be much shorter if you have multiple arguments of the same type. 

If an argument should implement multiple Traits, do it like this:
```rust
pub fn notify(item: &(impl Summary + Display)) {
// or
pub fn notify<T: Summary + Display>(item: &T) {
```

If shit gets real bad with Traits as argument types you can use `where`:
```rust
fn some_function<T, U>(t: &T, u: &U) -> i32
where
    T: Display + Clone,
    U: Clone + Debug,
{
```


# Lifetimes
---
Usually you just kill somebody to end a life, simple as that. But in rust it's referring to the lifetime of variables and where and when they get destroyed.
```rust
fn main() {
    let r;

    {
        let x = 5;
        r = &x;
    }

    println!("r: {}", r);
}
```

`r` borrows `x` but because `x` is declared in an inner scope it gets destroyed (and thus make `r` an invalid pointer) before r is used -> won't compile.