#include <stdio.h>
#include <stdbool.h>


int slen(const char* s1){
    const char* stop = s1;

    while (*stop != 0)
    {
        stop++;
    }

    return stop - s1;
}


int main(int argc, char const *argv[])
{
    char s[] = "Hi";

    printf("%d\n", slen(s));

    return 0;
}
