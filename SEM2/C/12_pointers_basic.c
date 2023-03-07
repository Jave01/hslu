#include <stdio.h>
#include <stdbool.h>


int main(int argc, char const *argv[])
{
    int val1 = 5;
    unsigned int *pval1 = &val1;

    printf("%u, %u, %d\n", &pval1, pval1, *pval1);
    return 0;
}
