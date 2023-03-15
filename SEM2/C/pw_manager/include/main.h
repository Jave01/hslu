#ifndef PW_MANAGER_MAIN_H
#define PW_MANAGER_MAIN_H

/**********************************************************************************************
 * Includes 
 **********************************************************************************************/
#include <stdbool.h>
#include <stdlib.h>
#include "file_handler.h"


/**********************************************************************************************
 * Types
 **********************************************************************************************/
typedef struct pw_list
{
    char* filename;     //!< Name of password file
    FILE* file;         //!< File pointer to the original file
    char* master_pw;    //!< Master password from this file
    int entry_count;    //!< Number of entries in pw file including master
}pw_list_t;


/**********************************************************************************************
 * Function Headers 
 **********************************************************************************************/
int get_entry_count(FILE* fp);
bool set_master_pw(char *content);
char* get_entry(FILE* fp, char* str, char* key);
bool set_entry(pw_list_t* pw_list, char* key, char* val);



#endif /* PW_MANAGER_MAIN_H */