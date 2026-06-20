$processNames = @(
    'SmartGrid.WebApi',
    'SmartGrid.ITSimulator',
    'SmartGrid.Functions'
)

foreach ($name in $processNames) {
    Get-Process -Name $name -ErrorAction SilentlyContinue | Stop-Process -Force
}

Write-Host 'Stopped local SmartGrid dev processes (if any were running).'
