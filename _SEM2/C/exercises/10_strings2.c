#include <stdio.h>
#include <stdbool.h>
#include <string.h>


void srev(char result[], const char* str1);
void bsort(char inp[][256]);


int main(int argc, char const *argv[])
{
    // char s1[] = "Hi there I'm Tim ";
    // char s2[] = "Hi Tim I'm a door";
    char s1[] = "one";
    char s2[] = "two";
    char s3[] = "three";
    char res[256] = {0};

    char arr[3][256] = {s3, s2, s1};
    bsort(arr);

    return 0;
}


void srev(char result[], const char* str1){
    int len = strlen(str1);

    for (int i = 0; i < len; i++)
    {
        result[i] = str1[len - i - 1];
    }
}

void bsort(char inp[][256]){
    int l, i, n, j;
    char temp[25];
    for(i = 1; i <= n; i++){
        for(j = 0; j <= n - i; j++){
            if(strcmp(inp[j],inp[j+1])>0)
            {
                strncpy(temp,inp[j], sizeof(temp) - 1);
                strncpy(inp[j],inp[j+1], sizeof(inp[j]) - 1);
                strncpy(inp[j+1],temp, sizeof(inp[j] + 1) - 1);
            }
        }
    }

    for (int i = 0; i < sizeof(inp); i++)
    {
        printf("%s", inp[i]);
    }
}