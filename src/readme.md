# FCG Catalog API

Este projeto é uma aplicação em camadas com .NET 8 para gerenciamento de catálogo de jogos, carrinho, pedidos e biblioteca do usuário.

Também possui integração assíncrona com RabbitMQ para o fluxo de pagamento.

## Estrutura da Solução

- `FCG.Catalog.WebApi`
  - Endpoints HTTP (`Controllers`), autenticação, Swagger e configuração de injeção de dependência.
- `FCG.Catalog.Application`
  - Serviços de aplicação, orquestração de regras, produtor e consumidor de eventos.
- `FCG.Catalog.Domain`
  - Modelos de domínio, DTOs e validações de negócio.
- `FCG.Catalog.Infra`
  - Contexto do EF Core, repositórios e acesso ao banco.
- `FCG.Core`
  - Contratos compartilhados e integrações transversais.
- `FCG.Catalog.Tests`
  - Testes automatizados.

## Pré-requisitos

- .NET 8 SDK
- SQL Server (ou LocalDB)
- RabbitMQ

## Como executar a API

Execute o projeto Web API:

`dotnet run --project src/FCG.Catalog.WebApi/FCG.Catalog.WebApi.csproj`

No ambiente de desenvolvimento, o Swagger fica disponível em:

- `/swagger`

Endpoints operacionais:

- `GET /health`
- `GET /metrics`

## Fluxo atual do sistema

1. O usuário adiciona jogos ao carrinho pelos endpoints de `Cart`.
2. O usuário finaliza a compra em `POST /Cart/CheckoutCart`.
3. No checkout, um pedido é criado e um `OrderPlacedEvent` é publicado no RabbitMQ (`queue:OrderPlacedEvent`).
4. Um fluxo externo de pagamento processa a cobrança e publica `PaymentProcessedEvent`.
5. O `PaymentProcessedEventConsumer` atualiza o status de pagamento do pedido.
6. Se aprovado, os jogos comprados são adicionados à biblioteca do usuário.

## Rotas da API

O padrão base das rotas segue o nome do controller: `/{Controller}/{Action}`.

### `GameController` (`/Game`)

- `POST /Game/RegisterGame` (Admin)
  - Cria um novo jogo.
- `GET /Game/GetAllGames` (Autenticado)
  - Retorna todos os jogos.
- `GET /Game/GetGameById/{id}` (Autenticado)
  - Retorna um jogo por id.
- `PUT /Game/UpdateGame/{id}` (Admin)
  - Atualiza os dados de um jogo.
- `DELETE /Game/DeleteGame/{id}` (Admin)
  - Remove um jogo.

### `CartController` (`/Cart`)

> Observação: os atributos de autorização estão comentados neste controller no momento.

- `GET /Cart/GetCartByUserId/{userId}`
  - Retorna o carrinho ativo do usuário.
  - Se não existir, cria um novo carrinho ativo.
- `POST /Cart/AddItemToCart`
  - Adiciona um jogo ao carrinho ativo.
- `DELETE /Cart/RemoveItemFromCart`
  - Remove um jogo do carrinho ativo.
- `DELETE /Cart/ClearCart/{userId}`
  - Remove todos os itens do carrinho ativo.
- `POST /Cart/CheckoutCart`
  - Valida consistência do carrinho e valor do pagamento.
  - Cria o pedido e marca o carrinho como finalizado.
  - Publica `OrderPlacedEvent`.

### `OrderController` (`/Order`)

- `POST /Order/RegisterOrder` (Admin)
  - Cria um pedido diretamente.
- `GET /Order/GetOrderById/{id}` (Autenticado)
  - Retorna os detalhes do pedido por id.
- `GET /Order/GetOrdersByUserId/{userId}` (Autenticado)
  - Retorna o histórico de pedidos do usuário.
- `PUT /Order/UpdateOrder/{id}` (Admin)
  - Atualiza os dados do pedido.

### `LibraryController` (`/Library`)

- `GET /Library/GetLibraryGamesByUserId/{userId}` (Autenticado)
  - Retorna os jogos disponíveis na biblioteca do usuário.

## Eventos de integração assíncrona

- `OrderPlacedEvent`
  - Publicado após a criação do pedido no checkout.
  - Contém dados do pedido e do pagamento para o processamento externo.
- `PaymentProcessedEvent`
  - Consumido por esta API.
  - Atualiza o status de pagamento do pedido e, quando aprovado, libera os jogos na biblioteca do usuário.
