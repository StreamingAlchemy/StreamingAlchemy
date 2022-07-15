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




 *******************************************************************************
 *****  Use NODE.JS to get selected responsesObject from audience members  *****
 *******************************************************************************
*/


// Initialize the required external libraries
const ParseXml = require('xml2js').parseStringPromise;  // XML2JS converts XML into JSON
const XPath = require('xml2js-xpath');      // XML2JS-XPATH provides access to elements via x-path
const fetch = require('node-fetch');
    
const FETCH_SETTINGS = { method: "Get" };

const VMIX_URL = 'http://localhost:8088/API'

let ValidResponseObject = {};       // This is where we store vaild responses to the question
let ResponderObject = {};           // This is where we keep a list of people that responded
let TotalValidResponses = 0;        // This is the total number of valide responses to the question



//******************************************************
// FUNCTION: sleep(ms)
//------------------------------------------------------
//          This is our own SLEEP function
//******************************************************

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));  // Uses setTimeout with a PROMISE to sleep for ms
}





//******************************************************
// FUNCTION: GetVMixAsJSON(vMix_URI, FetchSettings)
//------------------------------------------------------
//    This returns the vMix XML as a JSON Object
//******************************************************

async function GetVMixAsJSON(vMix_URI, FetchSettings)
{

    let result = null;              // To start, set the Web response Object for vMix XML to NULL
    let parsedJsonAPI = null;       // To start, set the parsedJSON Object for vMix XML to NULL

    // Load the vMix XML Data
    while(!result) {
        try {
            // Execute a GET Command on vMix XML using AXIOS Library    
            result = await fetch(vMix_URI, FetchSettings);
            // Result contains the Web response Object with the 'Data' Object,
            // inside the Web response Object, containing the vMix XML as a String
        } catch (ex) {
            // There was no result - likely because vMix hasn't started yet
            console.error('Failed to Get vMix API:', ex);
            result = null;
            // Let's wait 5 Seconds and try again
            await sleep(5000);
            continue;
        }
         
        // Pull the vMix XML into a JSON Object

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

    return parsedJsonAPI;   // Return a parsed JSO object representing the vMix XML state
}   





//******************************************************
// FUNCTION: ReadSocialMediaTitle(vMixJSON, input)
//------------------------------------------------------
//    This reads the Social Media input and returns
//    the Person, their Avatar, and their message
//******************************************************

async function ReadSocialMediaTitle(vMixJSON, input)
{
    let responseObject =        // Create a JSON Object with the social Media Details
    {
        name: null,
        avatarURI: null,
        answer: null
    };

    // Set up the XPath's we need to pull the NAME, AVATAR, an MESSAGE
    let rootXPath = "/vmix/inputs/input[@number=\'";
    rootXPath += String(input);
    rootXPath += "\']";

    let nameXPath = rootXPath;
    nameXPath += "/text[@name=\'FromName.Text\']";

    let avatarXPath = rootXPath;
    avatarXPath += "/image[@name=\'FromPhoto.Fill.Bitmap\']";

    let messageXPath = rootXPath;
    messageXPath += "/text[@name=\'Message.Text\']";

    // Using the vMix JSON Object and the XPaths, get the values
    responseObject.name = XPath.jsonText(XPath.find(vMixJSON, nameXPath).at());
    responseObject.avatarURI = XPath.jsonText(XPath.find(vMixJSON, avatarXPath).at());
    responseObject.answer = XPath.jsonText(XPath.find(vMixJSON, messageXPath).at());

    return responseObject;  // Return the Social Media Values as a JSON Object
}    





//************************************************************************
// FUNCTION: UpdaterespondersTitle(vMix_URI, input, responseData, slot)
//------------------------------------------------------------------------
//    This updates the vMix Title showing everyone that has
//    responded to the question
//************************************************************************

async function UpdaterespondersTitle(vMix_URI, input, responseData, slot)
{
    const FETCH_SETTINGS = { method: "Get" };
    let titleUpdateUrl = null;                  // This is where we store the vMix Web Command

    try
    {
        // Set The NAME of the person responding
        titleUpdateUrl = vMix_URI + '/?Function=SetText&Input=' + input + '&SelectedName=SM-NAME' + slot + '.Text&value=' + encodeURIComponent(responseData?.name);
        console.log("TITLE - Name: ",titleUpdateUrl);
        await fetch(titleUpdateUrl, FETCH_SETTINGS);

        // Set The AVATAR of the person responding
        titleUpdateUrl = vMix_URI + '/?Function=SetImage&Input=' + input + '&SelectedName=SM-AVATAR' + slot + '.Source&value=' + encodeURIComponent(responseData?.avatarURI);
        console.log("TITLE - Avatar: ",titleUpdateUrl);
        await fetch(titleUpdateUrl, FETCH_SETTINGS);
        
        // Make the BACKGROUND field visible
        titleUpdateUrl = vMix_URI + '/?Function=SetImageVisibleOn&Input=' + input + '&SelectedName=SM-BG' + slot + '.Source';
        console.log("TITLE - VisibleBG: ",titleUpdateUrl);
        await fetch(titleUpdateUrl, FETCH_SETTINGS);

        // Make the AVATAR field visible
        titleUpdateUrl = vMix_URI + '/?Function=SetImageVisibleOn&Input=' + input + '&SelectedName=SM-AVATAR' + slot + '.Source';
        console.log("TITLE - VisibleAVATAR: ",titleUpdateUrl);
        await fetch(titleUpdateUrl, FETCH_SETTINGS);
        
        // Make the NAME field visible
        titleUpdateUrl = vMix_URI + '/?Function=SetTextVisibleOn&Input=' + input + '&SelectedName=SM-NAME' + slot + '.Text';
        console.log("TITLE - VisibleText: ",titleUpdateUrl);
        await fetch(titleUpdateUrl, FETCH_SETTINGS);

    } catch (ex)    // Handle any errors
    {
        console.error('Failed to UpdaterespondersTitle [Input:', input, '] [Slot:', slot, '] [responseData:', JSON.stringify(responseData), '] [Error:', ex, ']');
    }
}





//************************************************************************
// FUNCTION: UpdateAnswersTitle(vMix_URI, input, responseData)
//------------------------------------------------------------------------
//    This updates the vMix Title showing everyone's specific
//    answer to the question that was posed
//************************************************************************

async function UpdateAnswersTitle(vMix_URI, input, responseData)
{
    const FETCH_SETTINGS = { method: "Get" };

    let titleUpdateUrl = null;  // This is where we will store the vMix Web API Call
    
    // This is where we will store the details about each answer given
    let respA = null;
    let respB = null;
    let respC = null;
    let respD = null;
    let index = null;
    
    // Pull the data for each valid response of each potential answer
    respA = responseData["#A"];
    console.log("\nresponses #A:\n", respA);
    respB = responseData["#B"];
    console.log("\nresponses #B:\n", respB);
    respC = responseData["#C"];
    console.log("\nresponses #C:\n", respC);
    respD = responseData["#D"];
    console.log("\nresponses #D:\n", respD);

    let responseFields = ['A', 'B', 'C', 'D'];

    // Loop through each set of responses and add the details of the people that responded that way

    for (let indexOuter = 0; indexOuter < responseFields.length; indexOuter++)  // Loop for each on the the answers
    {
        let currentField = responseFields[indexOuter];                  // Assign the current ASnswer field
        let currentResponseArray = responseData['#' + currentField];    // Pull out all of the current answers for it

        if (!currentResponseArray) { continue; }        // If no one chose that answer, continue to the next answer
        
        // Loop through everyone that supplied this answer

        for (let index = 0; index < currentResponseArray.length; index++)
        {
            // Fill in the NAME
            titleUpdateUrl = vMix_URI + '/?Function=SetText&Input=' + input + '&SelectedName=SM-NAME' + currentField + (index + 1) + '.Text&value=' + encodeURIComponent(currentResponseArray[index]?.name);
            console.log('#' + currentField +' Name: ',titleUpdateUrl);
            await fetch(titleUpdateUrl, FETCH_SETTINGS);
                
            // Fill in the AVATAR
            titleUpdateUrl = vMix_URI + '/?Function=SetImage&Input=' + input + '&SelectedName=SM-AVATAR' + currentField + (index + 1) + '.Source&value=' + encodeURIComponent(currentResponseArray[index]?.avatarURI);
            console.log('#' + currentField +' Avatar: ',titleUpdateUrl);
            await fetch(titleUpdateUrl, FETCH_SETTINGS);

            // Now make the entry visible
            titleUpdateUrl = vMix_URI + '/?Function=SetImageVisibleOn&Input=' + input + '&SelectedName=SM-BG' + currentField + (index + 1)  + '.Source';
            console.log("TITLE - InvisibleBG: ",titleUpdateUrl);
            await fetch(titleUpdateUrl, FETCH_SETTINGS);
    
            titleUpdateUrl = vMix_URI + '/?Function=SetImageVisibleOn&Input=' + input + '&SelectedName=SM-AVATAR' + currentField + (index + 1)  + '.Source';
            console.log("TITLE - InvisibleAVATAR: ",titleUpdateUrl);
            await fetch(titleUpdateUrl, FETCH_SETTINGS);
            
            titleUpdateUrl = vMix_URI + '/?Function=SetTextVisibleOn&Input=' + input + '&SelectedName=SM-NAME' + currentField + (index + 1)  + '.Text';
            console.log("TITLE - InvisibleText: ",titleUpdateUrl);
            await fetch(titleUpdateUrl, FETCH_SETTINGS);
        };
    }
}
   




//************************************************************************
// FUNCTION: InitRespondersTitle(vMix_URI, input)
//------------------------------------------------------------------------
//    This hides all of the fields in the Responders Title
//    (We turn them on when we write out an entry)
//************************************************************************

async function InitRespondersTitle(vMix_URI, input)
{
    const FETCH_SETTINGS = { method: "Get" };
    
    let index = 0;
    let titleUpdateUrl = null;

    
    try
    {
        // We support up to 22 responses so loop through all of them
        for (index=1; index<23;index++){
            titleUpdateUrl = vMix_URI + '/?Function=SetImageVisibleOff&Input=' + input + '&SelectedName=SM-BG' + index.toString() + '.Source';
            console.log("TITLE - InvisibleBG: ",titleUpdateUrl);
            await fetch(titleUpdateUrl, FETCH_SETTINGS);

            titleUpdateUrl = vMix_URI + '/?Function=SetImageVisibleOff&Input=' + input + '&SelectedName=SM-AVATAR' + index.toString() + '.Source';
            console.log("TITLE - InvisibleAVATAR: ",titleUpdateUrl);
            await fetch(titleUpdateUrl, FETCH_SETTINGS);
        
            titleUpdateUrl = vMix_URI + '/?Function=SetTextVisibleOff&Input=' + input + '&SelectedName=SM-NAME' + index.toString() + '.Text';
            console.log("TITLE - InvisibleText: ",titleUpdateUrl);
            await fetch(titleUpdateUrl, FETCH_SETTINGS);
        }
    } catch (ex)
    {
        console.error('Failed to UpdaterespondersTitle [Input:', input, '] [Slot:', slot, '] [responseData:', JSON.stringify(responseData), '] [Error:', ex, ']');
    }
}







//************************************************************************
// FUNCTION: InitAnswersTitle(vMix_URI, input)
//------------------------------------------------------------------------
//    This hides all of the fields in the Answers Title
//    (We turn them on when we write out an entry)
//************************************************************************

async function InitAnswersTitle(vMix_URI, input)
{
    const FETCH_SETTINGS = { method: "Get" };
    
    let index = 0;
    let titleUpdateUrl = null;

    // Since the field in this title are organized by response, loop through each possible response
    let responseFields = ['A', 'B', 'C', 'D'];

    for (let indexOuter = 0; indexOuter < responseFields.length; indexOuter++)
    {
        let currentField = responseFields[indexOuter];
        
        // There are 13 spots for each response, so hid the field for all of them
        for (let index = 0; index < 13; index++)
        {
            // Hide the BACKGROUND
            titleUpdateUrl = vMix_URI + '/?Function=SetImageVisibleOff&Input=' + input + '&SelectedName=SM-BG' + currentField + (index + 1)  + '.Source';
            console.log("TITLE - InvisibleBG: ",titleUpdateUrl);
            await fetch(titleUpdateUrl, FETCH_SETTINGS);
    
            // Hide the AVATAR
            titleUpdateUrl = vMix_URI + '/?Function=SetImageVisibleOff&Input=' + input + '&SelectedName=SM-AVATAR' + currentField + (index + 1)  + '.Source';
            console.log("TITLE - InvisibleAVATAR: ",titleUpdateUrl);
            await fetch(titleUpdateUrl, FETCH_SETTINGS);
            
            // Hide the NAME
            titleUpdateUrl = vMix_URI + '/?Function=SetTextVisibleOff&Input=' + input + '&SelectedName=SM-NAME' + currentField + (index + 1)  + '.Text';
            console.log("TITLE - InvisibleText: ",titleUpdateUrl);
            await fetch(titleUpdateUrl, FETCH_SETTINGS);
        };
    }
}
   






//************************************************************************
// FUNCTION: IsValidAnswer(viewerresp)
//------------------------------------------------------------------------
//    Look at a given answer and check if it is valid
//************************************************************************

async function IsValidAnswer(viewerresp)
{
    // This is a list of the possible valid responses
    const goodresponses = ['#A', '#B', '#C', '#D', '#E'];
    let answer = viewerresp.answer;

    // Check if the answer provided is in the valid list
    if (goodresponses.indexOf(answer) > -1) {
        console.log("In List")
        return true; // Return that response if Valid
    }

    console.log("Not in List")
    return false; // Return that the response isn't Valid
};






//************************************************************************
// FUNCTION: GetDynamicValue(parsedJsonAPI)
//------------------------------------------------------------------------
//    This returns a JSO Object will of the the vMix Dynamic Variable
//    Values stored in it
//************************************************************************

async function GetDynamicValue(parsedJsonAPI) {
    // Create a JSON Object to hold all of the Dynamic Values
    let dynamicValueObject = 
    {
        value1: null,
        value2: null,
        value3: null,
        value4: null
    };

    // Pull all of the value out from our most recent refresh
    dynamicValueObject.value1 = XPath.find(parsedJsonAPI, '/vmix/dynamic/value1').at();
    dynamicValueObject.value2 = XPath.find(parsedJsonAPI, '/vmix/dynamic/value2').at();
    dynamicValueObject.value3 = XPath.find(parsedJsonAPI, '/vmix/dynamic/value3').at();
    dynamicValueObject.value4 = XPath.find(parsedJsonAPI, '/vmix/dynamic/value4').at();

    return dynamicValueObject;
}






//************************************************************************
// FUNCTION: SetDynamicValue(vMix_URI, toUpdate, value)
//------------------------------------------------------------------------
//    This lets us set a specific Dynamic Variable's Value
//************************************************************************

async function SetDynamicValue(vMix_URI, toUpdate, value) {
    
    // Set up the vMix Web API Call
    let dynValueUpdateUrl = vMix_URI + '/?Function=SetDynamicValue'+ toUpdate + '&Value='+value;
    await fetch(dynValueUpdateUrl, FETCH_SETTINGS);

    return true;
}






//********************************************************************************
// FUNCTION: AddResponse(responsesObject, respondersObject, userresponseObject)
//--------------------------------------------------------------------------------
//    This will process all responses from the comments.  If the response
//    is valid, it will add it to a list of all vaild responses
//********************************************************************************

async function AddResponse(responsesObject, respondersObject, userresponseObject) {
    // Check if an required fields are missing/null
    if (!responsesObject || !respondersObject || !userresponseObject ||
        !userresponseObject.avatarURI || !userresponseObject.answer)
    {
        console.log("AddResponse: Null Value Found")
        return false; // The response is bad, so don't add it - just return
    }

    // We have a response - check if it is valid
    if (!IsValidAnswer(userresponseObject))
    {
        console.log("AddResponse: Not Valid Answer")
        return false; // The response isn't valid - return
    }

    // The response is valid, so see if the user already responded (first response only)
    if (respondersObject[userresponseObject.avatarURI])
    {
        console.log("AddResponse: User Already responded: " + respondersObject[userresponseObject.avatarURI]);
        console.log("ResponderObject:\n", respondersObject);
        return false;  // User already responded so don't add it
    }
    
    // Add new responses dynamically
    if (!responsesObject[userresponseObject.answer]) 
    {
      responsesObject[userresponseObject.answer] = [];
    }

    // We are storing all of the responses as elements of the specific answer
    responsesObject[userresponseObject.answer].push(userresponseObject); // Add object as elements associated with an answer

    // We are adding the UID (Image URL) of the user that responded
    respondersObject[userresponseObject.avatarURI] = true; // Add the user that responded to object (one response per user)

    return true;
}





//********************************************************************************
// FUNCTION: main()
//--------------------------------------------------------------------------------
//    This is our main processing loop. Functionality is controlled via
//    vMix's Dynamic Value 1. A value of 0 is used to bypass any specific
//    functionality in the loop.
//********************************************************************************

async function main()
{
    // Initialize the core values used by the processes in this loop
    let vMixJSON = null;        // The JSON representation of the vMix XML
    let viewerresponse = null;  // The JSON object where we store the user response
    let validAnswer = false;    // The flag indicating a valid answer
    let dynamicValues = null;   // The JSON Object where we store vMix's Dynamic Values
  
    await InitRespondersTitle(VMIX_URL, 2); // Clear out the vMix Title showing all responders
    await InitAnswersTitle(VMIX_URL, 3);    // Clear out the vMix Title showing the responders for each answer

    // Start our main processing loop
    while(true) {

        // Load the latest information from vMix on each loop
        vMixJSON = await GetVMixAsJSON(VMIX_URL, FETCH_SETTINGS);   // JSON format of vMix's XML
        dynamicValues = await GetDynamicValue(vMixJSON);            // JSON format of vMix's Dynamic Values

        // DynamicValue1 is use to control what functionality is executed on each loop pass


        // If DynamicValue1 equals '1' keep pulling the latest values
        // from the Social Media Title in vMix, and check for
        // a valid response:

        if (dynamicValues.value1 == 1) {
            viewerresponse = await ReadSocialMediaTitle(vMixJSON, 1);   // Refresh the JSO Object with the values from the SM Title
            viewerresponse.answer = viewerresponse.answer.trim().toUpperCase(); // Clean up & normalizethe returned answer
            validAnswer = await AddResponse(ValidResponseObject, ResponderObject, viewerresponse);  // Check if it is a valid response
            
            if (!validAnswer) {     // It isn't valid:
                await sleep(1000);  // Sleep for 1 sec...
                continue;           // ...and jump to the top of the loop
            }
            
            // It is Valid:
            TotalValidResponses++;  // Update the count of total valid responses
            await UpdaterespondersTitle(VMIX_URL, 2, viewerresponse, TotalValidResponses);
        }


        // If DynamicValue1 equals '2' add all of the valid responses
        // grouped by answer to the Answers Title in vMix:

        if (dynamicValues.value1 == 2) {
            // Update the Answers Title
            await UpdateAnswersTitle(VMIX_URL, 3, ValidResponseObject);
            // Set DynamicValue1 to '0' - effectively having the loop do nothing until a new command is indicated
            await SetDynamicValue(VMIX_URL, 1, 0);
        }
        await sleep(1000);  // Wait a second before starting the loop again

    }
}


// Launch the function 'main' to kick this off
main(); 