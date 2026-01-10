# FIAP.Users

API de gerenciamento de usuários desenvolvida como parte do projeto FIAP.

## 📋 Descrição

Microserviço responsável pelo gerenciamento de usuários, autenticação (JWT), perfis, carteiras e controle de acesso do sistema.

## 🏗️ Arquitetura

Este projeto segue uma arquitetura em camadas:

- **Users.API**: Camada de apresentação (Controllers, Program.cs)
- **Users.Domain**: Camada de domínio (Entidades, Serviços, Interfaces, JWT)
- **Users.Infrastructure**: Camada de infraestrutura (Repositórios, Migrations, Banco de dados)
- **Users.Domain.Shared**: DTOs e contratos compartilhados

## 🚀 Tecnologias

- .NET (C#)
- Entity Framework Core
- JWT (JSON Web Tokens) para autenticação
- RabbitMQ (Message Bus)
- Prometheus (Métricas)
- Grafana (Visualização)
- Docker
- Kubernetes (K8s) na AWS

## ☸️ Infraestrutura

Esta aplicação é implantada em um **cluster Kubernetes (K8s) na AWS**, utilizando:

- ConfigMaps para configurações
- Secrets para informações sensíveis (chaves JWT, strings de conexão)
- Deployments para orquestração de containers
- Services para descoberta e balanceamento de carga
- HPA (Horizontal Pod Autoscaler) para escalonamento automático
- Prometheus para coleta de métricas
- Grafana para visualização e dashboards de monitoramento
- RabbitMQ para comunicação assíncrona entre microserviços

Os arquivos de configuração do Kubernetes estão localizados na pasta `kubernetes/`.

## 📦 Build e Deploy

O projeto possui configuração de CI/CD através de GitHub Actions para deploy automático no Amazon ECR e Kubernetes.

## 🔧 Requisitos

- .NET SDK
- Docker (para containerização)
- Acesso ao cluster Kubernetes na AWS (para deploy)

## 🔐 Segurança

- Autenticação via JWT
- Validação de senhas robusta
- Middleware de validação e tratamento de erros

## 📝 Licença

Este projeto faz parte do projeto acadêmico FIAP.
