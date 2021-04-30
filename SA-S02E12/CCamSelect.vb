' STREAMING ALCHEMY - Season 02 Episode 12
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'
'


dim TotalControlInputs As Integer = 0   ' Total Number of INPUTS being controlled
dim ActiveControlIndex As Integer = 0   ' The index of the Active INPUT

dim InputKey as String = ""      ' The KEY of the Active INPUT

'Load the vMix XML Model:

dim VmixXML as new system.xml.xmldocument

VmixXML.loadxml(API.XML)

'Get XML Node for Dynamic Values 1:
dim Dynamic1Node As XmlNode = VmixXML.selectSingleNode("/vmix/dynamic/value1")

'Get a list of XML Nodes for all 'CCAM' labeled inputs:
dim AllInputsNodeList As XmlNodeList = VmixXML.selectSingleNode("/vmix/inputs").SelectNodes("/vmix/inputs/input[contains(@title,""CCAM"")]")

'Get the count of inputs we're controlling:
TotalControlInputs = AllInputsNodeList.Count

'Get the INPUT we're currently controlling from Dynamic Value 1

if ((Dynamic1Node Is Nothing) or (ActiveControlIndex > TotalControlInputs)) Then   'If not set then initialize it
    ActiveControlIndex = 0
   else 
    Try
        ActiveControlIndex = (CInt(Dynamic1Node.InnerText)+1) Mod TotalControlInputs 'Jump to next Input with LOOP to start
    Catch ex as Exception 'If set indeterminant value...
        ActiveControlIndex = 0  '...catch as an exception and initialize it to zero
    End Try

    API.Function("SetDynamicValue1", Value:=CStr(ActiveControlIndex))   'Save the Active Input in Dynamic Value 1
end if

' Get the unique Input KEY of the Active Input
InputKey = AllInputsNodeList.Item(ActiveControlIndex).Attributes.GetNamedItem("key").Value

' Set OUTPUT 2 to the Active Input
API.Function("SetOutput2", Input:=InputKey)