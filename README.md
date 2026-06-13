ASP.NET Core Web API
Entity Framework Core
PostgreSQL (ou outro banco compatível com EF Core)
JWT (JSON Web Token)
BCrypt para hash de senhas
C#
Repository Pattern
Middleware para tratamento global de exceções
Arquitetura do projeto

A estrutura segue uma arquitetura em camadas, organizada da seguinte forma:
- Controllers: responsáveis pelos endpoints da API
- Services: regras de negócio
- Repositories: acesso e manipulação de dados
- Domain: entidades e DTOs
- Data: configuração do DbContext e EF Core
- Exceptions: tratamento global de erros
- Autenticação e Autorização

O sistema utiliza autenticação baseada em JWT.
O login é realizado através do endpoint:
POST /auth/login

No processo de autenticação, o sistema:
Busca o usuário no banco de dados
Verifica se o usuário está ativo
Valida a senha utilizando BCrypt
Gera um token JWT assinado com HMAC SHA256

O token contém as seguintes claims:
Id do usuário
Email
Nome de usuário
Role (ADMIN, PROFESSOR ou USER)

Exemplo de resposta:
{
"token": "jwt_token_aqui"
}
O endpoint de logout apenas retorna uma resposta de sucesso, pois o controle de sessão é stateless via JWT.


Controle de acesso (Roles)
O sistema possui controle de permissões baseado em roles:
- ADMIN: acesso total ao sistema
- PROFESSOR: pode criar, editar e remover tarefas
- USER: pode visualizar apenas suas próprias tarefas

Endpoints da API
Autenticação
POST /auth/login → autentica o usuário e retorna o token JWT
POST /auth/logout → realiza logout (stateless)

Tarefas
GET /tasks/{id} → retorna uma tarefa pelo ID
GET /tasks → lista tarefas (ADMIN/PROFESSOR veem todas, USER vê apenas as suas)
POST /tasks → cria uma nova tarefa (ADMIN/PROFESSOR)
DELETE /tasks/{id} → remove uma tarefa (ADMIN/PROFESSOR)
PATCH /tasks/{id}/situacao → atualiza a situação da tarefa
GET /tasks/filter → filtra tarefas por status, usuário e período

Logs
GET /logs → retorna logs de usuários (apenas ADMIN)

Banco de dados
O projeto utiliza Entity Framework Core com as entidades principais:
- User
- Tarefa
- UsuarioLog

Os enums são armazenados como string no banco de dados, garantindo melhor legibilidade:
Exemplo de configuração:
modelBuilder.Entity<User>()
.Property(u => u.Role)
.HasConversion<string>();

modelBuilder.Entity<Tarefa>()
.Property(x => x.Situacao)
.HasConversion<string>();

Repository Pattern
O projeto implementa um repositório genérico com operações básicas:
- GetById
- GetAll
- Find
- Create
- Update
- Delete

Além disso, existem repositórios específicos para regras adicionais:

UserRepository → busca usuário por username
TarefaRepository → filtros e tarefas por usuário
UserLogRepository → logs de usuários
Regras de negócio
Usuários inativos (Situacao = "I") não podem realizar login
Senhas são armazenadas com hash BCrypt
Usuários comuns visualizam apenas suas próprias tarefas
ADMIN e PROFESSOR possuem acesso global às tarefas
A filtragem de tarefas pode ser feita por status, usuário e intervalo de datas
Tratamento de exceções

Foi implementado um middleware global para captura de erros de regra de negócio.

A BusinessException retorna automaticamente:
Status 400 (Bad Request)

Resposta padrão:

{
"error": "mensagem do erro"
}

Estrutura do projeto
trabalho2
│
├── Controllers
├── Services
├── Repositories
├── Domain
│ ├── Dtos
│ ├── Tarefas
│ └── Usuarios
├── Data
├── Exceptions
├── Migrations

Segurança
JWT com assinatura HMAC SHA256
Controle de acesso por roles
Middleware de autenticação do ASP.NET Core
Senhas criptografadas com BCrypt
Documentação com Swagger autenticado
Testes unitários com xUnit
Logs
