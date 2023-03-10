#include <stdio.h>

int main(){
    FILE* fp = fopen("file.txt", "r");
    char str1[10], str2[10], str3[10], str4[10];

    if(fp == NULL){
        return -1;
    }

    /* If the file has 3 words delimited by a whitespace
    it will write them into str1, str2 and str3 */
    fscanf(fp, "%s %s %s %s", str1, str2, str3, str4);

    printf("String1: %s\n", str1);
    printf("String2: %s\n", str2);
    printf("String3: %s\n", str3);
    printf("String4: %s\n", str4);

    fclose(fp);
    
    return 0;
} // end of main