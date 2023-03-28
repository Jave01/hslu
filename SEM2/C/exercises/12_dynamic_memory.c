#include <stdio.h>
#include <stdlib.h>

int main(int argc, char const *argv[])
{
    int len;
    char* inp;

    printf("Enter the length of the string: ");
    scanf("%u", &len);
    inp = malloc(len * sizeof(char));

    printf("Enter your message: ");
    scanf("%s", inp);

    printf("You entered: %s\n", inp);

    free(inp);

    return 0;
}
