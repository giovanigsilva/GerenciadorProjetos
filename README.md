# GerenciadorProjetos

Documentação profissional para configuração, execução e deploy local ou via Docker.

Índice
Visão Geral

Pré-requisitos

Configuração do Banco de Dados (Docker)

Execução da Aplicação Localmente

Execução da Aplicação via Docker

Comandos Úteis

Observações

Visão Geral
O GerenciadorProjetos é uma solução moderna para gestão de projetos, construída em .NET 9, com arquitetura em camadas, autenticação JWT, validação robusta e documentação automática via Swagger. O projeto está preparado para rodar tanto localmente quanto em containers Docker, facilitando o desenvolvimento, testes e deploy.

Pré-requisitos
.NET 9 SDK

Docker

Git

SQL Server (recomendado via Docker)

Configuração do Banco de Dados (Docker)
Para criar e subir um container SQL Server para desenvolvimento, execute:

bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=NovaSenhaForte123!" -p 1433:1433 --name sqlserver-dev -d mcr.microsoft.com/mssql/server:2022-lts
Usuário: sa

Senha: NovaSenhaForte123!

Porta: 1433

Dica: Altere a senha para produção.
O container pode ser gerenciado via Docker Desktop ou CLI.

Execução da Aplicação Localmente ou via docker
Clone o repositório:

bash
git clone [https://github.com/giovanigsilva/GerenciadorProjetos.git]
cd GerenciadorProjetos
Configure a connection string do banco no appsettings.Development.json:

json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=GerenciadorProjetos;User Id=sa;Password=NovaSenhaForte123!;TrustServerCertificate=True"
}
Restaure e rode a aplicação:

bash
dotnet restore
dotnet build
dotnet run --project ProjectManagement.Api/ProjectManagement.Api.csproj
Acesse a API em http://localhost:5000 ou conforme porta configurada.

Execução da Aplicação via Docker
1. Build da imagem
No diretório raiz do projeto:

bash
docker build -t gerenciadorprojetos-api .
2. Rodar o container da aplicação
bash
docker run -d -p 8080:80 --name gerenciadorprojetos-api \
  --env ConnectionStrings__DefaultConnection="Server=host.docker.internal,1433;Database=GerenciadorProjetos;User Id=sa;Password=NovaSenhaForte123!;TrustServerCertificate=True" \
  gerenciadorprojetos-api
O parâmetro host.docker.internal permite que o container acesse o SQL Server rodando no host local (ajuste se rodar em ambiente Linux puro).

3. Acesse a API
http://localhost:8080/swagger — documentação interativa via Swagger.

Comandos Úteis
Ação	Comando
Subir SQL Server no Docker	docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=NovaSenhaForte123!" -p 1433:1433 --name sqlserver-dev -d mcr.microsoft.com/mssql/server:2022-lts
Build da imagem da API	docker build -t gerenciadorprojetos-api .
Rodar container da API	docker run -d -p 8080:80 --name gerenciadorprojetos-api ... gerenciadorprojetos-api
Parar container	docker stop gerenciadorprojetos-api
Remover container	docker rm gerenciadorprojetos-api
Ver logs do container	docker logs gerenciadorprojetos-api
Observações
Ambiente Dev: Sempre utilize o ambiente de desenvolvimento para testes locais.

Banco de Dados: Certifique-se de que o banco está acessível antes de rodar a aplicação.

Variáveis de ambiente: Para produção, utilize variáveis de ambiente seguras para connection strings e secrets.

Banco de dados: Execute o start.sql para criar o banco, não utilizei migrations.

Contribua, reporte issues e fique à vontade para sugerir melhorias!
Se precisar de exemplos de payload, scripts de seed ou dúvidas sobre deploy, consulte a documentação interna ou abra uma issue no repositório.

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=NovaSenhaForte123!" -p 1433:1433 --name sqlserver-dev -d mcr.microsoft.com/mssql/server:2022-lts
