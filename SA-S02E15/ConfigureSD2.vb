' STREAMING ALCHEMY - Season 02 Episode 15
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'
'


dim ActiveInput as String = ""          ' The NUMBER of the INPUT in PROGRAM

dim ActiveInputKey as String = ""       ' The KEY of the INPUT in PROGRAM
dim MultiBoxInputKey as String = ""     ' The KEY of the Last MultiBox INPUT 
dim AssignedInputKey as String = ""     ' The KEY of the specified Layer

dim InLayers as Boolean                 ' Flag to test if Input is part of last MultiView


'Counters used for looping through the layers
dim CurrentLayerCount As Integer = 0    ' Total Number of Layers in CURRENT program Input
dim MultiViewLayerCount As Integer = 0  ' Total Number of Layers in Last MultiView
dim ActiveLayer As Integer = 0          ' The currently ACTIVE Layer


'vMix XML Model:
dim VmixXML as new system.xml.xmldocument

'Initialize Dynamic Control Values
API.Function("SetDynamicValue1",Value:="")
API.Function("SetDynamicInput1",Value:="")

Do
    'Load the vMix XML Model:
    VmixXML.loadxml(API.XML)

    'Get the XMLNode for the Input in PROGRAM:
    ActiveInput = VmixXML.selectSingleNode("/vmix/active").InnerText 

    'Get the KEY of the INPUT in PROGRAM
    ActiveInputKey = VmixXML.selectSingleNode("/vmix/inputs/input[@number=""" & ActiveInput & """]").Attributes.GetNamedItem("key").Value

    'Get the KEY of the INPUT with last MultiView:
    MultiBoxInputKey = VmixXML.selectSingleNode("/vmix/dynamic/value1").InnerText

    'Create an XMLNodeList of LAYERS in PROGRAM:
    dim AllLayersNodeList As XmlNodeList = VmixXML.selectSingleNode("/vmix/inputs/input[@number=""" & ActiveInput & """]").SelectNodes("overlay") 
    
    'Create an XMLNodeList of LAYERS in Last MultiView:
    dim MultiViewLayersNodeList As XmlNodeList 
    If MultiBoxInputKey <> "" Then
        MultiViewLayersNodeList = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""" & MultiBoxInputKey & """]").SelectNodes("overlay")  
        MultiViewLayerCount = MultiViewLayersNodeList.Count
    Else
        MultiViewLayerCount = 0
    End If 

    'Get the count of layers in the CURRENT input in PROGRAM:
    CurrentLayerCount = AllLayersNodeList.Count
   

    'If input in PROGRAM contains LAYERS...
    If CurrentLayerCount>0 THEN
        
        'Signal ACTIVATORS to light up correct number of buttons
        API.Function("SetDynamicInput1",Value:="")
        API.Function("SetDynamicInput1",Value:="ACTIVE " & CStr(CurrentLayerCount))

        'Get the KEY of input in PROGRAM
        ActiveInputKey = VmixXML.selectSingleNode("/vmix/inputs/input[@number=""" & ActiveInput & """]").Attributes.GetNamedItem("key").Value
        MultiViewLayerCount = CurrentLayerCount
        
        'Write the KEY into DynamicValue1 to be used by button pushes
        API.Function("SetDynamicValue1",Value:=ActiveInputKey)

    Else
        'Initialize Flag that INPUT in PROGRAM isn't a MultiView LAYER
        InLayers = False

        'Check if INPUT in PROGRAM is one of the layers in the last MULTIVIEW
        
        ActiveLayer = 0
        While (ActiveLayer < MultiViewLayerCount)
            'Check each layer in the last active MultiView 
            AssignedInputKey = MultiViewLayersNodeList.Item(ActiveLayer).Attributes.GetNamedItem("key").Value

            ' Is the Input in PROG is part of the last MultiView?
            If ActiveInputKey = AssignedInputKey Then
                ' The Input in PROG is a LAYER in the last MultiView
                InLayers = True
                Exit While
            End If
            ActiveLayer += 1
        End While

        If InLayers Then
            ' If the Input in PROGRAM is part of the Multiview, Reactivate Stream Deck Lights
            'API.Function("SetDynamicInput1",Value:="")
            'Sleep(250)
            'API.Function("SetDynamicInput1",Value:="ACTIVE " & MultiViewLayerCount)
            API.Function("SetDynamicInput1",Value:="")
            API.Function("SetDynamicInput1",Value:="ACTIVE " & CStr(MultiViewLayerCount))
        Else
            ' If the Input in PROGRAM isn't part of the Multiview, Reset and wait for next Multiview
            API.Function("SetDynamicInput1",Value:="300")
            API.Function("SetDynamicValue1",Value:="")
            Sleep(250)
        End If
    End If
    Sleep(250)
Loop While True




