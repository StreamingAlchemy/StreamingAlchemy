' PAUSE - Dynamic Script

API.FUNCTION("SetTickerSpeed",Input:="Prompter",SelectedName:="PROMPT.Text",Value:="0")
API.FUNCTION("LayerOn", Input:="Prompter-BG", Value:="1")
API.FUNCTION("PauseCountdown",Input:="Prompter",SelectedName:="TIMER.Text")


' PLAY - Dynamic Script

API.FUNCTION("SetTickerSpeed",Input:="Prompter",SelectedName:="PROMPT.Text",Value:="Dynamic2")
API.FUNCTION("LayerOff", Input:="Prompter-BG", Value:="1")
API.FUNCTION("PauseCountdown",Input:="Prompter",SelectedName:="TIMER.Text")
