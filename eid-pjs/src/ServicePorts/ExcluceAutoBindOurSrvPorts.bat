REM netsh interface ipv4 show excludedportrange protocol=tcp

net stop winnat

netsh int ipv4 add excludedportrange protocol=tcp startport=7172 numberofports=1
netsh int ipv4 add excludedportrange protocol=tcp startport=7171 numberofports=1
netsh int ipv4 add excludedportrange protocol=tcp startport=9200 numberofports=1
netsh int ipv4 add excludedportrange protocol=tcp startport=9600 numberofports=1
netsh int ipv4 add excludedportrange protocol=tcp startport=5601 numberofports=1
netsh int ipv4 add excludedportrange protocol=tcp startport=6379 numberofports=1
netsh int ipv4 add excludedportrange protocol=tcp startport=5015 numberofports=1

netsh int ipv4 add excludedportrange protocol=tcp startport=5432 numberofports=1


net start winnat
