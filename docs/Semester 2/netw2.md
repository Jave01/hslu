# NETW2

## Administratives

### Prüfung

Testat:

-   [ ] Packet Tracer Aufgabe Inter VLAN
-   [ ] Packet Tracer Aufgabe 2

!!! warning "MEP Infos"

    -   Hilfsmittel: 16 A5 Seiten (single-sided, ausgedruckt)
    -   Abschlussprüfung zählt 100%

## Inhalt

### Important Commands

???+ abstract "General commands"

    ```sh title=""
    no shutdown
    exit
    ```

???+ abstract "show commands"

    ```sh title=""  linenums="1"
    show ip interface brief
    show ipv6 interface brief
    show interface trunk
    show startup config
    show running config
    show version
    show ip route
    show interfaces port-channel
    show etherchannel summary
    show etherchannel port-channel
    show vlan brief
    show spanning-tree
    ```

#### Configuration

```sh title="Common configuration"
enable secret <pw>
service password-encryption
copy running-config startup-config
ip default-gateway <ip-address>
```

### Switches

#### Layer 3 Switches

These are just switches (Layer 2) with an integrated router.

#### Micro Segmentation

-   bi-directional communication
-   automatic speed adjustment

#### Management

To prepare a switch for remote management access, the switch must be configured with an IP address.

#### Switch Virtual Interface (SVI)

All switches are per default in VLAN-1

#### Network Access Layer Issues

| Error Type      | Description                                                               |
| --------------- | ------------------------------------------------------------------------- |
| Input errors    | Total number of errors (runts, giants, no buffer, CRC, frame, overrun)    |
| Runts           | Packets that are discarded, because too small                             |
| Giants          | Packets that are discarded, because too big                               |
| CRC             | CRC Errors were generated                                                 |
| Output Errors   | Sum of all errors that prevented the final transmission                   |
| Collisions      | Number of messages retransmitted                                          |
| Late Collisions | A collision that occurs after 512 bits of the frame have been transmitted |

### Switching Concepts

#### Switching in Networking

Ingress is the term for when a frame is entering an interface and Egress the term for a frame leaving the interface

A switch uses its MAC address table to make forwarding decisions.

!!! note "Note"

    A switch will never allow traffic to be forwarded out the interface it received the traffic

**Switch Forwarding Methods:**

-   **Store-and-forward switching:** Receives the entire frame and ensures the frame is valid.
-   **Cut-Through switching:** Forwards the frame immediately after determining the destination MAC address of an incoming frame and the egress port.

!!! note "Note"

    The preferred Cisco method is store-and-forward switching

#### Collision Domains

Every port is a collision domain.
Switches elliminates collision domains and reduce congestion

-   When there is full duplex on the link the collision domains are eliminated.
-   When there is one or more devices in half-duplex there will now be a collision domain.
-   There will now be contention for the bandwidth.
-   Collisions are now possible
-   Most devices, including Cisco and Microsoft use auto-negotiation as the default setting for duplex and speed.

#### Link Aggregation

Sometimes it's useful to use multiple links for communicating between two end-devices. Per default STP prevents such communication.
EtherChannel is a technology which allows link aggregation which will not be blocked by STP

-   Fast Ethernat and Gbit Ethernet cannot be mixed
-   Can consist of up to eight configured Ethernet ports
-   Both end devices must be configured equally
-   EtherChannels can be formed through negotiation using one of two protocols, Port Aggregation Protocol (PAgP) or Link Aggregation Control Protocl (LACP). PAgP is only availabe on Cisco devices.

### VLAN

Logische Verbindung (kreierung von Netzen) zwischen unterschiedlichen physischen Verbindungen

VLAN configs sind im flash gespeichert (`show flash`)

#### create VLANs

```sh title="Create and use a VLAN on switch" linenums="1"
# create vlan
configure terminal
vlan <vlan_id>
name <id_name>
exit

# assign vlan to interface
configure terminal
interface fastEthernet 0/1
switchport mode access
switchport access vlan <vlan_id>
exit

# --- extra information ---
# If you want to assign multiple ports to one VLAN: (e.g. 11-17)
conf t
vlan 10
int range fastEthernet 0/11-17 // alle zusammenfassen
switchport mode access vlan 10
```

VLAN interface: virtuelles Interface -> bezieht sich auf alle Interfaces welche in diesem VLAN sind.  
Trunk: Verbindung von VLANs für nächste Switch

VLAN Tag: VLAN ID für Trunck

#### Inter-VLAN Routing

VLANs verbinden (Trunks?)

Neue Methode für viele VLANs verbinden: Router on a Stick oder Layer 3 Switch

switchport ist layer 2. Wenn man `no switchport` eingibt, wird der aktive Port auf 3 initialisiert (Port ist routed).

Schritte:

1. VLANs auf Switch kreieren
2. Sub-interfaces auf router kreieren und die Netz-IP's zuweisen.
3. Trunk von Switch auf Router machen

```sh title="Sub-Interface einrichten Beispiel" linenums="1"
# enable interface
enable
conf t
int gigabitEthernet 0/1
no shut
exit

# create sub-interface
int g0/0.10 # <int>.<sub-interface-number> whereas the sub-interface number is usually the vlan number
encapsulation dot1Q <vlan_id_number>
ip address 172.17.10.1 255.255.255.0

show interfaces | include Gig|802.1q # display

# enable trunking on switch
interface <int-id> # int-id is the interface connected to the router or to the next switch
switchport mode trunk # enable trunking
no shut
exit
```

!!! warning "Note"

    The default gateway on the PC's has to be the IP address of the vlan on the router

### STP

Ethernet needs to be loop free. But for redundancy purpose, multiple paths must be present. Spanning Tree Protocol helps preventing circular communication.

1. Elect the root bridge
2. Elect the root ports
3. Elect designated ports
4. Elect alteernate (blocked) ports

Standard Cisco priority is 32769. Lower priority is preferable.

#### Root Bridge

Is the switch with the lowest priority. Path is determined by cost. Low bandwidth increases the 'cost' of the channel.

#### Root Port

Based on the cost, the algorithm chooses the port with the lowest cost (usually shortest path to root bridge). The port going to the bridge is the root port.

#### Designated port

The port where data income is preferred. All ports on the root bridge are designated ports. If one port isn't a root port it is a designated port.

#### Alternate ports

If a connection is between two designated ports (neither side is a root port) they become alternate (blocked) ports. Ports on the root bridge never get blocked. If all factors on the switches and ports are the same, decision making about blocked ports depends on the port ID **of the sender** (F0/1, F0/2 ...). If there are only two switches, the root bridge is the sender.
