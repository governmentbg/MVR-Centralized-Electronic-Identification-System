export ENV_FILE=../docker/env/test.env
export COMPOSE_FILE=../docker/docker-compose-modules.yaml
docker-compose -f ${COMPOSE_FILE} --env-file ${ENV_FILE} down