#include <pwd.h>
#include <unistd.h>
#include <stdio.h>
#include <stdlib.h>

int main(int argc, char **argv)
{
	struct passwd *pw = getpwuid(getuid());
 	if (pw == NULL)
  {
    perror("Error getting user info");
   	return 1;
  }
  int ret = setuid(0);
  if (ret != 0) 
  {
    perror("error while setting uid\n");
    return 1;
  }
  printf("id: %d\n", pw->pw_uid);
  printf("name: %s\n", pw->pw_name);
  printf("shell var: %s\n", getenv("USER"));
}
