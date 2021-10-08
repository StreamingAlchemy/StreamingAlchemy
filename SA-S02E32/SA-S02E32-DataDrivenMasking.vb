' STREAMING ALCHEMY - Season 02 Episode 32
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'

' *****************************************
' *****  Data-Driven Dynamic Masking  *****
' *****************************************


' Load the vmx XML Model:
dim vmxXML as new system.xml.xmldocument
vmxXML.loadxml(API.XML)

' Get the currently Active Zone stored in Dynamic Value 1
dim activeNodeValue as String 
activeNodeValue = vmxXML.SelectSingleNode("/vmix/dynamic/value1").InnerText
dim activeZoneIndex As Integer = Convert.ToInt32(activeNodeValue)


' Load the Zone breakouts into an XML Model:
dim requestXML as String = "G:\Episode 32\SA-S02E32-Zones.xml"
dim breakoutXML as XmlDocument = new XmlDocument()
breakoutXML.Load(requestXML)


' Generate XMLNodeList with all of the zones for this asset:
dim zoneList As XmlNodeList = breakoutXML.SelectNodes("/breakout/zone")

' zoneCount holds the number of zones that will be used with this asset
dim zoneCount As Integer = zoneList.Count


'Let's verify that we have a valid zone index for the XML we have loaded

If (activeZoneIndex < 0) Or (activeZoneIndex > (zoneCount)) Then ' Index is outside the available zones
  activeZoneIndex = 0   ' Since index isn't valid, default to the first zone in the list
else
  activeZoneIndex = activeZoneIndex-1 ' If index is valid, reduce by 1 for 0 indexed access
End If

' Now let's start pulling out the data for the selected zone

' Grab the label that describes what this zone hightlights
dim zoneLabel as String = zoneList(activeZoneIndex).Item("label").InnerText


' Pull out the pixel boundaries of the zone and reformat them to Integers
dim edgeLeft as Integer = Convert.ToInt32(zoneList(activeZoneIndex).Item("edgeL").InnerText)
dim edgeRight as Integer = Convert.ToInt32(zoneList(activeZoneIndex).Item("edgeR").InnerText)
dim edgeTop as Integer = Convert.ToInt32(zoneList(activeZoneIndex).Item("edgeT").InnerText)
dim edgeBottom as Integer = Convert.ToInt32(zoneList(activeZoneIndex).Item("edgeB").InnerText)

' Convert the pixel boundaries into the -2 to +2 position ranges in vMix
dim edgeLeftvmxdec as Decimal = ((2*edgeLeft)/1920)-2
dim edgeTopvmxdec as Decimal = 2-((2*edgeTop)/1080)
dim edgeRightvmxdec as Decimal = ((2*edgeRight)/1920)
dim edgeBottomvmxdec as Decimal = ((2*edgeBottom)/1080)*-1

' Convert  the position ranges to STRINGS that we can use in API.FUNCTION calls
dim edgeLeftvmx as String = edgeLeftvmxdec.ToString
dim edgeTopvmx as String = edgeTopvmxdec.ToString
dim edgeRightvmx as String = edgeRightvmxdec.ToString
dim edgeBottomvmx as String = edgeBottomvmxdec.ToString

' Change the positions of the 4 BLACK FLAGS that outline the MASK
API.Function("SetPanX", Input:=2, Value:=edgeLeftvmx)
API.Function("SetPanX", Input:=3, Value:=edgeRightvmx)
API.Function("SetPanY", Input:=4, Value:=edgeTopvmx)
API.Function("SetPanY", Input:=5, Value:=edgeBottomvmx)

' Assign the Zone Title to the GT Title's Label
API.Function("SetText",Input:="ZoneLabel",SelectedName:= "ZoneLabel.Text",Value:= zoneLabel)

' Disply the Title as an overlay on screen for 1.5 Seconds
API.Function("OverlayInput1In", Input:="ZoneLabel")   ' Go ON SCREEN
Sleep(1500)                                           ' Wait 1.5 Seconds
API.Function("OverlayInput1Out", Input:="ZoneLabel")  ' Go OFF SCREEN