# I2C

## Inhalt

### Strings

```cpp title="Common functions from strings.h"
strlen(s1); // Returns length. Relies on null termination

strcpy(s1, s2); //Copy string from s2 to s1. Does not check if s1 is big enough
strncpy(s1, s2, n); //Copy n characters from s2 to s1

strcat(s1, s2); // Concatenate s2 with s1. Does not check if s1 is big enough
strncat(s1, s2, n); // Concatenate n chars of s2 with s1

strcmp(s1, s2); // Compare s2 with s1. Return value is like .compareTo from java (-1, 0, 1)
strncmp(s1, s2, n); // Compare n chars of s2 with s1

strchr(str, c); // search char c in string str
strstr(s1, s2); // search substring s2 string s1

strtok(s1, s2); // legit die unlogischti Funktion ever. Sött strings ufsplitte aber zouberet aube chli strings usem nüüt füre wende NULL übergisch.

atof() // returns double value from string
atoi() // returns integer value from string
atol() // returns long value from string
atoll() // returns long long value from string

/* self explaining */
islower();
isuppeer();
isalpha();
isalnum();
iscntrol();
isprint();
isgraph(); // isprintable but without space
isdigit();
isxdigit();
isblank();
isspace();
ispunct();

toupper()
tolower()

```

### Pointers

**Void pointers**

-   mostly used for function paramaters, function casn accept any data type.
-   must first be explicitly cast to a data type before use.

### Dynamic memory

If you want to allocate memory on an address without creating a variable, you can manually allocate it with some functions.
For example if you want a pointer to an address with some storage without first creating a variablee, which would allocate the needed storage. Dynamic memory will always be allocated on the heap. The stack is another place where memory is allocated. On the stack are functionen arguments and local variables. When the execution ends, the space allocated to store arguments and local variables is freed.
Variables on the heap stay until the end of the program.

#### malloc

simplest way of allocating memory at runtime

```c title=""
malloc(<size in bytes>)
```

`malloc()` returns the adress where the memory is allocated. Note that the pointer must first be cast to the variable type used.  
It's good practice to not use static numbers for the size but to use the `sizeof()` method:

```c title=""
int *pNumber = (int*)malloc(25 * sizeof(int));
```

This way it will always reserve memory for <size> (in the example above 25) integers no matter on what system the code runs.  
`malloc()` will return `NULL` if the memory could not be allocated correctly.  
When the space is not needed anymore, the memory should be freed, so it can be used by other applications. To free the memory use `free()` on the pointer.

```c title=""
free(pNumber);
```

#### calloc

The calloc function allocates memory as a number of elements of a given size. It also initializes the allocated memory with zero.

```c title=""
int *pNumber = (int*)calloc(25, sizeof(int)); // 25 elements of size int will be allocated
```

all other aspects are similar to `malloc()`.

#### realloc

realloc enables you to reuse or extend memory that you previously allocated using `malloc()` or `calloc()`. This function is used to
dynamically reallocate memory. It requires two arguments. The first one is the pointer and the other one the size of bytes.

```c title=""
realloc(pNumber, 5);
```
