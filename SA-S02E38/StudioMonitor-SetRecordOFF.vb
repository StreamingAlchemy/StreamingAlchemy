' STREAMING ALCHEMY - Season 02 Episode 38
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'
'

' ****************************************************************
' *****  Stop NDI Studio Monitor Record from vMix Scripting  *****
' ****************************************************************

' Create XML Document for vMix XML Model
dim VmixXML as new system.xml.xmldocument

'Load the vMix XML Model:
VmixXML.loadxml(API.XML)
    
'Read the Studio Monitor IP from Dynamic Vale 1
DIM Monitor_IP As String = VmixXML.selectSingleNode("/vmix/dynamic/value1").InnerText

' The URL's are for the NDI Studio Monitor instances we want to Record On
Dim URL_Record as String = "http://" + Monitor_IP + "/v1/recording"
Console.WriteLine(URL_Record)

' Control is via a JSON object

Dim JsonPayload_Record as String = "{""recording"":false}"
Console.WriteLine(JsonPayload_Record)
Dim encoding as New System.Text.UTF8Encoding()
Dim JsonPayloadAsByteData_Record as Byte() = encoding.GetBytes(JsonPayload_Record)


' Initiate the Post Request
Dim postRequest as HttpWebRequest = HttpWebRequest.Create(URL_Record)  
postRequest.Method = "POST"
postRequest.ContentType = "application/x-www-form-urlencoded"
postRequest.ContentLength = JsonPayloadAsByteData_Record.Length 

Dim requestStream as Stream = postRequest.GetRequestStream()

requestStream.Write(JsonPayloadAsByteData_Record, 0, JsonPayloadAsByteData_Record.Length)
requestStream.Close()