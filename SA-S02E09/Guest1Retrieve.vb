'Set up and initialize variables

Dim GuestMapInput As Integer = 1
Dim GuestVideoInput As String = "3"
Dim CompositeInput As Integer = 2
Dim GuestTitleInput As Integer = 14

' Set up for working with Guest MetaData written by LiveToAir
Dim LiveToAirMetaDataFolder As String = "\\Alchemy-Studio\ActiveShow"

Dim xmlGuestsMetaData As New XmlDataDocument()
Dim xmlGuestNode As XmlNode

Dim GuestLocation As String
Dim GuestName As String

Dim xmlGuestsFileStream As New FileStream(LiveToAirMetaDataFolder+"\LTAMetaData.xml", FileMode.Open, FileAccess.Read)

xmlGuestsMetaData.Load(xmlGuestsFileStream)
xmlGuestNode = xmlGuestsMetaData.selectSingleNode("/LTA/Guest-1")

' Retrieve the Location value for each guest:

GuestLocation = xmlGuestNode.selectSingleNode("Location").InnerText
GuestName = xmlGuestNode.selectSingleNode("Name").InnerText

' XML Path: <LTA><Guest-1><Name>John Smith</Name><Company></Company><Location>Hamilton, NJ</Location>

' This will let us encode the Guest Location information into a URL friendly string

dim stringToEncode as String = GuestLocation
stringToEncode = stringToEncode.Replace("%", "%25")
stringToEncode = stringToEncode.Replace(" ", "%20")
stringToEncode = stringToEncode.Replace("!", "%21")
stringToEncode = stringToEncode.Replace("""", "%22")
stringToEncode = stringToEncode.Replace("#", "%23")
stringToEncode = stringToEncode.Replace("$", "%24")
stringToEncode = stringToEncode.Replace("&", "%26")
stringToEncode = stringToEncode.Replace("'", "%27")
stringToEncode = stringToEncode.Replace("(", "%28")
stringToEncode = stringToEncode.Replace(")", "%29")
stringToEncode = stringToEncode.Replace("*", "%2A")
stringToEncode = stringToEncode.Replace("+", "%2B")
stringToEncode = stringToEncode.Replace(",", "%2C")
stringToEncode = stringToEncode.Replace("-", "%2D")
stringToEncode = stringToEncode.Replace(".", "%2E")
stringToEncode = stringToEncode.Replace("/", "%2F")
stringToEncode = stringToEncode.Replace(":", "%3A")
stringToEncode = stringToEncode.Replace(";", "%3B")
stringToEncode = stringToEncode.Replace("<", "%3C")
stringToEncode = stringToEncode.Replace("=", "%3D")
stringToEncode = stringToEncode.Replace(">", "%3E")
stringToEncode = stringToEncode.Replace("?", "%3F")
stringToEncode = stringToEncode.Replace("@", "%40")
stringToEncode = stringToEncode.Replace("[", "%5B")
stringToEncode = stringToEncode.Replace("\", "%5C")
stringToEncode = stringToEncode.Replace("]", "%5D")
stringToEncode = stringToEncode.Replace("^", "%5E")
stringToEncode = stringToEncode.Replace("_", "%5F")
stringToEncode = stringToEncode.Replace("`", "%60")
stringToEncode = stringToEncode.Replace("{", "%7B")
stringToEncode = stringToEncode.Replace("|", "%7C")
stringToEncode = stringToEncode.Replace("}", "%7D")
stringToEncode = stringToEncode.Replace("~", "%7E")

' Initialize the text in our 'Guest Title' input

API.Function("SetText", input:=14, SelectedName:="GuestLocation.Text", value:="- Retrieving -")
API.Function("SetText", input:=14, SelectedName:="GuestName.Text", value:="- Connecting -")

' Hide MAP and direct browser to location

API.Function("SetLayer", input:=CompositeInput, value:="1,13")
API.function("BrowserNavigate",input:=GuestMapInput,value:="https://earth.google.com/web/search/"+stringToEncode)

' Play static in Guest video while location is recalled in browser

API.Function("MultiViewOverlayOn", input:=GuestMapInput,value:="1")
Sleep(3100)

' Turn Everything back on 

API.Function("MultiViewOverlayOff", input:=GuestMapInput,value:="1")
API.Function("SetLayer", input:=CompositeInput, value:="1,"+GuestVideoInput)

' Fill in data in Title Input

API.Function("SetText", input:=14, SelectedName:="GuestName.Text", value:=GuestName)
API.Function("SetText", input:=14,  SelectedName:="GuestLocation.Text", value:=GuestLocation)