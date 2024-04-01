# Networking 3

## Administratives  

>[!warning] Remember
>- **Labs:** SW6 & SW8
>- Bis 23. Mai: Jedes Chapter Exam auf netacad mit 85 % bestanden sein.

## MEP

Note:
- 5% Laborprotokolle
- 5% CCNA3 Netacad Module Exams
- 5% CCNA3 Final Exam (23. Mai)
- 85% elektronische Prüfung
	- open Book
	- Test Run am Donnerstag 23.Mai 2024


## Questions

1. **What is considered a best practice when configuring ACLs on vty lines?**
	1. Restrictions on all VTY lines
2. **When issuing commands:**
	1. They are added at the end of the existing ACL
3. **What conclusion can be drawn (packet count is shown)
	1. Router had not had any Telnet packets to \<IP>
4. **What does the CLI prompt look like with standard ACL**
	1. `Router(config-std-nacl)#`
5. **Limit Social Media access for local computers**
	1. Extended inbound on local-computers-interface of the router
 




## Terminologie

- **HMAC:** hashing message authentication code -> input wird zusammen mit einem key gehasht -> Origin authentication
- **Mitigation:** reduce risks
- **attack vector:** attacker abuses vulnerability
- **IOS (cisco version):** internet operating system



## ACL

### ACL Basics (Juicy part)

- **Standard ACL:** only filter at Layer 3 using the source IPv4 **only**. (used rarely)
- **Extended ACL:** filter at Layer 3 using source **and** destination IP. They can also filter at Layer 4 using TCP/UDP ports


What:
- applied sequentially
- Default: No ACL's configured
- List of **permit** or **deny** statements


Why:
- Limit network traffic (increase performance)
- Traffic flow control
- basic level of security

Where:
- In routers
- Where it has the greatest impact on efficiency
- Extended ACL: near source
- Standard ACL: near destination


Boundaries:
- max 4 ACL's/interface (IPv4 & IPv6 - both inbound or outbound)
- If match is found **rest of list is ignored**.
- Default: implicit deny every packet.
- Comment with `remark` 


Wildcard masks:
- reversed sub-net mask
- 0 - match
- 1 - ignore
- exact match to IP, mask must be: 0.0.0.0
- 24-subnet: 0.0.0.255


substitutions:
- **host** - equivalent to 0.0.0.0
- **any** - equivalent to 255.255.255.255


Guideline:
1. Write out what you want to do
2. Use a text editor to save all your ACLs
3. Document ACLs with `remark`
4. Test the ACLs on a development network

### Configurations


A rule must always be applied to an interface. Enter into the desired interface config and attach it.

```bash
interface G0/0/0
ip access-group {access-list-number | access-list-name} {in | out}
```


For viewing the configs:
```bash
show running-config # or
show ip interface g0/0/0
```


#### Numbered standard ACL syntax
```bash
access-list <access-list-number> {deny | permit | remark [text]} source [source-wildcard] [log]
```

- **log:** optional, generates and sends informational message when the ACE is matched

delete:
```bash
no access-list <access-list-number> # deletes an ACL
```


#### Named Standard ACL syntax

```bash
ip access-list standard <access-list-name> # enters config menu afterwards
```

>[!tip]
>Using capitalized names is not necessary, but it lets them stand out when printing running-config


Creating an extended named ACL example:

```bash
ip access-list extended FTP-Filter # enter ACL-conf mode
permit tcp 192.168.10.0 0.0.0.255 any eq ftp
permit tcp 192.168.10.0 0.0.0.255 any eq ftp-data
remark some comment text here
```



## NAT in  IPv4

- Inside address: local device
- outside: destination
- local - any IP address inside the local network
- global - any global address that appears outside of the local network


Meistens gebraucht:
 - Port Address Translation (PAT, Nat overload)


- Static: direct mapping from inside IP to outside IP
- Dynamic: dynamic port translation


### Important commands

[[SW03-NETW3-ENSA-6+8.pdf|SW03 slides]] p.25


## VPNs

- create private networks, end-to-end connections
- information within a private network over a public network
- encrypted traffic


Advantages:
- Cost savings
- security
- scalability - organizations can add new users easy
- Compatibility - gain secure access to corporate network


Types:
- site-to-site #TODO what?
- remote access


## Cloud computing

- SaaS
	- SAP
	- Office 365
- PaaS
	- Application
	- Data
- IaaS
	- Hetzner
	- Linode


AWS & Azure können alles sein

### Hypervisor Types

Type 1: 
- AWS
- Azure
- etc

Type 2:
- Has OS
- VirtualBox



### Virtualization

- **Software-Defined Networking (SDN)** - virtualises the network
- **OpenFlow** - manage traffic between routers, switches & AP
- **OpenStack** - virtualization _platform_ 
- **Orchestration** - is the process of automating the provisioning of network components


### Control Plane & Data Plane

Control plane (brains)
- used to make forwarding decisions
- layer 2 & 3 route forwarding mechanisms
- Processed by the CPU
- examples: routing protocol neighbour tables, topology tables, STP, ARP

Data plane (forwarding plane)
- Switch fabric connecting the various network ports on a device
- implements/executes forwarding
- Information processed by a special data plane processor (no CPU)


### SDN Types

- **(rarely) Device based SDN**
	- are programmable by applications running on the device itself
- **Controller-based SDN**
	- additional application layer
	- centralized controller, that has knowledge of all devices in the network
- **Policy-based SDN**
	- Similar to controller-based
	- additional policy layer that operates at a higher level of abstraction



### Spine-Leaf Topology

- Leaf switches: attach to the spines, never to each other
- Spine switches: only attach to the leaf and core switches
- APIC does not manipulate data path directly
- APIC centralizes the policy definition and programs the leaf switches


## Automation

- provide a a way to create failures faster
- robots: reduce risks to humans
- work 24/7
- collection of vast amounts of data to be quickly analyzed


## Software defined Networking (SDN)

Internet:
- Historisch implementiert als dezentraler Ansatz
- Verschiedene middleboxes


~2005:
- Interesse Network Control Plane neu überdenken -> SDN
- Trennung Control Plane und Data Plane
- Konsolidierung Control Plane: ein einziges Programm steuert mehrere Elemente

### Funktionsweise

Ursprünglich alle Switches:
- **Data-Plane:** Daten werden verbreitet (mit Nachschlagtabelle)
- **Control-Plane:** Kontrolliert wie Daten fliessen

**In SDN:** SDN Controller übernimmt komplette Control-Plane
- Steuert Switches mittels Protokolle wie OpenFlow
- Softwareinstanz auf Standardhardware
- Erlaub Programmierung


Netzwerkverkehr bestimmen: mit Routing-Tabellen
- load balancing nicht möglich
- Priorisierung nicht möglich

| SDN                                                                          | Klassisch                                                              |
| ---------------------------------------------------------------------------- | ---------------------------------------------------------------------- |
| Controller **nicht** in derselben Box wie die Forwarding Hardware            | Forwarding-Hardware und deren Steuerung befinden sich in derselben Box |
| **Zentralisierte Entscheidung** mit einer logisch globalen Sicht             | **Verteilter** Entscheidungs-/Routing-Algorithmus                      |
| Netzwerkfunktionen verwalten das globale Netzwerk                            | Netzwerkfunktionen müssen verteilt realisiert werden, fehleranfällig   |
| Für die zentralisierte Sicht müssen **neue Abstraktionen** entwickelt werden | Netzwerkabstraktionen sind in den verteilten Algorithmen inhärent      |
| **High-level Programmiersprache** zur Beschreibung der Netzwerkkonfiguration | Lokale Routing-Programmierung auf jedem einzelnen Switch               |


### OpenFlow

- Packet handling in Switches über _Regeln_
- Muster-basierter Abgleich der Header-Bits von Paketen
- Die Aktionen der Regeln bestimmen das Paketziel
	- Drop
	- Forward (destination & port)
	- Modify
	- Send to controller
- Zähler erfassen Statistiken wie die Anzahl der Bytes und Pakete

Regeln:

```txt
1: src=1.2.*.*, dest=3.4.5.* -> drop
2: src=*.*.*.*, dest=3.4.*.* -> forward(2)
3: src=10.1.2.3, dest=*.*.*.* -> send to controller
```



### OpenFlow Example

![[OpenFlow_Example.png]]