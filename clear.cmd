docker stop kafka zookeeper postgresdb recordprocessingjob parsingjob site nginxproxy
docker rm -f kafka zookeeper postgresdb recordprocessingjob parsingjob site nginxproxy
docker ps -a
docker rmi -f kafka zookeeper postgresdb recordprocessingjob parsingjob site nginxproxy
docker network prune -f
docker volume prune -f
docker system prune -f