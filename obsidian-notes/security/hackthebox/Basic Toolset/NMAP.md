# NMAP

## Host Discovery

General command syntax
```shell-session
nmap <scan types> <options> <target>
```


### Host discovery
---
We can discover hosts with sending an ICMP echo request, works only if firewall allows it and the hosts respond to an echo request.
```shell-session
sudo nmap <network-ip>/<range> -sn -oA tnet
```
|Scanning options|Description|
|-|-|
|-sn|Disable port scanning|
|-oA tnet|Stores the results in all formats starting with the name `tnet`|
|-iL \<listfile\>| uses all the ip's in the list rather than manually typing the range|
|-PE| (default if -sn is not specified) performs ICMP echo requests|
|--disable-arp-ping| self explanatory |

More strategies on [https://nmap.org/book/host-discovery-strategies.html](https://nmap.org/book/host-discovery-strategies.html)

>[!tip]
> by looking at the `ttl` the host OS can possibly be determined


### Host and Port scanning
---
Information we can get from a specific port from a host:
|**State**|**Description**|
|-|-|
|`open`|This indicates that the connection to the scanned port has been established. These connections can be **TCP connections**, **UDP datagrams** as well as **SCTP associations**.|
|`closed`|When the port is shown as closed, the TCP protocol indicates that the packet we received back contains an `RST` flag. This scanning method can also be used to determine if our target is alive or not.|
|`filtered`|Nmap cannot correctly identify whether the scanned port is open or closed because either no response is returned from the target for the port or we get an error code from the target.
|`unfiltered`|This state of a port only occurs during the **TCP-ACK** scan and means that the port is accessible, but it cannot be determined whether it is open or closed.|
|`open` \|`filtered`|If we do not get a response for a specific port, `Nmap` will set it to that state. This indicates that a firewall or packet filter may protect the port.|
|`closed` \| `filtered`|This state only occurs in the **IP ID idle** scans and indicates that it was impossible to determine if the scanned port is closed or filtered by a firewall.|


### Discovering open TCP ports
---
The option `-sS` scans the top 1000 ports with the `SYN` TCP packet. It is default as long as we execute the command as root, because of the socket permission required to create TCP packets. Otherwise the `-sT` option is default, which sets the port and scanning options automatically. 
If we manually define ports we can do it one by one with `-p 21,22,23,80,445`, by range `-p 23-450`, by top ports `--top-ports=10` from the `Nmap` database which are the most common used ports, by scanning all ports `-p-`  or by doing a fast scan `-F` which scans the top 100 ports. With `-Pn` ICMP echo requests are not sent. And `-n` disables DNS resolution.

### Trace the packets
---
With `--packet-trace` the individual sent packets will be displayed. The output could look something like this:
```shell-session
sudo nmap 10.129.2.28 -p 21 --packet-trace -Pn -n --disable-arp-ping

Starting Nmap 7.80 ( https://nmap.org ) at 2020-06-15 15:39 CEST
SENT (0.0429s) TCP 10.10.14.2:63090 > 10.129.2.28:21 S ttl=56 id=57322 iplen=44  seq=1699105818 win=1024 <mss 1460>
RCVD (0.0573s) TCP 10.129.2.28:21 > 10.10.14.2:63090 RA ttl=64 id=0 iplen=40  seq=0 win=0
Nmap scan report for 10.11.1.28
Host is up (0.014s latency).

PORT   STATE  SERVICE
21/tcp closed ftp
MAC Address: DE:AD:00:00:BE:EF (Intel Corporate)

Nmap done: 1 IP address (1 host up) scanned in 0.07 seconds
```

#### Options
|**Scanning Options**|**Description**|
|-|-|
|`10.129.2.28`|Scans the specified target.|
|`-p 21`|Scans only the specified port.|
|`--packet-trace`|Shows all packets sent and received.|
|`-n`|Disables DNS resolution.|
|`--disable-arp-ping`|Disables ARP ping.|

#### Request
|**Message**|**Description**|
|-|-|
|`SENT (0.0429s)`|Indicates the SENT operation of Nmap, which sends a packet to the target.|
|`TCP`|Shows the protocol that is being used to interact with the target port.|
|`10.10.14.2:63090 >`|Represents our IPv4 address and the source port, which will be used by Nmap to send the packets.|
|`10.129.2.28:21`|Shows the target IPv4 address and the target port.|
|`S`|Indicates that the SYN flag of the sent TCP packet is used.|
|`ttl=56 id=57322 iplen=44 seq=1699105818 win=1024 mss 1460`|Additional TCP Header parameters.|

#### Response
|**Message**|**Description**|
|-|-|
|`RCVD (0.0573s)`|Indicates a received packet from the target.|
|`TCP`|Shows the protocol that is being used.|
|`10.129.2.28:21 >`|Represents targets IPv4 address and the source port, which will be used to reply.|
|`10.10.14.2:63090`|Shows our IPv4 address and the port that will be replied to.|
|`RA`|RST and ACK flags of the sent TCP packet.|
|`ttl=64 id=0 iplen=40 seq=0 win=0`|Additional TCP Header parameters.|


#### Stealth scan
---
Connect scan performs a three-way handshake (`-sT`) and is considered more "stealthy", because Intrusion Detection Systems are less likely to raise an alert if no ports are left open. Depending on the firewall or business of the target it might take some time for a response in which case the `-Pn` option might miss the response. Also the TCP-ACK scan (`-sA`) sends only ACK packets, which only receives RST packets by open and closed ports and the traffic is also much harder to filter for an IDS. All connection attempts (SYN) are usually blocked by the firewall.


#### Service scan
---
`-sV` together with `--reason` gives very detailed information about a target and its services. Output can look like this (with optional `--packet-trace`):
```shell-session
sudo nmap 10.129.2.28 -Pn -n --disable-arp-ping --packet-trace -p 445 --reason  -sV

Starting Nmap 7.80 ( https://nmap.org ) at 2022-11-04 11:10 GMT
SENT (0.3426s) TCP 10.10.14.2:44641 > 10.129.2.28:445 S ttl=55 id=43401 iplen=44  seq=3589068008 win=1024 <mss 1460>
RCVD (0.3556s) TCP 10.129.2.28:445 > 10.10.14.2:44641 SA ttl=63 id=0 iplen=44  seq=2881527699 win=29200 <mss 1337>
NSOCK INFO [0.4980s] nsock_iod_new2(): nsock_iod_new (IOD #1)
NSOCK INFO [0.4980s] nsock_connect_tcp(): TCP connection requested to 10.129.2.28:445 (IOD #1) EID 8
NSOCK INFO [0.5130s] nsock_trace_handler_callback(): Callback: CONNECT SUCCESS for EID 8 [10.129.2.28:445]
Service scan sending probe NULL to 10.129.2.28:445 (tcp)
NSOCK INFO [0.5130s] nsock_read(): Read request from IOD #1 [10.129.2.28:445] (timeout: 6000ms) EID 18
NSOCK INFO [6.5190s] nsock_trace_handler_callback(): Callback: READ TIMEOUT for EID 18 [10.129.2.28:445]
Service scan sending probe SMBProgNeg to 10.129.2.28:445 (tcp)
NSOCK INFO [6.5190s] nsock_write(): Write request for 168 bytes to IOD #1 EID 27 [10.129.2.28:445]
NSOCK INFO [6.5190s] nsock_read(): Read request from IOD #1 [10.129.2.28:445] (timeout: 5000ms) EID 34
NSOCK INFO [6.5190s] nsock_trace_handler_callback(): Callback: WRITE SUCCESS for EID 27 [10.129.2.28:445]
NSOCK INFO [6.5320s] nsock_trace_handler_callback(): Callback: READ SUCCESS for EID 34 [10.129.2.28:445] (135 bytes)
Service scan match (Probe SMBProgNeg matched with SMBProgNeg line 13836): 10.129.2.28:445 is netbios-ssn.  Version: |Samba smbd|3.X - 4.X|workgroup: WORKGROUP|
NSOCK INFO [6.5320s] nsock_iod_delete(): nsock_iod_delete (IOD #1)
Nmap scan report for 10.129.2.28
Host is up, received user-set (0.013s latency).

PORT    STATE SERVICE     REASON         VERSION
445/tcp open  netbios-ssn syn-ack ttl 63 Samba smbd 3.X - 4.X (workgroup: WORKGROUP)
Service Info: Host: Ubuntu

Service detection performed. Please report any incorrect results at https://nmap.org/submit/ .
Nmap done: 1 IP address (1 host up) scanned in 6.55 seconds
```

If you want to get the name of a host you can use:
```sh
sudo nmap -sV -F -n -Pn [ip] --disable-arp-ping --reason
```


### Saving the Results
---
Results can be saved in three different formats
- Normal output (`-oN`) with the .nmap extension
- Grepable output (`-oG`) with the .gnmap extension
- XML output (`-oX`) with the .xml extension
- All three (`-oA`)

The XML file can be converted to a html file with the `xsltproc` command
```sh
xsltproc target.xml -o target.html
```


### Service enumeration
---
while doing a service scan you can get mid scan output when pressing space bar or defining `--stats-every=42s`.
With the verbosity level (`-v` / `-vv`) open ports will be displayed as soon as nmap detected them.

Sometimes nmap shows not all information because it didn't know how to handle it. E.g the banner of a server. To see more you can use `tcpdump` and `nc`.


## NMAP Scripting Engine (NSE)
---
Default scripts:
```shell-session
sudo nmap <target> -sC
```

Specific Scripts Category
```shell-session
sudo nmap <target> --script <category>
```

Defined Scripts:
```shell-session
sudo nmap <target> --script <script-name>,<script-name>,...
```


There are following categories of scripts:
|**Category**|**Description**|
|-|-|
|`auth`|Determination of authentication credentials.|
|`broadcast`|Scripts, which are used for host discovery by broadcasting and the discovered hosts, can be automatically added to the remaining scans.|
|`brute`|Executes scripts that try to log in to the respective service by brute-forcing with credentials.|
|`default`|Default scripts executed by using the `-sC` option.|
|`discovery`|Evaluation of accessible services.|
|`dos`|These scripts are used to check services for denial of service vulnerabilities and are used less as it harms the services.|
|`exploit`|This category of scripts tries to exploit known vulnerabilities for the scanned port.|
|`external`|Scripts that use external services for further processing.|
|`fuzzer`|This uses scripts to identify vulnerabilities and unexpected packet handling by sending different fields, which can take much time.|
|`intrusive`|Intrusive scripts that could negatively affect the target system.|
|`malware`|Checks if some malware infects the target system.|
|`safe`|Defensive scripts that do not perform intrusive and destructive access.|
|`version`|Extension for service detection.|
|`vuln`|Identification of specific vulnerabilities.|


With the aggressive option/aggressive scan (`-A`) the scan will be executed with service detection (`-sV`), OS detection (`-O`), traceroute (`--traceroute`) and with the default NSE scripts (`-sC`)


## Performance
---
There are some options to speed up a process. For example:
|command|effect|
|-|-|
|`--max-rtt-timeout`|maximum time to wait for a response, default is 100ms. There is alsow the `--initial-rtt-timeout` option.|
|`--max-retries`|how many times a retry should be made if no response is received, default is 10|
|`--min-rate`|Nmap tries to keep up this packet rate, sends packets simultaneously|

These speed-up options can have negative effects on the output result, e.g. not all hosts/ports are discovered or an alarm is raised on the destination network for to much traffic.

There are also six different timing templates (`-T`), default is `-T 3`.
|Option|description|
|-|-|
|`-T 0`|paranoid|
|`-T 1`|sneaky|
|`-T 2`|polite|
|`-T 3`|normal|
|`-T 4`|aggressive|
|`-T 5`|insane|

Some of the template options can also be set manually, the exact values can be seen [here]([https://nmap.org/book/performance-timing-templates.html](https://nmap.org/book/performance-timing-templates.html)).

## Firewall and IDS/IPS Evasion
---
Intrusion Detection System (IDS) scans the network for potential attacks and Intrusion Prevention System (IPS) complements IDS by taking specific defensive measures if a potential attack is detected.

### Detect IDS/IPS
when testing if an IDS is present, various VPN with different IP addresses are recommended, because network traffic from a specific IP address will probably get blocked.

- **IDS:** By aggressively scanning a port and its service a potential alarm can be raised for detecting if an IDS is present on the destination network.
- **IPS:** If we scan from a single host and this host gets blocked at any time, we know that the administrator has taken some security measures.

### Decoys
With `-D` decoy packets can be generated which generates random IP addresses. The amount of addresses can be specified with `RND:<num>`. Our real IP address is then randomly placed between the generated IP addresses.

Such a scan with 5 generated IP addresses can look like this:
```shell-session
sudo nmap 10.129.2.28 -p 80 -sS -Pn -n --disable-arp-ping --packet-trace -D RND:5
```

maybe a specific subnet is blocked from accessing the server. In this case a different source IP address can be used with `-S`.

### DNS Proxying
If we are in a DMZ it can be usefull to set the source port (`--source-port`) to 53. If the IDS isn't configured correctly by the administrator a DNS server inside the DMZ is trusted.

If we receive a non-filtered response from the client it is likely that the IDS/IPS filters are configured  weakly. We can test this by connecting to the target with netcat.
```shell-session
ncat -nv --source-port 53 <ip> <filtered-port>
```

