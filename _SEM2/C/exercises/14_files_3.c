#include <stdio.h>
#include <stdlib.h>
#include <strings.h>

int main(){
    char originalName[20] = "file.txt";
    FILE* fp = fopen(originalName, "r");
    char c;

    if(fp == NULL){
        return -1;
    }

    long pos;
    fseek(fp, -1, SEEK_END);
    pos = ftell(fp);
    while(pos >= 0){
        c = fgetc(fp);
        printf("%c", c);
        pos--;
        fseek(fp, pos, SEEK_SET);
    }
    printf("\n");
    fclose(fp);
    
    return 0;
} // end of main