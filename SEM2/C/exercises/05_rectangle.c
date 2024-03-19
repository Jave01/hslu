#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <math.h>

enum color{
    red = 0,
    green = 1,
    blue = 2
};

int main(int argc, char* argv[]){
    char str[] = "Hello my dear friend";

    if (argc != 3)
    {
        /* code */
        return -1;
    }
    
    double width = atoi(argv[1]);
    double height = atoi(argv[2]);
    double perimeter = 2 * width + 2 * height;
    double area = width * height;

    printf("%g, %g\n", perimeter, area);

    return 0;
}

