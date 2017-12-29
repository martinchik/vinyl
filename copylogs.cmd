rd /s /q C:\tmp\vmlogs
mkdir C:\tmp\vmlogs
mkdir C:\tmp\vmlogs\rpjoblogs
mkdir C:\tmp\vmlogs\parsingjoblogs
mkdir C:\tmp\vmlogs\kafkalogs
mkdir C:\tmp\vmlogs\sitelogs

docker cp recordprocessingjob:/app/Logs/. /tmp/vmlogs/rpjoblogs/.
docker cp parsingjob:/app/Logs/. /tmp/vmlogs/parsingjoblogs/.
docker cp kafka:/opt/kafka/logs/. /tmp/vmlogs/kafkalogs/.
docker cp site:/app/Logs/. /tmp/vmlogs/sitelogs/.