#!/bin/bash

echo Executing Deployment Script
docker-compose -H ${DEPLOYMENT_SERVER}:2375 up --build -d --remove-orphans