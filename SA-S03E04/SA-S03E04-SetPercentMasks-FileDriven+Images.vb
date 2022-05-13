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
'          STREAMING ALCHEMY - Season 03 Episode 04
'
'                Sponsored by Gnural Net, Inc.
'                     www.gnuralnet.com
'                       609-223-2434
'
'
' *************************************************************
' *****  Read XML File with Questions/Responses for Poll  *****
' *****  Build a Mask to cover a percent of 2 Color Bars  *****
' *****  Write Images and Response Details to an Overlay  *****
' *************************************************************



' **********************************************************


dim LayerIndex as Integer               ' This is the counter we use to address layers in the mask inputs

' Set up details needed to access the Mask Inputs
dim MaskAName as String = "Mask-A"
dim MaskBName as String = "Mask-B"
dim MaskAInput as String
dim MaskBInput as String


' Numeric Percent of responses to the two options in a poll question
dim OptionAPercent as Integer
dim OptionBPercent as Integer

'
' Set up details needed for displaying the percentages
dim PercentAFullBlockCount as Integer		' Number of 10's digit
dim PercentAPartialBlockCount as Integer	' Number of 1's digit

dim PercentBFullBlockCount as Integer		' Number of 10's digit
dim PercentBPartialBlockCount as Integer	' Number of 1's digit


dim FullBlockInputName as String = "Bar-10"		' Input name of the full block image
dim PartialBlockInputPrefix as String = "Bar-"	' Input name prefix for the partial block image


dim PercentAPartialBlockInput as String			' This is the input number for the last 'Bar' slot (the 1's digits) in A
dim PercentBPartialBlockInput as String			' This is the input number for the last 'Bar' slot (the 1's digits) in B

dim PercentAPartialBlockInputName as String		' This is the input name for the last 'Bar' slot (the 1's digits) in A
dim PercentBPartialBlockInputName as String		' This is the input name for the last 'Bar' slot (the 1's digits) in B

dim FullBlockInput as String					' Input number of the full block image



' *********************************************************
' Set up access to the vMix XML model

dim VmixXML as new system.xml.xmldocument
VmixXML.loadxml(API.XML)


' *********************************************************
' Open the file with the Polling Question Results

dim PollDataFolder As String = "c:\ActiveShow"
dim XMLQuestionData As New XmlDataDocument()
dim ResultsFileStream As New Filestream(PollDataFolder+"\PollingResults.xml", FileMode.Open, FileAccess.Read)

' Pull out the question/response details as an XML Node List

dim xmlQuestionNodeList As XMLNodeList
dim QuestionCount as Integer

' Load the file with the Polling Questions/Results and build Node List
XMLQuestionData.Load(ResultsFileStream)
xmlQuestionNodeList = XMLQuestionData.selectSingleNode("/PollingQuestions").SelectNodes("Question")

If ( xmlQuestionNodeList Is Nothing ) Then 
      Console.WriteLine("NodeList Failed - No Questions Found") 
End If
QuestionCount = xmlQuestionNodeList.count	' This is the number of Questions/Responses in the list

' 
' *********************************************************
' The Question/Response being displayed is in DynamicValue1  NOTE: Index runs from 0 to Count-1
dim ActiveQuestion as Integer = CInt(VmixXML.selectSingleNode("/vmix/dynamic/value1").InnerText)

' Here is where we store the details for the active question/response
dim QuestionAsked as String
dim OptionA as String
dim OptionB as String
dim PercentA as String
dim PercentB as String
dim ImageA as String
dim ImageB as String

' Pull out the data from the XML Node and assign it
QuestionAsked = xmlQuestionNodeList(ActiveQuestion).SelectSingleNode("QuestionAsked").InnerText
OptionA = xmlQuestionNodeList(ActiveQuestion).SelectSingleNode("OptionA").InnerText
OptionB = xmlQuestionNodeList(ActiveQuestion).SelectSingleNode("OptionB").InnerText

ImageA = "C:\ActiveShow\" & xmlQuestionNodeList(ActiveQuestion).SelectSingleNode("OptionAImage").InnerText
ImageB = "C:\ActiveShow\" & xmlQuestionNodeList(ActiveQuestion).SelectSingleNode("OptionBImage").InnerText

PercentA = xmlQuestionNodeList(ActiveQuestion).SelectSingleNode("OptionAPercent").InnerText
PercentB = xmlQuestionNodeList(ActiveQuestion).SelectSingleNode("OptionBPercent").InnerText
OptionAPercent = CInt(PercentA)
OptionBPercent = CInt(PercentB)

' For display purposes, add the % sign to the end of the value
PercentA = PercentA & "%"
PercentB = PercentB & "%"

' Use the data we extracted to populate the overlay

API.Function("SetText",Input:="PollResults",SelectedName:="Question.Text",Value:=QuestionAsked)
API.Function("SetText",Input:="PollResults",SelectedName:="OptionA.Text",Value:=OptionA)
API.Function("SetText",Input:="PollResults",SelectedName:="OptionB.Text",Value:=OptionB)
API.Function("SetText",Input:="PollResults",SelectedName:="PercentA.Text",Value:=PercentA)
API.Function("SetText",Input:="PollResults",SelectedName:="PercentB.Text",Value:=PercentB)

API.Function("SetImage", "PollResults", ImageA, 100, "ImageA.Source")
API.Function("SetImage", "PollResults", ImageB, 100, "ImageB.Source")


' ****************************************************************
' Now we set up the percent bars using Masks


' Using the Input Names, look up the Input numbers for Mask-A and Mask-B
MaskAInput = VmixXML.selectSingleNode("/vmix/inputs/input[@shortTitle=""" & MaskAName & """]").Attributes.GetNamedItem("number").Value
MaskBInput = VmixXML.selectSingleNode("/vmix/inputs/input[@shortTitle=""" & MaskBName & """]").Attributes.GetNamedItem("number").Value

' Get the number of the Full Block images input
FullBlockInput = VmixXML.selectSingleNode("/vmix/inputs/input[@shortTitle=""" & FullBlockInputName & """]").Attributes.GetNamedItem("number").Value

' Using the numerical percentage, isolate the 10's digit and the 1's digit
PercentAFullBlockCount = Math.Floor(OptionAPercent/10)	    ' Percent A - Number of layers with full block
PercentAPartialBlockCount = OptionAPercent mod 10	        ' Percent A - Size of Partial Block layer

PercentBFullBlockCount = Math.Floor(OptionBPercent /10)	    ' Percent B - Number of layers with full block
PercentBPartialBlockCount = OptionBPercent mod 10	        ' Percent B - Size of Partial Block layer

' Get the Partial Block (1's digit) image inputs for Option A and Option B
PercentAPartialBlockInputName = PartialBlockInputPrefix & CStr(PercentAPartialBlockCount)
PercentBPartialBlockInputName = PartialBlockInputPrefix & CStr(PercentBPartialBlockCount)

PercentAPartialBlockInput = VmixXML.selectSingleNode("/vmix/inputs/input[@shortTitle=""" & PercentAPartialBlockInputName & """]").Attributes.GetNamedItem("number").Value
PercentBPartialBlockInput = VmixXML.selectSingleNode("/vmix/inputs/input[@shortTitle=""" & PercentBPartialBlockInputName & """]").Attributes.GetNamedItem("number").Value


' Fill in the Full Block Layers for Mask-A

For LayerIndex = 1 to PercentAFullBlockCount
	' Assign the Full Block Input to it
	API.Function("SetLayer",input:=MaskAInput,value:=CStr(LayerIndex) & "," & FullBlockInput)
   
    ' Make the layer visible
	API.Function("LayerOn",input:=MaskAInput,value:=CStr(LayerIndex))
Next

' Now fill in the partial layer for Mask-A

' Set the layer to the correct partial image
API.Function("SetLayer", input:=MaskAInput, value:=CSTR(LayerIndex) & "," & PercentAPartialBlockInput)

' Make the partial layer visible
API.Function("LayerOn",input:=MaskAInput,value:=CSTR(LayerIndex))


For LayerIndex = LayerIndex+1 to 10
	API.Function("LayerOff",Input:=MaskAInput,value:=CSTR(LayerIndex))
Next

' Fill in the Full Block Layers for Mask-B

For LayerIndex = 1 to PercentBFullBlockCount
	' Assign the Full Block Input to it
	API.Function("SetLayer",input:=MaskBInput,value:=CStr(LayerIndex) & "," & FullBlockInput)
    
    ' Make the layer visible
	API.Function("LayerOn",input:=MaskBInput,value:=CStr(LayerIndex))
Next

' Now fill in the partial layer for Mask-B

' Set the layer to the correct partial image
API.Function("SetLayer", input:=MaskBInput, value:=CSTR(LayerIndex) & "," & PercentBPartialBlockInput)

' Make the partial layer visible
API.Function("LayerOn",input:=MaskBInput,value:=CSTR(LayerIndex))


For LayerIndex = LayerIndex+1 to 10
	API.Function("LayerOff",Input:=MaskBInput,value:=CSTR(LayerIndex))
Next
