' STREAMING ALCHEMY - Season 02 Episode 15
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'
'

' Set the Button # that triggers this script

' Script for Button 1
dim ButtonPressed as String = "0"       'Layers use 0-based indexing - so 'Button-1'            

dim ActiveInput as String = ""          ' The NUMBER of the INPUT in PROGRAM
dim ActiveInputKey as String = ""       ' The KEY of the INPUT in PROGRAM
dim MultiBoxInputKey as String = ""     ' The KEY of the Last MultiBox INPUT 
dim MultiBoxLayerKey as String = ""     ' The KEY of the Layer being Controlled 

dim InLayers as Boolean                 ' Flag to test if Input is part of last MultiView

'Load the vMix XML Model:

dim VmixXML as new system.xml.xmldocument
VmixXML.loadxml(API.XML)

'Get the XMLNode for the Input in PROGRAM:
ActiveInput = VmixXML.selectSingleNode("/vmix/active").InnerText 

'Get the KEY of the INPUT in PROGRAM
ActiveInputKey = VmixXML.selectSingleNode("/vmix/inputs/input[@number=""" & ActiveInput & """]").Attributes.GetNamedItem("key").Value

'Get the KEY of the INPUT with last MultiView:
MultiBoxInputKey = VmixXML.selectSingleNode("/vmix/dynamic/value1").InnerText


'Create an XMLNodeList of LAYERS in Last MultiView:
dim MultiViewLayersNodeList As XmlNodeList = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""" & MultiBoxInputKey & """]").SelectNodes("overlay") 


MultiBoxLayerKey = MultiViewLayersNodeList.Item(ButtonPressed).Attributes.GetNamedItem("key").Value

'Take the correct action when a button is pushed

If ActiveInputKey = MultiBoxInputKey Then
    'MultiView is in PROG so switch to Layer in MultiViewLayerKey associated with Button
    API.Function("PreviewInput", Input:=MultiBoxLayerKey)
    
    API.Function("SetTransitionEffect4", Value:="Merge")
    API.Function("Transition4")
 
    Sleep(50)
    API.Function("SetDynamicInput1",Value:="ACTIVE " & MultiViewLayersNodeList.Count)

Else If MultiBoxLayerKey = ActiveInputKey
    'Layer associated with Button is in PROG Input, so Switch back to MultiView Input
    API.Function("PreviewInput", Input:=MultiBoxInputKey)
  
    API.Function("SetTransitionEffect4", Value:="Merge")
    API.Function("Transition4")
Else
    'Layer associated with Button isn't in PROG Input, so Switch to new Layer Input associated with Button
    API.Function("PreviewInput", Input:=MultiBoxLayerKey)
    
    API.Function("SetTransitionEffect4", Value:="Merge")
    API.Function("Transition4")
End If