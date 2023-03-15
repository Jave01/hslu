#include <stdio.h>
#include <stdlib.h>
#include <strings.h>

int main(){
    char originalName[20] = "file.txt";
    char tempName[20] = "temp.txt";
    FILE* fOriginal = fopen(originalName, "r");
    FILE* fTemp = fopen(tempName, "w");
    char c;

    if(fOriginal == NULL || fTemp == NULL){
        return -1;
    }


    while((c = fgetc(fOriginal)) != EOF){
        if (islower(c))
        {
            fputc(c - 32, fTemp);
        } else{
            fputc(c, fTemp);
        }
    }

    fclose(fOriginal);
    remove(originalName);
    
    fclose(fTemp);
    rename(tempName, originalName);
    
    return 0;
} // end of main