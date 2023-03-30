#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <ctype.h>
#include <string.h>

/* Function prototypes */
char isnum(char* str);


int main(int argc, char const *argv[])
{
    /* code */
    char inp[20];
    long double min;
    double days;
    double years;


    while (1)
    {
        printf("Enter a time in minutes: ");
        scanf("%s", inp);
        min = atoi(inp);

        if (min < 1 || min > __INT_MAX__)
        {
            printf("Not a valid number\n");
        }
        else
        {
            days = min / 60 / 24;
            years = days / 365;
            printf("Minutes: \t%.2Lf\nDays: \t\t%.4f\nYears: \t\t%.4f\n", min, days, years);
        }
    }

    return 0;
}


/* function declarations */
char isnum(char* str){
    char comma_occured = false;

    int len = strlen(str);
    for (int i = 0; i < len; i++)
    {
        if(!isdigit(str[i])){
            if (str[i] == '.' && !comma_occured && i != len-1)
            {
                comma_occured = true;
            }else
            {
                return false;
            }
        }
    }
    return true;
}