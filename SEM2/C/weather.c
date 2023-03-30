#include <stdio.h>
#include <math.h>

#define YEARS   5
#define MONTHS  12


double sum(float *arr);


int main(int argc, char const *argv[])
{
    float rain[YEARS][MONTHS] =
    {
        {4.3,4.3,4.3,3.0,2.0,1.2,0.2,0.2,0.4,2.4,3.5,6.6},
        {8.5,8.2,1.2,1.6,2.4,0.0,5.2,0.9,0.3,0.9,1.4,7.3},
        {9.1,8.5,6.7,4.3,2.1,0.8,0.2,0.2,1.1,2.3,6.1,8.4},
        {7.2,9.9,8.4,3.3,1.2,0.8,0.4,0.0,0.6,1.7,4.3,6.2},
        {7.6,5.6,3.8,2.8,3.8,0.2,0.0,0.0,0.0,1.3,2.6,5.2}
    };

    char month_names[MONTHS][3]={"JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"};
    
    int year, month;
    float year_avg[YEARS], month_avg[MONTHS]; 

    for (int y=0; y < YEARS; y++){
        year_avg[y] = sum(rain[y]) / YEARS;
    }

    for (int m = 0; m < MONTHS; m++){
        for(int y = 0; y < YEARS; y++){
            month_avg[m] += rain[y][m];
        }
        month_avg[m] /= YEARS;
        puts(month_names[m]);
        printf(": %.2f", month_avg[m]);
    }

    

    return 0;
}


double sum(float* arr){
    double sum;
    for (int i = 0; i <= sizeof(*arr); i++)
    {
        sum += arr[i];
    }
    return sum;
}
