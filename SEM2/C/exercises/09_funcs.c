#include <stdio.h>
#include <math.h>

int gcd(int a, int b);
float abs(float a);
float sqrt(float a);

int main(int argc, char const *argv[])
{
    printf("gcd: %d\n", gcd(8, 6));
    printf("abs: %.2f\n", abs(-6.99));
    printf("abs: %.2f\n", sqrt(9));
    printf("abs: %.2f\n", sqrt(6.5));
    printf("abs: %.2f\n", sqrt(0));
    printf("abs: %.2f\n", sqrt(-4));
    return 0;
}


int gcd(int a, int b){
    if (a < 1 || b < 1)
    {
        return -1;
    }
    
    int smaller = a >= b ? b : a;

    int gcd = 1;
    for (int i = 2; i <= smaller; i++)
    {
        if ((a % i) == 0 && (b % i) == 0)
        {
            gcd = i;
        }
           
    }

    return gcd;
}

float abs(float a){
    return (a < 0 ? a*-1 : a);    
}

float sqrt(float a){
    // if (a == 0)
    // {
    //     return 0;
    // }
    
    // if (a < 0)
    // {
    //     printf("Negative value is invalid input for calculating sqrt");
    //     return -1;
    // }
    
    // return (float)pow(a, 1/2.0);
}