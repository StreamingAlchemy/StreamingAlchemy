' STREAMING ALCHEMY - Season 02 Episode 31
'
' Sponsored by Gnural Net, Inc.
'      www.gnuralnet.com
'         609-223-2434
'

' *******************************************************
' *****  Populate a 5 Day Weather Forecast Display  *****
' *******************************************************



' Load the vMix XML Model:
dim VmixXML as new system.xml.xmldocument
VmixXML.loadxml(API.XML)

' ZIP CODE of region is stored in Dynamic Value 1
dim zipCode As String = VmixXML.SelectSingleNode("/vmix/dynamic/value1").InnerText

' If ZIP CODE is blank, default it to the root ZIP CODE in the US
if zipCode="" then zipCode="10001"


' Load the Weather Forcast XML Model:
dim requestRAW as String = "https://api.openweathermap.org/data/2.5/forecast?zip="+zipCode+"&units=imperial&mode=xml&appid=**** Your Own APP ID Key ****"
dim weatherXML as XmlDocument = new XmlDocument()
weatherXML.Load(requestRAW)


' Generate XMLNodeList with each Day's forecast from 15:00-18:00:
dim DailyForecast As XmlNodeList = WeatherXML.SelectNodes("/weatherdata/forecast/time[contains(@from,""15:00"")]")

' Pull the Location Name that the Weather Forecast is for
dim ForecastLocation As String = WeatherXML.SelectSingleNode("/weatherdata/location/name").InnerText.ToUpper

dim Temp as String              ' Set up temporary string variable we can use for reformatting/transformations

' Create STRING arrays to hold the Weather details for the next 5 days

dim DayofWeek(5) as String      ' Name of the day itself (SUN, MON, TUE,...)
dim RealTemp(5) as String       ' Actual temperature
dim FeelTemp(5) as String       ' What the temperature feels like
dim Humidity(5) as String       ' Percentage Humidity
dim Clouds(5) as String         ' Cloud Coverage
dim WindSpeed(5) as String      ' Wind conditions
dim IconURL(5) as String        ' URL of the ICON representing the weather that day

' Set up reference pointers

dim counter as Integer = 0      ' Refers to the ordinal day in the forcast (0 indexed)
dim index as Integer = 0        ' Refers to VariableName index found in our GT TITLE (1 indexed)



' Assign the Location Name to the GT Title's Header (Upper Right)

API.Function("SetText",Input:="5DayWeather",SelectedName:= "LOCATION.Text",Value:= ForecastLocation)

' Loop through the 5 day forecast

For counter = 0 to DailyForecast.Count -1   ' Set up loop with 0 Index
    ' Pull out the DATE from the DATE/TIME Stamp, and convert it to a DAY of the Week
    DayOfWeek(counter) = datetime.Parse(DailyForecast.Item(counter).Attributes.GetNamedItem("from").Value.Split("T")(0)).DayOfWeek.ToString().ToUpper
    
    ' Pull out the REAL TEMPERATURE, Round to nearest degree, and reformat it with "°F"
    Temp=DailyForecast.Item(counter).Item("temperature").Attributes.GetNamedItem("value").Value
    RealTemp(counter)=Math.Round(CDbl(Temp)).ToString()+"°F"
    
    ' Pull out the FEEL TEMPERATURE, Round to nearest degree, and reformat it with "°F"
    Temp=DailyForecast.Item(counter).Item("feels_like").Attributes.GetNamedItem("value").Value
    FeelTemp(counter)="FEELS LIKE: "+Math.Round(CDbl(Temp)).ToString()+"°F"

    ' Pull out the HUMIDITY and reformat it with a "%"
    Humidity(counter)=DailyForecast.Item(counter).Item("humidity").Attributes.GetNamedItem("value").Value+"%"

    ' Pull out the CLOUD COVER and reformat it as all UPPERCASE
    Clouds(counter)=DailyForecast.Item(counter).Item("clouds").Attributes.GetNamedItem("value").Value.ToUpper()

    ' Pull out the WIND CONDITIONS and reformat it as all UPPERCASE
    WindSpeed(counter)=DailyForecast.Item(counter).Item("windSpeed").Attributes.GetNamedItem("name").Value.ToUpper()

    ' Pull out the ICON NAME and reformat it as a URL
    IconURL(counter)="http://openweathermap.org/img/w/"+DailyForecast.Item(counter).Item("symbol").Attributes.GetNamedItem("var").Value+".png"

    ' Since the Element Names in the GT TITLE are 1 Indexed, add 1 to the counter (which is 0 Indexed)
    index=counter+1

    ' Using all of the values assigned in the previous section, populate all of the Elements for the 'INDEX'-th Day
    API.Function("SetText",Input:="5DayWeather",SelectedName:= "LABEL-DAY"+index.ToString()+".Text",Value:= DayOfWeek(counter))
    API.Function("SetText",Input:="5DayWeather",SelectedName:= "TEMP-DAY"+index.ToString()+".Text",Value:= RealTemp(counter))
    API.Function("SetText",Input:="5DayWeather",SelectedName:= "FEEL-DAY"+index.ToString()+".Text",Value:= FeelTemp(counter))
    API.Function("SetText",Input:="5DayWeather",SelectedName:= "HUMD-DAY"+index.ToString()+".Text",Value:= Humidity(counter))
    API.Function("SetText",Input:="5DayWeather",SelectedName:= "CLOUD-DAY"+index.ToString()+".Text",Value:= Clouds(counter))
    API.Function("SetText",Input:="5DayWeather",SelectedName:= "WIND-DAY"+index.ToString()+".Text",Value:= WindSpeed(counter))
    API.Function("SetImage", "5DayWeather", IconURL(counter), 100, "ICON-DAY"+index.ToString()+".Source")
Next

