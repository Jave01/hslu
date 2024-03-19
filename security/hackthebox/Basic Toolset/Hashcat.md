# Hashcat
---
## Questions
---
- Responder tool?
- SMB Relay
- Sentence: They have been unable to perform a successful `SMB Relay` attack, so they need to obtain the cleartext password to gain a foothold in the Active Directory environment
- Kerberoasting attack

## General
---
For determine the used hash algorithm the tool `hashid` can be used. With `-m` the Hashcat id is returned besides the algorithm name. 

If `hashid` couldn't determine the hash definitely the example hashes from `hashcat` can help, with the narrowed down options from `hashid`.

The list of hashes and the examples can be found [here](https://hashcat.net/wiki/doku.php?id=example_hashes)
Or via `hashcat --example-hashes (| less)`.

General usage:
```shell-session
hashcat -a <attack-mode> -m <hash-type> <hash-file> <wordlist-file> -O
```

 ```
  # | Mode
  ===+======
  0 | Straight
  1 | Combination
  3 | Brute-force
  6 | Hybrid Wordlist + Mask
  7 | Hybrid Mask + Wordlist
  9 | Association
```



## Optimisations
---
- `-O` - enable optimised kernels (huge performance difference - max password length: 32)
- `-w` - enable a specific workload profile (1: use computer while hashcat is running, 3: only hashcat, no clue what 2 is but its the default.)


## Wordlists (`-a 0/1`)
---
With the option `-a 0` you put the path to one wordlist at the end. 
With `-a 1` you can give hashcat two wordlist and it will combine the individual words.


## Mask attack (`-a 3`)
---
A mask can be created using static characters, ranges of characters or numbers e.g. \[A-Z0-9\] or placeholders. Some placeholders are:

|**Placeholder**|**Meaning**|
|---|---|
|?l|lower-case ASCII letters (a-z)|
|?u|upper-case ASCII letters (A-Z)|
|?d|digits (0-9)|
|?h|0123456789abcdef|
|?H|0123456789ABCDEF|
|?s|special characters
| ?a | ?l?u?d?s|
|?b|0x00 - 0xff|


## Hybrid attacks (`-a 6/7`)
---
With hybrid attacks you can take the words of a wordlist an either append (`-a 6`) or prepend 
(`-a 7`) characters based on a mask. E.g.:
```shell-session
 hashcat -a 6 -m 0 hybrid_hash rockyou.txt '?d?s'
```

adds a digit and a special character to every word in `rockyou.txt`.


## Custom Wordlists
---

|tool|short description|
|-|-|
|`crunch`|good for known password structures like length and pattern|
|`cupp`|easy to use with graphical interface based on keywords|
|[kwprocessor](https://github.com/hashcat/kwprocessor)|tries to guess keyboard walks|
|[PRINCE](https://github.com/hashcat/princeprocessor)|takes in a wordlists and creates a chain of them|
|[CeWL](https://github.com/digininja/CeWL)|scrapes website and generates list based on the keywords found there|


General syntax for the commands
```shell-session
$ cewl -d <depth to spider> -m <minimum word length> -w <output wordlist> <url of website>
```


## Rules
---
Chasch richtig weirde stuff mache. Isch wie masks eifach komplexer. Meh infos [hie](https://hashcat.net/wiki/doku.php?id=rule_based_attack#implemented_compatible_functions).
Some default rules are in `/usr/share/hashcat/rules/`.
Also some nice rules are  [nsa-rules](https://github.com/NSAKEY/nsa-rules), [Hob0Rules](https://github.com/praetorian-code/Hob0Rules), and the [corporate.rule](https://github.com/sparcflow/HackLikeALegend/blob/master/old/chap3/corporate.rule).  

Also a good way is to create a rule file containing a string like this:
```shell-session
echo 'c so0 si1 se3 ss5 sa@ $2 $0 $1 $9' > rule.txt
```
and include it in the command with `-r`.
E.g.:
```shell-session
hashcat -a 0 -m 100 hash /path/rockyou.txt -r rule.txt
```


## Linux /etc/shadow
---
Every line looks like this:
```shell-session
root:$6$tOA0cyybhb/Hr7DN$htr2vffCWiPGnyFOicJiXJVMbk1muPORR.eRGYfBYUnNPUjWABGPFiphjIjJC5xPfFUASIbVKDAHS3vTW1qU.1:18285:0:99999:7:::
```

The line contains nine fields separated by colons. First the username and then the hash of the corresponding password. After that various attributes are saved, like creation time, last change time and expiry.
The hash itself is split into 3 parts as well, delimited by `$`.
- **1.** - The number value determines the hashing algorithm, 6 stands for SHA-512.
- **2.** - (16 bytes) The salt of the hash.
- **3.** - The actual hash.


## Cracking Miscellaneous stuff
---
You first need to extract the hash string with tools like john the ripper.
The github repo:
```shell-session
https://github.com/magnumripper/JohnTheRipper.git
```

A lot of extracting tools in python are [here](https://github.com/openwall/john/tree/bleeding-jumbo/run).

You can then use tools like office2john.py (some are python scripts, some are binary commands)


## Cracking MIC (Message Integrity Check)
---
Cracking the hash from a 4-way handshake in a connection attempt to an Wireless Access Point.
[Airodump-ng](https://www.aircrack-ng.org/doku.php?id=airodump-ng) is a tool for capturing these handshakes.

>[!warning] Deprecated (use the new method below)
>For extracting the hash from the handshake use the online service [cap2hashcat online](https://hashcat.net/cap2hashcat) or the `hc-utils` tool from the github repository:
>```shell-session
>git clone https://github.com/hashcat/hashcat-utils.git
>cd hashcat-utils/src
>make
>```
>
>then you can use
>```sh
./cap2hccapx.bin <input.cap> <output.hccapx>
>```
>


Use the `hcxpcapngtool` tool (can be installed with package manager)
```bash
hcxpcapngtool network.cap -o hashes
```

more info [here](https://hashcat.net/forum/thread-10253.html) also about capturing the handshakes.

>[!tip]
>Hashcat mode is `22000` for the new format files.
 



## Cracking PMKID
---
PMKID -> pairwise master key identifier
Can be used against access points which use WPA/WPA2-PSK (pre-shared key)
I don't really know how this PMK protocol / process works, but apparently you don't need to `deauth` a client for capturing the necessary data.

>[!warning] Deprecated
>For extracting a PMKID hash from the cap file you can use [hcxtools](https://github.com/ZerBea/hcxtools).
>Syntax is:
>```shell-session
>hcxpcaptool -z <output_file> <input_file.cap>
>```
>Hashcat mode is `16800`.

>[!info] New way
>Use the same tool and hashcat mode as in MIC
