' STREAMING ALCHEMY - Season 02 Episode 17
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'


'Script for PTT-OFF



dim OperatorAudioInput as Integer

'vMix XML Model:
dim VmixXML as new system.xml.xmldocument

VmixXML.loadxml(API.XML)



OperatorAudioInput = CInt(VmixXML.selectSingleNode("/vmix/inputs/input[@title=""OperatorAudio""]").Attributes.GetNamedItem("number").Value)

API.Function("AudioBusOff", Input:=OperatorAudioInput, Value:="M")
API.Function("AudioBusOff", Input:=OperatorAudioInput, Value:="A")
API.Function("AudioBusOff", Input:=OperatorAudioInput, Value:="B")
