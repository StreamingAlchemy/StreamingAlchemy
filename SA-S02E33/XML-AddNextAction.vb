' STREAMING ALCHEMY - Season 02 Episode 33
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'

' ******************************************
' *****  Add NEXT Slide Trigger Point  *****
' ******************************************


' Load the vmx XML Model:
dim vmixXML as new system.xml.xmldocument
vmixXML.loadxml(API.XML)

' Define the 3 core XML Nodes we leverage
Dim RootNode as XMLNode         ' The Root Node of the XML Trigger Map 
Dim NextOffsetNode as XMLNode   ' The XML Node associated with Trigger Offsets
Dim ActionNode as XMLNode       ' The XML Node assciated with the Action at a Trigger point


' The details about the Video Input being used to map the transitions
Dim VideoInputName as String = "Video"
Dim VideoInputNumber as String


' Get the XMLNode of the Video Input
Dim VideoNodeXML as XMLNode = VmixXML.selectSingleNode("/vmix/inputs/input[@title=""" + VideoInputName + """]")
If VideoNodeXML Is Nothing Then
    Console.WriteLine("Video Input NULL!")
else
    ' Set the INPUT NUMBER associated with the Video Input
    VideoInputNumber = VideoNodeXML.Attributes.GetNamedItem("number").Value
End If

' Set up the XML Document File for our Trigger Points
Dim EventOffsetTimes as XmlDocument = new XmlDocument() 	' XML document of the slide transition times
Dim FILE_PATH as String = "C:\\Users\\Alchemy-Studio\\Desktop\\Active Show\\SA-SlideTimes.xml"

' See if the Trigger XML File already exists
Try 
	EventOffsetTimes.Load(FILE_PATH)
Catch ex as Exception
    ' If the file doesn't exist, create and initialize it here
	Console.WriteLine("Failed to Load Time File: " & ex.Message)
    RootNode = EventOffsetTimes.CreateElement("slideTimes")     ' Set the XML Root Node
    EventOffsetTimes.AppendChild(RootNode)                      ' Append it to the empty XML Document
    EventOffsetTimes.Save(FILE_PATH)                            ' Save it out to the file system
End Try

' If the XML Document file already existed, set the RootNode to the first child
RootNode = EventOffsetTimes.FirstChild

' Lets capture the Time Offset in the video at the time the button was pressed
' Read the Time Position from the Video XML in the 'position' attribute 
Dim TimeOffset as String = VideoNodeXML.Attributes.GetNamedItem("position").Value

' Modify the file with the Slide Transition Times
' We create a child element "trigger" for the Offset Time with the Attribute "action" set to "Next"
NextOffsetNode = EventOffsetTimes.CreateElement("trigger")  ' XML Node with the new Element "trigger"
ActionNode = EventOffsetTimes.CreateAttribute("action")     ' XML Node with the new Attribute "action"
ActionNode.Value = "Next"                                   ' Set the "action" to "Next"
NextOffsetNode.InnerText = TimeOffset                       ' Assign "Offset Time" to the "trigger" 
NextOffsetNode.Attributes.SetNamedItem(ActionNode)          ' Assign "Next" to the "action"
RootNode.AppendChild(NextOffsetNode)                        ' Write the new entry to the XML Model
EventOffsetTimes.Save(FILE_PATH)                            ' Save the XML Model

