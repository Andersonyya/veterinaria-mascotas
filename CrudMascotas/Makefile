PROJECT_NAME=CrudMascotas
IMAGE_NAME=veterinaria-mascotas
IMAGE_TAG=v1

build:
	dotnet build

run:
	dotnet run

docker-build:
	docker build -t $(IMAGE_NAME):$(IMAGE_TAG) .

docker-run:
	docker run -d -p 8080:8080 --name $(IMAGE_NAME) $(IMAGE_NAME):$(IMAGE_TAG)

docker-stop:
	docker stop $(IMAGE_NAME) || true
	docker rm $(IMAGE_NAME) || true

compose-up:
	docker compose up -d

compose-down:
	docker compose down
