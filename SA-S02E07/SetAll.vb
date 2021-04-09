
dim I As Integer
dim MultiBoxInputOffset As Integer = 9
dim MultiBoxInput As Integer

dim Slot1Guest As String
dim Slot2Guest As String
dim Slot3Guest As String
dim Slot4Guest As String

dim Dynamic1Val as String = ""

dim VmixXML as new system.xml.xmldocument
dim SlotXML as new system.xml.xmldocument

'Load the vMix XML Model:

VmixXML.loadxml(API.XML)

'Isolate the XML Nodes for Dynamic Values 1 and 2:
 
dim Dynamic1Node As XmlNode = VmixXML.selectSingleNode("/vmix/dynamic/value1")
dim Dynamic2Node As XmlNode = VmixXML.selectSingleNode("/vmix/dynamic/value2")

'DynamicValue1 contains XML describing Guest/Box Assignments
'DynamicValue2 contains the Guest Number to be assigned to BOX 2

if Dynamic1Node Is Nothing Then
    Console.WriteLine("Invalid Node for Dynamic 1")
else

'Load the Guest/Box XML:
     
    SlotXML.LoadXml(Dynamic1Node.InnerText)



'Retrieve the Guest/Box assignments from the stored XML in DynamicValue1:

    Slot1Guest = SlotXML.selectSingleNode("/Slots/Slot1").InnerText
    Slot2Guest = SlotXML.selectSingleNode("/Slots/Slot2").InnerText
    Slot3Guest = SlotXML.selectSingleNode("/Slots/Slot3").InnerText
    Slot4Guest = SlotXML.selectSingleNode("/Slots/Slot4").InnerText
 
End if

'Set the VALUE argument used by SetMultiViewOverlay for each Guest Slot 

dim Slot1 As String="4" & "," & Slot1Guest
dim Slot2 As String="3" & "," & Slot2Guest
dim Slot3 As String="2" & "," & Slot3Guest

I=0
Do While I<4

MultiBoxInput=I+MultiBoxInputOffset

API.Function("SetMultiViewOverlay",Input:=MultiBoxInput,Value:=Slot1)
API.Function("SetMultiViewOverlay",Input:=MultiBoxInput,Value:=Slot2)
API.Function("SetMultiViewOverlay",Input:=MultiBoxInput,Value:=Slot3)
I=I+1
Loop