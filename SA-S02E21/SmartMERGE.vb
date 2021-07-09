' STREAMING ALCHEMY - Season 02 Episode 21
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'


' *************************************************************
' *****  Adjust LAYERS to perform Clean MERGE Transition  *****
' *************************************************************


'The KEYS of PROGRAM and PREVIEW
dim PROGInput as String = ""            ' The KEY (AS String) of the INPUT NOW in PROGRAM
dim PREVInput as String = ""            ' The KEY (AS String) of the INPUT NOW in PREVIEW

'INPUT NUMBERS of PROGRAM and PREVIEW
dim PROGInputNumber as String = ""      ' The NUMBER (AS String) of the INPUT NOW in PROGRAM
dim PREVInputNumber as String = ""      ' The NUMBER (AS String) of the INPUT NOW in PREVIEW

dim FoundLayer as String = ""           ' The INDEX of the Layer we Found


'LAYER Counts
dim TotalPROGLayers as Integer = 0      ' Total Number of layers in PROGRAM
dim TotalPREVLayers as Integer = 0      ' Total Number of layers in PREVIEW



'XML Components
dim PROGInputNodeList As XMLNodeList    ' NodeList of PROGRAM layers
dim PROGInputNode As XMLNode            ' The XMLNode of PROGRAM INPUT
dim PREVInputNodeList As XMLNodeList    ' NodeList of PREVIEW layers
dim PREVInputNode As XMLNode            ' The XMLNode of PREVIEW INPUT

dim FoundLayerNode As XMLNode           ' The XMLNode to test for Specific Layer with PROG or PREV input



' *****  PROCESSING STARTS HERE  *****

' Load the vMix XML Model
dim VmixXML as new system.xml.xmldocument
VmixXML.loadxml(API.XML)


' *****  GET DETAILS FOR INPUTS IN PROGRAM AND PREVIEW *****

'Get the XMLNode for the Input in PROGRAM:
PROGInputNumber = VmixXML.SelectSingleNode("/vmix/active").InnerText
PROGInputNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@number=" & PROGInputNumber & "]")
PROGInput = PROGInputNode.Attributes.GetNamedItem("key").Value

'Get the XMLNode for the Input in PREVIEW:
PREVInputNumber = VmixXML.SelectSingleNode("/vmix/preview").InnerText
PREVInputNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@number=" & PREVInputNumber & "]")
PREVInput = PREVInputNode.Attributes.GetNamedItem("key").Value 

'The XMLNodeList of LAYERS in PROGRAM:
PROGInputNodeList = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PROGInput & """]").SelectNodes("overlay") 

'The XMLNodeList of LAYERS in PREVIEW:
PREVInputNodeList = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PREVInput & """]").SelectNodes("overlay") 

'Get the count of layers we're working with:
TotalPROGLayers = PROGInputNodeList.Count

'Check to see if INPUT in PROGRAM has any layers

If TotalPROGLayers<=0 AND TotalPREVLayers>0 Then  'LAYERS are in PREVIEW with just an INPUT in PROGRAM
        'Check if PROGRAM's Input is in one of the PREVIEW's Layers and Assign it to FoundLAYER
    FoundLAYERNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PREVInput & """]/overlay[@key=""" & PROGInput & """]")
    
    If NOT FoundLAYERNode IS NOTHING Then   ' If there is a LAYER with the INPUT in PROGRAM...
        FoundLAYER = FoundLAYERNode.Attributes.GetNamedItem("index").Value    ' ...Assign the INDEX of the LAYER
        API.Function("MoveMultiViewOverlay", Input:=PREVInputNumber, value:=(CInt(FoundLAYER) + 1) & ",10")    'PREV:Move FoundLAYER to LAYER 10
        Sleep(200)
        API.Function("Merge")
        Sleep(1000)
        API.Function("MoveMultiViewOverlay", Input:=PREVInputNumber, value:="10," & (CInt(FoundLAYER) + 1))     'PROG:Move Layer 10 back to FoundLAYER
    Else
        API.Function("Merge")
    End If
    
ElseIf TotalPROGLayers>0 AND TotalPREVLayers<=0 Then  'LAYERS are in PROGRAM with an just INPUT in PREVIEW
    
    'Check if PROGRAM's Input is in one of the PREVIEW's Layers and Assign it to FoundLAYER
     FoundLAYERNode = VmixXML.SelectSingleNode("/vmix/inputs/input[@key=""" & PROGInput & """]/overlay[@key=""" & PREVInput & """]")
    
    If NOT FoundLAYERNode IS NOTHING Then   ' If there is a LAYER with the INPUT in PREVIEW...
        FoundLAYER = FoundLAYERNode.Attributes.GetNamedItem("index").Value    ' ...Assign the INDEX of the LAYER
        API.Function("MoveMultiViewOverlay", Input:=PROGInputNumber, value:=(CInt(FoundLAYER) + 1) & ",10")    'PROG:Move FoundLAYER to LAYER 10
        Sleep(200)
        API.Function("Merge")
        Sleep(1000)
        API.Function("MoveMultiViewOverlay", Input:=PROGInputNumber, value:="10," & (CInt(FoundLAYER) + 1)) 'PROG:Move Layer 10 back to FoundLAYER
    Else
        API.Function("Merge")  
    End If

Else 'Either both PROGRAM and PREVIEW have layers, or both have no layers
    API.Function("Merge")   'Just do the MERGE without any adjustments
End If

