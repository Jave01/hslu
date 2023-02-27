#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>


int main(int argc, char const *argv[])
{
    if (argc < 2)
    {
        printf("Argument range not given or not valid");
    }
    
    int range = atoi(argv[1]);
    int primes[(int)(range / 2) + 2];
    primes[0] = 2;
    primes[1] = 3;
    int prime_index = 2;

    printf("All prime numbers up to 100: ");

    bool denominator_found;
    for (int i = 5; i <= range; i += 2){
        denominator_found = false;
        for (int j = 2; j <= i / 2; j++){
            if (i % j == 0)
            {
                denominator_found = true;
                break;
            }   
        }
        if(!denominator_found){
            primes[prime_index] = i;
            prime_index++;
        }
    }
    for (int i = 0; i < sizeof(primes); i++)
    {
        if (primes[i + 1] != 0)
        {
            printf("%d, ", primes[i]);
        } else {
            printf("%d\n", primes[i]);
            break;
        }
    }
    
    return 0;
}
