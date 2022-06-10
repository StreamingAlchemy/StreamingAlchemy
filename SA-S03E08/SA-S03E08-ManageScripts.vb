'           
'        _____ _                            _             
'       / ____| |                          (_)            
'      | (___ | |_ _ __ ___  __ _ _ __ ___  _ _ __   __ _ 
'       \___ \| __| '__/ _ \/ _` | '_ ` _ \| | '_ \ / _` |
'       ____) | |_| | |  __/ (_| | | | | | | | | | | (_| |
'      |_____/ \__|_|  \___|\__,_|_| |_| |_|_|_| |_|\__, |
'          /\   | |    | |                           __/ |
'         /  \  | | ___| |__   ___ _ __ ___  _   _  |___/ 
'        / /\ \ | |/ __| '_ \ / _ \ '_ ` _ \| | | |       
'       / ____ \| | (__| | | |  __/ | | | | | |_| |       
'      /_/    \_\_|\___|_| |_|\___|_| |_| |_|\__, |       
'                                             __/ |       
'                                            |___/    
'
'          STREAMING ALCHEMY - Season 03 Episode 08
'
'                Sponsored by Gnural Net, Inc.
'                     www.gnuralnet.com
'                       609-223-2434
'
'
' *************************************************************
' *****  Manage Scripts Used For Our Basic vMix Prompter  *****
' *************************************************************



' **********************************************************

dim LayerIndex as Integer               ' This is the counter we use to address layers in the prompter inputs

dim VmixXML as new system.xml.xmldocument
VmixXML.loadxml(API.XML)


' **********************************************
' ****  Extract Details For Current Script  ****
' **********************************************

' Set up What we need to read the script
dim ScriptFolder As String = "c:\ActiveShow"
dim XMLScriptData As New XmlDataDocument()
dim XMLScriptNodeList As XMLNodeList
dim SegmentCount as Integer
dim SegmentIndex as Integer
dim ScriptText as String = ""
dim SegmentText as String = ""
dim SegmentTitle as String = "Multiple Segments"
dim SegmentNotes as String = ""

' Open the file with the Script XML
dim ScriptFileStream As New Filestream(ScriptFolder+"\Script-Gnural.xml", FileMode.Open, FileAccess.Read)

' Load the file with the Script XML
XMLScriptData.Load(ScriptFileStream)

If ( XMLScriptData Is Nothing ) Then 
      Console.WriteLine("Unable To Load Script")
Else
      Console.WriteLine("Script Loaded")
End If

' Get a list of All the segments in the script
' Console.WriteLine("XML Found: " & XMLScriptData.selectSingleNode("/PollingQuestions").InnerXML)

XMLScriptNodeList = XMLScriptData.selectSingleNode("/Script").SelectNodes("Segment")
If ( XMLScriptNodeList Is Nothing ) Then 
      Console.WriteLine("NodeList Failed - No Segments Found") 
End If

SegmentCount = XMLScriptNodeList.count
Console.Writeline("Number of segments in script: " & CStr(SegmentCount))

' DynamicValue1 has the active Segment (either ALL or a Segment Number)
dim ActiveSegment as String = VmixXML.selectSingleNode("/vmix/dynamic/value1").InnerText

If ActiveSegment = "ALL" Then
    ' For ALL, combine the Script text from all segments
    For SegmentIndex = 0 To SegmentCount-1
        ' Loop through all of the segments
        SegmentText = XMLScriptNodeList(SegmentIndex).SelectSingleNode("Text").InnerText
        SegmentNotes = SegmentNotes + XMLScriptNodeList(SegmentIndex).SelectSingleNode("Name").InnerText + ", "
        ScriptText = ScriptText & SegmentText
    Next
Else
    ' For a single segment, pull the individual Script text from Segment Details
    SegmentIndex = CInt(ActiveSegment)-1
    'SegmentText = XMLScriptNodeList(SegmentIndex).SelectSingleNode("Text").InnerText
    SegmentTitle = XMLScriptNodeList(SegmentIndex).SelectSingleNode("Name").InnerText
    SegmentNotes = XMLScriptNodeList(SegmentIndex).SelectSingleNode("Notes").InnerText
    ScriptText = XMLScriptNodeList(SegmentIndex).SelectSingleNode("Text").InnerText
End If

' Update PROMPTER title with script and segment information

API.Function("SetText",Input:="Prompter",SelectedName:="PROMPT.Text",Value:=ScriptText)
API.Function("SetText",Input:="Prompter",SelectedName:="Segment-Title.Text",Value:=SegmentTitle)
API.Function("SetText",Input:="Prompter",SelectedName:="Segment-Notes.Text",Value:=SegmentNotes)



