'           
'        _____ _                            _             
'       / ____| |                          (_)            
'      | (___ | |_ _ __ ___  __ _ _ __ ___  _ _ __   __ _ 
'       \___ \| __| '__/ _ \/ _` | '_ ` _ \| | '_ \ / _` |
'       ____) | |_| | |  __/ (_| | | | | | | | | | | (_| |
'      |_____/ \__|_|  \___|\__,_|_| |_| |_|_|_| |_|\__, |
'          /\   | |    | |                           __/ |
'         /  \  | | ___| |__   ___ _ __ ___  _   _  |___/ 
'        / /\ \ | |/ __| '_ \ / _ \ '_ ` _ \| | | |       
'       / ____ \| | (__| | | |  __/ | | | | | |_| |       
'      /_/    \_\_|\___|_| |_|\___|_| |_| |_|\__, |       
'                                             __/ |       
'                                            |___/    
'
'          STREAMING ALCHEMY - Season 03 Episode 10
'
'                Sponsored by Gnural Net, Inc.
'                     www.gnuralnet.com
'                       609-223-2434
'
'
' *************************************************************
' *****  Load Player Data based on Jersey Number & Team   *****
' *************************************************************

' *********************************************************
' Run when TEAM A is selected
' *********************************************************


' Set up Player Data variables

dim PlayerFullName as String	    ' Full Name of Player
dim PlayerFirstName as String	    ' First Name of Player
dim PlayerLastName as String	    ' Last Name of Player
dim PlayerPosition as String	    ' Position of Player

dim PlayerImageURI as String	    ' Image URI of Player
dim PlayerAge as String	    		' Age of Player
dim PlayerHeight as String	    	' Height of Player
dim PlayerWeight as String	    	' Weight of Player


dim BlankJerseyInput as Integer = 2	'The vMix Input Number of the Blank Jersey
dim GFX1Input as Integer = 3  		'The vMix Input Number of the First GFX Title

	
    ' Set up access to the vMix XML model
    dim VmixXML as new system.xml.xmldocument
	VmixXML.loadxml(API.XML)

	dim TeamAPlayerNumber as String = VmixXML.selectSingleNode("/vmix/dynamic/value1").InnerText
	dim TeamBPlayerNumber as String = VmixXML.selectSingleNode("/vmix/dynamic/value2").InnerText
	
	dim PlayerNumber as String = VmixXML.selectSingleNode("/vmix/dynamic/value3").InnerText

	If (PlayerNumber = "") Then
		 PlayerNumber = VmixXML.selectSingleNode("/vmix/dynamic/value1").InnerText
	End If

	If (PlayerNumber = "") Then
			API.Function("SetText", input:=BlankJerseyInput, SelectedName:="LASTNAME.Text", value:="")
	End If

	' Open the Team Roster File
	dim TeamRosterFolder As String = "C:/ActiveShow"
	dim TeamARosterData As New XmlDataDocument()
	dim TeamARosterFileStream As New Filestream(TeamRosterFolder+"\TeamRoster.xml", FileMode.Open, FileAccess.Read)

	
	' Load the file with Team A roster and build Node List
	dim TeamARosterNodeList As XMLNodeList

	TeamARosterData.Load(TeamARosterFileStream)
	TeamARosterFileStream.Close()

	TeamARosterNodeList = TeamARosterData.selectSingleNode("/Roster/TeamA").ChildNodes

	' Check if any players are found for Team A 
	If ( TeamARosterNodeList Is Nothing ) Then 
    	Console.WriteLine("NodeList Failed - No Roster Found")
		API.Function("SetText", input:=BlankJerseyInput, SelectedName:="LASTNAME.Text", value:="No Team")
	End If

	' *********************************************************
	' Now get the details for the entered player number
	' *********************************************************

	dim PlayerXMLNode as XMLNode

	' This will create an XML Node with the selected player
	PlayerXMLNode = TeamARosterData.SelectSingleNode("/Roster/TeamA/Player[@number='" + PlayerNumber + "']")

    ' If no player with that number is found indicate it on blank jersey and exit	
	If PlayerXMLNode is Nothing
		console.writeline("No Player on Team A Listed in Roster")
		API.Function("SetText", input:=BlankJerseyInput, SelectedName:="LASTNAME.Text", value:="Not Found")
		Exit Sub
	Else
		' If player is found, Fill in the variables with their data
        
        PlayerFirstName = TeamARosterData.SelectSingleNode("/Roster/TeamA/Player[@number='" + PlayerNumber + "']/FirstName").InnerText
		PlayerLastName = TeamARosterData.SelectSingleNode("/Roster/TeamA/Player[@number='" + PlayerNumber + "']/LastName").InnerText
		PlayerPosition = TeamARosterData.SelectSingleNode("/Roster/TeamA/Player[@number='" + PlayerNumber + "']/Position").InnerText
		PlayerImageURI = TeamARosterData.SelectSingleNode("/Roster/TeamA/Player[@number='" + PlayerNumber + "']/ImageURI").InnerText
		PlayerAge = TeamARosterData.SelectSingleNode("/Roster/TeamA/Player[@number='" + PlayerNumber + "']/Age").InnerText
		PlayerHeight = TeamARosterData.SelectSingleNode("/Roster/TeamA/Player[@number='" + PlayerNumber + "']/Height").InnerText
		PlayerWeight = TeamARosterData.SelectSingleNode("/Roster/TeamA/Player[@number='" + PlayerNumber + "']/Weight").InnerText
	End If

	' Populate the title templates with their data
	PlayerFullName = PlayerFirstName+" "+PlayerLastName

	API.Function("SetText", input:=BlankJerseyInput, SelectedName:="LASTNAME.Text", value:=PlayerLastName)
	API.Function("SetText", input:=GFX1Input, SelectedName:="PLAYERNAME.Text", value:=PlayerFullName)
	API.Function("SetText", input:=GFX1Input, SelectedName:="PLAYERPOSITION.Text", value:=PlayerPosition)
	API.Function("SetImage", input:=GFX1Input, SelectedName:="PLAYERIMAGE.Source", value:=(TeamRosterFolder+"/"+PlayerImageURI))
	API.Function("SetText", input:=GFX1Input, SelectedName:="PLAYERLASTNAME.Text", value:=PlayerLastName)
	API.Function("SetText", input:=GFX1Input, SelectedName:="PLAYERNUMBER.Text", value:=PlayerNumber)
	API.Function("SetText", input:=GFX1Input, SelectedName:="PLAYERAGE.Text", value:=PlayerAge)
	API.Function("SetText", input:=GFX1Input, SelectedName:="PLAYERHEIGHT.Text", value:=PlayerHeight)
	API.Function("SetText", input:=GFX1Input, SelectedName:="PLAYERWEIGHT.Text", value:=PlayerWeight)

	' Display Team A Jersey and hide Team B Jersey
	API.Function("SetImageVisibleOn", input:=GFX1Input, SelectedName:="JERSEYA.Source")
	API.Function("SetImageVisibleOff", input:=GFX1Input, SelectedName:="JERSEYB.Source")

	' Set the current Team A player to this Player Number
	API.Function("SetDynamicValue1", Value:=PlayerNumber)
	' Reset KeyPad value for a new Player Number to be entered 
	API.Function("SetDynamicValue3", Value:="")

        



	

