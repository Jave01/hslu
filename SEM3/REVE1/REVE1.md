# REVE1

## Good to know

Beautiful (graphical) version of `diff`: Meld

## Glossary:

| Term | Description |
| ---- | ---- |
| CISC | Complex Instruction Set Computer (only Intel) |
| RISC | Reduces Instruction Set Computer (easier to make fast) |
| PAF | Procedure activation frame |
| ELF | Executable and Linking Format |


## Registers
#TODO #registers

| Register                      | Description                               |
| ---- | ---- |
| `%rsp` | Stack pointer - always points to bottom of stack |
| `%rex` |  |
| `%ebp` | base pointer - function arguments are located at `%ebp + x` |
|  |  |
|  |  |
|  |  |
|  |  |
|  |  |
|  |  |
|  |  |
|  |  |
| `%rax` | Return result of function |

Parameter 1: `8(%ebp`
Parameter 2: `16(%ebp`



## Assembly commands
#commands #TODO 

| Command                   | Description                                                                                                                                                           |
| ------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `leaq (%rdi, %rsi), %eax` | Load effective address, basically transfer calculate with pointers. similar to `t = x+y`,                                                                                            |
| `movx src,dest`           | [[05.The.Processor.Interface.pdf]] p.77. src / dest can not both be memory addresses. move different amount of bytes based of `x`<br>`q` -> 8 bytes<br>`l` -> 4 bytes |
|                           |                                                                                                                                                                       |


### Difference `ldaq` and `movx`

```
movq a, b
```


### Address computation examples
#computations

- `%rdx` = `0xF000`
- `%rcx` = `0x100`

| Expression | Address Computation | Address |
| ---- | ---- | ---- |
| `0x8(%rdx)` | `rdx` + 8 | 0xF008 |
| `(%rdx, %rcx)` | `rdx` + `rcx` | 0xF100 |
| `(%edx, %ecx, 4)` | `edx` + `ecx` * 4 | 0xF400 |
| `0x80(,%rdx,2)` | `rdx` * 2 + 0x80 | 0x1E080 |


## C Programming
#C

Everything is C

### Why C / C++?

- Ubiquity
	- operating systems
	- device drivers
	- embedded computing, edge devices, IoT
	- virtual machines of higher-level languages written in C/C++
		- CPython
		- JavaScript engines
- Power
	- direct access to memory
	- expressive, yet terse


"With great power comes great responsibility"
- Easy to shoot yourself in the foot
- Writing good, standards-compliant code is not hard and will make your life much easier.
- Bad code is not a question of the language


If you want to execute statements parallel (order doesn't matter): separate statements by ','.


### Unions
#unions
- put several data values at the same memory address
- biggest member determines total size of struct
- useful to bypass the type system

```c
void print_fhex(float f)
{
	union {
		int i;
		float f;
	} if;
	
	if.f = f;
	printf("%04x\n", if.i);
}
```



### Type mixing
#default
If you mix signed and unsigned types the whole expression is considered as unsigned.


### Data types in memory
#datatypes

How are the different types stored in memory
#### Arrays
just the datatypes joined together.


```c
// Example with 2D Array
int arr[2][4]

/* 
Memory-layout:
[x, x, x, x][x, x, x, x]
*/
```

#### Struct
Example:
```c
struct A {
	int a;
	int b;
}

struct A x;
struct A y;

/* 
Memory layout:

y.b [         ] - 0x10b
y.a [         ] - 0x108
x.b [         ] - 0x104
x.a [         ] - 0x100

*/
```

>[!note]
>Insert padding to ensure type is not sliced into multiple words
>align at `sizeof(T)`.
>Which essentially means that an `int` must be at an address divisible by 4. A `short` on an address divisible by 2, a `char` at an address divisible by 1 etc.



## C++ Programming

Print to console:

```c++
#include <iostream>
using namespace std;

int main(){
	std::cout << "Hello World\n";
}
```


Lambdas
 `[captures ] ( params ) -> <ret> { body }`

Example:
```cpp
sort(a.begin(), a.end(), [](const int& x, const int& y) -> bool
{
	return x > y;
});
```


## Processor Architecture
#architecture #CPU

See also: ![[CPU Architectures.canvas|CPU Architectures]]

Processor life:
1. Fetch next instruction
	- fetch from where
2. Decode instruction
	- instruction format
	- supported operations
	- supported operands
	- where do operands come from / go to?
3. Execute instruction
	- Implementation
4. Go to 1


### Instruction Set Architecture (ISA)
#CPU #ISA

- The visible interface between software and hardware
- What the user (OS, compiler, programmer) needs to know in order to reason about how the machine behaves.
- Abstracted from the details of how it may accomplish its task.
- Intel syntax example:
$$add \space r_{dest}, r_{src1}, r_{src2}$$


**Programm counter (PC):** pointer to next instruction in memory
**Control Register**: x83_32: 8 registers, 64bit: 16 register (one is stack pointer)

Defines also Operations:
- arithmetic and logical
- data transfer
- control 
- system (kernel mode, user mode)
- floating point
- string
- multimedia, AI (matrix multiplications)


### Intel x86 Processors
#x86 #CPU

- Still dominate market
- Evolutionary design
	- backwards compatible to 1978
	- Added more features as time goes on
- Complex instruction set computer
	- Many different instructions with many different formats
	- Hard to match performance of RISC
	- Good speed, bad power consumption.


### x86-64
#x86_64 #CPU #registers 

- 64 bit
- Register
	- 16 general purpose
	- PC (`%rip`)
	- condition codes
- backwards compatible
- x86
	- Just Intel processor names
	- 8086
	- 8186
	- 8286

![[x86_64_integer_registers.png]]


### IA32 (traditional x86)
#IA32 #x86 

- 2 operands, max 1 memory
- fewer registers (8)
- PC is called `%eip`
- no 16 byte data type

General form:
- `operation [operands]`

Two-operand format:
- `operation operand2, operand1`
- first operand also servers as destination

Operand size specifier
- postfix to the operation
- b (1 byte), w (2 bytes), I (4 bytes), q (quad word -> 8 bytes -x86_64 only)

Operand types
- immediate ($ - constants), register (%), or memory
- at most one memory operand
- reigster size specification
    - rX (64bit), eX (32bit), X (16bit), Xh/l (8bit)
    - example: `rax`, `dax`, `ax`, `ah/l`

#registers 
![[IA32_integer_registers.png]]



### Condition Codes
#conditions #conditioncodes

starting at [[05.The.Processor.Interface.pdf]] p. 126

Implicitly set by all (except `lea`) arithmetic operations (side effect of computation)

- CF - Carry Flag (for unsigned)
	- set if carry out from most significant bit (unsigned Overflow)
- ZF - Zero Flag (for signed)
	- set if t == 0 (all bits 0)
- SF - Sign Flag
	- set if t < 0 (as signed)
- OF - Overflow Flag
	- set if two's-complement (signed) overflow


>[!note]
>These bits are not set by `lea`.


>[!example]
>```armasm
>sub $1, %rcx
>cmp $0, %rcx
>jne do_begin
>```
>above code can be shortened to:
>```armasm
>sub $1, %rcx ; -> sets ZF
>jne do_begin ; basically jnz do_begin
>```


`cmp b,a` == `a-b`
`testq b,a` == `a&b`

Reading codes (is set? -> very rare):
#flowcontrol

| setx | Condition | Description |
| ---- | ---- | ---- |
| sete | ZF | Equal / Zero |
| setne | ~ZF | Not Equal / Not Zero |
| sets | SF | Negative |
| setns | ~SF | Non-negative |
| setg | ~(SF^OF)&~ZF  | Greater (signed) |
| setge | ~(SF^OF) | Greater or equal (signed) |
| setl | (SF^OF) | Less (signed) |
| setle | (SF^OF)\|ZF | Less or equal (signed) |
| seta | ~CF & ~ZF | Above (unsigned) |
| setb | CF | Below (unsigned) |

>[!warning]
>`setX` sets only the lowest 8 Bits -> only use on 8 Bit registers


`jumpx` -> same extension like `setx` with the extension of `jmp` -> jumps always to destination.

`cmovx` -> conditional `mov` based on $x$.
Motivation for `cmovx` is to reduce jumps, so that the instruction pipelines don't need to be flushed so often.


### Control transfer
#bufferoverflow #call #functions

[[05.The.Processor.Interface.pdf]] p.153

- `call label`
	- push address of next instruction onto stack
	- jump to **label**.
- `ret`
	- pop return address from stack
	- jump to address


### parameter passing:

- IA32: all parameters on stack
- x86_64: first 6 arguments in registers, remaining on stack

On x86_64:
1. param: `%rdi`
2. param: `%rsi`
3. param: `%rdx`


### Local Variable Mapping
#variables

- Setting variables to fixed memory address fails for recursive functions.
- Allocate on runtime stack
	- push them from registers onto stack if called recursively


### Register Saving Conventions
#conventions

- Conventions between caller and callee - is my value in register x still the same?
- Can registers be used for temporary storage?

- Calling conventions:
	- "Caller Save"
		- registers that the callee can overwrite
		- caller saves temporary values in its frame before the call
	- "Callee save"
		- registers that the callee must preserve before overwriting with a new value
		- Callee saves temporary values in its frame before using

#registers 
![[x86_64_calling_convention.png]]

![[IA32_calling_convention.png]]


>[!note]
>If you're not calling other functions always use "caller saved".



## Translation of C to Assembly

Source: [[06.Translation.to.Assembly.pdf]]

![[Assembly_general_structure.png]]

Labels:
- string followed by ':' (e.g. `main:`, `hstr:`)

Differences:
- Intel: `jmp <label>`
- RISC-V: `branch <label>`

Conditional branches (`if (operand1 <cond> operand2) goto <label>`):
- Intel: `cmp` first then `j<cond> <label>`
- RISC-V: `b<cond> operand1, operand2, <label>`


Loop in assembly:

[[06.Translation.to.Assembly.pdf]] p.23

```c
do {
	i++;
}while(i < 10)
```
is

```armasm
movl a,v
cmp a,b
jne %a
```


Jump with dereference:

```armasm
jmp *%rax          # got to address stored as value in %rax 
```

For Switch-Case tables, indirect jumping with a table may be used [[06.Translation.to.Assembly.pdf]] p.37

**Jumping:**
- Direct: 
	- jmp .L2
	- Jump target is denoted by label .L2
- Indirect: 
	- `jmp *.L4(,%rdi,8)`
	- Start of jump table: .L4
	- Must scale by factor of 8 (labels have 64-bits = 8 bytes addresses on x86_64)
	- Fetch target from effective Address .L4 + rdi\*8
	- Only for 0 ≤ x ≤ \<table max\>



## Program Execution

Functions need definitions -> if external linker gets it after compilation.


### What do Linkers do?

- merges separate code and data sections into single sections
- relocates symbols from their relative locations in the .o files to their final absolute memory locations in the executable
- updates all references to these symbols to reflect their new positions

### Three Kinds of Object Files

- Relocatable object file (.o)
	- Code and data that can be combined with other relocatable object files.
- Executable object file (a.out)
	- Contains code and data which can be copied into memory and executed
- Shared object file (.so)
	- DLL on Windows


### ELF

- Header
	- Word size, byte ordering, file type
- Segment header table
	- page size, virtual addresses memory segments
- .text section
	- Code
- .rodata section
	- read only data, printf strings, jump tables, ...
- .data section
	- initialized global variables
- .bss section
	- Uninitialised global variables
	- "Block Started by Symbol"
	- Has section header but occupies no space
- .symtab section
	- Symbol table
	- Procedure and static variable names
- .re.text section
- .rel.data section
	- Relocation info for .data section
	- Addresses of pointer data that will need to be modified in the merged executable
- .debug/.line section
	- info for symbolic debugging (`gcc -g`)
- Section header table
	- offsets and sizes of each section


### Linker Symbols

- Global symbols
	- can be referenced by other modules
	- non static global variables
- External symbols
	- global symbols that are referenced by module $m$ but defined by some other module
	- declarations marked with `external`
- Local Symbols
	- that are defined and referenced exclusively by module $m$
	- C functions with `static`
	- local linker symbols are NOT local program variables


### Linker Symbol Rules

[[07.Program.Execution.pdf]] p.29



### Relocating Code and Data

1. Take all object files
2. calculate addresses
3. generate table with calculated offset jumps (`call <PC-relative offset`)
4. save in [[#ELF]] file


