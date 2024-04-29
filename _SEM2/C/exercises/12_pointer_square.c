#include <stdio.h>
#include <stdbool.h>


void square(int* i){
    *i * *i;
}


int main(int argc, char const *argv[])
{
    int i = 4;
    square(&i);
    printf("%d\n", i);

    return 0;
}
