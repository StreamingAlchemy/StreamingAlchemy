' SCRIPT: Show-Q
API.Function("Pause",Input:="NewQuestion")
API.Function("Restart",Input:="NewQuestion")
API.Function("SetTextVisible", Input:="QuestionTitle", SelectedName:="QUESTION.Text")
API.Function("Play",Input:="NewQuestion")


' SCRIPT: Show-R#
API.Function("SetDynamicValue1", Value:=1)
API.Function("SetDynamicValue2", Value:=#)

' SCRIPT: NextQ
API.Function("ScriptStart", Value:="QuestReset")
API.Function("DataSourceNextRow", Value:="Questions,XML")
API.Function("SetDynamicValue1", Value:=8)


' SCRIPT: WrongAnswer
API.Function("SetDynamicValue1", Value:=2)


' SCRIPT: FullReset
API.Function("SetTextVisibleOff", Input:="QuestionTitle", SelectedName:="QUESTION.Text")
API.Function("SetTextVisibleOff", Input:="QuestionTitle", SelectedName:="RESP1.Text")
API.Function("SetImageVisibleOn",Input:="QuestionTitle",SelectedName:="CAP1.Source")
API.Function("SetTextVisibleOff", Input:="QuestionTitle", SelectedName:="RESP2.Text")
API.Function("SetImageVisibleOn",Input:="QuestionTitle",SelectedName:="CAP2.Source")
API.Function("SetTextVisibleOff", Input:="QuestionTitle", SelectedName:="RESP3.Text")
API.Function("SetImageVisibleOn",Input:="QuestionTitle",SelectedName:="CAP3.Source")
API.Function("SetTextVisibleOff", Input:="QuestionTitle", SelectedName:="RESP4.Text")
API.Function("SetImageVisibleOn",Input:="QuestionTitle",SelectedName:="CAP4.Source")
API.Function("SetTextVisibleOff", Input:="QuestionTitle", SelectedName:="RESP5.Text")
API.Function("SetImageVisibleOn",Input:="QuestionTitle",SelectedName:="CAP5.Source")
API.Function("SetTextVisibleOff", Input:="QuestionTitle", SelectedName:="RESP6.Text")
API.Function("SetImageVisibleOn",Input:="QuestionTitle",SelectedName:="CAP6.Source")
API.Function("SetTextVisibleOff", Input:="QuestionTitle", SelectedName:="RESP7.Text")
API.Function("SetImageVisibleOn",Input:="QuestionTitle",SelectedName:="CAP7.Source")
API.Function("SetImageVisibleOff",Input:="QuestionTitle",SelectedName:="REDX1.Source")
API.Function("SetImageVisibleOff",Input:="QuestionTitle",SelectedName:="REDX2.Source")
API.Function("SetImageVisibleOff",Input:="QuestionTitle",SelectedName:="REDX3.Source")
API.Function("SetImageVisibleOff",Input:="QuestionTitle",SelectedName:="FAILURE.Source")
API.Function("SetDynamicValue1", Value:=8)