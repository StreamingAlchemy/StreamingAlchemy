' STREAMING ALCHEMY - Season 02 Episode 14
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'
'


dim ActiveInput as String = ""          ' The NUMBER of the INPUT in PREVIEW

dim VmixXML as new system.xml.xmldocument
VmixXML.loadxml(API.XML)

'Get the Input in PREVIEW from the vMix XML:
ActiveInput = VmixXML.selectSingleNode("/vmix/preview").InnerText 

'Set the top layer source back to 'NONE':

API.Function("SetMultiViewOverlay",Input:=ActiveInput, Value:="10,None")