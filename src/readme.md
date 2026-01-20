# Projeto RabbitMQ com .NET 10

Este projeto demonstra a comunicação assíncrona entre aplicações utilizando **RabbitMQ** com **.NET 10**, aplicando uma arquitetura em camadas inspirada em *Clean Architecture*.

## Estrutura do Projeto

- **FCG.Catalog.WebApi**  
  Projeto principal da solução, responsável por expor os endpoints HTTP (controllers).

- **FCG.Catalog.Core**  
  Projeto de apoio contendo abstrações e classes base, incluindo componentes relacionados à mensageria (RabbitMQ).

- **FCG.Catalog.Infra**  
  Responsável pela infraestrutura, contendo as *migrations* e as classes de acesso a dados, incluindo a conexão com o banco de dados SQL Server.

- **FCG.Catalog.Application**  
  Contém as regras de aplicação, serviços utilizados pelos controllers, além dos *producers* e *consumers* do RabbitMQ.

- **FCG.Catalog.Domain**  
  Contém as entidades de domínio, DTOs, modelos e validações de negócio.

- **FCG.Catalog.Tests**  
  Projeto responsável pelos testes automatizados da solução.

## Pré-requisitos

- .NET 10 SDK
- Docker
- Docker Compose

## Como executar o projeto

### 1. Iniciar o RabbitMQ

```bash
docker-compose up -d
```

### 2. Executar a aplicação

Executar o projeto **FCG.Catalog.WebApi** via Visual Studio ou CLI do .NET.

### 3. Utilização da aplicação — Endpoints e Producer

Após iniciar a aplicação, é possível acessar e testar os endpoints de **CRUD do recurso Game**.

Também está disponível o endpoint **POST de Order (Pedido)**, responsável por:

- Persistir o pedido no banco de dados;
- Publicar uma mensagem do tipo **OrderPlacedEvent** em uma fila do RabbitMQ.

### 4. Utilização da aplicação — Consumer

Durante a execução da aplicação, um serviço **consumer** é iniciado automaticamente, ficando responsável por escutar a fila **PaymentProcessedEvent**.

A implementação deste consumer encontra-se no arquivo  
`PaymentProcessedEventConsumer.cs`, no projeto **FCG.Catalog.Application**.

Ao receber uma mensagem:

- O status do pagamento é avaliado;
- O registro do pedido (**Order**) é atualizado no banco de dados;
- Caso o pagamento seja aprovado, um registro é inserido na tabela **Catalog**, adicionando o jogo à biblioteca do cliente.
