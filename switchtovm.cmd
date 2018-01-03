docker-machine ls
docker-machine env ubuntu-vinyl
cmd "$(docker-machine env ubuntu-vinyl)"
REM run this text - @FOR /f "tokens=*" %i IN ('docker-machine env ubuntu-vinyl') DO @%i
REM run this text and view * in active column- docker-machine ls