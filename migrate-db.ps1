$migrationName = Read-Host "Digite o nome da migration"

dotnet ef migrations add $migrationName `
    --project EchoProject.Infrastructure `
    --startup-project EchoProject.Api `
    --verbose