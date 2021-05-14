' STREAMING ALCHEMY - Season 02 Episode 14
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'
'


dim PREVActiveInput as String = ""      ' The NUMBER of the INPUT in PREVIEW
dim PROGActiveInput as String = ""      ' The NUMBER of the INPUT in PROGRAM
dim ActiveOverlay as String = ""        ' The NUMBER of the Active Overlay
dim LayerInputNumber as String = ""     ' The NUMBER of the Layer I'm working on now
dim LayerInputTitle as String = ""      ' The TITLE of the Layer I'm working on now
dim AssignedInputKey as String = ""     ' The KEY of the specified Layer


dim TotalLayers As Integer = 0          ' Total Number of LAYERS being Labeled
dim ActiveLayer As Integer = 0          ' Total Number of LAYERS being Labeled

dim OverlayXML as new system.xml.xmldocument

'Load the vMix XML Model:
dim LayerNode As XMLNode

dim VmixXML as new system.xml.xmldocument
VmixXML.loadxml(API.XML)

'Get the XMLNode for the Inputs in PREVIEW and PROGRAM:
PREVActiveInput = VmixXML.selectSingleNode("/vmix/preview").InnerText
PROGActiveInput = VmixXML.selectSingleNode("/vmix/active").InnerText

If PREVActiveInput = PROGActiveInput Then
    Exit Sub 'Do NOT DISPLAY if Preview is also in Program
End If

API.Function("SetMultiviewOverlay",Input:=PREVActiveInput,  Value:="10,None")
API.Function("MultiViewOverlayOff",Input:=PREVActiveInput,  Value:="10")

'Reload the vMix XML to reflect the change in the MultiView we just did
VmixXML.loadxml(API.XML)

'Create an XMLNodeList of LAYERS:
dim AllLayersNodeList As XmlNodeList = VmixXML.selectSingleNode("/vmix/inputs/input[@number=""" & PREVActiveInput & """]").SelectNodes("overlay") 

'Get the count of layers we're working with:
TotalLayers = AllLayersNodeList.Count

If TotalLayers<=0 Then Exit Sub 'Exit if there are no layers
    
'Assign the overlay with correct # of Slots to the TOP Layer
' Overlay should be named SLOT-ID# where # corresponds to the number of layers

' Get the XMLNode of the correct overlay
LayerNode=VmixXML.selectSingleNode("/vmix/inputs/input[@title=""SLOT-ID" & CStr(TotalLayers) & """]")
ActiveOverlay = LayerNode.Attributes.GetNamedItem("number").Value


If ActiveOverlay Is Nothing Then Exit Sub 'If there is no active overlay for this layer count just exit

'Hide and Set the TOP LAYER (10) to the ActiveOverlay  
API.Function("MultiViewOverlayOff",Input:=PREVActiveInput,  Value:="10")  
API.Function("SetMultiViewOverlay",Input:=PREVActiveInput, Value:="10," & CStr(ActiveOverlay))

'Identify the Input associated with each layer and set the values in the Active Overlay to the Input Numbers
For ActiveLayer = 0 To TotalLayers-1
    'Get the Input KEY associated with this Layer
    AssignedInputKey = AllLayersNodeList.Item(ActiveLayer).Attributes.GetNamedItem("key").Value

    'Based on KEY, get the Input's NUMBER and TITLE to update
    LayerInputNumber = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""" & AssignedInputKey & """]").Attributes.GetNamedItem("number").Value
    LayerInputTitle = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""" & AssignedInputKey & """]").Attributes.GetNamedItem("title").Value
    If LayerInputNumber Is Nothing Then Exit Sub
    
    'Set the Input NUMBER and TITLE for this layer in the ActiveOverlay
    API.Function("SetText", Value:=LayerInputNumber, SelectedName:=("SLOT" & CStr(ActiveLayer+1) & ".Text"), Input:=ActiveOverlay)
    Console.WriteLine("NAME" & CStr(ActiveLayer+1) & "=" &LayerInputTitle)
    API.Function("SetText", Value:=LayerInputTitle, SelectedName:=("NAME" & CStr(ActiveLayer+1) & ".Text"), Input:=ActiveOverlay)
Next

'Turn the LAYER with the Label Overlay ON
Sleep(200) 'Let the overlay updates complete
API.Function("MultiViewOverlayOn",Input:=PREVActiveInput,  Value:="10")




