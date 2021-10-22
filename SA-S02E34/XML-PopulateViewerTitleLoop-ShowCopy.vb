' STREAMING ALCHEMY - Season 02 Episode 34
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'

' ********************************************
' *****  Populate Viewer Title Template  *****
' ********************************************


' Load the vmx XML Model:
dim vmixXML as new system.xml.xmldocument
vmixXML.loadxml(API.XML)

' Define variables

' Data Elements Taken From A Viewer Comment
Dim ViewerName as String            ' Storage for the Viewer Name as a String
Dim ViewerImageURI as String        ' Storage for the Viewer Image URI as a String
Dim ViewerSourceURI as String       ' Storage for the Viewer Image URI as a String
Dim ViewerMessage as String         ' Storage for the Viewer Message as a String

' The Name and Number of the Input with the GT TITLE displaying activers viewers
Dim ViewerTitleName as String = "ViewerTitle"
Dim ViewerTitleInputNumber as String

Dim ViewerCount as Integer          ' Current count of Active Viewers
Dim SelectedViewer as Integer       ' The index to the viewer we are actively working on

' ***** START PROCESSING *****

' Get the XMLNode of the Video Input to find the Input Number
Dim ViewerNodeXML as XMLNode = VmixXML.selectSingleNode("/vmix/inputs/input[@title=""" + ViewerTitleName + """]")
If ViewerNodeXML Is Nothing Then
    ' If it isn't found log an error and exit
    Console.WriteLine("Viewer Input Not Found")
    Exit Sub        'End processing and leave
else
    ' If it is found, save the INPUT Number
    ViewerTitleInputNumber = ViewerNodeXML.Attributes.GetNamedItem("number").Value
End If


' Set up the XML Document File for our Viewer Data
Dim ViewerDataXML as XmlDocument = new XmlDocument() 	' XML document with the Viewer Data
Dim FILE_PATH as String = "C:\\Users\\Alchemy-Studio\\Desktop\\Active Show\\SA-ViewerData.xml"


' Use a Main Loop to keep looking for new commenters to display
While True

    ' See if the Viewer Data XML File already exists

    Dim WaitForFile As Boolean = True       ' Viewer File creation status

    While WaitForFile
        Try 
            ViewerDataXML.Load(FILE_PATH)   ' Try to load the XML File with Viewers
        Catch ex as Exception
            Sleep(2000)
            Continue While      ' If the file doesn't exist, just keep waiting for it
        End Try
        WaitForFile = False     ' The file exists so start processing (Stop Waiting)   
    End While


    ' Generate an XMLNodeList with all of the active viewers:
    dim ViewerList As XmlNodeList = ViewerDataXML.SelectNodes("/activeViewers/viewer")
    ViewerCount = ViewerList.Count ' This holds the number of viewers


    ' Cap Active Viewers Processed to first 24 (Total SEATS Available)
    If(ViewerCount > 24) Then ViewerCount = 24

    ' Lets loop through the active viewers
    ' (Since XMLNodeLists are 0-Indexed, loop from 0 to the count-1)
    For SelectedViewer = 0 To ViewerCount-1

        ' Lets load up the details about the selected viewer:
        ' MESSAGE / NAME / IMAGE / SOURCE
        ViewerMessage = ViewerList.Item(SelectedViewer).InnerText
        ViewerName = ViewerList.Item(SelectedViewer).Attributes.GetNamedItem("name").Value
        ViewerImageURI = ViewerList.Item(SelectedViewer).Attributes.GetNamedItem("imageURI").Value
        ViewerSourceURI = ViewerList.Item(SelectedViewer).Attributes.GetNamedItem("sourceURI").Value

        ' Assign these viewer details to the corresponding viewer slots
        API.Function("SetText", input:=ViewerTitleInputNumber, SelectedName:="Name"+SelectedViewer.ToString()+".Text",Value:=ViewerName)
        API.Function("SetImage", ViewerTitleInputNumber, ViewerImageURI, 100,"Image"+SelectedViewer.ToString()+".Source")
        API.Function("SetImage", ViewerTitleInputNumber, ViewerSourceURI, 100,"Source"+SelectedViewer.ToString()+".Source")

    Next

    For SelectedViewer = ViewerCount To 23
        ' Lets Blank Out the remaining slots:

        ViewerName = ""
        ViewerImageURI = "C:\\Users\\Alchemy-Studio\\Desktop\\Active Show\\OpenSeat.png"
        ViewerSourceURI = ""

        API.Function("SetText", input:=ViewerTitleInputNumber, SelectedName:="Name"+SelectedViewer.ToString()+".Text",Value:=ViewerName)
        API.Function("SetImage", ViewerTitleInputNumber, ViewerImageURI, 100,"Image"+SelectedViewer.ToString()+".Source")
        API.Function("SetImage", ViewerTitleInputNumber, ViewerSourceURI, 100,"Source"+SelectedViewer.ToString()+".Source")
    Next

    Sleep(2000)     ' Wait 2 Seconds before Checking for additional viewers
End While