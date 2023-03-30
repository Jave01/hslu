#include <stdio.h>
#include <string.h>
#include <stdbool.h>


int slen(const char *str);
void scat(char result[], const char str1[], const char str2[]);
bool scmp(const char str1[], const char str2[]);

int main(int argc, char const *argv[])
{
    // char s1[] = "Hi there I'm Tim ";
    // char s2[] = "Hi Tim I'm a door";
    char s1[] = "1234";
    char s2[] = "1234";

    printf("%d\n", scmp(s1, s2));
    return 0;
}


int slen(const char *str){
    int len = 0;
    int i = 0;

    while (str[i] != '\0')
    {
        i++;
    }
    return i;
}


void scat(char result[], const char str1[], const char str2[]){
    char c;
    int len1 = slen(str1);
    int len2 = slen(str2);

    for (int i = 0; i <= slen(str1); i++)
    {
        result[i] = str1[i];
    }
    for (int i = 0; i < slen(str2); i++)
    {
        result[len1 + i] = str2[i];
    }
    result[len1 + len2 + 1] = '\0';
}


bool scmp(const char str1[], const char str2[]){
    char c;
    int i = 0;

    while (true)
    {
        if (str1[i] == str2[i])
        {
            if (str1[i] == '\0')
            {
                return true;
            }
        } else {
            return false;
        }
        i++;
    }
}