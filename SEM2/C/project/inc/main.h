#ifndef _MAIN_H
#define _MAIN_H

/**********************************************************************************************
 * Includes 
 **********************************************************************************************/
#include <stdbool.h>
#include <stdlib.h>


/**********************************************************************************************
 * Constants 
 **********************************************************************************************/
#define MAX_KEY_LEN     30
#define MAX_VAL_LEN     50
#define MAX_LINE_LEN    MAX_KEY_LEN + MAX_VAL_LEN + 2 //!< Key + Value + ':' + '\n'


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



#endif /* _MAIN_H */