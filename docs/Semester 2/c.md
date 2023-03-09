# I2C

## Inhalt

### Strings

**Functions from strings.h**:

```C
    strlen(s1); // Returns length. Relies on null termination

    strcpy(s1, s2); //Copy string from s2 to s1. Does not check if s1 is big enough
    strncpy(s1, s2, n); //Copy n characters from s2 to s1

    strcat(s1, s2); // Concatenate s2 with s1. Does not check if s1 is big enough
    strncat(s1, s2, n); // Concatenate n chars of s2 with s1

    strcmp(s1, s2); // Concatenate s2 with s1. Return value is like .compareTo from java (-1, 0, 1)
    strncmp(s1, s2, n); // Concatenate n chars of s2 with s1

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

- mostly used for function paramaters, function can accept any data type.
- must first be explicitly cast to a data type before use.

### Dynamic Memory

If you want to allocate memory on an address without creating a variable, you can manually allocate it with some functions.  
For example if you want a pointer to an address with some storage without first creating a variable, which allocates the needed storage.
