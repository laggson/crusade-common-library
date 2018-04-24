# Gets the line with the build date from Build.cs and replaces it with today
$path = ".\Build.cs"
$lineLike = "*public const string Date*"

$wholeFile = Get-Content $path

for($i=0; $i -lt $wholeFile.length; $i++)
{
	if($wholeFile[$i] -like $lineLike)
	{
		$wholeLine = $wholeFile[$i]
		$date = $wholeLine.Split("=")[1]
		$newDate = ' "' + (Get-Date -UFormat "%d.%m.%Y") + '";'
		
		$wholeFile[$i] = $wholeLine.Replace($date, $newDate)
		break
	}
}

Set-Content -Path $path -Value $wholeFile