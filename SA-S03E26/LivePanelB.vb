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
'          STREAMING ALCHEMY - Season 03 Episode 26
'
'                Sponsored by Gnural Net, Inc.
'                     www.gnuralnet.com
'                       609-223-2434
'
'
' *********************************************
' *****  Set PANEL-B As The Active Panel  *****
' *********************************************

' Set Panel B Guests On-Air in LiveToAir (Channels 4-6)

dim getRequest as HttpWebRequest
try
    getRequest = HttpWebRequest.Create("http://localhost:56000/Macro/?Command=OnAirChannel&Payload=4")  
    getRequest.Method = "GET"
    getRequest.GetResponse()
catch ex as Exception
end try
try
    getRequest = HttpWebRequest.Create("http://localhost:56000/Macro/?Command=OnAirChannel&Payload=5")  
    getRequest.Method = "GET"
    getRequest.GetResponse()
catch ex as Exception
end try
try
    getRequest = HttpWebRequest.Create("http://localhost:56000/Macro/?Command=OnAirChannel&Payload=6")  
    getRequest.Method = "GET"
    getRequest.GetResponse()
catch ex as Exception
end try

'Set Panel A Guests Off-Air in LiveToAir (Channels 1-3)

try
    getRequest = HttpWebRequest.Create("http://localhost:56000/Macro/?Command=OffAirChannel&Payload=1")  
    getRequest.Method = "GET"
    getRequest.GetResponse()
catch ex as Exception
end try

try
    getRequest = HttpWebRequest.Create("http://localhost:56000/Macro/?Command=OffAirChannel&Payload=2")  
    getRequest.Method = "GET"
    getRequest.GetResponse()
catch ex as Exception
end try

try
    getRequest = HttpWebRequest.Create("http://localhost:56000/Macro/?Command=OffAirChannel&Payload=3")  
    getRequest.Method = "GET"
    getRequest.GetResponse()
catch ex as Exception
end try

' Turn-Off Panel A Audio in vMix
API.Function("AudioChannelMatrixApplyPreset", input:="1", value:="OFF-AIR")
API.Function("AudioOff", input:="3")
API.Function("AudioOff", input:="4")
API.Function("AudioOff", input:="5")

'Load Panel A into Green Room
API.Function("SetLayer", input:="21", value:="2,3")
API.Function("SetLayer", input:="21", value:="3,4")
API.Function("SetLayer", input:="21", value:="4,5")
API.Function("SetLayer", input:="21", value:="6,1")

' Load Panel B into Active Slots
API.Function("Cube",Input:="6",Mix:=1)
API.Function("Cube",Input:="7",Mix:=2)
API.Function("Cube",Input:="8",Mix:=3)
API.Function("Cube",Input:="2",Mix:=4)

' Turn-On Panel B Audio in vMix
API.Function("AudioChannelMatrixApplyPreset", input:="2", value:="ON-AIR")
API.Function("AudioOn", input:="6")
API.Function("AudioOn", input:="7")
API.Function("AudioOn", input:="8")