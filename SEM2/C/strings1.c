#include <stdio.h>
#include <string.h>

int main(int argc, char const *argv[])
{
    char str[] = "Hello World, Hi there";

    char *new_str = strtok(str, " ");

    printf("%s\n", str);
    printf("%c\n", new_str[1]);

    return 0;
}
