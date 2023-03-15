//
// Created by dave on 14.03.23.
//

#include "file_handler.h"
#include <stdio.h>

int main(){

    FILE* fp = fopen("my_pw_manager.pmds", "wb");

    unsigned char buffer[5] = {0xFE, 0xFF, 0xEE, 0xAA, 0xBB};
    unsigned char inp[5] = {0};

    fwrite(buffer, sizeof(buffer), 1, fp);
    fclose(fp);

    fp = fopen("my_pw_manager.pmds", "rb");

    fread(inp, sizeof(inp), 1, fp);

    for (int i = 0; i < 5; ++i) {
        printf("%x ", inp[i]);
    }

    printf("\nFinished");
    return 0;
}