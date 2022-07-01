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
'          STREAMING ALCHEMY - Season 03 Episode 10
'
'                Sponsored by Gnural Net, Inc.
'                     www.gnuralnet.com
'                       609-223-2434
'
'
' ****************************************************
' *****  Enter Player Number To Select Graphics  *****
' ****************************************************



'           *****************************
'             Handle Digit Press for #7
'           *****************************


dim DigitPress as String = "7"

' Set up access to the vMix XML model
dim VmixXML as new system.xml.xmldocument
VmixXML.loadxml(API.XML)

' Load in previous digits pressed (If Any)
dim PlayerNumber as String = VmixXML.selectSingleNode("/vmix/dynamic/value3").InnerText

' If we have less than two digits stored, append this digit to it
if PlayerNumber.Length() < 2
    PlayerNumber += DigitPress
Else
    console.writeline("Number must be 1 or 2 digits!")
End If

' Set DynamicValue3 to player number before assignment
API.Function("SetDynamicValue3", Value:=PlayerNumber)

' Show the Player number on our Blank Jersey
API.Function("SetText", input:=2, SelectedName:="NUMBER.Text", value:=PlayerNumber)	

