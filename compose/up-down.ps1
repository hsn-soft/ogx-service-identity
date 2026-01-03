docker network create -d bridge local.infrastructure-network

docker compose -f compose/docker-compose.yml -p ogx-service-identity-local-compose build

docker compose -f compose/docker-compose.yml -p ogx-service-identity-local-compose up -d
docker compose -p ogx-service-identity-local-compose logs --follow

docker compose -f compose/docker-compose.yml -p ogx-service-identity-local-compose down
