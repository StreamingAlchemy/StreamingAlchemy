' STREAMING ALCHEMY - Season 02 Episode 36
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'
'

' ***************************************************************
' *****  Keep SuperJoy Selection in sync with vMix PREVIEW  *****
' ***************************************************************


dim PrevInput As String         ' This is where we store the Input currently in PREVIEW
dim OldPrev As String = ""      ' This is where we store the last Input we sent to SuperJoy

Dim ActiveCAM As WebRequest         ' This is where we set the Camera Selection API Call
Dim TriggerResponse As WebResponse  ' This is where we put the response when we Trigger the WebRequest

' Create XML Document for vMix XML Model
dim VmixXML as new system.xml.xmldocument

' Continuously loop to see which input is in PREVIEW
While TRUE
    'Load the vMix XML Model:
    VmixXML.loadxml(API.XML)
    
    'Get Number of the Input in Preview:
    PrevInput = VmixXML.selectSingleNode("/vmix/preview").InnerText

    If PrevInput = OldPrev Then ' If PREVIEW has the INPUT we already set in the SuperJoy
        Sleep(1000)             ' Sleep for 1 Second...
        Continue While          ' ...then go back and check again
    Else                        ' If PREVIEW has a different INPUT
        OldPrev = PrevInput     ' Save a copy of it in OldPrev

        ' Trigger the SuperJoy HTTP API Call to select a specific CAMERA in a specific GROUP
        ActiveCAM = WebRequest.Create("http://10.0.0.123/cgi-bin/joyctrl.cgi?f=camselect&group=1&camid=" & PrevInput)
        TriggerResponse = ActiveCAM.GetResponse()   ' Execute WebRequest and receive the response...
        TriggerResponse.Close()                         ' ... then close out the Request
    End if
    Sleep(1000)                                         ' Sleep for 1 Second...
End While                                               ' ...and continue checking if PREVIEW changes