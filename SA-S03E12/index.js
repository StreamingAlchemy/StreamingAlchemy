/*          
        _____ _                            _             
       / ____| |                          (_)            
      | (___ | |_ _ __ ___  __ _ _ __ ___  _ _ __   __ _ 
       \___ \| __| '__/ _ \/ _` | '_ ` _ \| | '_ \ / _` |
       ____) | |_| | |  __/ (_| | | | | | | | | | | (_| |
      |_____/ \__|_|  \___|\__,_|_| |_| |_|_|_| |_|\__, |
          /\   | |    | |                           __/ |
         /  \  | | ___| |__   ___ _ __ ___  _   _  |___/ 
        / /\ \ | |/ __| '_ \ / _ \ '_ ` _ \| | | |       
       / ____ \| | (__| | | |  __/ | | | | | |_| |       
      /_/    \_\_|\___|_| |_|\___|_| |_| |_|\__, |       
                                             __/ |       
                                            |___/    

          STREAMING ALCHEMY - Season 03 Episode 12

                Sponsored by Gnural Net, Inc.
                     www.gnuralnet.com
                       609-223-2434




 *************************************************************************
 *****  Use NODE.JS to get selected responses from audience members  *****
 *************************************************************************
*/


// Initialize the required external libraries
const ParseXml = require('xml2js').parseStringPromise;  // XML2JS converts XML into JSON
const XPath = require('xml2js-xpath');      // XML2JS-XPATH provides access to elements via x-path
const fetch = require('node-fetch');
    
const VMIX_URL = 'http://localhost:8088/API'
const RESPONSES_URI = "./Responses.json";
const FETCH_SETTINGS = { method: "Get" };

const VMIX_SOCIAL_MEDIA_TITLE_INPUT = 4;
const VMIX_RESPONDERS_TITLE_INPUT = 5;
const VMIX_RESPONSES_TITLE_INPUT = 6;

const MAX_RESPOONSES = 22;



// This is our own SLEEP function
function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));  // Uses setTimeout with a PROMISE to sleep for ms
}


// Read the 'Responses.json' file to get the number of responses
async function GetResponseCount()
{
    let responseData = null
    try
    {
        const resp = await fetch(RESPONSES_URI, FETCH_SETTINGS)
        responseData = await resp.json()
    } catch (ex) 
    {
        console.error('Response File Missing.');
    }

    if (!responseData)
    {
        console.error('Response File Empty.');
    }

    let totalResponses = responseData.responseCount;

    if (!totalResponses)
    {
        console.error('No Character Found with ID:', id);
        return null;
    }

    return totalResponses;
}



async function UpdateRespondersTitle(input, responseData, slot)
{
    try
    {
        let titleUpdateUrl = VMIX_URL + '/?Function=SetText&Input=' + input + '&SelectedName=CHARNAME-TEAM' + team + '.Text&value=' + encodeURIComponent(responseData?.name);
        await fetch(titleUpdateUrl, FETCH_SETTINGS);

        titleUpdateUrl = VMIX_URL + '/?Function=SetText&Input=' + input + '&SelectedName=TYPES-TEAM' + team + '.Text&value=' + encodeURIComponent(responseData?.type?.reduce((prev, curr) => prev + ', ' + curr));
        await fetch(titleUpdateUrl, FETCH_SETTINGS);

        titleUpdateUrl = VMIX_URL + '/?Function=SetText&Input=' + input + '&SelectedName=WEAKS-TEAM' + team + '.Text&value=' + encodeURIComponent(responseData?.weaknesses?.reduce((prev, curr) => prev + ', ' + curr));
        await fetch(titleUpdateUrl, FETCH_SETTINGS);
        
        titleUpdateUrl = VMIX_URL + '/?Function=SetImage&Input=' + input + '&SelectedName=CHARIMAGE-TEAM' + team + '.Source&value=' + encodeURIComponent(responseData?.img);
        await fetch(titleUpdateUrl, FETCH_SETTINGS);
    } catch (ex)
    {
        console.error('Failed to UpdateRespondersTitle [Input:', input, '] [Team:', team, '] [responseData:', JSON.stringify(responseData), '] [Error:', ex, ']');
    }
}


async function ReadSocialMediaTitle(input)
{
    let responseObject = 
    {
        name: null,
        avatarURI: null,
        answer: null
    };

    try
    {
        // Find the DataTitle Input
        DataTitleNodeXML = VmixXML.selectSingleNode("/vmix/inputs/input[@title=""" + DataTitleInputName + """]")
        If DataTitleNodeXML Is Nothing Then
            Console.WriteLine("Data Title Input Not Found")
        else
            // Set the INPUT NUMBER associated with the Data Title Input
            DataTitleInputNumber = DataTitleNodeXML.Attributes.GetNamedItem("number").Value
        End If
    } catch (ex)
    {
        console.error('Failed to UpdateRespondersTitle [Input:', input, '] [Team:', team, '] [responseData:', JSON.stringify(responseData), '] [Error:', ex, ']');
    }
}

async function GetVMixAsJSON(vMix_URI)
{
        /********************************************************
              Load the vMix XML Data and 
         ********************************************************/

    let result = null;  // To start, set the Web Response Object for vMix XML to NULL
    
    while(!result) {
        try {
            // Execute a GET Command on vMix XML using AXIOS Library    
            result = await fetch(VMIX_URL, FETCH_SETTINGS);
            // Result contains the Web Response Object with the 'Data' Object,
            // inside the Web Response Object, containing the vMix XML as a String
        } catch (ex) {
            // There was no result - likely because vMix hasn't started yet
            console.error('Failed to Get vMix API:', ex);
            result = null;
            // Let's wait 5 Seconds and try again
            await sleep(5000);
            continue;
        }
         
        // Pull the vMix XML into a JSON Object
        let parsedJsonAPI = null;
        try {
            let vmixXmlAsText = await result.text();
            parsedJsonAPI = await ParseXml(vmixXmlAsText ?? '');
      
            if (!parsedJsonAPI)
            {
                console.error('Failed to Parse vMix XML');
                await sleep(5000);
                continue;
            }
        } catch (ex)
        {
            console.error('Failed to Parse vMix XML:', ex);
            result = null;
            await sleep(5000);
            continue;
        }
    }
    return parsedJsonAPI;
}    

// Core functionality replacing script
async function main()
{
    let responseCount = null;
    let newResponseData = null;
    let vMixJSON = null;
  
    // Load the vMix Data Model as a JSON Object
    vMixJSON = await GetVMixAsJSON(VMIX_URL);
  
    // Now wait until we have an audience response
    while(!newResponseData) {
        newResponseData = await ReadSocialMediaTitle(parsedJsonAPI, VMIX_SOCIAL_MEDIA_TITLE_INPUT);
        if (newResponseData.answer = null) {newResponseData = null;} // If the response isn't valid, keep checking
        await sleep(500);
    }
    
    // We have a valid response, start processing
    await GetResponseCount()
    await UpdateRespondersTitle(input, responseData, slot);

    
//        let currentRound = XPath.find(parsedJsonAPI, '/vmix/dynamic/value3').at();
//        let team1CharacterID = XPath.find(parsedJsonAPI, '/vmix/dynamic/value1').at();
//        let team2CharacterID = XPath.find(parsedJsonAPI, '/vmix/dynamic/value2').at();

        if (team2CharacterID !== lastTeam2CharacterID)
        {
            let team2responseData = await GetResponses(team2CharacterID);
            if (!team2responseData)
            {
                console.error('Invalid Team-2 Character Data');
            } else {
                await UpdateRespondersTitle(VMIX_RESPONDERS_TITLE_INPUT, responseData, index);
            }
        }

        await sleep(1000);
    }``
}

// Launch the function 'main' to kick this off
main(); 