[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO") | Out-Null
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoExtended") | Out-Null
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.ConnectionInfo") | Out-Null
[System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SmoEnum") | Out-Null
Import-Module BitsTransfer
$backupSourceDir = 'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQLEXPRESS\MSSQL\Backup\'
$serverInstance = ".\SQLEXPRESS"
$mainDB = "macheteStageProd"
$accountsDB = "machetelogStageProd"
$logDB = "aspnetdbStageProd"
$archiveDir = "C:\Archives\"
$bakExt = ".bak"
$zipExt = ".zip"
$zipexe = "c:\program files (x86)\7-zip\7z.exe "
$backupCopy = "\\casaserv\Common Documents\Administracion\2011 Machete\SQL_BACKUPS"
$password = ConvertTo-SecureString -String "qwe1234" -AsPlainText -Force
$cred = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList "casaserv\jimmy", $password

function MAIN () {
    $a = Get-Date
    $predicate = "SQL" + $a.Year + "-" + $a.Month + "-" + $a.Day + "-machete-"
    backupDB $backupSourceDir $predicate $mainDB
    backupDB $backupSourceDir $predicate $accountsDB
    backupDB $backupSourceDir $predicate $logDB    
    $archiveFile = $archiveDir + $predicate + $mainDB + $zipExt
    [array]$arguments = "a", $archiveFile, $($backupSourceDir+$predicate+"*")
    $arguments
    & $zipexe $arguments 
    # I hate doing this, but fighting with powershell seems like a waste
    # can't get invoke-expression to take params, can't get wait to work with &
    Start-sleep -milliseconds 50000
    rm $($backupSourceDir + $predicate + "*")
    Start-BitsTransfer -source $archiveFile -Destination $backupCopy -Credential $cred    
}

function backupDB ([System.String] $backupSourceDir, [System.String] $predicate, [System.String] $databaseName) {
    $backupFile = $predicate + $databaseName + $bakExt
    $server = New-Object ("Microsoft.SqlServer.Management.Smo.Server") ($serverInstance)

    $dbBackup = new-Object ("Microsoft.SqlServer.Management.Smo.Backup")
    $dbRestore = new-object ("Microsoft.SqlServer.Management.Smo.Restore")

    $dbBackup.Database = $databaseName
    $dbBackup.Devices.AddDevice($backupSourceDir + $backupFile, "File")
    $dbBackup.Action="Database"
    $dbBackup.Initialize = $TRUE
    $dbBackup.SqlBackup($server)

    #if(!(Test-Path \\dataserver\folder\dbName.bak)){
    #  $smtp = new-object Net.Mail.SmtpClient("emailserver")
    #  $smtp.Send("from", "to", "Backups not working", "Action required immediately for Full Backup")
    #  Exit
    #}

    $dbRestore.Devices.AddDevice($backupSourceDir + $backupFile, "File")
    if (!($dbRestore.SqlVerify($server))){
     $smtp = new-object Net.Mail.SmtpClient("casaserv")
     $smtp.Send("machete@casa-latina.org", "jimmy.carter.ii@gmail.com", "Backups not valid", "SQL Server. Backup verify failed. Action required immediately for Full Backup")

     Exit
    }    
}


MAIN
