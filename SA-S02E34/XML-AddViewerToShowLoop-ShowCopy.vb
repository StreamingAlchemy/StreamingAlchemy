' STREAMING ALCHEMY - Season 02 Episode 34
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'

' ********************************
' *****  Add Viewer To Show  *****
' ********************************


' Load the vmx XML Model:
dim vmixXML as new system.xml.xmldocument
vmixXML.loadxml(API.XML)

' Define the components we will use to collect Viewer Data
Dim RootNode as XMLNode         ' The Root Node of the XML Viewer List
Dim ViewerNode as XMLNode       ' The XML Node associated with all Viewer Details
Dim NameNode as XMLNode         ' The XML Node assciated with the Viewer Name
Dim ImageURINode as XMLNode     ' The XML Node assciated with the Viewer Image URI
Dim SourceURINode as XMLNode    ' The XML Node assciated with the Viewer Social Media Logo URI

dim ViewerList As XmlNodeList   ' XML Nodes of all viewers that have commented

Dim ViewerName as String        ' Storage for the Viewer Name as a String
Dim ViewerImageURI as String    ' Storage for the Viewer Image URI as a String
Dim ViewerSourceURI as String   ' Storage for the Viewer Image URI as a String
Dim ViewerMessage as String     ' Storage for the Viewer Message as a String

Dim ExistingImageURI as String    ' Storage for the checking for dups via unique Image URIString

' The details about the Data Title Input being used to access the Viewer Data
Dim DataTitleInputName as String = "DataTitle"
Dim DataTitleInputNumber as String

' Asset names in the GT Title - DataTitle
Dim DataTitleMessageName as String = "Message.Text"             ' Message Text Stored Here
Dim DataTitleFromFullName as String = "fromFullName.Text"       ' Name Text stored Here
Dim DataTitleFromPhotoName as String = "fromPhoto.Source"       ' Photo URI Text stored Here
Dim DataTitleFromSourceName as String = "fromSource.Source"     ' Source URI Text stored Here

DIM NewViewer as Boolean = True     ' Initialize test for entry being New Viewer
Dim ViewerCount as Integer          ' Number of Active Viewers

Dim DataTitleNodeXML as XMLNode ' The XML Node assciated with the Data Title Input [in vMix]

' Set up the XML Document File for our Viewer Data
Dim ViewerDataXML as XmlDocument = new XmlDocument() 	' XML document with the Viewer Data
Dim FILE_PATH as String = "C:\\Users\\Alchemy-Studio\\Desktop\\Active Show\\SA-ViewerData.xml"

Dim SelectedViewer as Integer   ' The index to the viewer we are actively working on

' Now keep running in background
While True
    vmixXML.loadxml(API.XML)        ' Reload VMIX XML each Loop

    ' Find the DataTitle Input
    DataTitleNodeXML = VmixXML.selectSingleNode("/vmix/inputs/input[@title=""" + DataTitleInputName + """]")
    If DataTitleNodeXML Is Nothing Then
        Console.WriteLine("Data Title Input Not Found")
    else
        ' Set the INPUT NUMBER associated with the Data Title Input
        DataTitleInputNumber = DataTitleNodeXML.Attributes.GetNamedItem("number").Value
    End If


    ' See if the Viewer Data XML File already exists
    Try 
        ViewerDataXML.Load(FILE_PATH)
    Catch ex as Exception
        ' If the file doesn't exist, create and initialize it here
        RootNode = ViewerDataXML.CreateElement("activeViewers")     ' Set the XML Root Node
        ViewerDataXML.AppendChild(RootNode)                         ' Append it to the empty XML Document
        ViewerDataXML.Save(FILE_PATH)                               ' Save it out to the file system
    End Try

    ' If the XML Document file already existed, set the RootNode to the first child
    RootNode = ViewerDataXML.FirstChild
    NewViewer = True

    ' Pull the Viewer Information from the Data Title
    ViewerName = DataTitleNodeXML.selectSingleNode("//text[@name=""" + DataTitleFromFullName + """]").InnerText
    ViewerImageURI = DataTitleNodeXML.selectSingleNode("//image[@name=""" + DataTitleFromPhotoName + """]").InnerText
    ViewerMessage = DataTitleNodeXML.selectSingleNode("//text[@name=""" + DataTitleMessageName + """]").InnerText
    ViewerSourceURI = DataTitleNodeXML.selectSingleNode("//image[@name=""" + DataTitleFromSourceName + """]").InnerText


    ' Generate an XMLNodeList with all of the active viewers:
    ViewerList = ViewerDataXML.SelectNodes("/activeViewers/viewer")
    ViewerCount = ViewerList.Count ' This holds the number of viewers

    ' Lets loop through the active viewers
    For SelectedViewer = 0 To ViewerCount-1
        ' Compare Unique Viewer URI to avoid duplicates
        ExistingImageURI = ViewerList.Item(SelectedViewer).Attributes.GetNamedItem("imageURI").Value
        If String.Compare(ViewerImageURI, ExistingImageURI) = 0 Then
            Console.WriteLine("Existing Viewer")
            NewViewer = False
        End If
    Next

    ' ALTERNATE - Check for KEY WORD: if ViewerMessage.contains("#join") Then
    ' This will check if the viewer entered "#join" to add them to the list

    if NewViewer Then
        
        ' We create a child element "viewer" with attributes "name" amd "imageURI"
        ViewerNode =  ViewerDataXML.CreateElement("viewer")         ' XML Node with the new Element "viewer"
        NameNode =  ViewerDataXML.CreateAttribute("name")           ' XML Node with the new Attribute "name"
        NameNode.Value = ViewerName                                 ' Assign ViewerName to the "name" Attribute
    
        ImageURINode =  ViewerDataXML.CreateAttribute("imageURI")   ' XML Node with the new Attribute "imageURI"
        ImageURINode.Value = ViewerImageURI                         ' Assign ViewerImageURI to the "imageURI" Attribute

        SourceURINode =  ViewerDataXML.CreateAttribute("sourceURI")   ' XML Node with the new Attribute "SourceURI"
        SourceURINode.Value = ViewerSourceURI                         ' Assign ViewerSourceURI to the "sourceURI" Attribute

        ViewerNode.Attributes.SetNamedItem(NameNode)                ' Add the "name" attribute
        ViewerNode.Attributes.SetNamedItem(ImageURINode)            ' Add the "imageURI" attribute
        ViewerNode.Attributes.SetNamedItem(SourceURINode)           ' Add the "sourceURI" attribute
        ViewerNode.InnerText = ViewerMessage                        ' Add the message as the element
        RootNode.AppendChild(ViewerNode)                            ' Append the new viewer information to the XML Model
        ViewerDataXML.Save(FILE_PATH)                               ' Save the XML Model
    End If
    Sleep(2000)
End While