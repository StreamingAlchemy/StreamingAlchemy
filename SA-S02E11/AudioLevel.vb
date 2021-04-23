
dim Guest1-AudioLevel As String
dim Guest2-AudioLevel As String
dim Guest3-AudioLevel As String
dim Guest4-AudioLevel As String

dim VmixXML as new system.xml.xmldocument

'Load the vMix XML Model:

VmixXML.loadxml(API.XML)

'Isolate the XML Nodes for each Guest Input:
 
dim Guest1Node As XmlNode = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""9f2c6956-b33d-4276-91fb-3b3ebf1119ea""]")
dim Guest2Node As XmlNode = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""9f2c6956-b33d-4276-91fb-3b3ebf1119ea""]")
dim Guest3Node As XmlNode = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""9f2c6956-b33d-4276-91fb-3b3ebf1119ea""]")
dim Guest4Node As XmlNode = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""9f2c6956-b33d-4276-91fb-3b3ebf1119ea""]")

'    Console.WriteLine("Invalid Node for Dynamic 1")



'Retrieve the audio level for each guest:

Guest1-AudioLevel = Guest1Node.selectSingleNode("/meterF1").InnerText
Guest2-AudioLevel = Guest2Node.selectSingleNode("/meterF1").InnerText
Guest3-AudioLevel = Guest3Node.selectSingleNode("/meterF1").InnerText
Guest4-AudioLevel = Guest4Node.selectSingleNode("/meterF1").InnerText

Console.WriteLine("Guest1: " & Guest1-AudioLevel & "  Guest2:" & Guest1-AudioLevel & "Guest3: " & Guest3-AudioLevel & "  Guest4:" & Guest4-AudioLevel)
    
