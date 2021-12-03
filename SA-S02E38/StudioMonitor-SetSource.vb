' STREAMING ALCHEMY - Season 02 Episode 38
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'
'

' *******************************************************************
' *****  Control NDI Studio Monitor Source from vMix Scripting  *****
' *******************************************************************


' Dim JsonPayload-Record as String = "{""recording"":true""}"
' Dim JsonPayloadAsByteData-Record as Byte() = encoding.GetBytes(JsonPayload)
' Dim URL-Record as String = "http://10.0.0.244:80/v1/recording"

' Create XML Document for vMix XML Model
dim VmixXML as new system.xml.xmldocument

'Load the vMix XML Model:
VmixXML.loadxml(API.XML)
    
'Read the Studio Monitor IP from Dynamic Vale 1 and the NDI Source Name from Dynamic Value 2
DIM Monitor_IP As String = VmixXML.selectSingleNode("/vmix/dynamic/value1").InnerText
DIM Monitor_Source As String = VmixXML.selectSingleNode("/vmix/dynamic/value2").InnerText

' The URL's are for the NDI Studio Monitor instances we want to change sources for
Dim URL_Source as String = "http://" + Monitor_IP + "/v1/configuration"

' Control is via a JSON object
' Set the source from Monitor_Source
Dim JsonPayload_Source as String = "{""version"":1,""NDI_source"":""" + Monitor_Source + """}"
Dim encoding as New System.Text.UTF8Encoding()
Dim JsonPayloadAsByteData_Source as Byte() = encoding.GetBytes(JsonPayload_Source)


' Initiate the Post Request
Dim postRequest as HttpWebRequest = HttpWebRequest.Create(URL_Source)  
postRequest.Method = "POST"
postRequest.ContentType = "application/x-www-form-urlencoded"
postRequest.ContentLength = JsonPayloadAsByteData_Source.Length 

Dim requestStream as Stream = postRequest.GetRequestStream()

requestStream.Write(JsonPayloadAsByteData_Source, 0, JsonPayloadAsByteData_Source.Length)
requestStream.Close()