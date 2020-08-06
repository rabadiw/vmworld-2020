Describe 'Reminders API' {

	BeforeAll {
		$remindersUrl = 'http://localhost:62741/api/reminders'
	}

	It 'Given Accept header of XML' {
		
		$resp = Invoke-WebRequest -Headers @{"Accept" = "application/xml" } -Method Get -Uri $remindersUrl

		$resp.StatusCode | Should -Be 200

		([xml]$resp.Content).ArrayOfReminderDto.ReminderDto.Count | Should -Be 3

		#Write-Information $resp.Content -InformationAction Continue

	}

	It 'Given Accept header of JSON' {
		$resp = Invoke-WebRequest -Headers @{"Accept" = "application/json" } -Method Get -Uri $remindersUrl
		$resp.StatusCode | Should -Be 200

		($resp.Content | ConvertFrom-JSON).Count | Should -Be 3

		#Write-Information $resp.Content -InformationAction Continue		
	}
}
