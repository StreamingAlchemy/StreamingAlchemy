' STREAMING ALCHEMY - Season 02 Episode 33
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'

' ****************************************************
' *****  Play Back Video with Triggered Actions  *****
' ****************************************************


' Load the vmx XML Model:
dim vmixXML as new system.xml.xmldocument
vmixXML.loadxml(API.XML)

' Define variables for details around inputs with Video, Slides, and a Layered Combination

' The Video Input being Used to map the triggered actions
Dim VideoInputName as String = "Video"
Dim VideoInputNumber as String

' The Slides Input being Used
Dim SlideInputName as String = "Slides"
Dim SlideInputNumber as String

' The Combo Input being Used
Dim ComboInputName as String = "Combo"
Dim ComboInputNumber as String


' Define variables for details around time offsets in the playing video
Dim VideoOffsetPosition as String
Dim VideoOffsetPositionInt as Integer

' Define variables for details around the next trigger offset in the XML Document 
Dim TriggerTimeOffset as String
Dim TriggerTimeOffsetInt as Integer

' Define variables for details working with the trigger offsets
Dim TriggerCount as Integer
Dim ActiveTrigger as Integer
Dim TriggerAction as String




' Get the XMLNode of the Video Input to find the Input Number
Dim VideoNodeXML as XMLNode = VmixXML.selectSingleNode("/vmix/inputs/input[@title=""" + VideoInputName + """]")
If VideoNodeXML Is Nothing Then
    ' If it isn't found log an error
    Console.WriteLine("Video Input NULL!")
else
    ' If it is found, save the INPUT Number
    VideoInputNumber = VideoNodeXML.Attributes.GetNamedItem("number").Value
End If

' Get the XMLNode of the Slide Input to find the Input Number
Dim SlideNodeXML as XMLNode = VmixXML.selectSingleNode("/vmix/inputs/input[@shortTitle=""" + SlideInputName + """]")
If SlideNodeXML Is Nothing Then
    ' If it isn't found log an error
    Console.WriteLine("Slide Input NULL!")
else
    ' If it is found, save the INPUT Number
    SlideInputNumber = SlideNodeXML.Attributes.GetNamedItem("number").Value
End If


' Get the XMLNode of the Combo Input to find the Input Number
Dim ComboNodeXML as XMLNode = VmixXML.selectSingleNode("/vmix/inputs/input[@shortTitle=""" + ComboInputName + """]")
If ComboNodeXML Is Nothing Then
    ' If it isn't found log an error
    Console.WriteLine("Combo Input NULL!")
else
    ' If it is found, save the INPUT Number
    ComboInputNumber = ComboNodeXML.Attributes.GetNamedItem("number").Value
    Console.WriteLine("Combo Input Number: "+ComboInputNumber)
End If



' File Path of the XML with Trigger Times
Dim FILE_PATH as String = "C:\\Users\\Alchemy-Studio\\Desktop\\Active Show\\SA-SlideTimes.xml"

' Define and LOAD the XML document of the Trigger Times associated with actions
Dim TriggerTimes as XmlDocument = new XmlDocument()
TriggerTimes.Load(FILE_PATH)

' Generate an XMLNodeList with all of the triggers and actions:
dim TriggerActionList As XmlNodeList = TriggerTimes.SelectNodes("/slideTimes/trigger")
TriggerCount = TriggerActionList.Count ' This holds the number of triggered actions to be taken
Console.WriteLine("Transitions: "+CStr(TriggerCount))

' Initialize everything and start the Trigger Driven Playback
VideoOffsetPositionInt = 0                              ' Initialize variable with current video offset
API.Function("Restart", input:=SlideInputNumber)        ' Restart Slide Input at the first slide
Sleep(200)                                                          ' Wait for action to execute
API.Function("SetPosition", value:="0", input:=VideoInputNumber)    'Reset Video Input to start of video
Sleep(200)                                                          ' Wait for action to execute
API.Function("Play", input:=VideoInputNumber)           ' Start playing the video

' With the video started, we now need to trigger actions when they should happen

' Since XMLNodeLists are 0-Indexed, loop from 0 to the count-1
For ActiveTrigger = 0 To TriggerCount-1
    ' Lets load up the active trigger time and action it should launch
    TriggerTimeOffsetInt = CInt(TriggerActionList.Item(ActiveTrigger).InnerText)
    TriggerAction = TriggerActionList.Item(ActiveTrigger).Attributes.GetNamedItem("action").Value

    ' Now we stay in the "WHILE" loop until the position in the video playhead crosses the trigger point
    While (TriggerTimeOffsetInt > VideoOffsetPositionInt) ' Stay in this loop while Trigger Time > Video Offset
        ' Refresh the vMix XML Model to get latest data
        vmixXML.loadxml(API.XML)  
        ' Get the XML associated with the playing video
        VideoNodeXML = VmixXML.selectSingleNode("/vmix/inputs/input[@title=""" + VideoInputName + """]")
        ' Pull out the "POSITION" attribute (the offset of the playhead from the start of the video)
        VideoOffsetPosition = VideoNodeXML.Attributes.GetNamedItem("position").Value
        VideoOffsetPositionInt = CInt(VideoOffsetPosition) 'Convert it to an INTEGER for comparison
        ' Wait 1/10 Second then loop back to see if we can leave the WHILE Loop
        Sleep(100)
    End While

    ' Arriving here means we have passed the current Trigger Time and need to take an action
    Console.WriteLine("Action: " + TriggerAction.ToString() + " ActiveTrigger: " + ActiveTrigger.ToString() + " Trigger Count: " + TriggerCount.ToString())
    ' ** ACTION ** - Go To The NEXT SLIDE
    If String.Compare(TriggerAction, "Next") = 0 then API.Function("NextPicture", input:=SlideInputNumber)
    ' ** ACTION ** - Go To The PREV SLIDE
    If String.Compare(TriggerAction, "Prev") = 0 then API.Function("PreviousPicture", input:=SlideInputNumber)
    ' ** ACTION ** - Merge to the COMBO Input
    If String.Compare(TriggerAction, "Combo") = 0 then API.Function("Merge", input:=ComboInputNumber)
    ' ** ACTION ** - Merge to the VIDEO Input
    If String.Compare(TriggerAction, "Video") = 0 then API.Function("Merge", input:=VideoInputNumber)
    ' ** ACTION ** - Merge to the SLIDE Input
    If String.Compare(TriggerAction, "Slide") = 0 then API.Function("Merge", input:=SlideInputNumber)
    
   ' If there are still more triggers to process Loop back - Otherwise exit
Next

