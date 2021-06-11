' STREAMING ALCHEMY - Season 02 Episode 17
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'

'**************************************
'*****     Script for OFF-AIR     *****
'**************************************



dim TotalInputs as INTEGER          'Total Inputs In VMIX Session
dim CurrentInput as INTEGER         'Current Input being inspected

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

For CurrentInput = 0 To TotalInputs-1
    
    InputAudioRoutes = AllInputsNodeList.Item(CurrentInput).Attributes.GetNamedItem("audiobusses")      'Grab the XML with the 'audiobuses'

    If InputAudioRoutes Is Nothing Then Continue For    'If it doesnt exist [eg IMAGES, TITLES], skip the rest of the loop
    
    'When OFF-AIR, remove all audio sources from BUS A and BUS B
    API.Function("AudioBusOff", Input:=CurrentInput+1, Value:="A")
    API.Function("AudioBusOff", Input:=CurrentInput+1, Value:="B")
    
    'Get the OPERATOR Input
    OperatorAudioInput = CInt(VmixXML.selectSingleNode("/vmix/inputs/input[@title=""OperatorAudio""]").Attributes.GetNamedItem("number").Value)
    API.Function("AudioBusOff", Input:=OperatorAudioInput, Value:="M")
    API.Function("AudioBusOn", Input:=OperatorAudioInput, Value:="A")       'Route OPERATOR Audio to CALLER
    API.Function("AudioBusOff", Input:=OperatorAudioInput, Value:="B")

    CallerAudioInput = CInt(VmixXML.selectSingleNode("/vmix/inputs/input[@title=""CallerAudio""]").Attributes.GetNamedItem("number").Value)
    API.Function("AudioBusOff", Input:=CallerAudioInput, Value:="M")
    API.Function("AudioBusOff", Input:=CallerAudioInput, Value:="A")
    API.Function("AudioBusOn", Input:=CallerAudioInput, Value:="B")         'Route CALLER Audio to OPERATOR

Next