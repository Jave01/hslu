# Secure Project

This repo is meant to contain everything CTF-environment-related.

## Caution!

While everything is inside the docker-containers, this environment also intentionally includes the vulnerability to break out of the containers. Therefore only start it in an isolated environment.
If you don't want to you can remove the lines

```
volumes:
  - /var/run/docker.sock:/tmp/mimimi/docker.sock
```

So the docker socket doesn't get mounted into the container. But even then: **this project is not tested enough to guarantee a secure isolation. There is no guarantee that there aren't other ways 
to break out of the container or that there are no possibilities to harm the host system.** Either test it yourself or use it solely in an isolated environment itself, like a VM.

## Installation

Everything needed to create the CTF environment is inside the docker containers. So you should be able to start and build it with docker-compose and everything should be up and running.

### Prerequisites

- Docker
- Docker-compose

### Docker environment

download the repo and start the environment:

```
https://github.com/Jave01/secproj.git
cd secproj/docker_stuff
docker-compose up -d --build
```

note that docker needs root-privileges.



