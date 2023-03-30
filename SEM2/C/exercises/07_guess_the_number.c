#include <stdio.h>
#include <stdlib.h>
#include <time.h>

#define bool    _Bool
#define false   0
#define true    1



int main(int argc, char const *argv[])
{
    // init random num generator
    time_t t;
    srand((unsigned) time(&t));
    printf("You have to guess the right number between 1-20\n");
    
    int ranNumber = rand() % 20 + 1;
    int user_guess;
    int attempts = 1;
    do{
        printf("\n%d. attempt\n", attempts);
        printf("Your guess: ");
        scanf("%d", &user_guess);

        if (user_guess == ranNumber)
        {
            printf("You nailed it\n");
            return 0;
        } else if (user_guess > ranNumber)
        {
            printf("You're too high\n");
        } else 
        {
            printf("You're too low\n");
        }
        attempts++;
        }while (attempts <= 5);
        printf("Sorry, you have no more attempts left.\n");
    return 0;
}
