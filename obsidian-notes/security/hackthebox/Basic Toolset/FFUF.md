# FFUF
---
The term `fuzzing` refers to a testing technique that sends various types of user input to a certain interface to study how it would react.

## General
---
Some wordlists are under `SecLists/Discovery/Web-Content.`
Common used configuration flags:
```shell-session
HTTP OPTIONS:
  -H               Header `"Name: Value"`, separated by colon. Multiple -H flags are accepted.
  -X               HTTP method to use (default: GET)
  -b               Cookie data `"NAME1=VALUE1; NAME2=VALUE2"` for copy as curl functionality.
  -d               POST data
  -recursion       Scan recursively. Only FUZZ keyword is supported, and URL (-u) has to end in it. (default: false)
  -recursion-depth Maximum recursion depth. (default: 0)
  -u               Target URL
...SNIP...

MATCHER OPTIONS:
  -mc              Match HTTP status codes, or "all" for everything. (default: 200,204,301,302,307,401,403)
  -ms              Match HTTP response size
...SNIP...

FILTER OPTIONS:
  -fc              Filter HTTP status codes from response. Comma separated list of codes and ranges
  -fs              Filter HTTP response size. Comma separated list of sizes and ranges
...SNIP...

INPUT OPTIONS:
...SNIP...
  -w               Wordlist file path and (optional) keyword separated by colon. eg. '/path/to/wordlist:KEYWORD'

OUTPUT OPTIONS:
  -o               Write output to file
...SNIP...

EXAMPLE USAGE:
  Fuzz file paths from wordlist.txt, match all responses but filter out those with content-size 42.
  Colored, verbose output.
    ffuf -w wordlist.txt -u https://example.org/FUZZ -mc all -fs 42 -c -v
...SNIP...
```


## Directory Fuzzing
---
After specifying the wordlist we can reference it with a defined keyword to define where we want to fuzz. For example:
```shell-session
ffuf -w /my/wordlist.txt:FUZZ -u http://SERVER_IP:PORT/FUZZ
```
We "named" our wordlist `FUZZ` and referenced it in the specified URL.
Example list: `SecList/Discovery/Web-Content/xyz.txt`

## Page fuzzing
---
Determine which file extension the website uses to render its pages. One way is to look at the server type in the html header and guessing it. For example apache server probably uses .php or IIS probably uses .asp or .aspx.
OR
We fuzz it.
Almost same command like in[[FFUF#Directory Fuzzing| Directory Fuzzing]] but instead of putting the reference where the page name would be, place it where the extension would be:
```shell-session
ffuf -w /my/wordlist.txt:FUZZ -u http://SERVER_IP:PORT/index.FUZZ
```
Example wordlist for extensions can be: `SecLists/Discovery/Web-Content/web-extensions.txt`
In this wordlists the dots before the extensions are already there, so don't put one before the reference in the URL. It is also possible to use two wordlists for scanning multiple things at the same time like the directories and extensions. Or you just use `indexFUZZ`.

>[!tip]
>The wordlist like contains already a dot which means you have to write `FUZZ` after `index` without the dot.


## Recursive Fuzzing
---
Can be done with `-recursion`.
Always add a depth when fuzzing recursively with `-recursion-depth`. The extension can be specified with `-e`. For printing full URL use `-v`.

## Sub-domain fuzzing
---
Useful wordlists can be found in `SecLists/Discover/DNS` e.g. `sub-domains-top1million-5000.txt`. Which is a rather short list.
Command is similar as the ones before just place the reference at the right place (e.g. `http://FUZZ.domain.com`).
However this only works if the subdomain is listed in a DNS servers

## VHost fuzzing
---
VHost is basically a sub-domain on the same IP address as the main sub-domain. For fuzzing VHosts we actually fuzz the http header. This can be done like this:
```shell-session
ffuf -w /my/list:FUZZ -u http://academy.htb:PORT/ -H 'Host: FUZZ.academy.htb'
```
this way we will always get a 200 response because the only the header is changed. But if the site exists and is public the size of the return value changes.

>[!tip]
> For filtering the responses you can use filter (`-fx`) or match (`-mx`) values.
> And you have to add the topdomain to `/etc/hosts`!


## Parameter fuzzing
---
For fuzzing GET parameters just insert FUZZ in the URL like this:
```shell-session
-u http://admin.academy.htb:PORT/admin/admin.php?FUZZ=key
```
a possible wordlist for that could be:
```shell-session
SecLists/Discovery/Web-Content/burp-parameter-names.txt
```

>[!note]
>For fuzzing POST parameters you have to pass the list in the data (`-d`) field and set `Content Type` in the header (`-H`) to `Content-Type: application/x-www-form-urlencoded`.

A possible fuzzing command may then look like this:
```shell-session
ffuf -w [...]/burp-parameter-names.txt:FUZZ -u http://example.com:PORT/site.php -X POST -d 'FUZZ=key' -H 'Content-Type: application/x-www-form-urlencoded' -fs xxx
```
 and afterwards get the content with curl:
 ```shell-session
curl http://admin.academy.htb:PORT/admin/admin.php -X POST -d 'id=key' -H 'Content-Type: application/x-www-form-urlencoded'
```
