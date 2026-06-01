# UserApi

API interna de gestão de usuários desenvolvida em C# com ASP.NET Core.  
Consome dados do [JSONPlaceholder](https://jsonplaceholder.typicode.com/), transforma as informações no padrão da empresa e expõe um endpoint REST documentado.

---

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Conexão com a internet (para chamar a API externa)

Verifique sua versão instalada:
```bash
dotnet --version
# deve exibir 8.x.x
```

---

## Como executar

```bash
# 1. Clone o repositório
git clone https://github.com/seu-usuario/UserApi.git
cd UserApi

# 2. Entre na pasta do projeto
cd src/API

# 3. Rode a aplicação
dotnet run
```

A API estará disponível em `http://localhost:5000`.  
O Swagger (documentação interativa) abre automaticamente na raiz: `http://localhost:5000`.

---

## Endpoint disponível

### `GET /users/{id}`

Retorna os dados transformados de um usuário.

**Parâmetro:** `id` — número inteiro de 1 a 10 (IDs disponíveis no JSONPlaceholder)

**Exemplo de chamada:**
```bash
curl http://localhost:5000/users/1
```

**Exemplo de resposta (`200 OK`):**
```json
{
  "id": 1,
  "fullName": "Leanne Graham",
  "email": "sincere@april.biz",
  "domain": "april.biz",
  "username": "bret",
  "phone": "17705842",
  "website": "https://hildegard.org",
  "companyName": "Romaguera-Crona"
}
```

**Respostas de erro:**

| Status | Situação |
|--------|----------|
| `400`  | ID inválido (ex: `/users/0` ou `/users/-1`) |
| `404`  | Usuário não encontrado (ex: `/users/99`) |
| `503`  | API externa indisponível |

---

## Arquitetura

O projeto segue **DDD simplificado** com separação em 4 camadas:

```
/src
  /API             → Controllers, Program.cs (entrada HTTP)
  /Application     → Serviços, DTOs, interfaces de aplicação
  /Domain          → Entidades e interfaces (contratos)
  /Infrastructure  → Chamada HTTP ao JSONPlaceholder
```

| Camada | Responsabilidade |
|--------|-----------------|
| **API** | Recebe a requisição, chama o serviço, devolve a resposta HTTP |
| **Application** | Orquestra o caso de uso e aplica as transformações |
| **Domain** | Define as entidades e os contratos — sem dependências externas |
| **Infrastructure** | Implementa a busca na API externa (JSONPlaceholder) |

---

## Transformações aplicadas

| Campo original | Campo retornado | Transformação |
|----------------|-----------------|---------------|
| `name` | `fullName` | Renomeação |
| `email` | `email` | Convertido para minúsculas |
| *(derivado)* | `domain` | Extraído do e-mail (parte após `@`) |
| `username` | `username` | Convertido para minúsculas |
| `phone` | `phone` | Apenas dígitos (remove traços, parênteses etc.) |
| `website` | `website` | Normalizado para `https://` |
| `company.name` | `companyName` | Extraído do objeto aninhado |

---

## Decisões técnicas

- **`IHttpClientFactory`** em vez de `new HttpClient()`: evita problemas de esgotamento de sockets em produção.
- **Injeção de dependência** em todas as camadas: facilita testes e troca de implementações.
- **Exceção personalizada (`ExternalApiException`)**: separa erros de infraestrutura dos erros de negócio.
- **Swagger na raiz (`/`)**: facilita o teste manual sem precisar lembrar da URL.
- **Validação do ID no Controller**: retorna `400` antes de fazer qualquer chamada externa.

---

## Possíveis melhorias futuras

- Adicionar cache para não repetir chamadas externas para o mesmo ID
- Testes unitários com xUnit e Moq
- Variável de ambiente para a URL base da API externa
- Health check endpoint (`/health`)
