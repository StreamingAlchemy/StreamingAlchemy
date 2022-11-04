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
' *****  Set PANEL-A As The Active Panel  *****
' *********************************************

' Set Panel A Guests On-Air in LiveToAir (Channels 1-3)

dim getRequest as HttpWebRequest
try
    getRequest = HttpWebRequest.Create("http://localhost:56000/Macro/?Command=OnAirChannel&Payload=1")  
    getRequest.Method = "GET"
    getRequest.GetResponse()
catch ex as Exception
end try
try
    getRequest = HttpWebRequest.Create("http://localhost:56000/Macro/?Command=OnAirChannel&Payload=2")  
    getRequest.Method = "GET"
    getRequest.GetResponse()
catch ex as Exception
end try
try
    getRequest = HttpWebRequest.Create("http://localhost:56000/Macro/?Command=OnAirChannel&Payload=3")  
    getRequest.Method = "GET"
    getRequest.GetResponse()
catch ex as Exception
end try

'Set Panel B Guests Off-Air in LiveToAir (Channels 4-6)

try
    getRequest = HttpWebRequest.Create("http://localhost:56000/Macro/?Command=OffAirChannel&Payload=4")  
    getRequest.Method = "GET"
    getRequest.GetResponse()
catch ex as Exception
end try

try
    getRequest = HttpWebRequest.Create("http://localhost:56000/Macro/?Command=OffAirChannel&Payload=5")  
    getRequest.Method = "GET"
    getRequest.GetResponse()
catch ex as Exception
end try

try
    getRequest = HttpWebRequest.Create("http://localhost:56000/Macro/?Command=OffAirChannel&Payload=6")  
    getRequest.Method = "GET"
    getRequest.GetResponse()
catch ex as Exception
end try

' Turn-Off Panel B Audio in vMix
API.Function("AudioChannelMatrixApplyPreset", input:="2", value:="OFF-AIR")
API.Function("AudioOff", input:="6")
API.Function("AudioOff", input:="7")
API.Function("AudioOff", input:="8")

'Load Panel B into Green Room
API.Function("SetLayer", input:="21", value:="2,6")
API.Function("SetLayer", input:="21", value:="3,7")
API.Function("SetLayer", input:="21", value:="4,8")
API.Function("SetLayer", input:="21", value:="6,2")

' Load Panel A into Active Slots
API.Function("Cube",Input:="3",Mix:=1)
API.Function("Cube",Input:="4",Mix:=2)
API.Function("Cube",Input:="5",Mix:=3)
API.Function("Cube",Input:="1",Mix:=4)

' Turn-On Panel A Audio in vMix
API.Function("AudioChannelMatrixApplyPreset", input:="1", value:="ON-AIR")
API.Function("AudioOn", input:="3")
API.Function("AudioOn", input:="4")
API.Function("AudioOn", input:="5")
