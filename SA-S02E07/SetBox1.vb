dim PreviewInput As Integer

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
'DynamicValue2 contains the Guest Number to be assigned to BOX 1

if Dynamic1Node Is Nothing Then
    Console.WriteLine("Invalid Node for Dynamic 1")
else

'Load the Guest/Box XML:
     
    SlotXML.LoadXml(Dynamic1Node.InnerText)

'Set the XML describing BOX 1 to the selected guest

SlotXML.selectSingleNode("/Slots/Slot1").InnerText = Dynamic2Node.InnerText

'Reconstruct the XML for all of the Guest/Box assignments:

    Slot1Guest = SlotXML.selectSingleNode("/Slots/Slot1").InnerText
    Slot2Guest = SlotXML.selectSingleNode("/Slots/Slot2").InnerText
    Slot3Guest = SlotXML.selectSingleNode("/Slots/Slot3").InnerText
    Slot4Guest = SlotXML.selectSingleNode("/Slots/Slot4").InnerText
    Dynamic1Val = "<Slots><Slot1>" & Slot1Guest & "</Slot1><Slot2>" & Slot2Guest & "</Slot2><Slot3>" & Slot3Guest & "</Slot3><Slot4>" & Slot4Guest & "</Slot4></Slots>"
    
'Update DynamicValue1 with the new XML description:

    API.Function("SetDynamicValue1", Value:=Dynamic1Val)

End if

'Update the guests shown in PreviewInput to match the new XML mapping

dim Slot1 As String="4" & "," & Slot1Guest
dim Slot2 As String="3" & "," & Slot2Guest
dim Slot3 As String="2" & "," & Slot3Guest

PreviewInput=16

API.Function("SetMultiViewOverlay",Input:=PreviewInput,Value:=Slot1)
API.Function("SetMultiViewOverlay",Input:=PreviewInput,Value:=Slot2)
API.Function("SetMultiViewOverlay",Input:=PreviewInput,Value:=Slot3)

