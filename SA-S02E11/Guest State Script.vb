
dim VmixXML as new system.xml.xmldocument

'Load the vMix XML Model:

VmixXML.loadxml(API.XML)

'Isolate the XML Nodes for each Guest Input:
 
dim Guest1Node As XmlNode = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""6eaed1d2-cf4e-4ef2-86c1-8edafc73f981""]")
dim Guest2Node As XmlNode = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""f4e90d87-733a-4802-a9bc-dd557aaa9a23""]")
dim Guest3Node As XmlNode = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""fa6ad503-5e42-4524-a830-611d74d7dc66""]")
dim Guest4Node As XmlNode = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""9f2c6956-b33d-4276-91fb-3b3ebf1119ea""]")


If Guest1Node.Attributes.GetNamedItem("muted").Value = "False" Then
    If Guest1Node.Attributes.GetNamedItem("audiobusses").Value.contains("M") Then
        API.Function("SetImage", "StateOverlay", "C:\Users\JM\Desktop\Active\OnAir-Icon.png", 100, "G1-STATE.Source")
    else
        API.Function("SetImage", "StateOverlay", "C:\Users\JM\Desktop\Active\OffAir-Icon.png", 100, "G1-STATE.Source")
    end if
else
    API.Function("SetImage", "StateOverlay", "C:\Users\JM\Desktop\Active\Mute-Icon.png", 100, "G1-STATE.Source")
end if


If Guest2Node.Attributes.GetNamedItem("muted").Value = "False" Then
    If Guest2Node.Attributes.GetNamedItem("audiobusses").Value.contains("M") Then
        API.Function("SetImage", "StateOverlay", "C:\Users\JM\Desktop\Active\OnAir-Icon.png", 100, "G2-STATE.Source")
    else
        API.Function("SetImage", "StateOverlay", "C:\Users\JM\Desktop\Active\OffAir-Icon.png", 100, "G2-STATE.Source")
    end if
else
    API.Function("SetImage", "StateOverlay", "C:\Users\JM\Desktop\Active\Mute-Icon.png", 100, "G2-STATE.Source")
end if


If Guest3Node.Attributes.GetNamedItem("muted").Value = "False" Then
    If Guest3Node.Attributes.GetNamedItem("audiobusses").Value.contains("M") Then
        API.Function("SetImage", "StateOverlay", "C:\Users\JM\Desktop\Active\OnAir-Icon.png", 100, "G3-STATE.Source")
    else
        API.Function("SetImage", "StateOverlay", "C:\Users\JM\Desktop\Active\OffAir-Icon.png", 100, "G3-STATE.Source")
    end if
else
    API.Function("SetImage", "StateOverlay", "C:\Users\JM\Desktop\Active\Mute-Icon.png", 100, "G3-STATE.Source")
end if



If Guest4Node.Attributes.GetNamedItem("muted").Value = "False" Then
    If Guest4Node.Attributes.GetNamedItem("audiobusses").Value.contains("M") Then
        API.Function("SetImage", "StateOverlay", "C:\Users\JM\Desktop\Active\OnAir-Icon.png", 100, "G4-STATE.Source")
    else
        API.Function("SetImage", "StateOverlay", "C:\Users\JM\Desktop\Active\OffAir-Icon.png", 100, "G4-STATE.Source")
    end if
else
    API.Function("SetImage", "StateOverlay", "C:\Users\JM\Desktop\Active\Mute-Icon.png", 100, "G4-STATE.Source")
end if

