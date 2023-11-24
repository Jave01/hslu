# Hydra
---
Typical tools besides hydra
-   `Ncrack`
-   `wfuzz`
-   `medusa`
-   `patator`


## Hydra
---
### Basic
Basic syntax for pre-built credential combinations:
```sh
hydra -C /path/to/list <ip> -s <port> http-get / 
```

Basic syntax for separate username & password combinations
```sh
hydra -L /path/to/unames -P /path/to/passwds <ip> -s <port> <method> / 
```

additional flags:

|flag|effect|
|-|-|
|-f|exit after valid combination found|
|-u|try every user with a password and then move on instead of the other way around|

Very simple  default credentials are in `SecList/Passwords/Default-Credentials/ftp-betterdefaultpasslist.txt` .


### Forms
---
For brute-forcing forms there exists a custom syntax for sending the credentials in the right format, which looks like follows:
```shell
"/login.php:[user parameter]=^USER^&[password parameter]=^PASS^:[FAIL/SUCCESS]=[success/failed string]"
```

Where FAIL/SUCCESS string is a string in the html form hydra is looking for, to determine if the credentials where valid. It can be set like this for FAIL:
```bash
[...]:F=<form name='login'"
```

The syntax for the post data can easily be read in the browser dev tools -> right click -> copy POST data.


### Service
---
can be attacked by specifying `service://SERVER_IP:PORT` at the end. i.e.
```shell-session
hydra -L bill.txt -P william.txt -u -f ssh://178.35.49.134:22 -t 4
```
add `-t 4` to limit the parallel processes to 4 (max for ssh) otherwise attempts may be dumped.

## Custom wordlists
---

| tool | pro/use case |
|-|-|
|`cupp`|easy to use -> create wordlist|
|[rsmangler](https://github.com/digininja/RSMangler)|fast permutation & combination of list|
|[The Mentalist](https://github.com/sc0tfree/mentalist.git)|fast permutation & combination of list|
|[Username Anarchy](https://github.com/urbanadventurer/username-anarchy)|create username list|


remove some passwords from list
```bash
sed -ri '/^.{,7}$/d' william.txt            # remove shorter than 8
sed -ri '/[!-/:-@\[-`\{-~]+/!d' william.txt # remove no special chars
sed -ri '/[0-9]+/!d' william.txt            # remove no numbers
```



