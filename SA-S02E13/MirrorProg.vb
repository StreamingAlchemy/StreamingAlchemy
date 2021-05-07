' STREAMING ALCHEMY - Season 02 Episode 13
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'
'


dim Mix1Input as String = ""          ' The NUMBER of the INPUT in MIX1
dim Mix4Input as String = ""          ' The NUMBER of the INPUT in MIX2

'Load the vMix XML Model:

dim VmixXML as new system.xml.xmldocument

While True
    'Load the vMix XML Model:
    VmixXML.loadxml(API.XML)

    'Get the XMLNode for the Input in PREVIEW:
    Mix1Input = VmixXML.selectSingleNode("/vmix/active").InnerText 

    If Mix1Input <> Mix4Input Then
        API.Function("Cut", Input:=Mix1Input, Mix:="3")
        Mix4Input = Mix1Input
    End If
    Sleep(250)
End While

