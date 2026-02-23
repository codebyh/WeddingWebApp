param(
    [Parameter(Mandatory = $true, HelpMessage = "One or more entries in 'username:password' format.")]
    [string[]]$User,

    [ValidateSet("SHA256", "SHA384", "SHA512")]
    [string]$HashAlgorithm = "SHA256",

    [int]$Iterations = 100000,
    [int]$SaltBytes = 16,
    [int]$HashBytes = 32,
    [ValidateSet("Username", "RandomNumeric")]
    [string]$IdMode = "RandomNumeric",
    [int]$RandomIdLength = 12,

    [switch]$IncludePlaintextPassword,

    [string]$OutputPath = ""
)

$ErrorActionPreference = "Stop"

if ($Iterations -le 0) {
    throw "Iterations must be greater than 0."
}
if ($SaltBytes -le 0) {
    throw "SaltBytes must be greater than 0."
}
if ($HashBytes -le 0) {
    throw "HashBytes must be greater than 0."
}
if ($RandomIdLength -le 0) {
    throw "RandomIdLength must be greater than 0."
}

function Normalize-Username {
    param([string]$Username)

    if ([string]::IsNullOrWhiteSpace($Username)) {
        throw "Username is required."
    }

    return $Username.Trim().ToLowerInvariant()
}

function Get-PasswordHashData {
    param(
        [string]$Password,
        [string]$Algorithm,
        [int]$PasswordIterations,
        [int]$SaltSize,
        [int]$HashSize,
        [System.Security.Cryptography.RNGCryptoServiceProvider]$Rng
    )

    if ([string]::IsNullOrEmpty($Password)) {
        throw "Password is required."
    }

    $salt = New-Object byte[] $SaltSize
    $Rng.GetBytes($salt)

    $derive = New-Object System.Security.Cryptography.Rfc2898DeriveBytes(
        $Password,
        $salt,
        $PasswordIterations,
        ([System.Security.Cryptography.HashAlgorithmName]::$Algorithm)
    )
    try {
        $hash = $derive.GetBytes($HashSize)
    }
    finally {
        $derive.Dispose()
    }

    return @{
        PasswordSalt = [Convert]::ToBase64String($salt)
        PasswordHash = [Convert]::ToBase64String($hash)
    }
}

function New-RandomNumericString {
    param(
        [int]$Length,
        [System.Security.Cryptography.RNGCryptoServiceProvider]$Rng
    )

    if ($Length -le 0) {
        throw "Length must be greater than 0."
    }

    $chars = New-Object char[] $Length
    $buffer = New-Object byte[] $Length
    $Rng.GetBytes($buffer)
    for ($i = 0; $i -lt $Length; $i++) {
        $digit = $buffer[$i] % 10
        $chars[$i] = [char]([int][char]'0' + $digit)
    }

    # avoid leading zero for cleaner IDs
    if ($chars[0] -eq '0') {
        $chars[0] = '1'
    }

    return -join $chars
}

$rng = New-Object System.Security.Cryptography.RNGCryptoServiceProvider
try {
    $nowUtc = [DateTime]::UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
    $documents = @()

    foreach ($entry in $User) {
        $separatorIndex = $entry.IndexOf(":")
        if ($separatorIndex -le 0) {
            throw "Invalid user entry '$entry'. Expected format: username:password"
        }

        $rawUsername = $entry.Substring(0, $separatorIndex)
        $password = $entry.Substring($separatorIndex + 1)
        $username = Normalize-Username -Username $rawUsername
        $documentId = if ($IdMode -eq "Username") {
            $username
        }
        else {
            New-RandomNumericString -Length $RandomIdLength -Rng $rng
        }
        $hashData = Get-PasswordHashData `
            -Password $password `
            -Algorithm $HashAlgorithm `
            -PasswordIterations $Iterations `
            -SaltSize $SaltBytes `
            -HashSize $HashBytes `
            -Rng $rng

        $document = [ordered]@{
            id                    = $documentId
            pk                    = $documentId
            docType               = "adminUser"
            username              = $username
            passwordHash          = $hashData.PasswordHash
            passwordSalt          = $hashData.PasswordSalt
            passwordIterations    = $Iterations
            passwordHashAlgorithm = $HashAlgorithm
            isActive              = $true
            createdUtc            = $nowUtc
        }

        if ($IncludePlaintextPassword) {
            $document["password"] = $password
        }

        $documents += [pscustomobject]$document
    }

    $json = $documents | ConvertTo-Json -Depth 5
    if ([string]::IsNullOrWhiteSpace($OutputPath)) {
        $json
    }
    else {
        Set-Content -Path $OutputPath -Value $json -Encoding UTF8
        Write-Host "Generated $($documents.Count) admin document(s) at: $OutputPath"
    }
}
finally {
    $rng.Dispose()
}
