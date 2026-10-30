# Employee API

API REST em **C# e ASP.NET Core (.NET 8)** para cadastrar, listar, consultar e remover funcionários.

Os dados ficam **em memória**: ao reiniciar a aplicação, tudo volta ao estado inicial. A lista começa com dois funcionários de exemplo. O acesso simultâneo é sincronizado dentro de cada processo da API.

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git para clonar o repositório.

## Como rodar

```sh
git clone https://github.com/CobkCobinho/BasicApi.git
cd BasicApi
dotnet run --project EmployeeApi.csproj --urls http://localhost:5000
```

A API fica disponível em `http://localhost:5000`. O arquivo [EmployeeApi.http](EmployeeApi.http) contém exemplos para um editor compatível com arquivos HTTP.

## Endpoints

| Método | Rota | Resultado |
| --- | --- | --- |
| GET | `/employees` | `200 OK` com a lista de funcionários. |
| GET | `/employees/{id}` | `200 OK` com o funcionário ou `404 Not Found`. |
| POST | `/employees` | `201 Created` com o funcionário e seu endereço no cabeçalho `Location`; dados inválidos retornam `400 Bad Request`. |
| DELETE | `/employees/{id}` | `204 No Content` ou `404 Not Found` quando o funcionário não existe. |

## Cadastro e validação

```json
{
  "name": "Maria Silva",
  "position": "Designer",
  "salary": 7000
}
```

- `name` e `position` são obrigatórios, não podem conter apenas espaços e aceitam até 100 caracteres cada. Espaços no início e no fim são removidos antes do armazenamento.
- `salary` é obrigatório e deve ser um número decimal maior ou igual a zero.
- O `id` é gerado pela API; não é necessário enviá-lo no cadastro.
- As respostas de validação seguem o formato `ValidationProblemDetails`, com os campos inválidos em `errors`.

## Exemplos com curl

Os exemplos abaixo usam a sintaxe de um terminal Bash. Também é possível usar as requisições do arquivo `EmployeeApi.http` em um editor compatível.

```sh
# Listar
curl http://localhost:5000/employees

# Consultar por ID
curl http://localhost:5000/employees/1

# Criar
curl -i -X POST http://localhost:5000/employees \
  -H 'Content-Type: application/json' \
  -d '{"name":"Maria Silva","position":"Designer","salary":7000}'

# Remover
curl -i -X DELETE http://localhost:5000/employees/1
```

## Compilar e testar

Na raiz do repositório:

```sh
dotnet build EmployeeApi.sln
dotnet test EmployeeApi.sln
```

Os testes de integração usam **xUnit** e o servidor em memória do ASP.NET Core. Cobrem cadastro, validação, limites dos campos, geração de ID, consulta pelo endereço retornado, exclusão, IDs inexistentes e cadastros simultâneos com leitura da lista.

O GitHub Actions restaura as dependências, compila a solução em Release e executa os testes em pushes e pull requests para `main`.

## Estrutura

```text
BasicApi/
├── Contracts/CreateEmployeeRequest.cs    # Entrada e validação do cadastro
├── Controllers/EmployeesController.cs    # Endpoints e armazenamento em memória
├── Models/Employee.cs                   # Modelo retornado pela API
├── Tests/EmployeeApi.Tests/              # Testes de integração
├── Program.cs                           # Inicialização da aplicação
├── EmployeeApi.csproj                   # Projeto web (.NET 8)
├── EmployeeApi.sln                      # Solução com a API e os testes
├── EmployeeApi.http                     # Exemplos de requisições
└── .github/workflows/build.yml           # Compilação e testes automáticos
```
