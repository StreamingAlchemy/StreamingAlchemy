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
'          STREAMING ALCHEMY - Season 03 Episode 01
'
'                Sponsored by Gnural Net, Inc.
'                     www.gnuralnet.com
'                       609-223-2434
'
'
' ***********************************************************
' *****  Selecting Best Available Camera for Target #2  *****
' ***********************************************************



' **********************************************************
' The NUMBER of the Target Set we want to use
' For this script we are setting the TARGET to 2 
dim TargetSet as String = "2"   
' **********************************************************

' Target Related Variables
dim TargetShotCamera as String = ""         ' The CAMERA associated with a priority for a given target
dim TargetSources(12) As String             ' Input Keys of Target sources
Dim TargetSets As String                    ' Count of total Targets with Sets of Shots
dim TargetCount As Integer                  ' Integer of Toital Targets
dim TargetIndex As Integer                  ' Index for looping through Targets

dim ShotIndex As Integer = 0                ' Index for the Target Shot being evaluated
dim ShotCount As Integer = 0                ' Number of Shots associated with a specific Target
dim ShotKey As String                       ' The Input Key of the Shot
dim ShotName As String                      ' Name (title) of the VMix Virtual Input associated with a camera shot
dim ShotCameraName As String                ' Name (title) of the Camera Input associated with a Virtual Input
dim ShotCameraKey As String                 ' KEY of the Camera Input associated with a Virtual Input
dim FailoverShotName As String              ' Name of shot to take if no other shot is available

dim XmlTargetNodeList As XmlNodeList        ' The list of all Target Nodes

' Layer Related Variables
dim LayerInputNumber as String = ""         ' The NUMBER of the Layer I'm working on now
dim LayerInputTitle as String = ""          ' The TITLE of the Layer I'm working on now
dim LayerInputKey as String = ""            ' The KEY of the Layer I'm working on now
dim TotalLayers As Integer = 0              ' Total Number of Layers in PROG
dim CurrentLayer As Integer = 0             ' The layer currently being examined

'Camera Related Variables
dim CameraPriorityIndex As Integer = 0      ' Index for looping through camera priorities for Target 1
dim CameraPrioritySize As Integer = 0       ' Count of the camera priorities for Target 1
dim CameraCount As Integer = 0              ' Total Number of Cameras
dim CameraIndex As Integer = 0              ' Index for the Camera being evaluated
dim CameraName As String                    ' Name (title) of a Camera Input
dim CameraKey As String                     ' KEY of a Camera Input

dim XmlCameraNodeList As XmlNodeList        ' XML Node List with all of the Cameras

' PROGRAM Related Variables
dim LiveSources(12) As String              ' Input Keys of sources in PROGRAM
dim LiveInput as String = ""               ' The NUMBER of the INPUT in PROGRAM
dim LiveSourcesCount as Integer            ' The total count of INPUTs in PROGRAM
dim LiveSourcesIndex as Integer            ' The INPUT in PROGRAM being worked on

dim OverlayKey As String                   ' KEY of Overlay Input found in VMix PROGRAM (active)
dim OverlayIndex As Integer                ' Index used to loop through overlay layers in PROGRAM
dim OverlayCount As Integer                ' Total number of overlay layers in PROGRAM

' VMix Related Variables
dim InputCount As Integer = 0               ' Number of Inputs in vMix Session
dim InputIndex As Integer = 0               ' Current Input being worked on
Dim PreviewKey As String = ""               ' This will be the Input Key assigned to PREVIEW
Dim PreviewInput As String = ""             ' This will be the Input Number assigned to PREVIEW


' Temp Variables
dim GeneralString1 As String = ""           ' A multi-use string holder for testing
dim GeneralString2 As String = ""           ' A multi-use string holder for testing
dim GeneralIndex As Integer =0              ' A multi-use Index for testing



' ********************************************************
' **** Begin Collecting Structural Information Needed ****
' ********************************************************


' ---------------------------------
' ---- Load the vMix XML Model ----
' ---------------------------------

dim VmixXML as new system.xml.xmldocument
VmixXML.loadxml(API.XML)


' ----------------------------------------------------
' ---- Process the Target/Shot/Camera Information ----
' ----------------------------------------------------

' Open the file with Target/Camera MetaData
dim MetaDataFolder As String = "c:\ActiveShow"
dim XMLTargetsMetaData As New XmlDataDocument()
dim TargetsFileStream As New Filestream(MetaDataFolder+"\XMLTargetsMetaData.xml", FileMode.Open, FileAccess.Read)

' Load the file with Target/Shot/Camera MetaData
XMLTargetsMetaData.Load(TargetsFileStream)

' Get a list of All the Cameras Involved in the production
xmlCameraNodeList = XMLTargetsMetaData.selectSingleNode("/Targets/Cameras").SelectNodes("Camera")
CameraCount = xmlCameraNodeList.count

' Now generate a Node List of the Shots/Cameras associated with the Current Target
XmlTargetNodeList = XMLTargetsMetaData.selectSingleNode("/Targets/Target[@set=" & TargetSet & "]").SelectNodes("Shot")
ShotCount = XmlTargetNodeList.count
Console.WriteLine("Shots: " & ShotCount)

' Get the FAILOVER shot if no other shot is found
FailOverShotName = XMLTargetsMetaData.selectSingleNode("/Targets/Failover").InnerText
Console.WriteLine("Failover Input: " & FailoverShotName)

' Allocate space for the input keys of the Camera Inputs and Shot Inputs being used
' [We needed to find the counts associated with each list before allocating them]
dim CameraKeys(CameraCount) as String
dim ShotKeys(ShotCount) as String
dim ShotCameraKeys(ShotCount) as String

' Fill the Camera List with all of their associated Input KEYS
For CameraIndex = 0 To CameraCount-1
    CameraName = xmlCameraNodeList.Item(CameraIndex).InnerText
    CameraKey = VmixXML.selectSingleNode("/vmix/inputs/input[@title=""" & CameraName & """]").Attributes.GetNamedItem("key").Value
    CameraKeys(CameraIndex)=CameraKey
'    Console.WriteLine("Camera #" & CameraIndex & ": " & CameraName & "   Key: " & CameraKey)
Next

' ----------------------------------------------------------------------------------------------
' ---- Fill the Shot List with all of their associated Input KEYS                           ----
' ---- Fill the ShotCamera List with the Input KEYS of the Camera associated with each Shot ----
' ----------------------------------------------------------------------------------------------

For ShotIndex = 0 To ShotCount-1    ' Loop through all of the Shots for the current Target
    ' Pull the Input Names of the Shots and Cameras from the XML File
    ShotName = xmlTargetNodeList.Item(ShotIndex).Attributes.GetNamedItem("input").Value
    ShotCameraName = xmlTargetNodeList.Item(ShotIndex).Attributes.GetNamedItem("camera").Value

    ' Use the Input Names to get the Input Keys
    ShotKey = VmixXML.selectSingleNode("/vmix/inputs/input[@shortTitle=""" & ShotName & """]").Attributes.GetNamedItem("key").Value
    ShotCameraKey = VmixXML.selectSingleNode("/vmix/inputs/input[@title=""" & ShotCameraName & """]").Attributes.GetNamedItem("key").Value
    
    ' Save the Input Keys
    ShotKeys(ShotIndex) = ShotKey
    ShotCameraKeys(ShotIndex) = ShotCameraKey
    Console.WriteLine("Shot #" & ShotIndex &": "& ShotName & "   Key: " & ShotKey & "  Array: " & ShotKeys(ShotIndex) &"  Camera: " & ShotCameraName & "   Camera Key: " & ShotCameraKey)
Next


' ---------------------------------------------------------------------------------------
' ---- To know which cameras are associated with shots in PROGRAM, set Up Dictionary ----
' ---- that Maps ALL Virtual Inputs (Shots) for ALL Targets to their Cameras Inputs  ----
' ---------------------------------------------------------------------------------------

' Create a dictionary of Strings associating Virtual Input Keys with Camera Input Keys
dim CameraMap as New System.Collections.Generic.Dictionary(Of String, String)
dim AssociatedCameraKey As String = Nothing


'if XMLTargetsMetaData.selectSingleNode("/Targets") is Nothing Then
'    Console.WriteLine("Invalid XMLTarget")
'End If
'Console.WriteLine(XMLTargetsMetaData.selectSingleNode("/Targets").OuterXml)

' Load the number of Target Sets (Sets of Shots associated with a Target)
TargetSets = XMLTargetsMetaData.selectSingleNode("/Targets").Attributes.GetNamedItem("sets").Value
TargetCount = CInt(TargetSets)

' Loop through the Target sets
For TargetIndex = 1 To TargetCount
    TargetSet = CStr(TargetIndex)
    
    ' For each Target Set generate a Node List of the Shots/Cameras associated with this Target
    XmlTargetNodeList = XMLTargetsMetaData.selectSingleNode("/Targets/Target[@set=" & TargetSet & "]").SelectNodes("Shot")
    ShotCount = XmlTargetNodeList.count ' This is the number of Shots associated with this target

    ' Loop through each Shot 
    For ShotIndex = 0 To ShotCount-1
        ' Get the Input Names (titles) of the Shot and associated Camera
        ShotName = xmlTargetNodeList.Item(ShotIndex).Attributes.GetNamedItem("input").Value
        ShotCameraName = xmlTargetNodeList.Item(ShotIndex).Attributes.GetNamedItem("camera").Value

        ' Using the Names, look up the KEYS associated with these Inputs
        ShotKey = VmixXML.selectSingleNode("/vmix/inputs/input[@shortTitle=""" & ShotName & """]").Attributes.GetNamedItem("key").Value
        ShotCameraKey = VmixXML.selectSingleNode("/vmix/inputs/input[@title=""" & ShotCameraName & """]").Attributes.GetNamedItem("key").Value

        ' Add these Shot Key/Camera Key Pairs to the Dictionary
        CameraMap.Add(ShotKey, ShotCameraKey)
    Next
Next



' ------------------------------------------------
' ---- Get the Input Keys for sources in PROG ----
' ------------------------------------------------

dim PROGLayersNodeList As XmlNodeList

'Get the XMLNode for the Input in PROGRAM:
LiveInput = VmixXML.selectSingleNode("/vmix/active").InnerText 
Console.WriteLine("Active Input " + LiveInput)

' LiveSources() will hold the keys to all of the inputs associated with PROGRAM

' Assign the Main Input Key to the first (0th) index of LiveSources
LiveSources(0) = VmixXML.selectSingleNode("/vmix/inputs/input[@number=""" & LiveInput & """]").Attributes.GetNamedItem("key").Value

'Create an XMLNodeList of any Overlay Layers (composited in Main Input) in the PROG:
PROGLayersNodeList = VmixXML.selectSingleNode("/vmix/inputs/input[@number=""" & LiveInput & """]").SelectNodes("overlay")

'OverlayKey = VmixXML.selectSingleNode("/vmix/inputs/input[@number=""" & LiveInput & """]").Attributes.GetNamedItem("key").Value
Console.WriteLine("Base Input Title: " & GeneralString1)

'Get the count of layers we're working with:
OverlayCount = PROGLayersNodeList.Count
LiveSourcesCount = OverlayCount+1   'The total number of sources equals Main Layer + Overlay Layers

' Loop through the NodeList with Overlay Layers and add their Input Keys to LiveSources
For OverlayIndex=0 To OverlayCount-1
    LiveSources(OverlayIndex+1) = PROGLayersNodeList.Item(OverlayIndex).Attributes.GetNamedItem("key").value
Next


' ------------------------------------------------------------------
' ---- Now we start the processing to find the best Target Shot ----
' ------------------------------------------------------------------

' LiveSources() has all the Input Keys in PROGRAM
' ShotKeys() has the Keys of the Shots in the current Target
' ShotCameraKeys() has the Keys of the Cameras in the Shots in the current Target
' CameraMap (Dictionary) has map of all Shot Keys to Camera Keys

' Loop through the Shots we have for the Current Target
For ShotIndex = 0 To ShotCount-1
    PreviewKey = ShotKeys(ShotIndex)    ' Assign that Key on a PROVISIONAL BASIS to what we place in PREVIEW
    

    ' Loop through the sources that are in PROGRAM
    For LiveSourcesIndex = 0 To LiveSourcesCount-1

'        Console.WriteLine("Live: " & LiveSources(LiveSourcesIndex) & "  Camera: " & ShotCameraKeys(ShotIndex) & "  Shot: " & ShotKeys(ShotIndex))
'        If ((ShotKeys(ShotIndex) = LiveSources(LiveSourcesIndex)) Or (ShotCameraKeys(ShotIndex) = LiveSources(LiveSourcesIndex))) Then
'            PreviewKey = ""
'        End If

        ' If the Exact Same Shot is in Target and PROGRAM, assign it to PREVIEW
        If (ShotKeys(ShotIndex) = LiveSources(LiveSourcesIndex)) Then
            Exit for    ' This is a GOOD SHOT so stop looking
        End If

        ' If the Same Camera is in Target and PROGRAM, it is no good
        If  (ShotCameraKeys(ShotIndex) = LiveSources(LiveSourcesIndex)) Then
            PreviewKey = ""     ' This isn't a good shot
        End If
        
        ' If a Shot using the Same Camera as the Target Shot is in PROGRAM, it is no good
        If CameraMap.TryGetValue(LiveSources(LiveSourcesIndex), AssociatedCameraKey) Then ' Look up shot in the dictionary
            If (ShotCameraKeys(ShotIndex) = AssociatedCameraKey) Then   ' If the Associated Camera is the same as in the New Target Shot Camera...
                PreviewKey = ""     ' ...this isn't a good shot
            End If
        End If
    Next

    ' Check if the Shot we just checked was any good
    If PreviewKey <> "" Then 
        Exit for    ' It is good so stop looking
    End If
    ' This Shot is not good, so check the next one
Next

' If we found a good shot for the selected Target...
If (PreviewKey <> "") Then
    Console.WriteLine("Preview Key: " & PreviewKey)
    PreviewInput = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""" & PreviewKey & """]").Attributes.GetNamedItem("number").Value
    API.Function("PreviewInput", Input:=PreviewInput)   ' ...assign it to preview
Else
    PreviewInput = VmixXML.selectSingleNode("/vmix/inputs/input[@title=""" & FailoverShotName & """]").Attributes.GetNamedItem("number").Value
    API.Function("PreviewInput", Input:=PreviewInput)   ' ...assign it to preview
End If