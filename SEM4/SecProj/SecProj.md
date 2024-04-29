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
>	- [ ] remove `create_users.sh` on ssh-root directory
>	- [ ] remove unused folder structure for project
>	- [ ] Extensive testing lol



>[!note]
>- Most of them not ssh (only celestials?)
>- First one only rbash



## Documentation

- Raw literature projects are 20-40 pages
- Pages are reduced based on amount of work put into practical part
- 5-7 pages about HackTheBox paths




## Steps / Solution

1. Hint to guest user in robots.txt
2. Anonymous ftp (hint to HEAD-request)
3. guest credentials in header for `login.php`
4. Find groots password in webserver content
5. privilege escalation Groot to Drax -> random script in shared folder
7. Drax password in bash_history - some false paths in drax home directory
8. From Drax to Rocket stenographic `wavsteg` 
9. From Rocket to Gamora with Cronjob - random bash script
10. From Gamora to Star-Lord with #TODO 
11. From Star-Lord reverse engineer password manager
12. Master password is `05448718094`.
13. EGO's password is stored in password manager
14. Elevate to EGO
15. EGO's favorite memory is a string appended to the image
16. https://www.hackingarticles.in/hack-the-box-challenge-crimestoppers-walkthrough/
17. EGO -> root with hint in encrypted Thunderbird file to SUID command which can be executed
18. Break out of Container with socket


Passwords:
- EGO: 1_4m_7h3_C3n73r_0f_7h3_Un1v3r53