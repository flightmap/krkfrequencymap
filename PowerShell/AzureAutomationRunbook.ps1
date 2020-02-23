
$url = "https://data-live.flightradar24.com/zones/fcgi/feed.js?bounds=50.17,49.89,19.27,20.46&faa=1
&mlat=1&flarm=1&adsb=1&gnd=1&air=1&vehicles=1&estimated=1&maxage=14400&gliders=1&stats=1"
$sqlConnectionString = "Server=tcp:______.database.windows.net;Database=______;User ID=______writer;Password=______;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;"

$executions = 0;
while($executions -ne 360)
{
    $executions++ ; 
    Write-output "Execution number: $executions; $($executions*10) seconds running"
    sleep -seconds 10
    try
    {       
        $res = Invoke-WebRequest -Uri $url -UseBasicParsing
        if ($res.StatusCode -eq 200) 
        {
            $json = ConvertFrom-Json $res.Content
            $data = $json | Select-Object * -ExcludeProperty full_count,version,stats
            $names = $data | Get-Member -MemberType NoteProperty | select name
            foreach ($n in $names) 
            {
                $o = $data | select-object -Property $n.Name -ExpandProperty $n.Name

                $date = Get-Date
                $entry = New-Object -TypeName PSObject
                Add-Member -InputObject $entry -MemberType NoteProperty -Name Date -Value $date.ToString()
                Add-Member -InputObject $entry -MemberType NoteProperty -Name X -Value $o[1]
                Add-Member -InputObject $entry -MemberType NoteProperty -Name Y -Value $o[2]
                Add-Member -InputObject $entry -MemberType NoteProperty -Name Z -Value $o[4]
                Add-Member -InputObject $entry -MemberType NoteProperty -Name Track -Value $o[3]
                Add-Member -InputObject $entry -MemberType NoteProperty -Name Airplane -Value $o[8]
                Add-Member -InputObject $entry -MemberType NoteProperty -Name From -Value $o[11]
                Add-Member -InputObject $entry -MemberType NoteProperty -Name To -Value $o[12]
                Add-Member -InputObject $entry -MemberType NoteProperty -Name FlightNo -Value $o[13]
                Add-Member -InputObject $entry -MemberType NoteProperty -Name FlightNo2 -Value $o[16]

                
                $Conn = New-Object System.Data.SqlClient.SqlConnection($sqlConnectionString) 

                $query = "INSERT INTO [dbo].[tab_flightdataIDs]			 ([date],[X],[Y],[Z],[Track],[Airplane],[From],[To],[FlightNo],[FlightNo2]) 				 VALUES  ('$($entry.Date)','$($entry.X)','$($entry.Y)','$($entry.Z)','$($entry.Track)','$($entry.Airplane)','$($entry.From)','$($entry.To)','$($entry.FlightNo)','$($entry.FlightNo2)');"
                
                # Open the SQL connection 
                $Conn.Open() 

                # Define the SQL command to run.
                $Cmd = new-object system.Data.SqlClient.SqlCommand($query, $Conn) 
                $Cmd.CommandTimeout=120 

                # Execute the SQL command 
                $Da = New-Object system.Data.SqlClient.SqlDataAdapter($Cmd) 
                $Ds = New-Object System.Data.DataSet
            
                $Da.Fill($Ds) | out-null

                # Close the SQL connection 
                $Conn.Close()

            }	
        }
        else
        {
			Write-Output "CANNOT LOAD DATA FROM FR."
			Write-Output $res
        }
        
    } 
    catch
    {
        Write-Output $_.Exception.Message
    }
}





 
 
 
