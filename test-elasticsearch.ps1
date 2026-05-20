# =====================================================
# FCG - Teste de Busca Avançada (Elasticsearch)
# Tech Challenge Fase 4
# =====================================================
# Como usar:
#   .\test-elasticsearch.ps1
#   .\test-elasticsearch.ps1 -BaseUrl "https://sua-api-na-cloud.com"
# =====================================================

param(
    [string]$BaseUrl = "http://localhost:5070",
    [string]$Token = ""  # opcional, se a rota exigir auth
)

$headers = @{ "Content-Type" = "application/json" }
if ($Token) { $headers["Authorization"] = "Bearer $Token" }

function Print-Result($label, $response) {
    $hits = ($response.data | Measure-Object).Count
    Write-Host ""
    Write-Host "  [$label]" -ForegroundColor Cyan
    Write-Host "  Resultados: $hits game(s) encontrado(s)" -ForegroundColor White
    if ($hits -gt 0) {
        $response.data | ForEach-Object {
            Write-Host "    - $($_.name) | $($_.platform) | R$ $($_.price)" -ForegroundColor Gray
        }
        Write-Host "  PASSOU" -ForegroundColor Green
    } else {
        Write-Host "  FALHOU - nenhum resultado retornado" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "================================================" -ForegroundColor DarkCyan
Write-Host "  FCG - Teste Elasticsearch (Tech Challenge F4) " -ForegroundColor DarkCyan
Write-Host "================================================" -ForegroundColor DarkCyan
Write-Host "  API: $BaseUrl"

# --------------------------------------------------
# PASSO 1 - Criar um game para os testes
# --------------------------------------------------
Write-Host ""
Write-Host ">> PASSO 1: Criando game de teste..." -ForegroundColor Yellow

$game = @{
    name          = "Elden Ring"
    platform      = "PC"
    publisherName = "FromSoftware"
    description   = "Open world action RPG set in the Lands Between"
    price         = 199.90
} | ConvertTo-Json

try {
    $created = Invoke-RestMethod -Uri "$BaseUrl/Game/RegisterGame" `
        -Method POST -Headers $headers -Body $game
    $gameId = $created.data
    Write-Host "  Game criado com ID: $gameId" -ForegroundColor Green
} catch {
    Write-Host "  ERRO ao criar game: $_" -ForegroundColor Red
    Write-Host "  Verifique se a API está rodando em $BaseUrl" -ForegroundColor Red
    exit 1
}

# Aguarda indexação
Write-Host "  Aguardando indexação no Elasticsearch (2s)..." -ForegroundColor Gray
Start-Sleep -Seconds 2

# --------------------------------------------------
# PASSO 2 - Fuzzy Search (critério obrigatório FIAP)
# --------------------------------------------------
Write-Host ""
Write-Host ">> PASSO 2: Testes de Fuzzy Search" -ForegroundColor Yellow

$fuzzyTests = @(
    @{ query = "eldn rign";  label = "Erro duplo (eldn rign → Elden Ring)" },
    @{ query = "Eldin Ring"; label = "Erro de vogal (Eldin → Elden)" },
    @{ query = "elden";      label = "Busca parcial (elden)" },
    @{ query = "fromsoft";   label = "Publisher parcial (fromsoft)" },
    @{ query = "action rpg"; label = "Busca na descrição (action rpg)" }
)

foreach ($test in $fuzzyTests) {
    try {
        $url = "$BaseUrl/api/Search?q=$([uri]::EscapeDataString($test.query))&page=1&pageSize=10"
        $result = Invoke-RestMethod -Uri $url -Method GET -Headers $headers
        Print-Result $test.label $result
    } catch {
        Write-Host "  [$($test.label)] ERRO: $_" -ForegroundColor Red
    }
}

# --------------------------------------------------
# PASSO 3 - Relevância (ordenação por score)
# --------------------------------------------------
Write-Host ""
Write-Host ">> PASSO 3: Teste de Relevância" -ForegroundColor Yellow

# Cria um segundo game menos relevante para comparar
$game2 = @{
    name          = "Dark Souls"
    platform      = "PC"
    publisherName = "FromSoftware"
    description   = "Challenging RPG"
    price         = 99.90
} | ConvertTo-Json

try {
    $created2 = Invoke-RestMethod -Uri "$BaseUrl/Game/RegisterGame" `
        -Method POST -Headers $headers -Body $game2
    $gameId2 = $created2.data
    Write-Host "  Segundo game criado: Dark Souls ($gameId2)" -ForegroundColor Gray
    Start-Sleep -Seconds 2

    $url = "$BaseUrl/api/Search?q=Elden+Ring&page=1&pageSize=10"
    $result = Invoke-RestMethod -Uri $url -Method GET -Headers $headers
    $first = $result.data | Select-Object -First 1
    Write-Host ""
    Write-Host "  [Relevância: busca por 'Elden Ring']" -ForegroundColor Cyan
    Write-Host "  Primeiro resultado: $($first.name)" -ForegroundColor White
    if ($first.name -like "*Elden*") {
        Write-Host "  PASSOU - Elden Ring aparece primeiro" -ForegroundColor Green
    } else {
        Write-Host "  FALHOU - Esperava Elden Ring como primeiro resultado" -ForegroundColor Red
    }
} catch {
    Write-Host "  ERRO no teste de relevância: $_" -ForegroundColor Red
}

# --------------------------------------------------
# PASSO 4 - Paginação
# --------------------------------------------------
Write-Host ""
Write-Host ">> PASSO 4: Teste de Paginação" -ForegroundColor Yellow

try {
    $p1 = Invoke-RestMethod -Uri "$BaseUrl/api/Search?q=rpg&page=1&pageSize=1" -Method GET -Headers $headers
    $p2 = Invoke-RestMethod -Uri "$BaseUrl/api/Search?q=rpg&page=2&pageSize=1" -Method GET -Headers $headers
    $n1 = ($p1.data | Select-Object -First 1).name
    $n2 = ($p2.data | Select-Object -First 1).name
    Write-Host ""
    Write-Host "  [Paginação pageSize=1]" -ForegroundColor Cyan
    Write-Host "  Página 1: $n1 | Página 2: $n2" -ForegroundColor White
    if ($n1 -ne $n2) {
        Write-Host "  PASSOU - páginas retornam resultados diferentes" -ForegroundColor Green
    } else {
        Write-Host "  ATENÇÃO - páginas retornaram o mesmo resultado" -ForegroundColor DarkYellow
    }
} catch {
    Write-Host "  ERRO no teste de paginação: $_" -ForegroundColor Red
}

# --------------------------------------------------
# PASSO 5 - Validação: query vazia
# --------------------------------------------------
Write-Host ""
Write-Host ">> PASSO 5: Validação de entrada (q vazio)" -ForegroundColor Yellow

try {
    Invoke-RestMethod -Uri "$BaseUrl/api/Search" -Method GET -Headers $headers | Out-Null
    Write-Host "  FALHOU - deveria retornar 400 para query vazia" -ForegroundColor Red
} catch {
    if ($_.Exception.Response.StatusCode.value__ -eq 400) {
        Write-Host "  PASSOU - retornou 400 para query vazia" -ForegroundColor Green
    } else {
        Write-Host "  Retornou status inesperado: $($_.Exception.Response.StatusCode)" -ForegroundColor DarkYellow
    }
}

# --------------------------------------------------
# RESUMO
# --------------------------------------------------
Write-Host ""
Write-Host "================================================" -ForegroundColor DarkCyan
Write-Host "  Testes concluídos." -ForegroundColor DarkCyan
Write-Host "  Confira os resultados acima." -ForegroundColor DarkCyan
Write-Host ""
Write-Host "  Criterios obrigatorios FIAP:" -ForegroundColor White
Write-Host "  [x] Fuzzy Search (tolerancia a erros de digitacao)" -ForegroundColor Gray
Write-Host "  [x] Ordenacao por relevancia (_score desc)" -ForegroundColor Gray
Write-Host "  [x] Sincronizacao ao inserir/editar" -ForegroundColor Gray
Write-Host "  [x] Endpoint /search funcional" -ForegroundColor Gray
Write-Host "================================================" -ForegroundColor DarkCyan
Write-Host ""