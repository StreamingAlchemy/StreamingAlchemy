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
'          STREAMING ALCHEMY - Season 04 Episode 07
'
'                Sponsored by Gnural Net, Inc.
'                     www.gnuralnet.com
'                       609-223-2434
'
'
' ************************************************
' *****  Game Show Quiz - Control Framework  *****
' ************************************************


dim i as Integer                        ' General index variable for looping

dim Action as Integer = 0               ' Number of the Action to be performed
dim ActionValue as Integer = 0          ' Value associated with the action to be performed

dim NamePlayerA as String = ""	        ' Name of Player A
dim NamePlayerB as String = ""          ' Name of Player B
dim ScorePlayerA as Integer = 0         ' Score for Team A
dim ScorePlayerB as Integer = 0         ' Score for Team B

dim ActiveQuestion as Integer = 0       ' Index of the Active Question
dim ValidResponses as Integer = 7       ' Number of Valid Responses
dim GaveResponse(7) as Boolean          ' Was this response given?

dim CorrectResponses as Integer = 0     ' Number of Correct Responses Given
dim WrongResponses as Integer = 0       ' Number of Correct Responses Given
dim MaxWrongResponses as Integer = 3    ' Number of Wrong Responses Allowed
dim ResponseValue as Integer = 0        ' Value of the Selected Correct Response
dim CurrentRound as Integer = 0         ' Round of the Quiz Show


dim PlayerAImageURL as String	    ' Image A URL of Player
dim PlayerBImageURL as String	    ' Image B URL of Player

' VMix Input Names
dim QuizTitleInput as String = "QuestionTitle"          'Title Input - Main Quiz Title
dim WinnerOverlayInput as String = "WinnerOverlay"      'Title Input - Winner Overlay
dim LoserOverlayInput as String = "LoserOverlay"        'Title Input - Loser Overlay
dim NewQuestionAudioInput as String = "NewQuestion"     'AUDIO Input - New Question Sound
dim RightAnswerAudioInput as String = "RightAnswer"     'AUDIO Input - Right Answer Sound
dim WrongAnswerAudioInput as String = "WrongAnswer"     'AUDIO Input - Wrong Answer Sound
dim WinnerAudioInput as String = "Winner"               'AUDIO Input - Success Sound
dim LoserAudioInput as String = "Loser"                 'AUDIO Input - Failure Sound

' *********************************************************
' Run as a loop in the background checking action requests
' *********************************************************
API.Function("SetDynamicValue1", Value:=0)
API.Function("SetDynamicValue2", Value:=0)



While(true)
    ' Set up access to the vMix XML model
    dim VmixXML as new system.xml.xmldocument
	VmixXML.loadxml(API.XML)
	
    Action = CInt(VmixXML.selectSingleNode("/vmix/dynamic/value1").InnerText)
	ActionValue = CInt(VmixXML.selectSingleNode("/vmix/dynamic/value2").InnerText)

	Select Case Action
        Case 0                  ' No Action Requested so SLEEP for 250ms
            Sleep(250) 

        Case 1                  ' Handle A Correct Response
            ' ACTION = Correct Answer   ACTIONVALUE = Specific Answer Selected (1-7)
            API.Function("SetDynamicValue1", Value:=0)      ' Reset the ACTION 
            API.Function("SetDynamicValue2", Value:=0)      ' Reset the Action Value
            
            If GaveResponse(ActionValue)        ' If this specific answer was already given
                ' Ignore this key press and continue
                Console.WriteLine("CASE 1: Already Gave Answer")
            Else
                ' If this specific answer is new
                GaveResponse(ActionValue) = True                ' Indicated this specific answer is given
                CorrectResponses = CorrectResponses + 1         ' Increment the tortal number of correct responses for question

                ' Reset Right Answer audio to play again
                API.Function("Pause",Input:="RightAnswer")
                API.Function("Restart",Input:="RightAnswer")

                ' Show the answer on screen along with a check box
                API.Function("SetTextVisibleOn", Input:="QuestionTitle", SelectedName:="RESP" & CStr(ActionValue) & ".Text")
                API.Function("SetImageVisibleOff",Input:="QuestionTitle",SelectedName:="CAP" & CStr(ActionValue) & ".Source")

                API.Function("Play", Input:="RightAnswer")   ' Play the Right Answer audio
                
                Console.WriteLine("CASE 1: New Correct Answer")
                ' Increase the total correct score by 1 and update the display
                ScorePlayerA = ScorePlayerA + 1             
                API.Function("SetText", Input:=QuizTitleInput, SelectedName:="ScoreA.Text", Value:=CStr(ScorePlayerA))
            End If

            ' See if all of the correct answers for this question have been given
            Console.WriteLine("Correct: " & CorrectResponses & "and Valid: " & ValidResponses)
            If CorrectResponses >= ValidResponses Then
                API.Function("OverlayInput1In", Input:=WinnerOverlayInput)
                API.Function("Play",Input:=WinnerAudioInput)
            End If

        Case 2          ' Handle An Incorrect Response

            API.Function("SetDynamicValue1", Value:=0)      ' Reset the ACTION 
            
            WrongResponses = WrongResponses + 1                     ' Increase the count of Wrong Responses

            ' Reset Wrong Answer audio to play again
            API.Function("Pause",Input:=WrongAnswerAudioInput)
            API.Function("Restart",Input:=WrongAnswerAudioInput)

            ' Display a Large X on screen for a moment and Play WrongAnswer Sound
            API.Function("SetImageVisibleOn",Input:=QuizTitleInput,SelectedName:="REDX0.Source")
            API.Function("Play",Input:=WrongAnswerAudioInput)
            Sleep(800)
            API.Function("SetImageVisibleOff",Input:=QuizTitleInput,SelectedName:="REDX0.Source")

            Console.WriteLine("Wrong Responses Count: " & WrongResponses)

            ' Post an X on the screen for the first 3 wrong answers
            If (WrongResponses <= MaxWrongResponses) Then
                API.Function("SetImageVisibleOn",Input:=QuizTitleInput,SelectedName:="REDX" & CStr(WrongResponses) & ".Source")
            End If

            ' If you have 3 wrong responses on a question, you lose
            If (WrongResponses >= MaxWrongResponses) Then
                API.Function("Play",Input:=LoserAudioInput)
                API.Function("OverlayInput1In", Input:=LoserOverlayInput)
            End If


        Case 7      'Question Reset 
            API.Function("SetDynamicValue1", Value:=0)
            
            Sleep(300)
            For i = 1 to 7
                GaveResponse(i) = False
                API.Function("SetImageVisibleOn",Input:="QuestionTitle",SelectedName:="CAP" & CStr(i) & ".Source")
            Next
            CorrectResponses = 0
            WrongResponses = 0 

            Console.WriteLine("Question Reset")

        Case 8      'Full Reset - All Elements
            API.Function("SetDynamicValue1", Value:=0)
            API.Function("SetDynamicValue2", Value:=0)

            Sleep(300)
            For i = 1 to 7
                GaveResponse(i) = False
                API.Function("SetImageVisibleOn",Input:="QuestionTitle",SelectedName:="CAP" & CStr(i) & ".Source")
            Next
            CorrectResponses = 0
            WrongResponses = 0 
            ScorePlayerA = 0
            ScorePlayerB = 0
            API.Function("SetText", Input:=QuizTitleInput, SelectedName:="ScoreA.Text", Value:="--")
            Console.WriteLine("Full Reset")

    End Select
    
    Sleep(200)
End While
