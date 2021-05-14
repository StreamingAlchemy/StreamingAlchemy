' STREAMING ALCHEMY - Season 02 Episode 14
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'
'


dim ActiveInput as String = ""          ' The NUMBER of the INPUT in PROGRAM
dim ActiveOverlay as String = ""        ' The NUMBER of the Active Overlay
dim LayerInputNumber as String = ""     ' The NUMBER of the Layer I'm working on now
dim LayerInputTitle as String = ""      ' The TITLE of the Layer I'm working on now
dim AssignedInputKey as String = ""     ' The KEY of the specified Layer

'Counters used for looping through the layers
dim TotalLayers As Integer = 0          ' Total Number of Layers being Labeled
dim ActiveLayer As Integer = 0          ' The currently ACTIVE Layer

dim OverlayXML as new system.xml.xmldocument

'Load the vMix XML Model:
dim LayerNode As XMLNode

dim VmixXML as new system.xml.xmldocument
VmixXML.loadxml(API.XML)

'Get the XMLNode for the Input in PREVIEW:
ActiveInput = VmixXML.selectSingleNode("/vmix/active").InnerText 

'Create an XMLNodeList of LAYERS:
dim AllLayersNodeList As XmlNodeList = VmixXML.selectSingleNode("/vmix/inputs/input[@number=""" & ActiveInput & """]").SelectNodes("overlay") 

'Get the count of layers we're working with:
TotalLayers = AllLayersNodeList.Count

If TotalLayers<=0 Then Exit Sub 'Exit if there are no layers
    
'Assign the overlay with correct # of Slots to Overlay 4
' Overlay should be named SLOT-ID# where # corresponds to the number of layers

' Get the XMLNode of the correct overlay
LayerNode=VmixXML.selectSingleNode("/vmix/inputs/input[@title=""SLOT-ID" & CStr(TotalLayers) & """]")
ActiveOverlay = LayerNode.Attributes.GetNamedItem("number").Value


If ActiveOverlay Is Nothing Then Exit Sub 'If there is no active overlay for this layer count just exit


'Identify the Input associated with each layer and set the values in the Active Overlay to the Input Numbers
For ActiveLayer = 0 To TotalLayers-1
    AssignedInputKey = AllLayersNodeList.Item(ActiveLayer).Attributes.GetNamedItem("key").Value
    
    LayerInputNumber = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""" & AssignedInputKey & """]").Attributes.GetNamedItem("number").Value
    LayerInputTitle = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""" & AssignedInputKey & """]").Attributes.GetNamedItem("title").Value
    If LayerInputNumber Is Nothing Then Exit Sub
    
    'Set the Input Number for this layer in the ActiveOverlay
   
    API.Function("SetText", Value:=LayerInputNumber, SelectedName:=("SLOT" & CStr(ActiveLayer+1) & ".Text"), Input:=ActiveOverlay)
    API.Function("SetText", Value:=LayerInputTitle, SelectedName:=("NAME" & CStr(ActiveLayer+1) & ".Text"), Input:=ActiveOverlay)
    
Next

API.Function("OverlayInput4In",Input:=CStr(ActiveOverlay))




