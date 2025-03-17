# Financial Control API

## Descrição
API para controle de transações financeiras diárias, utilizando:
- **.NET 8**
- **Redis para caching**
- **AWS SQS para filas**
- **MongoDB para persistência**
- **JWT para autenticação**
- **Testes automatizados com xUnit**

## Como Rodar

### Requisitos
- Docker instalado
- Redis instalado (`docker run --name redis -d -p 6379:6379 redis`)
- MongoDB rodando (`docker run --name mongo -d -p 27017:27017 mongo`)

### Passos
1. Clone o repositório
2. Navegue até a pasta do projeto
3. Execute `dotnet restore`
4. Execute `dotnet run`

## Testes
Para rodar os testes, execute:
```
dotnet test
```

## Diagramas
Os diagramas da arquitetura estão no arquivo `docs/architecture.pdf`.
