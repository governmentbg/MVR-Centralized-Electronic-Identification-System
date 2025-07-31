export ENV_FILE=../docker/env/dev.env
export COMPOSE_FILE=../docker/docker-compose-dependencies.yaml
docker-compose -f ${COMPOSE_FILE} --env-file ${ENV_FILE} up -d