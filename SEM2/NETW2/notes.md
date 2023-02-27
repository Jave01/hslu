# Prüfung

- Hilfsmittel: 16 A5 Seiten (single-sided, ausgedruckt)
- Abschlussprüfung ist 100%

# Important Commands

## General

    $ no shutdown
    $ exit

## Show commands

    $ show ip interface brief
    $ show ipv6 interface brief
    $ show startup config
    $ show running config
    $ show version
    $ show ip route

## Configuration

    $ enable secret <pw>
    $ service password-encryption
    $ copy running-config startup-config

# Switches

## Layer 3 Switches

These are just switches (Layer 2) with an integrated router.

## Micro Segmentation

- bi-directional communication
- automatic speed adjustment

## Management

To prepare a switch for remote management access, the switch must be configured with an IP address.

## Switch Virtual Interface (SVI)

All switches are per default in VLAN-1

## Network Access Layer Issues

| Error Type      | Description                                                               |
| --------------- | ------------------------------------------------------------------------- |
| Input errors    | Total number of errors (runts, giants, no buffer, CRC, frame, overrun     |
| Runts           | Packets that are discarded, because too small                             |
| Giants          | Packets that are discarded, because too big                               |
| CRC             | CRC Errors were generated                                                 |
| Output Errors   | Sum of all errors that prevented the final transmission                   |
| Collisions      | Number of messages retransmitted                                          |
| Late Collisions | A collision that occurs after 512 bits of the frame have been transmitted |
