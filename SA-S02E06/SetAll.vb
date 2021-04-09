Console.WriteLine("--- Starting ---")

dim I As Integer
dim Slot As Integer

dim VmixXML as new system.xml.xmldocument

VmixXML.loadxml(API.XML)

dim Dynamic1Node As XmlNode = VmixXML.selectSingleNode("/vmix/dynamic/value1")
dim Dynamic4Node As XmlNode = VmixXML.selectSingleNode("/vmix/dynamic/value4")

dim Dynamic1Val as String = ""
dim Dynamic4Val as String = ""

if Dynamic1Node Is Nothing Then
    Console.WriteLine("Invalid Node for Dynamic 1")
else
    Console.WriteLine("Dynamic1Value: " & Dynamic1Node.InnerText)
    Dynamic1Val = Dynamic1Node.InnerText
End if

If Dynamic4Node Is Nothing Then
    Console.WriteLine("Invalid Node for Dyanmic 2")
else
    Console.WriteLine("Dynamic4Value: " & Dynamic4Node.InnerText)
    Dynamic4Val = Dynamic4Node.InnerText
End If


dim SlotX As String=Dynamic4Val & "," & Dynamic1Val

I=8
Do While I<12

API.Function("SetMultiViewOverlay",Input:=I,Value:=SlotX)
I=I+1
Loop