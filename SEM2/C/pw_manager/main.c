/* Standard libraries */
#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <string.h>
#include "file_handler.h"

/* Custom imports */
#include "main.h"




int main()
{








// ---------------------------------------------------------
//    FILE* fpw = fopen("pw.txt", "r");
//
//    if(fpw == NULL){
//        /* writing to stderr if file is invalid */
//        perror("Error opening file");
//        return(-1);
//    }
//
//    pw_list_t passwds = {
//        .file = fpw,
//        .entry_count = get_entry_count(passwds.file),
//        .filename = "pw.txt"
//    };

    // char* master = malloc(MAX_VAL_LEN * sizeof(char));
    // get_entry(passwds.file, master, "master");
    // bool pw_valid = false;
    // char* inp;
    // char* inp2;
    // while (!pw_valid)
    // {
    //     if (master == NULL)
    //     {
    //         printf("Please define master password\n> ");
    //         scanf("%s", inp);
    //         printf("repeat\n> ");
    //         scanf("%s", inp2);
    //         if (inp != inp2)
    //         {

    //         }else{

    //             printf("Master password successfully set.\n");
    //             break;
    //         }
    //     }
    // }
    // #endregion


    // printf("s:%s\n", val);

//    set_entry(&passwds, "key4", "val4");
//
//    fclose(passwds.file);
//    passwds.file = NULL;
    return 0;
}


/** 
 * Returns the number of lines which aren't empty.
 * @param fp pointer to the file
 * @return the number of lines
*/
int get_entry_count(FILE* fp){
    int lines=0;

    rewind(fp);

    char buffer[MAX_LINE_LEN] = {0};
    while (fgets(buffer, MAX_LINE_LEN, fp) != NULL) {
        if (strcmp(buffer, "\n") != 0) // check if line is empty
        {
            lines++;
        }
    }
    rewind(fp);

    return lines;
}


/** Returns the value of the entry based on the key.
 * Writes the string into the str variable and returns the pointer to the value.
 * @param fp  pointer to file
 * @param str destination where the value gets written into
 * @param key key to search for in the file
 * @return pointer to the value string
*/
char* get_entry(FILE* fp, char* str, char* key){
    int range = get_entry_count(fp);

    rewind(fp);

    if (range < 1)
    {
        printf("Not enough entries (%d)", range);
    }

    char c;
    for (int i = 0; i < range; i++)
    {
        for (int j = 0; j <= strlen(key); j++)
        {
            c = fgetc(fp);
            if (c != key[j])
            {
                if (c == ':'){
                    return fgets(str, MAX_VAL_LEN, fp);
                }
                else{
                    break;
                }
            }
        }
        while ((c = fgetc(fp)) != '\n' && c != EOF);// go to next entry
    }
    rewind(fp);
    return NULL;
}


/** Change or create entry in pw list.
 * 
*/
bool set_entry(pw_list_t* pw_list, char* key, char* val){
    if (strlen(key) > MAX_KEY_LEN || strlen(key) < 1)
    {
        printf("Key size invalid (%ld/%d chars)\n", strlen(key), MAX_KEY_LEN);
        return false;
    } else if (strlen(val) > MAX_VAL_LEN){
        printf("Value size invalid (%ld/%d chars)\n", strlen(val), MAX_VAL_LEN);
        return false;
    }

    char* entry_value = malloc(MAX_LINE_LEN * sizeof(char));
    char* entry_p = get_entry(pw_list->file, entry_value, key);
    free(entry_value);

    /* create new entry_value if it doesn't exist */
    if (entry_p == NULL)
    {
        fclose(pw_list->file);
        pw_list->file = fopen(pw_list->filename, "a");

        /* insert newline if there isn't one */
        fseek(pw_list->file, -1, SEEK_END);
        char c = fgetc(pw_list->file);
        if ((c) != '\n'){
            fputc('\n', pw_list->file);
        }

        /* add content */
        fputs(key, pw_list->file);
        fputc(':', pw_list->file);
        fputs(val, pw_list->file);

        fclose(pw_list->file);

        pw_list->file = fopen(pw_list->filename, "r");
        pw_list->entry_count++;

        printf("added content successfully\n");

        return true;

    } else {
        /* copy content */
        FILE* ftemp = fopen("temp.txt", "w");

        char c;
        while ((c = fgetc(pw_list->file)) != EOF)
        {
            fputc(c, ftemp);
        }
        fseek(pw_list->file, -1, SEEK_END);

        if (fgetc(pw_list->file) != '\n'){
            fputc('\n', ftemp);
        }

        /* replace old file */
        fclose(pw_list->file);
        remove(pw_list->filename);
        rename("temp.txt", pw_list->filename);
    }
}