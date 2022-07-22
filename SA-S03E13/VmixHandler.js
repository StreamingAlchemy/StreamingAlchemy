const ParseXml = require('xml2js').parseStringPromise;  // XML2JS converts XML into JSON
const XPath = require('xml2js-xpath');      // XML2JS-XPATH provides access to elements via x-path
const fetch = require('node-fetch');


const FETCH_SETTINGS = { method: "Get" };           // This is the file access method being used

//******************************************************
// FUNCTION: GetVMixAsJSON(vMix_URI, FetchSettings)
//------------------------------------------------------
//    This returns the vMix XML as a JSON Object
//******************************************************

async function GetVMixAsJSON(vMix_URI)
{

    let result = null;              // To start, set the Web response Object for vMix XML to NULL
    let parsedJsonAPI = null;       // To start, set the parsedJSON Object for vMix XML to NULL

    // Load the vMix XML Data
    while(!result) {
        try {
            // Execute a GET Command on vMix XML using AXIOS Library    
            result = await fetch(vMix_URI, FETCH_SETTINGS);
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

//************************************************************************
// FUNCTION: InitAnswersTitle(vMix_URI, input)
//------------------------------------------------------------------------
//    This hides all of the fields in the Answers Title
//    (We turn them on when we write out an entry)
//************************************************************************

async function InitAnswersTitle(vMix_URI, input, numberOfSlotsPerRow)
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
        for (let index = 0; index < numberOfSlotsPerRow; index++)
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

async function UpdateRespondersTitle(vMix_URI, input, responseData, slot)
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

async function InitRespondersTitle(vMix_URI, input, numberOfSlots)
{
    const FETCH_SETTINGS = { method: "Get" };
    
    let index = 0;
    let titleUpdateUrl = null;

    
    try
    {
        // We support up to 22 responses so loop through all of them
        for (index=1; index<numberOfSlots;index++){
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


async function UpdateQuestionTitle(vMix_URI, questionObject, input)
{
    const FETCH_SETTINGS = { method: "Get" };

    let titleUpdateUrl = null;  // This is where we will store the vMix Web API Call

    titleUpdateUrl = vMix_URI + '/?Function=SetText&Input=' + input + '&SelectedName=TEXT-QUESTION.Text&value=' + encodeURIComponent(questionObject.questionText);
    await fetch(titleUpdateUrl, FETCH_SETTINGS);

    let responseFields = Object.keys(questionObject.options).sort();

    // Loop through each set of responses and add the details of the people that responded that way

    for (let indexOuter = 0; indexOuter < responseFields.length; indexOuter++)  // Loop for each on the the answers
    {
        let currentField = responseFields[indexOuter];                  // Assign the current ASnswer field

        if (!questionObject?.options?.[currentField]) { continue; }        // If no one chose that answer, continue to the next answer
        
        // Loop through everyone that supplied this answer
        titleUpdateUrl = vMix_URI + '/?Function=SetText&Input=' + input + '&SelectedName=TEXT-OPTION' + currentField.replace('#','') + '.Text&value=' + encodeURIComponent(questionObject.options[currentField]);
        await fetch(titleUpdateUrl, FETCH_SETTINGS);
    }
}

module.exports.GetVMixAsJSON = GetVMixAsJSON;
module.exports.GetDynamicValue = GetDynamicValue;
module.exports.SetDynamicValue = SetDynamicValue;
module.exports.InitAnswersTitle = InitAnswersTitle;
module.exports.ReadSocialMediaTitle = ReadSocialMediaTitle;
module.exports.UpdateRespondersTitle = UpdateRespondersTitle;
module.exports.UpdateAnswersTitle = UpdateAnswersTitle;
module.exports.InitRespondersTitle = InitRespondersTitle;
module.exports.UpdateQuestionTitle = UpdateQuestionTitle;

