# Secure Project

## Remember before deploying

>[!danger] TODO before deploying
>- [ ] On host-server
>	- [ ] change root password
>	- [ ] disable ssh for non-intended users (test it)
>	- [ ] root user ssh only with RSA key possible
>	- [ ] delete all unused users
>	- [ ] change all users passwords
>- [ ] On VM/Docker image
>	- [ ] change root password
>	- [ ] delete all unused users
>	- [ ] change all users passwords
>	- [ ] use privilege escalation script
>	- [ ] delete escalation script
>	- [ ] Delete unused ftp files
>	- [ ] Delete unused home files
>	- [ ] Check (sudo) permissions for each user
>	- [ ] Extensive testing lol



## Documentation

- Raw literature projects are 20-40 pages
- Pages are reduced based on amount of work put into practical part
- 5-7 pages about HackTheBox paths




## Steps / Solution

1. Hint to guest user in robots.txt
2. Anonymous ftp (hint to HEAD-request)
3. guest credentials
4. (some privilege escalation to drax - maybe with python script)
5. drax password in bash_history - some false paths in drax home directory