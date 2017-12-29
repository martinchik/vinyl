docker-machine ls
docker-machine env ubuntu-vinyl
cmd "$(docker-machine env ubuntu-vinyl)"
@FOR /f "tokens=*" %i IN ('docker-machine env ubuntu-vinyl') DO @%i
docker-machine ls