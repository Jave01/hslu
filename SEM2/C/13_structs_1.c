#include <stdio.h>
#include <stdbool.h>


struct Employee{
    char *name;
    int hire_date;
    float salary;
};


int main(int argc, char const *argv[])
{
    struct Employee e = {
        .name = "Jan",
        .hire_date = 20230101,
        .salary = 1000
    };

    char name[256];
    int hire_date;
    float salary;

    printf("name, hire_date (YYYYMMDD), salary: ");
    scanf("%s%d%f", name, &hire_date, &salary);

    struct Employee entered_employee = {
        .name = name,
        .hire_date = hire_date,
        .salary = salary
    };
    
    printf("Initial employee: %s %d %.2f\n", e.name, e.hire_date, e.salary);
    printf("Entered employee: %s %d %.2f\n", entered_employee.name, entered_employee.hire_date, entered_employee.salary);
    return 0;
}
