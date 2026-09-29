# Employee API

API REST simples em C# (ASP.NET Core) para gerenciar funcionários.
Os dados ficam **em memória**: ao reiniciar a aplicação, tudo volta ao estado inicial.

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

## Como rodar

```bash
dotnet run
```

O terminal mostrará a URL, algo como `http://localhost:5000`.

## Endpoints

| Método | Rota              | Descrição                    |
|--------|-------------------|------------------------------|
| GET    | `/employees`      | Lista todos os funcionários  |
| GET    | `/employees/{id}` | Busca um funcionário pelo id |
| POST   | `/employees`      | Cria um funcionário          |
| DELETE | `/employees/{id}` | Remove um funcionário        |

## Exemplos (curl)

```bash
# Listar
curl http://localhost:5000/employees

# Buscar por id
curl http://localhost:5000/employees/1

# Criar
curl -X POST http://localhost:5000/employees \
  -H "Content-Type: application/json" \
  -d '{"name":"Maria Silva","position":"Designer","salary":7000}'

# Remover
curl -X DELETE http://localhost:5000/employees/1
```

## Estrutura

```
EmployeeApi/
├── Controllers/EmployeesController.cs   # Endpoints da API
├── Models/Employee.cs                   # Modelo de dados
├── Program.cs                           # Ponto de entrada
├── EmployeeApi.csproj                   # Configuração do projeto
└── .github/workflows/build.yml          # CI: restaura e compila
```
