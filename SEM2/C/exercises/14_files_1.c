#include <stdio.h>
#include <stdlib.h>

int main(){
    FILE* fp = fopen("file.txt", "r");
    char c;

    if(fp == NULL){
        return -1;
    }

    int lines = 0;
    /* If the file has 3 words delimited by a whitespace
    it will write them into str1, str2 and str3 */
    while((c = fgetc(fp)) != EOF){
        if (c == '\n')
        {
            lines++;
        }
    }

    lines++;

    printf("%d\n", lines);

    fclose(fp);
    
    return 0;
} // end of main