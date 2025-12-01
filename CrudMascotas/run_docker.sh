#!/bin/bash
set -e

echo "Construyendo imagen Docker..."
docker build -t veterinaria-mascotas:v1 .

echo "Deteniendo contenedor previo (si existe)..."
docker stop veterinaria-mascotas 2>/dev/null || true
docker rm veterinaria-mascotas 2>/dev/null || true

echo "Levantando contenedor en http://localhost:8080 ..."
docker run -d -p 8080:8080 --name veterinaria-mascotas veterinaria-mascotas:v1

echo "Listo."
