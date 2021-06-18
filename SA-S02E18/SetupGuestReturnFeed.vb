' STREAMING ALCHEMY - Season 02 Episode 18
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'


' *****************************************************
' *****  Adaptive Return Feeds for Remote Guests  *****
' *****************************************************


'The KEYS of the Main INPUTS we are using
dim PROGInput as String = ""            ' The KEY of the INPUT NOW in PROGRAM

dim HOSTInput as String = ""            ' The KEY of Host INPUT
dim GUEST1Input as String = ""          ' The KEY of Guest 1 INPUT
dim GUEST2Input as String = ""          ' The KEY of Guest 2 INPUT
dim GUEST3Input as String = ""          ' The KEY of Guest 3 INPUT
dim GUEST4Input as String = ""          ' The KEY of Guest 4 INPUT

'The KEYS of INPUTS we are manipulating
dim Content1PROGLayer As String = ""    ' The input number of the first Layer of CONTENT in PROGRAM
dim Content2PROGLayer As String = ""    ' The input number of the second Layer of CONTENT in PROGRAM

dim Person1PROGLayer As String = ""     ' The input number of the first Other Person in PROGRAM
dim Person2PROGLayer As String = ""     ' The input number of the second Other Person in PROGRAM

dim tempKey1 as String = ""             ' Temp KEY storage when looking up corresponding Input NUMBER
dim tempKey2 as String = ""             ' Temp KEY storage when looking up corresponding Input NUMBER

dim Content0Key as String = ""          ' Content KEY storage when looking up corresponding Input NUMBER
dim Content1Key as String = ""          ' Content KEY storage when looking up corresponding Input NUMBER

dim LayerInputKEY as String = ""        ' The KEY of the Layer I'm working on now


'Counters and looping indicies
dim TotalPROGLayers as Integer = 0      ' Total Number of layers in PROGRAM
dim AssetPROGLayers as Integer = 0      ' Total Number of layers in PROGRAM with Non-People Assets Should be 
dim CurrentPROGLayer As Integer = 0     ' The Current Layer in PROGRAM being worked on

'INPUT NUMBERS used to make MultiView API Calls
dim PROGInputNumber as String = ""      ' The NUMBER (AS String) of the INPUT NOW in PROGRAM
dim HOSTInputNumber as String = ""      ' The NUMBER (AS String) of the HOST INPUT

dim ReturnInputNumber as String = "15"  ' The NUMBER (AS String) of the RETURN INPUT CURRENTLY INPUT 15


'XML Components
dim PROGInputNodeList As XMLNodeList    ' NodeList of PROGRAM layers
dim ContentNodes As XmlNodeList         ' NodeList of PROGRAM layers with CONTENT (Not Guests)
dim PeopleNodes As XmlNodeList          ' NodeList of PROGRAM layers with OTHER PEOPLE (Not Guest 1 or Content)

dim PROGInputNode As XMLNode            ' The XMLNode of PROGRAM INPUT

dim GUEST1InputNode As XMLNode          ' The XMLNode of Guest 1 INPUT
dim GUEST2InputNode As XMLNode          ' The XMLNode of Guest 2 INPUT
dim GUEST3InputNode As XMLNode          ' The XMLNode of Guest 3 INPUT
dim GUEST4InputNode As XMLNode          ' The XMLNode of Guest 4 INPUT
dim HOSTInputNode As XMLNode            ' The XMLNode of Host INPUT
dim TEMPInputNode As XMLNode            ' The XMLNode to test for Specific Layers in PROG input



' *****  PROCESSING STARTS HERE  *****

' Load the vMix XML Model
dim VmixXML as new system.xml.xmldocument
VmixXML.loadxml(API.XML)

' Get the XMLNode and INPUT KEY of Guest 1
GUEST1InputNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@title=""Guest-01""]")
GUEST1Input = GUEST1InputNode.Attributes.GetNamedItem("key").Value

' Get the XMLNode and INPUT KEY of Guest 2
GUEST2InputNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@title=""Guest-02""]")
GUEST2Input = GUEST2InputNode.Attributes.GetNamedItem("key").Value

' Get the XMLNode and INPUT KEY of Guest 3
GUEST3InputNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@title=""Guest-03""]")
GUEST3Input = GUEST3InputNode.Attributes.GetNamedItem("key").Value

' Get the XMLNode and INPUT KEY of Guest 4
GUEST4InputNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@title=""Guest-04""]")
GUEST4Input = GUEST4InputNode.Attributes.GetNamedItem("key").Value

' Get the XMLNode and INPUT KEY of HOST
HOSTInputNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@title=""Host""]")
HOSTInput = HOSTInputNode.Attributes.GetNamedItem("key").Value
HOSTInputNumber = HOSTInputNode.Attributes.GetNamedItem("number").Value

' *****  GET DETAILS FOR INPUT IN PROGRAM  *****

'Get the XMLNode for the Input in PROGRAM:
PROGInputNumber = VmixXML.SelectSingleNode("/vmix/active").InnerText
PROGInputNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@number=" & PROGInputNumber & "]")
PROGInput = PROGInputNode.Attributes.GetNamedItem("key").Value 

'The XMLNodeList of LAYERS in program:
PROGInputNodeList = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PROGInput & """]").SelectNodes("overlay") 

'Get the count of layers we're working with:
TotalPROGLayers = PROGInputNodeList.Count


' *****  CONFIGURE RETURN FEED FOR GUEST 1  *****

'Check to see if INPUT in PROGRAM has any layers

If TotalPROGLayers<=0 Then  'There are no layers - so check if the base input is another guests/host

    ' The logic here is that we display small video windows at the bottom of the GUEST RETURN FEED with all of the other guests and host
    ' However, if any guests/host are displayed as part of main video being sent back, we remove them from this bottom row

    If PROGInput = GUEST2Input Then  
        API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=2)    ' If Guest 2 is in PROGRAM turn off their video window...
    Else
        API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=2)     ' ...otherwise make sure it is turned on
    End If
    If PROGInput = GUEST3Input Then  
        API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=3)    ' If Guest 3 is in PROGRAM turn off their video window...
    Else
        API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=3)     ' ...otherwise make sure it is turned on
    End If
    If PROGInput = GUEST4Input Then  
        API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=4)    ' If Guest 4 is in PROGRAM turn off their video window...
    Else
        API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=4)     ' ...otherwise make sure it is turned on
    End If
    If PROGInput = HOSTInput Then  
        API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=5)    ' If HOST is in PROGRAM turn off their video window...
    Else
        API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=5)     ' ...otherwise make sure it is turned on
    End If
    
    If PROGInput = GUEST1Input  ' The program video is the Guest we're sending this video back to
            
            ' In this case, we'll send the guest the HOST video instead
            API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=5)     ' Turn Off host video layer in bottom row
            API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=7)     ' Turn Off the Multi Content Layers
            API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=8)
            API.Function("SetMultiViewOverlay", Input:=ReturnInputNumber, Value:="6," & HostInputNumber)    ' Assign Host Video to single content layer
            API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=6)      ' Turn On Single Content Layer
    Else    ' The program video is another input
            
            API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=7)     ' Turn Off the Multi Content Layers
            API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=8)
            API.Function("SetMultiViewOverlay", Input:=ReturnInputNumber, Value:="6," & PROGInputNumber)    ' Assign Program Input to single content layer
            API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=6)      ' Turn On Single Content Layer
    End If
    
Else    'There are Layers, so we need to deconstruct them
    
    ' Check if GUEST 1 video are in any of the layers
    TEMPInputNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PROGInput & """]/overlay[@key=""" & GUEST1Input & """]") 

    If TEMPInputNode Is Nothing Then    ' Guest 1 is NOT being shown in program  
        ' If any guests/host are displayed as part of main video being sent back, we remove them from this bottom row

        If VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PROGInput & """]/overlay[@key=""" & GUEST2Input & """]") IsNot Nothing Then  
            API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=2)    ' If Guest 2 is in PROGRAM turn off their video window...
        Else
            API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=2)     ' ...otherwise make sure it is turned on
        End If
        If VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PROGInput & """]/overlay[@key=""" & GUEST3Input & """]") IsNot Nothing Then  
            API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=3)    ' If Guest 3 is in PROGRAM turn off their video window...
        Else
            API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=3)     ' ...otherwise make sure it is turned on
        End If
        If VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PROGInput & """]/overlay[@key=""" & GUEST4Input & """]") IsNot Nothing Then  
            API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=4)    ' If Guest 4 is in PROGRAM turn off their video window...
        Else
            API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=4)     ' ...otherwise make sure it is turned on
        End If
        If VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PROGInput & """]/overlay[@key=""" & HOSTInput & """]") IsNot Nothing Then  
            API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=5)    ' If HOST is in PROGRAM turn off their video window...
        Else
            API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=5)     ' ...otherwise make sure it is turned on
        End If

        ' Since Guest 1 is NOT in PROGRAM:
        API.Function("SetMultiViewOverlay", Input:=ReturnInputNumber, Value:="6," & PROGInputNumber)   ' Set Single Return Layer to INPUT in PROGRAM
        API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=6)                         ' Turn ON Single Return Layer
        API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=7)                        ' Turn OFF Multi Content Layer 1
        API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=8)                        ' Turn OFF Multi Content Layer 2
    
    Else    '  Guest 1 IS being shown in program 
        
        ' With Guest 1 in PROGRAM, we can't use it directly so we need to pull it apart

        ' Turn on all of the other guest videos
        API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=2)
        API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=3)
        API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=4)
        API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=5)

        ' First find what layers there are with just content (Slides, Media, Etc) - If Any

        ' Create a NodeList of Layers WITHOUT without Guests or Host
        ContentNodes = VmixXML.SelectNodes("/vmix/inputs/input[@key=""" & PROGInput & """]/overlay/@key[.!=""" & GUEST1Input & """ and .!=""" & GUEST2Input & """ and .!=""" & GUEST3Input & """ and .!=""" & GUEST4Input & """ and .!=""" & HOSTInput & """]")
        
        If ContentNodes.Count <> 0 then         ' There is a layer with Content
            
            If ContentNodes.Count = 1 Then      ' There is only ONE Content layer
                
                ' Get the KEY of the INPUT in that content layer and find its NUMBER
                tempKey1 = ContentNodes.Item(0).Value
                Content1PROGLayer = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & tempKey1 & """]").Attributes.GetNamedItem("number").Value
                
                          
                API.Function("SetMultiViewOverlay", Input:=ReturnInputNumber, Value:="6," & Content1PROGLayer) ' Set Single Return Layer to Content Input
                API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=6)                         ' Turn ON Single Return Layer
                API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=7)                        ' Turn OFF Multi Return Layer 1
                API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=8)                        ' Turn OFF Multi Return Layer 2
           
           Else If ContentNodes.Count > 1 Then     ' There is more than ONE content layer (We'll only work with TWO)
                
                Content0Key = ContentNodes.Item(0).Value    ' This is the KEY of the INPUT in the First CONTENT Layer
                Content1Key = ContentNodes.Item(1).Value    ' This is the KEY of the INPUT in the Second CONTENT Layer

                ' Since the API Calls need Input NUMBERS, use the KEYS to find them
                Content1PROGLayer = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & Content0Key & """]").Attributes.GetNamedItem("number").Value
                Content2PROGLayer = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & Content1Key & """]").Attributes.GetNamedItem("number").Value

                API.Function("SetMultiViewOverlay", Input:=ReturnInputNumber, Value:="7," & Content1PROGLayer) ' Set Multi Return Layer to first Content INPUT
                API.Function("SetMultiViewOverlay", Input:=ReturnInputNumber, Value:="8," & Content2PROGLayer) ' Set Multi Return Layer to second Content INPUT
                API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=6)                        ' Turn OFF Single Return Layer
                API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=7)                         ' Turn On Multi Return Layer 1
                API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=8)                         ' Turn On Multi Return Layer 2

            End If 
            
        Else    ' There are no layers with Content - see if there are layers with Other People

            ' Create an XMLNodelist with OTHER GUESTS or HOST (Specifically NOT with GUEST1)
            PeopleNodes = VmixXML.SelectNodes("/vmix/inputs/input[@key=""" & PROGInput & """]/overlay/@key[.!=""" & GUEST1Input & """]")

            ' Start off with all of the People Layers turned ON in the RETURN          
            API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=2)
            API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=3)
            API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=4) 
            API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=5)

            
            If PeopleNodes.Count = 1 Then       ' There is exactly 1 OTHER Person in PROGRAM (Not GUEST 1)
                
                ' Get the Input KEY of this person and find their INPUT NUMBER
                tempKey1 = PeopleNodes.Item(0).Value
                Person1PROGLayer = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & tempKey1 & """]").Attributes.GetNamedItem("number").Value
                
                ' Find out which OTHER PERSON they are and shut off their Return Layer
                API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=2)          ' If Guest 2 is in PROGRAM turn off their video window...
                If tempKey1 = GUEST2Input Then
                    API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=2)    ' If Guest 2 is OTHER PERSON turn off their video window...
                End If

                If tempKey1 = GUEST3Input Then
                    API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=3)    ' If Guest 3 is OTHER PERSON turn off their video window...
                End If
                                
                If tempKey1 = GUEST4Input Then
                    API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=4)    ' If Guest 3 is OTHER PERSON turn off their video window...
                End If
                                
                If tempKey1 = HOSTInput Then
                    API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=5)    ' If HOST is OTHER PERSON turn off their video window...
                End If

                ' Now adjust the other Return Layers
                API.Function("SetMultiViewOverlay", Input:=ReturnInputNumber, Value:="6," & Person1PROGLayer)  ' Set Single Return Layer to OTHER PERSON
                API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=6)                         ' Turn ON Single Return Layer
                API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=7)                        ' Turn OFF Multi Return Layer 1
                API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=8)                        ' Turn OFF Multi Return Layer 2
            End If

            If PeopleNodes.Count > 1 Then   ' There are more than 1 OTHER PEOPLE in PROGRAM - we'll just work with 2 in this code
                
                ' Get the Input KEY of first person and find their INPUT NUMBER
                tempKey1 = PeopleNodes.Item(0).Value
                Person1PROGLayer = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & tempKey1 & """]").Attributes.GetNamedItem("number").Value
                
                ' Get the Input KEY of second person and find their INPUT NUMBER
                tempKey2 = PeopleNodes.Item(1).Value
                Person2PROGLayer = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & tempKey2 & """]").Attributes.GetNamedItem("number").Value
                
                ' Find out which OTHER PEOPLE they are and shut off their Return Layers
                If tempKey1 = GUEST2Input or tempKey2 = GUEST2Input Then
                    API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=2)    ' If Guest 2 is OTHER PERSON turn off their video window...
                End If

                If tempKey1 = GUEST3Input  or tempKey2 = GUEST3Input Then
                    API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=3)    ' If Guest 3 is OTHER PERSON turn off their video window...
                End If
                                
                If tempKey1 = GUEST4Input  or tempKey2 = GUEST4Input Then
                    API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=4)    ' If Guest 3 is OTHER PERSON turn off their video window...
                End If
                                
                If tempKey1 = HOSTInput  or tempKey2 = HOSTInput Then
                    API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=5)    ' If HOST is OTHER PERSON turn off their video window...
                End If

                ' Now adjust the other Return Layers
                API.Function("SetMultiViewOverlay", Input:=ReturnInputNumber, Value:="7," & Person1PROGLayer)   ' Set Secondary One Return to OTHER PERSON 1
                API.Function("SetMultiViewOverlay", Input:=ReturnInputNumber, Value:="8," & Person2PROGLayer)   ' Set Secondary Two Return to OTHER PERSON 2
                API.Function("MultiViewOverlayOff", Input:=ReturnInputNumber, Value:=6)                         ' Turn OFF Single Return Layer
                API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=7)                          ' Turn ON Multi Return Layer 1
                API.Function("MultiViewOverlayOn", Input:=ReturnInputNumber, Value:=8)                          ' Turn ON Multi Return Layer 2
            End If

        End If 

    End If

End If

