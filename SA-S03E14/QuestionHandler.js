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
 *****     In Part 2 of this project we will use NODE.JS to select and    *****
 *****     display Questions that viewers will be able to respond to      *****
 *****     using what we developed in Part 1 of this project.             *****
 ******************************************************************************
*/


// We are reorganizing the code to split out functionality in modules
const General = require('./GeneralFunctions');      // This module has general functions we will use across projects

async function LoadQuestion(questionFilePath, questionIndex)
{
    // First check if we have a question number specified
    if (questionIndex === null || questionIndex === undefined)    // Check if there is no value in DynamicValue2
    {
        console.error('Invalid Question Index');
        return null;
    }

    // If there is a question number specified, lets fetch the details for it

    let questionsJSON = await General.LoadJSONFile(questionFilePath);    // Load the file with the questions
    if (!questionsJSON?.questionDetails)
    {
        console.error('Questions File Not Found at Path:', questionFilePath);
        return null;
    }

    if (questionsJSON.questionDetails.length <= questionIndex)    // Check if the the question number requested is there 
    {
        console.error('Question Index:', questionIndex,'Exceeds the Maximum number of Questions:', questionsJSON.questionDetails.length);
        return null;
    }

    // At this point, we have a valid question number specified

    let desiredQuestion = questionsJSON.questionDetails[questionIndex];     // Load in the details for this question

    // Now lets validate that the specified question is good:

    // ---> Check if there is a issue with retrieving the question
    if (!desiredQuestion)
    {
        console.error('Null Question Found for Question Index:', questionIndex);  // If there is, wait then continue the loop
        return null;
    }

    console.log('DesiredQuestion:', desiredQuestion);

    // ---> Check if the question text is actually there 
    if (desiredQuestion.questionText === null || desiredQuestion.questionText === undefined)
    {
        console.error('Null QuestionText Found for Question Index:', questionIndex);    // If it isn't, wait then continue the loop
        return null;
    }

    // ---> Check if the question response options are actually there 
    if (desiredQuestion.options === null || desiredQuestion.options === undefined)
    {
        console.error('Null Question Options Found for Question Index:', questionIndex);    // If they aren't, wait then continue the loop
        return null;
    }

    // ---> Check if the correct answer indicator for the question is there 
    if (desiredQuestion.Correct === null || desiredQuestion.Correct === undefined)
    {
        console.error('Null Question Correct for Question Index:', questionIndex);  // If it isn't, wait then continue the loop
        return null;
    }

    return desiredQuestion;
}

module.exports.LoadQuestion = LoadQuestion;
