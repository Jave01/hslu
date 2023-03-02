#include <stdio.h>

#define HOURS_PER_WEEK              40
#define TAX_RATE_FIRST_300          0.15
#define TAX_RATE_ADDITIONAL_150     0.20
#define TAX_RATE_EXCEED_450         0.25    

int main(int argc, char const *argv[])
{
    int hours, exceed;
    float taxes, gross_pay, net_pay; 

    printf("Enter the amount of hours you worked:\n> ");
    scanf("%d", &hours);
    
    exceed = hours > HOURS_PER_WEEK ? hours - HOURS_PER_WEEK : 0;
    hours -= exceed;
    gross_pay = 12 * hours + 12 * 1.5 * exceed;

    if (gross_pay >= 450)
    {
        taxes += 300 * TAX_RATE_FIRST_300;
        taxes += 150 * TAX_RATE_ADDITIONAL_150;
        taxes += (gross_pay - 450) * TAX_RATE_EXCEED_450;
    } else if (gross_pay >= 300)
    {
        taxes += 300 * TAX_RATE_FIRST_300;
        taxes += (gross_pay - 300) * TAX_RATE_ADDITIONAL_150;
    } else
    {
        taxes += gross_pay * TAX_RATE_FIRST_300;
    }
    
    net_pay = gross_pay - taxes;

    printf("gross_pay:\t%.2f\ntaxes:\t\t%.2f\nnet_pay:\t%.2f\n",gross_pay, taxes, net_pay);
    
    return 0;
}
