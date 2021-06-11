' STREAMING ALCHEMY - Season 02 Episode 17
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'

'*************************************
'*****     Script for ON-AIR     *****
'*************************************



dim TotalInputs as INTEGER          'Total Inputs In VMIX Session
dim CurrentInput as INTEGER         'Current Input being inspected

dim AudioRoutes as String           'The string from VMIX XML with 'audiobusses'

dim OperatorAudioInput as Integer   'The input feeding the OPERATOR Audio
dim CallerAudioInput as Integer     'The input feeding the CALLER Audio


'vMix XML Model:
dim VmixXML as new system.xml.xmldocument

VmixXML.loadxml(API.XML)


'Create an XMLNodeList of all INPUTS:
dim AllInputsNodeList As XmlNodeList = VmixXML.selectSingleNode("/vmix/inputs").SelectNodes("input") 

'Get the count of all of the INPUTS
TotalInputs = AllInputsNodeList.Count

dim InputAudioRoutes as XMLNode     'Used to hold the XML of the Current Input Audio Routes

For CurrentInput = 0 To TotalInputs-1   'Loop through all of the INPUTS [0 Indexed]
    
    InputAudioRoutes = AllInputsNodeList.Item(CurrentInput).Attributes.GetNamedItem("audiobusses")      'Grab the XML with the 'audiobuses'

    If InputAudioRoutes Is Nothing Then Continue For    'If it doesnt exist [eg IMAGES, TITLES], skip the rest of the loop
    AudioRoutes = InputAudioRoutes.Value        'Assign the routing string here
   
     If AudioRoutes.Contains("M") Then          'If Input's audio is routed to Master, also route it to BUS A and BUS B
        API.Function("AudioBusOn", Input:=CurrentInput+1, Value:="A")   'Caller will hear this input
        API.Function("AudioBusOn", Input:=CurrentInput+1, Value:="B")   'Operator will hear this input
    End If

    'Get the OPERATOR Input
    OperatorAudioInput = CInt(VmixXML.selectSingleNode("/vmix/inputs/input[@title=""OperatorAudio""]").Attributes.GetNamedItem("number").Value)
    API.Function("AudioBusOff", Input:=OperatorAudioInput, Value:="M")
    API.Function("AudioBusOff", Input:=OperatorAudioInput, Value:="A")  'Take OPERATOR off of all audio buses
    API.Function("AudioBusOff", Input:=OperatorAudioInput, Value:="B")

    'Get the CALLER Input
    CallerAudioInput = CInt(VmixXML.selectSingleNode("/vmix/inputs/input[@title=""CallerAudio""]").Attributes.GetNamedItem("number").Value)
    API.Function("AudioBusOn", Input:=CallerAudioInput, Value:="M")     'Feed CALLER Audio to Master (ON-AIR)
    API.Function("AudioBusOff", Input:=CallerAudioInput, Value:="A")
    API.Function("AudioBusOn", Input:=CallerAudioInput, Value:="B")     'Feed CALLER Audio to OPERATOR (MONITOR)
Next