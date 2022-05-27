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
'          STREAMING ALCHEMY - Season 03 Episode 06
'
'                Sponsored by Gnural Net, Inc.
'                     www.gnuralnet.com
'                       609-223-2434
'
'
' *************************************************************
' *****  Manage the remote guests active in a Green Room  *****
' *************************************************************



' *********************************************************
' Set up Green Room Details
' *********************************************************

dim ChannelIndex as Integer         ' Counter to address layers in the Green Room
dim GreenRoomSlots as Integer = 9  ' Number of slots in the green room
dim ActiveChannel as Integer		' Channel currently selected
dim LTAChannelGuestState as String	' Guest State of selected channel
dim LTAChannelGuestName as String	' Guest Name of selected channel

dim LTAFirstInput as Integer = 11         'The vMix Input Number of the first LTA Input (9 Consecutive Inputs)
dim GreenRoomGuestInput as Integer = 9    'The vMix Input Number of the Green Room guest input (Layers)

' *********************************************************
' Run as a loop in the background checking channel status
' *********************************************************
while (true)
	
    ' Set up access to the vMix XML model
    dim VmixXML as new system.xml.xmldocument
	VmixXML.loadxml(API.XML)


	' Open the file with the channel status details
	dim LTAChannelStatusFolder As String = "C:\LTA"
	dim LTAChannelStatusData As New XmlDataDocument()
	dim LTAChannelStatusFileStream As New Filestream(LTAChannelStatusFolder+"\LiveToAirMetadata.xml", FileMode.Open, FileAccess.Read)

	
	' Load the file with the channel status and build Node List
	dim LTAChannelStatusNodeList As XMLNodeList

	LTAChannelStatusData.Load(LTAChannelStatusFileStream)
	LTAChannelStatusFileStream.Close()
	LTAChannelStatusNodeList = LTAChannelStatusData.selectSingleNode("/LTA").ChildNodes

	If ( LTAChannelStatusNodeList Is Nothing ) Then 
    	Console.WriteLine("NodeList Failed - No Channels Found") 
	End If

	' *********************************************************
	' Now get the details for the state of each channel
	' *********************************************************
	For ActiveChannel = 0 to GreenRoomSlots-1
		' Pull the Name of guest in the selected channel
		LTAChannelGuestName = LTAChannelStatusNodeList(ActiveChannel).SelectSingleNode("Name").InnerText
		Console.Writeline("Channel: " & CStr(ActiveChannel) & "  State:" & LTAChannelGuestState & "  Name:" & LTAChannelGuestName )

		' Pull the Guest State of guest in the selected channel
		LTAChannelGuestState = LTAChannelStatusNodeList(ActiveChannel).SelectSingleNode("GuestState").InnerText

		Select Case LTAChannelGuestState
			Case "None"
				API.Function("SetLayer",input:=GreenRoomGuestInput,value:=CStr(ActiveChannel+1) & "," & "1")
				API.Function("SetText",Input:="GR-Overlay",SelectedName:="Name-CH" & CStr(ActiveChannel+1) & ".Text",Value:="OPEN SEAT")
				API.Function("SetColor",Input:="GR-Overlay",SelectedName:="BG-CH" & CStr(ActiveChannel+1) & ".Fill.Color",Value:="#075DAD")
			Case "Assigned"
				API.Function("SetLayer",input:=GreenRoomGuestInput,value:=CStr(ActiveChannel+1) & "," & "4")
				API.Function("SetText",Input:="GR-Overlay",SelectedName:="Name-CH" & CStr(ActiveChannel+1) & ".Text",Value:=LTAChannelGuestName)
				API.Function("SetColor",Input:="GR-Overlay",SelectedName:="BG-CH" & CStr(ActiveChannel+1) & ".Fill.Color",Value:="#AABB00")
			Case "OffAir"
				API.Function("SetLayer",input:=GreenRoomGuestInput,value:=CStr(ActiveChannel+1) & "," & CStr(ActiveChannel+LTAFirstInput))
				API.Function("SetText",Input:="GR-Overlay",SelectedName:="Name-CH" & CStr(ActiveChannel+1) & ".Text",Value:=LTAChannelGuestName)
				API.Function("SetColor",Input:="GR-Overlay",SelectedName:="BG-CH" & CStr(ActiveChannel+1) & ".Fill.Color",Value:="#108B00")
			Case "OnAir"
				API.Function("SetLayer",input:=GreenRoomGuestInput,value:=CStr(ActiveChannel+1) & "," & "3")
				API.Function("SetText",Input:="GR-Overlay",SelectedName:="Name-CH" & CStr(ActiveChannel+1) & ".Text",Value:=LTAChannelGuestName)
				API.Function("SetColor",Input:="GR-Overlay",SelectedName:="BG-CH" & CStr(ActiveChannel+1) & ".Fill.Color",Value:="#BE2501")
			Case Else
		End Select

	Next
	Sleep(300)
End while