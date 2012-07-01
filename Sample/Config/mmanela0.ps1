# Import from a common settings file
import-config (Resolve-Path .\common.ps1)

# Set a normal variable
$config:Name = "Matt"

# Set a dictionary variable
$config:Langs = @{
  "en-us" = 1033;
  "it-it" = 1040;
  "de-de" = 1031;
  "es-es" = 3082;
}

# Set an array variable
$config:Colors = @("Red", "Blue", "Green", "Yellow")

# Use inherited value to create new derived value of datetime
$config:Tomorrow = $config:Today.AddDays(1)

$config:ThingEnabled = $true