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
'          STREAMING ALCHEMY - Season 03 Episode 30
'
'                Sponsored by Gnural Net, Inc.
'                     www.gnuralnet.com
'                       609-223-2434
'
'
' ****************************************************************
' *****  Paint In Input Details On Custom MultiView Overlay  *****
' ****************************************************************



' **********************************************************

' Inputs Used To Construct the Custom MultiView

dim MV0108Input as String = "1"         ' Input with MultiView Sources 1-8
dim MV0916Input as String = "2"         ' Input with MultiView Sources 9-16
dim MVBGRings as String = "3"           ' The NUMBER of the INPUT with the MultiView Ring (BG Square)
dim MVOverlayInput as String = "4"      ' The NUMBER of the INPUT with the MultiView Overlay
dim PREVOverlayInput as String = "6"    ' The NUMBER of the INPUT with the Preview Overlay
dim PREVTitleInput as String = "7"      ' The NUMBER of the INPUT with the Preview Title Frame

' Values Used To Analyze vMix XML

dim ActiveInput as String = ""          ' The NUMBER of the INPUT in PROGRAM
dim PreviewInput as String = ""         ' The NUMBER of the INPUT in PREVIEW

dim AssignedInputKey as String = ""     ' The KEY of the specified Layer
dim MVSlotInputNumber as String = ""    ' The NUMBER of the Layer I'm working on now [Used As NUMBER Label]
dim LayerInputTitle as String = ""      ' The TITLE of the Layer I'm working on now [Used As NAME Label]
dim MVSlotLabel as String = ""          ' Use to assemble the full label
dim MVInputTitle(16) as String          ' Store the Titles of all of the MV Inputs
dim PREVvalue as String                 ' Used for Preview Popup

'Counters used for looping through the layers
dim MV0108Layers As Integer = 0         ' Total Number of Layers in MV 1-8
dim MV0916Layers As Integer = 0         ' Total Number of Layers in MV 9-16
dim MVSlotIndex As Integer = 0          ' The Layer currently being worked on
dim LayerIndex As Integer = 0           ' The Layer associated with the data
dim MVSlotLayerOffset As Integer = 0    ' Handle offset between MV Slots and Input Layers (When NONE are in layers)


'Variables to work with the vMix XML Model:
dim LayerNode As XMLNode
dim VmixXML as new system.xml.xmldocument
dim MV0108XMLNodeList As XmlNodeList  
dim MV0916XMLNodeList As XmlNodeList

' *****************************************************************************************
' FUTURE: Fill In The Initial Titles for MV to reduce rewriting (Currently Not Being Used)
VmixXML.loadxml(API.XML)

For MVSlotIndex = 0 To MV0108Layers-1
    AssignedInputKey = MV0108XMLNodeList.Item(MVSlotIndex).Attributes.GetNamedItem("key").Value
    
    MVInputTitle(MVSlotIndex+1) = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""" & AssignedInputKey & """]").Attributes.GetNamedItem("title").Value
Next
' *****************************************************************************************



' /////////////////////////////////////////////////////////////////////////
' /////////////////////////////////////////////////////////////////////////
' Start Looping in the background to dynamically update the MultiView

While(True)

    VmixXML.loadxml(API.XML)    ' Load the vMix XML to update status

    'Get the XMLNode for the Input in PROGRAM and PREVIEW:
    ActiveInput = VmixXML.selectSingleNode("/vmix/active").InnerText        ' PROGRAM Input
    PreviewInput = VmixXML.selectSingleNode("/vmix/preview").InnerText      ' PREVIEW Input
    
    ' We will assign the current PREVIEW input to Layer 1 in PREVIEW OVERLAY
    PREVvalue = "1," & PreviewInput
    API.Function("SetLayer", Value:=PREVvalue, Input:=PREVOverlayInput)
    

    'Create an XMLNodeList of LAYERS in both MV Inputs:
    MV0108XMLNodeList = VmixXML.selectSingleNode("/vmix/inputs/input[@number=""" & MV0108Input & """]").SelectNodes("overlay") 
    MV0916XMLNodeList = VmixXML.selectSingleNode("/vmix/inputs/input[@number=""" & MV0916Input & """]").SelectNodes("overlay") 


    'Get the count of layers we're working with:
    MV0108Layers = MV0108XMLNodeList.Count  ' Count of used layers in Input used in MV1-8
    MV0916Layers = MV0916XMLNodeList.Count  ' Count of used layers in Input used in MV9-16


    ' Set up indicies for looping the layers
    MVSlotIndex = 0             ' The Multiview Slot we are working on (Zero-based indexing)
    MVSlotLayerOffset = 0       ' Offset for the number of unassigned layers

    ' /////////////////////////////////////////////////////////////////////////
    'Loop thru Input 1 Layers to fill in the first 8 MV slots (1-8)

    While(MV0108Layers > 0)     ' We will decrement as we loop through the layers

        ' Get the KEY of the INPUT in the current layer 
        AssignedInputKey = MV0108XMLNodeList.Item(MVSlotIndex - MVSlotLayerOffset).Attributes.GetNamedItem("key").Value
    
        ' Load it's INDEX (Which Layer it is) and it's associated TITLE and INPUT Number
        LayerIndex = MV0108XMLNodeList.Item(MVSlotIndex - MVSlotLayerOffset).Attributes.GetNamedItem("index").Value
        MVSlotInputNumber = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""" & AssignedInputKey & """]").Attributes.GetNamedItem("number").Value
        LayerInputTitle = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""" & AssignedInputKey & """]").Attributes.GetNamedItem("title").Value

        ' ********************************************************************************
        ' NOTE: If some Layers are set to NONE, "MVSlotIndex - MVSlotLayerOffset" points
        ' to the layer that corresponds to the current MultiView Slot
        ' ********************************************************************************


        ' Check if there are layers - Exit While if none
        If MVSlotInputNumber Is Nothing Then 
            MVSlotInputNumber = ""
            LayerInputTitle = ""
            Exit Sub
        END IF


    'Set the TITLE and INPUT Number for this Slot in the MultiView Title
        
        ' First check if there is no Input associated with this MV Slot:
        
        If MVSlotIndex <> CInt(LayerIndex) Then ' Check if the current MV Slot is empty ("none")
            Console.WriteLine("No corresponding Layer for Slot")
            MVSlotLabel = "UNASSIGNED"  ' Set TITLE to 'UNASSIGNED'
            MVSlotInputNumber = "--"    ' Set INPUT to '--'
            
            ' Apply changes to the GT Title
            API.Function("SetText", Value:=MVSlotInputNumber, SelectedName:=("NUMB-CH" & CStr(MVSlotIndex+1) & ".Text"), Input:=MVOverlayInput)
            API.Function("SetText", Value:=MVSlotLabel, SelectedName:=("NAME-CH" & CStr(MVSlotIndex+1) & ".Text"), Input:=MVOverlayInput)
            API.Function("SetColor", Value:="#2d2d2d", SelectedName:=("BGSTAT-CH" & CStr(MVSlotIndex+1) & ".Fill.Color"), Input:=MVOverlayInput)
            API.Function("SetColor", Value:="#1d1d1d", SelectedName:=("BGRING-CH" & CStr(MVSlotIndex+1) & ".Fill.Color"), Input:=MVBGRings)
            API.Function("SetTextColour", Value:="#3376FE", SelectedName:=("NUMB-CH" & CStr(MVSlotIndex+1) & ".Text"), Input:=MVOverlayInput)
            API.Function("SetTextColour", Value:="#3376FE", SelectedName:=("NAME-CH" & CStr(MVSlotIndex+1) & ".Text"), Input:=MVOverlayInput)

            MVSlotIndex += 1        ' Now point to the next MV Slot
            MVSlotLayerOffset += 1  ' Since this MV Slot has no corresponding Layer, increment Offset
            Continue While          ' Jump back to the top of the WHILE LOOP
        End If

       
        ' If the length of the TITLE is over 20 Characters, truncate it
        If (LayerInputTitle.Length <= 20) Then 
            MVSlotLabel = LayerInputTitle
        Else
            MVSlotLabel = LayerInputTitle.substring(0, 20)
        End If

        ' Apply the TITLE and INPUT Number to the GT Title
        API.Function("SetText", Value:=MVSlotInputNumber, SelectedName:=("NUMB-CH" & CStr(MVSlotIndex+1) & ".Text"), Input:=MVOverlayInput)
        API.Function("SetText", Value:=MVSlotLabel, SelectedName:=("NAME-CH" & CStr(MVSlotIndex+1) & ".Text"), Input:=MVOverlayInput)

        ' Now set the correct Colors for everything
        If MVSlotInputNumber = ActiveInput Then ' If in PROGRAM, set RING and NUMBER to RED
            API.Function("SetColor", Value:="#FF2222", SelectedName:=("BGSTAT-CH" & CStr(MVSlotIndex+1) & ".Fill.Color"), Input:=MVOverlayInput)
            API.Function("SetColor", Value:="#FF2222", SelectedName:=("BGRING-CH" & CStr(MVSlotIndex+1) & ".Fill.Color"), Input:=MVBGRings)
            API.Function("SetTextColour", Value:="#FF2222", SelectedName:=("NUMB-CH" & CStr(MVSlotIndex+1) & ".Text"), Input:=MVOverlayInput)
            API.Function("SetTextColour", Value:="#FFFFFF", SelectedName:=("NAME-CH" & CStr(MVSlotIndex+1) & ".Text"), Input:=MVOverlayInput)
        Else If MVSlotInputNumber = PreviewInput Then  ' If in PREVIEW, set RING and NUMBER to GREEN
            API.Function("SetColor", Value:="#1c9e49", SelectedName:=("BGSTAT-CH" & CStr(MVSlotIndex+1) & ".Fill.Color"), Input:=MVOverlayInput)
            API.Function("SetColor", Value:="#1c9e49", SelectedName:=("BGRING-CH" & CStr(MVSlotIndex+1) & ".Fill.Color"), Input:=MVBGRings)
            API.Function("SetTextColour", Value:="#1c9e49", SelectedName:=("NUMB-CH" & CStr(MVSlotIndex+1) & ".Text"), Input:=MVOverlayInput)
            API.Function("SetTextColour", Value:="#FFFFFF", SelectedName:=("NAME-CH" & CStr(MVSlotIndex+1) & ".Text"), Input:=MVOverlayInput)
            ' Populate the Pop-Up PREVIEW Template
            API.Function("SetText", Value:=MVSlotLabel, SelectedName:=("PREV-NAME.Text"), Input:=PREVTitleInput)
            API.Function("SetText", Value:=MVSlotInputNumber, SelectedName:=("PREV-NUMBER.Text"), Input:=PREVTitleInput)
        Else  ' Otherwise, set RING and NUMBER to default colors
            API.Function("SetColor", Value:="#2d2d2d", SelectedName:=("BGSTAT-CH" & CStr(MVSlotIndex+1) & ".Fill.Color"), Input:=MVOverlayInput)
            API.Function("SetColor", Value:="#1d1d1d", SelectedName:=("BGRING-CH" & CStr(MVSlotIndex+1) & ".Fill.Color"), Input:=MVBGRings)
            API.Function("SetTextColour", Value:="#FFFFFF", SelectedName:=("NUMB-CH" & CStr(MVSlotIndex+1) & ".Text"), Input:=MVOverlayInput)
            API.Function("SetTextColour", Value:="#FFFFFF", SelectedName:=("NAME-CH" & CStr(MVSlotIndex+1) & ".Text"), Input:=MVOverlayInput)
        End If
        MV0108Layers -= 1   ' Decrement the remaining layers to process by 1
        MVSlotIndex += 1    ' Increment the slot we are working on by 1
    End While

    ' Reset the indicies for looping the layers
    MVSlotIndex = 0             ' The Multiview Slot we are working on (Zero-based indexing)
    MVSlotLayerOffset = 0       ' Offset for the number of unassigned layers

    'Loop thru Input 2 Layers to fill in the next 8 MV slots (9-16)
    While(MV0916Layers > 0)

        ' Get the KEY of the INPUT in the current layer 
        AssignedInputKey = MV0916XMLNodeList.Item(MVSlotIndex - MVSlotLayerOffset).Attributes.GetNamedItem("key").Value
    
        ' Load it's INDEX (Which Layer it is) and it's associated TITLE and INPUT Number
        LayerIndex = MV0916XMLNodeList.Item(MVSlotIndex - MVSlotLayerOffset).Attributes.GetNamedItem("index").Value
        MVSlotInputNumber = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""" & AssignedInputKey & """]").Attributes.GetNamedItem("number").Value
        LayerInputTitle = VmixXML.selectSingleNode("/vmix/inputs/input[@key=""" & AssignedInputKey & """]").Attributes.GetNamedItem("title").Value
    
    
        ' ********************************************************************************
        ' NOTE: If some Layers are set to NONE, "MVSlotIndex - MVSlotLayerOffset" points
        ' to the layer that corresponds to the current MultiView Slot
        ' ********************************************************************************
        ' Check if there are layers - Exit While if none
        If MVSlotInputNumber Is Nothing Then 
            MVSlotInputNumber = ""
            LayerInputTitle = ""
            Exit Sub
        END IF
 
        'Set the TITLE and INPUT Number for this Slot in the MultiView Title
        
        ' First check if there is no Input associated with this MV Slot
        If MVSlotIndex <> CInt(LayerIndex) Then ' Check if the current MV Slot is empty ("none")
            MVSlotLabel = "UNASSIGNED"  ' Set TITLE to 'UNASSIGNED'
            MVSlotInputNumber = "--"    ' Set INPUT to '--'
            
            ' Apply changes to the GT Title
            API.Function("SetText", Value:=MVSlotInputNumber, SelectedName:=("NUMB-CH" & CStr(MVSlotIndex+9) & ".Text"), Input:=MVOverlayInput)
            API.Function("SetText", Value:=MVSlotLabel, SelectedName:=("NAME-CH" & CStr(MVSlotIndex+9) & ".Text"), Input:=MVOverlayInput)
            API.Function("SetColor", Value:="#2d2d2d", SelectedName:=("BGSTAT-CH" & CStr(MVSlotIndex+9) & ".Fill.Color"), Input:=MVOverlayInput)
            API.Function("SetColor", Value:="#1d1d1d", SelectedName:=("BGRING-CH" & CStr(MVSlotIndex+9) & ".Fill.Color"), Input:=MVBGRings)
            API.Function("SetTextColour", Value:="#3376FE", SelectedName:=("NUMB-CH" & CStr(MVSlotIndex+9) & ".Text"), Input:=MVOverlayInput)
            API.Function("SetTextColour", Value:="#3376FE", SelectedName:=("NAME-CH" & CStr(MVSlotIndex+9) & ".Text"), Input:=MVOverlayInput)

            MVSlotIndex += 1        ' Now point to the next MV Slot
            MVSlotLayerOffset += 1  ' Since this MV Slot has no corresponding Layer, increment Offset
            Continue While          ' Jump back to the top of the WHILE LOOP
        End If

 
        ' We do have a Layer corresponding to the current MV Slot

        ' If the length of the TITLE is over 20 Characters, truncate it       
        If (LayerInputTitle.Length <= 20) Then 
            MVSlotLabel = LayerInputTitle
        Else
            MVSlotLabel = LayerInputTitle.substring(0, 20)
        End If

        ' Apply the TITLE and INPUT Number to the GT Title
        API.Function("SetText", Value:=MVSlotInputNumber, SelectedName:=("NUMB-CH" & CStr(MVSlotIndex+9) & ".Text"), Input:=MVOverlayInput)
        API.Function("SetText", Value:=MVSlotLabel, SelectedName:=("NAME-CH" & CStr(MVSlotIndex+9) & ".Text"), Input:=MVOverlayInput)

        ' Now set the correct Colors for everything        
        If MVSlotInputNumber = ActiveInput Then ' If in PROGRAM, set RING and NUMBER to RED
            API.Function("SetColor", Value:="#FF2222", SelectedName:=("BGSTAT-CH" & CStr(MVSlotIndex+9) & ".Fill.Color"), Input:=MVOverlayInput)
            API.Function("SetColor", Value:="#FF2222", SelectedName:=("BGRING-CH" & CStr(MVSlotIndex+9) & ".Fill.Color"), Input:=MVBGRings)
            API.Function("SetTextColour", Value:="#FF2222", SelectedName:=("NUMB-CH" & CStr(MVSlotIndex+9) & ".Text"), Input:=MVOverlayInput)
            API.Function("SetTextColour", Value:="#FFFFFF", SelectedName:=("NAME-CH" & CStr(MVSlotIndex+9) & ".Text"), Input:=MVOverlayInput)
        Else If MVSlotInputNumber = PreviewInput Then  ' If in PREVIEW, set RING and NUMBER to GREEN
            API.Function("SetColor", Value:="#1c9e49", SelectedName:=("BGSTAT-CH" & CStr(MVSlotIndex+9) & ".Fill.Color"), Input:=MVOverlayInput)
            API.Function("SetColor", Value:="#1c9e49", SelectedName:=("BGRING-CH" & CStr(MVSlotIndex+9) & ".Fill.Color"), Input:=MVBGRings)
            API.Function("SetTextColour", Value:="#1c9e49", SelectedName:=("NUMB-CH" & CStr(MVSlotIndex+9) & ".Text"), Input:=MVOverlayInput)
            API.Function("SetTextColour", Value:="#FFFFFF", SelectedName:=("NAME-CH" & CStr(MVSlotIndex+9) & ".Text"), Input:=MVOverlayInput)
            ' Populate the Pop-Up PREVIEW Template
            API.Function("SetText", Value:=MVSlotLabel, SelectedName:=("PREV-NAME.Text"), Input:=PREVTitleInput)
            API.Function("SetText", Value:=MVSlotInputNumber, SelectedName:=("PREV-NUMBER.Text"), Input:=PREVTitleInput)
        Else  ' Otherwise, set RING and NUMBER to default colors
            API.Function("SetColor", Value:="#2d2d2d", SelectedName:=("BGSTAT-CH" & CStr(MVSlotIndex+9) & ".Fill.Color"), Input:=MVOverlayInput)
            API.Function("SetColor", Value:="#1d1d1d", SelectedName:=("BGRING-CH" & CStr(MVSlotIndex+9) & ".Fill.Color"), Input:=MVBGRings)
            API.Function("SetTextColour", Value:="#FFFFFF", SelectedName:=("NUMB-CH" & CStr(MVSlotIndex+9) & ".Text"), Input:=MVOverlayInput)
            API.Function("SetTextColour", Value:="#FFFFFF", SelectedName:=("NAME-CH" & CStr(MVSlotIndex+9) & ".Text"), Input:=MVOverlayInput)
        End If
        MV0916Layers -= 1   ' Decrement the remaining layers to process by 1
        MVSlotIndex += 1    ' Increment the slot we are working on by 1
    End While

Sleep(750)
End While