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

#TODO

| Begriff                 | Definition                                                                                                |
| ----------------------- | --------------------------------------------------------------------------------------------------------- |
| **HMAC:** <br><br>      | hashing message authentication code -> input wird zusammen mit einem key gehasht -> Origin authentication |
| **Mitigation:** <br>    | reduce risks                                                                                              |
| **attack vector**       | attacker abuses vulnerability                                                                             |
| **IOS (cisco version)** | internet operating system                                                                                 |
|                         |                                                                                                           |
| **LSA**                 |                                                                                                           |
| **LSR**                 |                                                                                                           |
| **LSU**                 |                                                                                                           |
| **OSPF**                | ... Basically Dijkstra on a network                                                                       |
|                         |                                                                                                           |


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



## OSPFv2 Basics

- Link-state routing protocol
	- Link
		- Interface on a router
		- Network segment
		- Stub network
	- Link-state
		- Network prefix, prefix length and cost
		- **The same on all routers**
	- Areas
		- Division of the routing domain to help control routing update traffic
		- see [[#Areas]]
	- Alternative for RIP (Routing Information Protocol)
		- Faster convergence
		- Scaling to larger network implementations



### Databases and Tables

| Database                   | Table          | Description  |
| -------------------------- | -------------- | ------------ |
| Adjacency Database         | Neighbor Table | #TODO unique |
| Link-state Database (LSDB) | Topology Table | Global       |
| Forwarding Database        | Routing Table  |              |




### Process

1. Router gathers information about routers it can reach -> into Neighbor Table
2. Router build the Topology Table with all routers in OSPF Area
3. Router performs Dijkstra Shortes-Path First (SPF) algorithm on the routers in the Topology Table



### Areas

group of routers that share the same link-state information in their LSDB's.

OSPF can be implemented as
- Single-Area OSPF
- Multiarea
	- Multiple ares, **hirarchical**
	- all areas must connect to backbone area (area 0)
	- Adv.
		- Smaller routing tables
		- reduced link-state update overhead
		- Reduced frequency of SPF calculations



### OSPFv3

- Is meant for IPv6
- But includes support for both IPv4 & IPv6
- Same functionality as OSPFv2
- Also used SPF algorithm
- Has separate processes from OSPFv2


### OSPF Packets

- 1. **Hello packet**
	- Discover OSPF neighbors and establish neighbor adjacencies.
	- advertise parameters on which two routers must agree to become neighbors
		- Area ID
		- Auth-Type
		- Network Mask
		- Hello Interval
		- Dead Interval
	- Elect the **Designated Router** (DR) and **Backup Designated Router** (BDR) on multi-access networks like Ethernet.
		- Not required in Point-to-point links
- 2. **Database Description (DBD)**
- 3. Something
- 4. **Link-State Update (LSU)**
	- Include OSPF routing updates
	- 11 different types of OSPFv2 Link-State Advertisements (LSAs)
	- LSU and LSA are often used interchangeably, but the correct hierarchy is LSU packets contain LSA messages.
- 5. Something



### Designated Router

**Issues**:
- Creation of multiple adjacencies on shared media.
- Extensive flooding of LSAs (OSPF init, topology change)


**Solution**:
- BDR also elected (in case the DR fails)
- All other routers become DROTHER
- DR only used for the dissemination of LSAs. It still uses best next-hop router for packet routing.



## OSPFv2: Additional Concepts

- Router ID
	- 32-bit value represented as an IPv4 address
	- uniquely identifies an OSPF router
	- outer with the highest router ID will be the master in the adjacency -> sends their database descriptor (DBD) packets first (Exchange State)
	- Used for the DR and BDR election
	- Can be assigned manually or automatically
	- Can be a loopback interface


![[SW07-NETW3-ENSA-1+2.pdf#page=22]]


### Questions

- 15 - What will an OSPF router prefer to use first
- 22 - Default OSPF cost
- 23 - What is the OSPF cost
- 26 - Command to produce output
- 27 - Command is used to verify OSPF is enabled
- 34. method to make the new router ID effective
- 37. Which task has to be performed on Router 1
- 40. Match OSPF state
- 41. router is unreachable
- 42. three states are involved
- 5. faciliate hirarchical routing
- 

## WAN Concepts

A WAN is a telecommunications network that spans over a relatively large geographical area and is required to connect beyond the boundary of the LAN.


### Terminology

- **Private WAN** - WAN dedicated to one user
- **CE** - Custom Edge -> router in an end-LAN
- **PE** - makes connection into MPLS cloud


### Purpose


- interconnect remote users, networks and sites
- owned and managed by internet service / providers
- WAN services are provided for a fee
- provide low to high bandwidth speeds over long distances


### Topologies

- **Point-to-Point**
	- point-to-point circuit between two endpoints.
	- involves Layer 2 through service provider network.
	- transparent to customer network.
- **Hub-and-Spoke**
	- single interface on hub router to be shared by all spoke circuits
	- can be interconnected through hub router using virtual circuits
	- routers can only communicate with each other through the hub router
	- *single point of failure*
- **Dual-homed**
	- enhanced redundancy, load balancing, ability to implement backup service provider
	- more expensive than single-homed
	- More difficult, additional configuration
- **Fully Meshed**
	- most fault-tolerant topology
	- virtual circuits to connect all sites
- **Partially Meshed**
	- Connects many but not all sites



### Layers

Most WAN's focus on Layer 1 and Layer 2 in OSI.

**Layer 1 Protocols:**
- Yeah, well...
- dinner > class


**Layer 2 Protocols:**
- Broadband
- Wireless
- Ethernet WAN
- Multiprotocol Label Switching (MPLS)
- -- _below are less used_ -- 
- Point-to-Point (PPP)
- high-Level Data Link
- Frame Relay
- Asynchronous Transfer Mode (ATM)


### WAN Devices

- Voiceband Modem - uses telephone lines
- DSL Modem  - known as broadband modems, can be connected to DTE over Ethernet
- CSU/DSU - leased lines, connects digital device to digital line
- Optical Converter
- Wireless Router - wirelessly connect to WAN provider
- WAN Core devices - backbone, Layer 3 switches


### Global not global whatver

From PC:
- Inside global: other-side interface of router on PC LAN
- Outside global: Server
- Inside local: PC

### Questions

- 10 - From perspective of users behind the NAT router
- 22 - Which two types of VPNs are examples of enterprise-managed remote access VPNs
- 39 - What has to be done in order to complete the static NAT configuration



## QoS

Give packets priority.

### Network Transmission Quality

How network transmission characteristics impact quality.

- Too much traffic across the network -> queue hold) the packets in memory until resources become available
- Queuing causes delay
- If packets stack up, memory fills up until packets are dropped
- One technique can help with classifying data into multiple queues

**Begriffe:**
- Congestion (Verstopfung) - more traffic than interface can handle -> causes delay
- Jitter - non-consistent delay, adjust with buffer, is on networking device


>[!note]
>digital signal processor (DSP) makes up for little losses. If Jitter exceeds what DSP can handle, audio problems are heard.

### Traffic Characteristics

Minimum network requirements for voice, video and data traffic



**Stats for good Audio:**
- delay < 150 ms
- Jitter < 30 ms
- loss < 1%

**Good video:**
- delay < 200-400 ms
- Jitter < 30-50 ms
- loss < 1% (3% is still enough)



### Queuing Algorithms

Queuing algorithms used by networking devices
#TODO 




### QoS Models

Describe the different QoS models.

**Differentiated Services:**
- DiffServ divides network traffic into classes. Classes can be assigned level
- Each device identifies class of packet and prioritize based of it

Avoid loss:
- Increase link capacity
- Guarantee enough bandwidth and increase buffer space for bursts
	- Can guarantee bandwidth and prioritize drop-sensitive applications:
	- WFQ
	- CBWFQ
	- LLQ
- Drop lower-priority packets before congestion occurs.


### QoS Implementation Techniques

Explain how QoS uses mechanisms to ensure transmission quality

**Tools:**
- ja
- nei
- villech


**Marking at Layer 2:**
- in VLAN 802.1Q
- 3 bits
- The higher the number the higher the priority


**Marking at Layer 3:
- v4 and v6 specify an 8 bit field
- IPv4: Type of Service (TOS)
- IPv6: 



## Network Design

### Hierarchical Networks

Evolving organizations require networks that can scale and support:
- Converged network traffic
- critical applications
- diverse business needs 
- centralized administrative control


- Access Layer: provides access to the user
- Distribution Switch: somewhere in the building
- Core Layer: in the server-room


### Scalable Networks



### Switch Hardware


### Router Hardware



## Device Discovery

>[!note]
>CDP - Cisco
>LLDP - every other


Some useful commands

```bash
lldp run
show lldp neighbors [detail]
```


## Logs

NTP - Network Time Protocol


### SNMP

- Can be used to observe CPU utilization


### Syslog

- Every message contains a severity level and facility - smaller numbers are higher priority