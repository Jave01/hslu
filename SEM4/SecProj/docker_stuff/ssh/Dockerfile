FROM ubuntu:latest

RUN apt-get update && apt-get install -y openssh-server


# create users
RUN useradd Groot -p groot -m
RUN useradd Drax -p drax -m
RUN useradd Rocket -p rocket -m
RUN useradd Gamora -p gamora -m
RUN useradd Starlord -p starlord -m
RUN useradd EGO -p ego


# copy the ssh_config files to the container
COPY sshd_config /etc/ssh/sshd_config
# COPY ssh_config /etc/ssh/ssh_config

# create the ssh directory
RUN mkdir /var/run/sshd

# set the root password
RUN echo 'root:root' | chpasswd

EXPOSE 22

# start the ssh service
CMD ["/usr/sbin/sshd", "-D"]
