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

          STREAMING ALCHEMY - Season 03 Episode 09

                Sponsored by Gnural Net, Inc.
                     www.gnuralnet.com
                       609-223-2434




 ***********************************************************************
 *****  Use NODE.JS to get details of Game Cards for competitions  *****
 ***********************************************************************
*/


// Initialize the required external libraries
const ParseXml = require('xml2js').parseStringPromise;  // XML2JS converts XML into JSON
const XPath = require('xml2js-xpath');                  // XML2JS-XPATH provides access to elements via x-path
const fetch = require('node-fetch');                    // Used for all of our HTTP GET Requests
  
// Settings to access Game Character Data [JSON Formatted]
const DATA_URL = "https://raw.githubusercontent.com/Biuni/PokemonGO-Pokedex/master/pokedex.json";
const FETCH_SETTINGS = { method: "Get" };

// Settings to access vMix's XML and Web API
const VMIX_URL = 'http://localhost:8088/API'
const VMIX_TITLE_INPUT = 2;

// This is our own SLEEP function
function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));  // Uses setTimeout with a PROMISE to sleep for ms
}



/****************************************************************************/
/****    FUNCTION: GetCharacterById - Read A Specific Character's data   ****/
/****    from the URL and return it as aJSON Object (characterData)      ****/
/****************************************************************************/

async function GetCharacterById(id)
{
    let characterData = null        // Initialize the data return to NULL
    try
    {
        const resp = await fetch(DATA_URL, FETCH_SETTINGS)      // Fetch the entire Character Data Set + Headers
        characterData = await resp.json()                       // Extract just the JSON portion of the Data Set
    } catch (ex) 
    {
        console.error('Failed to GetCharacterById:', id, ex);   // If you can't access any of the data, display an error
    }

    if (!characterData)
    {
        console.error('No Character Data Found was found');   // If you could access the URL but no data was found, display error
    }

    let desiredCharacter = characterData.pokemon.find((character) => character?.id == id);  // Pull the data for the specific Character ID

    if (!desiredCharacter)
    {
        console.error('No Character Found with ID:', id);   // If there was no data associated with the requested Character ID, display error
        return null;
    }

    return desiredCharacter;    // We found the data we are looking for so have the function return it
}



/****************************************************************************/
/****   FUNCTION: UpdateRound - Display the current Round in the Title   ****/
/****************************************************************************/

// Call this to update the ROUND displayed in GT TITLE
async function UpdateRound(input, newRound)
{
    try // Do this to make sure we catch any errors
    {
        // Create a vMix Web API to update the ROUND text with newRound in vMix Input input (with encoded URI)
        let roundUpdateUrl = VMIX_URL + '/?Function=SetText&Input=' + input + '&SelectedName=ROUND.Text&value=' + encodeURIComponent(newRound);
        await fetch(roundUpdateUrl, FETCH_SETTINGS);    // Send the command to vMix
    } catch (ex)
    {
        console.error('Failed to UpdateRound:', ex);    // If something went wrong, display error
    }
}



/****************************************************************************/
/****   FUNCTION: UpdateTitleByInput - Update all of a Team's Character  ****/
/****   information in the GT Title                                      ****/
/****************************************************************************/

async function UpdateTitleByInput(input, team, characterData)
{
    try // Do this to make sure we catch any errors
    {
        // Build Web API Function to update the Character's Name
        let titleUpdateUrl = VMIX_URL + '/?Function=SetText&Input=' + input + '&SelectedName=CHARNAME-TEAM' + team + '.Text&value=' + encodeURIComponent(characterData?.name);
        await fetch(titleUpdateUrl, FETCH_SETTINGS);   // Send the command to vMix

        // Build Web API Function to update the Character's Type (REDUCE combines everythin in the Type List)
        titleUpdateUrl = VMIX_URL + '/?Function=SetText&Input=' + input + '&SelectedName=TYPES-TEAM' + team + '.Text&value=' + encodeURIComponent(characterData?.type?.reduce((prev, curr) => prev + ', ' + curr));
        await fetch(titleUpdateUrl, FETCH_SETTINGS);   // Send the command to vMix

        // Build Web API Function to update the Character's Weaknesses (REDUCE combines everythin in the Weaknesses List)
        titleUpdateUrl = VMIX_URL + '/?Function=SetText&Input=' + input + '&SelectedName=WEAKS-TEAM' + team + '.Text&value=' + encodeURIComponent(characterData?.weaknesses?.reduce((prev, curr) => prev + ', ' + curr));
        await fetch(titleUpdateUrl, FETCH_SETTINGS);   // Send the command to vMix
        
        // Build Web API Function to update the Character's Image (GT TITLE accepts URL's - with encoded URI)
        titleUpdateUrl = VMIX_URL + '/?Function=SetImage&Input=' + input + '&SelectedName=CHARIMAGE-TEAM' + team + '.Source&value=' + encodeURIComponent(characterData?.img);
        await fetch(titleUpdateUrl, FETCH_SETTINGS);   // Send the command to vMix
    } catch (ex)
    {
        // If anything went wrong, display an error and associated details
        console.error('Failed to UpdateTitleByInput [Input:', input, '] [Team:', team, '] [CharacterData:', JSON.stringify(characterData), '] [Error:', ex, ']');
    }
}




/****************************************************************************/
/****   FUNCTION: Main - This is the loop that runs in the background    ****/
/****   looking to reach to changes in vMix's Dynamic Values             ****/
/****************************************************************************/
async function main()
{
    // Initialize variables for Previous settings so we know whyen things change
    let lastTeam1CharacterID = null;
    let lastTeam2CharacterID = null;
    let lastRound = null;

    console.log("Process is now running in the background...")

    // Now keep looping in the background
    while (true)
    {
        let result = null;  // To start, set the Web Response Object for vMix XML to NULL

        try // Do this to make sure we catch any errors
        {   
            // Execute a GET Command on vMix XML using AXIOS Library   
            // This is the equilivent of what we do inside of vb scripts 
            result = await fetch(VMIX_URL, FETCH_SETTINGS);
            // 'result' contains the Web Response Object - the 'Data' Object is
            // inside the Web Response Object, containing the vMix XML as a String
        } catch (ex) {
            // There was no result - likely because vMix hasn't started yet
            console.error('Failed to Get vMix API:', ex);
            // Let's wait 10 Seconds and try again
            await sleep(10000);
            continue;
        }

        // Since Node.js is JSON friendly, pull the vMix XML into a JSON Object
        let parsedJsonAPI = null;
        try
        {
            let vmixXmlAsText = await result.text();    // Pull out the vMix XML as a string
            parsedJsonAPI = await ParseXml(vmixXmlAsText ?? '');    // Convert the string in JSON [Set it to '' if NULL to avoid crash]

            if (!parsedJsonAPI)
            {
                console.error('No data to Parse for vMix XML');     // If there is no XML to parse...
                await sleep(10000);                                 // ...wait 10 seconds and try again
                continue;
            }
        } catch (ex)
        {
            console.error('Failed to Parse vMix XML:', ex);         // The data failed to parse...
            await sleep(10000);                                     // ...wait 10 seconds and try again
            continue;
        }

        // We are using vMix's Dynamic Values for identifing Team Characters and the current ROUND
        let currentRound = XPath.find(parsedJsonAPI, '/vmix/dynamic/value3').at();      // This is the current ROUND
        let team1CharacterID = XPath.find(parsedJsonAPI, '/vmix/dynamic/value1').at();  // This is the Character ID being used by TEAM-1
        let team2CharacterID = XPath.find(parsedJsonAPI, '/vmix/dynamic/value2').at();  // This is the Character ID being used by TEAM-2

        // Check if the ROUND has been updated
        if (lastRound !== currentRound)     // Is the Current ROUND different from the Previous Round?
        {
            await UpdateRound(VMIX_TITLE_INPUT, currentRound);      // If so, update the ROUND in the Title
        }

        if (team1CharacterID !== lastTeam1CharacterID)     // Is the Current CHARACTER for TEAM-1 different from the Previous CHARACTER?
        {
            let team1CharacterData = await GetCharacterById(team1CharacterID);      // If so, get the new CHARACTER's data
            if (!team1CharacterData)
            {
                console.error('Invalid Team-1 CharacterData');      // If there was a problem doing that, display an error
            } else {
                await UpdateTitleByInput(VMIX_TITLE_INPUT, 1, team1CharacterData);      // If all is good, update the CHARACTER in the Title
            }
        }

        if (team2CharacterID !== lastTeam2CharacterID)    // Is the Current CHARACTER for TEAM-1 different from the Previous CHARACTER?
        {
            let team2CharacterData = await GetCharacterById(team2CharacterID);      // If so, get the new CHARACTER's data
            if (!team2CharacterData)
            {
                console.error('Invalid Team-2 Character Data');     // If there was a problem doing that, display an error
            } else {
                await UpdateTitleByInput(VMIX_TITLE_INPUT, 2, team2CharacterData);  // If all is good, update the CHARACTER in the Title
            }
        }

        await sleep(1000);  // Now wait for 1 Second and check it all again
    }``
}

// Launch the function 'main' to kick this off
main(); 