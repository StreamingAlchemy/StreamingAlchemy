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

          STREAMING ALCHEMY - Season 03 Episode 13

                Sponsored by Gnural Net, Inc.
                     www.gnuralnet.com
                       609-223-2434




 ******************************************************************************
 *****  Use NODE.JS to get select and show questions to audience members  *****
 ******************************************************************************
*/

    
// We are reorganizing the code to split out functionality in modules

const vMixHandler = require('./VmixHandler');         // This module has functions related to vMix Specifics
const QuestionHandler = require('./QuestionHandler');
const General = require('./GeneralFunctions');        // This module has general functions we will use across projects


// Set up the constants we will use to access everything

const QUESTIONS_FILE_PATH = './Questions.json';     // This is the path to the QUESTONs file 
const VMIX_URL = 'http://localhost:8088/API';        // This is the URI of our running vMix System
const SLEEP_TIME = 1000;                            // This is the interval we'll use to sleep
const VMIX_QUESTION_TITLE_INPUT_NUMBER = 4;
const VMIX_RESPONSES_TITLE_INPUT_NUMBER = 2;
const VMIX_QUESTION_AND_RESPONSES_TITLE_INPUT_NUMBER = 3;
const VMIX_STUDIO_TITLE_INPUT_NUMBER = 5;
const NUMBER_OF_SLOTS_PER_ROW = 18;


let ValidResponseObject = {};       // This is where we store vaild responses to the question
let ResponderObject = {};           // This is where we keep a list of people that responded
let TotalValidResponses = 0;        // This is the total number of valide responses to the question


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
    let lastQuestionIndex = -1;     // We have no active question when when start
  
    await vMixHandler.InitRespondersTitle(VMIX_URL, VMIX_RESPONSES_TITLE_INPUT_NUMBER, 22); // Clear out the vMix Title showing all responders
    await vMixHandler.InitAnswersTitle(VMIX_URL, VMIX_QUESTION_AND_RESPONSES_TITLE_INPUT_NUMBER, NUMBER_OF_SLOTS_PER_ROW);    // Clear out the vMix Title showing the responders for each answer

    // Start our main processing loop
    while(true) {

        // Load the latest information from vMix on each loop
        vMixJSON = await vMixHandler.GetVMixAsJSON(VMIX_URL);                   // JSON format of vMix's XML
        dynamicValues = await vMixHandler.GetDynamicValue(vMixJSON);            // JSON format of vMix's Dynamic Values


        if (dynamicValues?.value2 !== lastQuestionIndex &&
            dynamicValues?.value2 !== null &&
            dynamicValues?.value2 !== undefined)
        {
            let desiredQuestion = await QuestionHandler.LoadQuestion(QUESTIONS_FILE_PATH, dynamicValues.value2);
            if (desiredQuestion)
            {
                await vMixHandler.UpdateQuestionTitle(VMIX_URL, desiredQuestion, VMIX_QUESTION_TITLE_INPUT_NUMBER);
                await vMixHandler.UpdateQuestionTitle(VMIX_URL, desiredQuestion, VMIX_QUESTION_AND_RESPONSES_TITLE_INPUT_NUMBER);
                await vMixHandler.UpdateQuestionTitle(VMIX_URL, desiredQuestion, VMIX_STUDIO_TITLE_INPUT_NUMBER);
                lastQuestionIndex = dynamicValues.value2;
            }
        }

        // DynamicValue1 is use to control what functionality is executed on each loop pass
        // If DynamicValue1 equals '1' keep pulling the latest values
        // from the Social Media Title in vMix, and check for
        // a valid response:

        if (dynamicValues.value1 == 1) {
            viewerresponse = await vMixHandler.ReadSocialMediaTitle(vMixJSON, 1);   // Refresh the JSO Object with the values from the SM Title
            viewerresponse.answer = viewerresponse.answer.trim().toUpperCase(); // Clean up & normalizethe returned answer
            validAnswer = await AddResponse(ValidResponseObject, ResponderObject, viewerresponse);  // Check if it is a valid response
            
            if (!validAnswer) {     // It isn't valid:
                await General.sleep(SLEEP_TIME);  // Sleep for SLEEP_TIME sec...
                continue;           // ...and jump to the top of the loop
            }
            
            // It is Valid:
            TotalValidResponses++;  // Update the count of total valid responses
            await vMixHandler.UpdateRespondersTitle(VMIX_URL, VMIX_RESPONSES_TITLE_INPUT_NUMBER, viewerresponse, TotalValidResponses);
        }


        // If DynamicValue1 equals '2' add all of the valid responses
        // grouped by answer to the Answers Title in vMix:

        if (dynamicValues.value1 == 2) {
            // Update the Answers Title
            await vMixHandler.UpdateAnswersTitle(VMIX_URL, VMIX_QUESTION_AND_RESPONSES_TITLE_INPUT_NUMBER, ValidResponseObject);
            await vMixHandler.UpdateAnswersTitle(VMIX_URL, VMIX_STUDIO_TITLE_INPUT_NUMBER, ValidResponseObject);
            // Set DynamicValue1 to '0' - effectively having the loop do nothing until a new command is indicated
            await vMixHandler.SetDynamicValue(VMIX_URL, 1, 0);
        }


        if (dynamicValues.value1 == 3)
        {
            ValidResponseObject = {};       // This is where we store vaild responses to the question
            ResponderObject = {};           // This is where we keep a list of people that responded
            TotalValidResponses = 0;        // This is the total number of valide responses to the question
            await vMixHandler.InitRespondersTitle(VMIX_URL, VMIX_RESPONSES_TITLE_INPUT_NUMBER, 22);
            await vMixHandler.InitAnswersTitle(VMIX_URL, VMIX_QUESTION_AND_RESPONSES_TITLE_INPUT_NUMBER, NUMBER_OF_SLOTS_PER_ROW);
            await vMixHandler.SetDynamicValue(VMIX_URL, 1, 0);
        }
        await General.sleep(SLEEP_TIME);  // Wait a second before starting the loop again

    }
}


// Launch the function 'main' to kick this off
main(); 