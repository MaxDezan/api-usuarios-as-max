# API de Gerenciamento de Usuários

## Descrição
API REST para gerenciamento de usuários construída com ASP.NET Core (Minimal APIs), seguindo princípios de Clean Architecture, utilizando Entity Framework Core com SQLite (Code‑First + Migrations), DTO Pattern, Repository/Service Pattern e validações com FluentValidation.

A API permite operações CRUD (Create, Read, Update e Soft Delete) sobre usuários, garantindo validação de dados, unicidade de e‑mail e normalização (e‑mail em lowercase). O soft delete é implementado marcando `Ativo=false` e aplicando filtro global para retornar apenas usuários ativos nas consultas.

## Tecnologias Utilizadas
- .NET 8.0
- ASP.NET Core Minimal APIs
- Entity Framework Core 8
- SQLite
- FluentValidation
- Dependency Injection (built‑in)

## Padrões de Projeto Implementados
- Repository Pattern (`IUsuarioRepository` / `UsuarioRepository`)
- Service Pattern (`IUsuarioService` / `UsuarioService`)
- DTO Pattern (`UsuarioCreateDto`, `UsuarioReadDto`, `UsuarioUpdateDto`)
- Clean Architecture (camadas Domain / Application / Infrastructure / API)

## Como Executar o Projeto

### Pré‑requisitos
- .NET SDK 8.0 
### Passos
1. Clonar o repositório
2. Entrar na pasta `APIUsuarios` (raiz do projeto)
3. Restaurar a ferramenta local do EF (manifesto incluído em `.config/dotnet-tools.json`):
   ```
   dotnet tool restore
   ```
4. Criar a migration inicial (criará a pasta `Migrations/`):
   ```
   dotnet tool run dotnet-ef migrations add InitialCreate
   ```
5. Aplicar a migration no SQLite (`usuarios.db`):
   ```
   dotnet tool run dotnet-ef database update
   ```
6. Executar a aplicação:
   ```
   dotnet run
   ```

Observação: se preferir usar a ferramenta global, instale uma versão compatível com .NET 8, por exemplo `dotnet tool install -g dotnet-ef --version 8.*`.

## Endpoints
- GET `/usuarios` — lista usuários (200)
- GET `/usuarios/{id}` — obtém usuário por ID (200/404)
- POST `/usuarios` — cria usuário (201/400/409)
- PUT `/usuarios/{id}` — atualiza usuário (200/400/404/409)
- DELETE `/usuarios/{id}` — remove (soft delete) (204/404)

## Exemplos de Requisições (curl)

Criar usuário (201):
```
curl -X POST http://localhost:5189/usuarios \
  -H "Content-Type: application/json" \
  -d '{
    "nome":"Maria Silva",
    "email":"maria.silva@exemplo.com",
    "senha":"segredo123",
    "dataNascimento":"1990-05-10",
    "telefone":"(11) 91234-5678"
  }'
```

Criar com e‑mail duplicado (409): repetir a requisição com o mesmo e‑mail.

Criar inválido (400):
```
curl -X POST http://localhost:5189/usuarios \
  -H "Content-Type: application/json" \
  -d '{
    "nome":"Ma",
    "email":"invalido",
    "senha":"123",
    "dataNascimento":"2010-01-01"
  }'
```

Listar (200):
```
curl http://localhost:5189/usuarios
```

Obter por ID (200/404):
```
curl http://localhost:5189/usuarios/1
```

Atualizar (200):
```
curl -X PUT http://localhost:5189/usuarios/1 \
  -H "Content-Type: application/json" \
  -d '{
    "nome":"Maria S. Atualizada",
    "email":"maria.silva@exemplo.com",
    "dataNascimento":"1990-05-10",
    "telefone":"(11) 93456-7890",
    "ativo":true
  }'
```

Remover (204):
```
curl -X DELETE http://localhost:5189/usuarios/1
```

## Estrutura do Projeto
```
APIUsuarios/
├── Domain/
│   └── Entities/
│       └── Usuario.cs
├── Application/
│   ├── DTOs/
│   │   ├── UsuarioCreateDto.cs
│   │   ├── UsuarioReadDto.cs
│   │   └── UsuarioUpdateDto.cs
│   ├── Interfaces/
│   │   ├── IUsuarioRepository.cs
│   │   └── IUsuarioService.cs
│   ├── Services/
│   │   └── UsuarioService.cs
│   └── Validators/
│       ├── UsuarioCreateDtoValidator.cs
│       └── UsuarioUpdateDtoValidator.cs
├── Infrastructure/
│   ├── Persistence/
│   │   └── AppDbContext.cs
│   └── Repositories/
│       └── UsuarioRepository.cs
├── Program.cs
├── appsettings.json
├── APIUsuarios.csproj
└── (Migrations/ será gerada após as migrations)
```

### Descrição de pastas e arquivos principais:
- Domain: contém as entidades de domínio puras (regras e dados centrais). `Usuario.cs` define o modelo persistido.
- Application:
  - DTOs: objetos de transporte usados na API para entrada/saída (não expõem senha no read/update).
  - Interfaces: contratos de serviços e repositórios (`IUsuarioService`, `IUsuarioRepository`).
  - Services: implementação da lógica de negócio/orquestração (`UsuarioService`).
  - Validators: regras de validação com FluentValidation para os DTOs.
- Infrastructure:
  - Persistence: `AppDbContext` (EF Core) e mapeamentos; define índice único de e‑mail e filtro global de ativos.
  - Repositories: implementações dos repositórios que acessam o banco (`UsuarioRepository`).
- Program.cs: configuração mínima da aplicação (DI, DbContext) e mapeamento dos endpoints Minimal API.
- appsettings.json: connection string do SQLite e configurações básicas.
- Migrations/: será criada após rodar os comandos de migrations (histórico do schema do banco).
- APIUsuarios.csproj: arquivo de projeto com referências de pacotes (.NET 8, EF Core, FluentValidation).

## Decisões Técnicas
- Índice único para `Email` e normalização para lowercase.
- `Ativo=true` como default e filtro global para retornar somente ativos.
- `DataCriacao` com `CURRENT_TIMESTAMP` no SQLite; `DataAtualizacao` preenchida em atualizações/soft delete.
- Validações com FluentValidation (nome, e‑mail, senha no create, idade mínima 18, telefone BR opcional).
- Regras de negócio de unicidade mantidas no Service para controle de status codes (409).

## Autor
- Nome: Max Dezan Rossi
- Curso: Análise e Desenvolvimento de Sistemas

### Vídeo
Link do vídeo da demonstração: (adicione aqui)
