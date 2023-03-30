#include <stdio.h>

int main(int argc, char const *argv[])
{
    /* code */
    enum Company{
        GOOGLE, FACEBOOK, XEROX, YAHOO, EBAY, MICROSOFT
    };

    enum Company x = XEROX;
    enum Company g = GOOGLE;
    enum Company e = EBAY;

    printf("%d\n", x);
    printf("%d\n", g);
    printf("%d\n", e);

    return 0;
}
