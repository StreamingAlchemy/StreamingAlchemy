
// Initialize the required external libraries
const ParseXml = require('xml2js').parseStringPromise;  // XML2JS converts XML into JSON
const XPath = require('xml2js-xpath');      // XML2JS-XPATH provides access to elements via x-path
const fetch = require('node-fetch');
    
const FETCH_SETTINGS = { method: "Get" };

const VMIX_URL = 'http://localhost:8088/API'

// Lets set up a set of sample responses as JSON Object

let Responses = [

    {
        "name":"Mandy Devina", 
        "answer":"#B",
        "avatarURI":"/images/Response-F03.png"
    },
    {
        "name":"Ally Andersen", 
        "answer":"#D",
        "avatarURI":"/images/Response-F04.png"
    },
    {
        "name":"Celia Morton", 
        "answer":"#C",
        "avatarURI":"/images/Response-F05.png"
    },
    {
        "name":"Stu Harvey", 
        "answer":"#D",
        "avatarURI":"/images/Response-M03.png"
    },
    {
        "name":"Stephen Curson", 
        "answer":"#D",
        "avatarURI":"/images/Response-M04.png"
    },
    {
        "name":"Elijah Astor", 
        "answer":"#A",
        "avatarURI":"/images/Response-M05.png"
    },
    {
        "name":"Charlotte Resler", 
        "answer":"#B",
        "avatarURI":"/images/Response-F06.png"
    },
    {
        "name":"Amelia Vester", 
        "answer":"#A",
        "avatarURI":"/images/Response-F07.png"
    },
    {
        "name":"Isabella Dee", 
        "answer":"#C",
        "avatarURI":"/images/Response-F08.png"
    },
    {
        "name":"Hazel Marsh", 
        "answer":"#B",
        "avatarURI":"/images/Response-F09.png"
    },
    {
        "name":"Mia Gianna", 
        "answer":"#D",
        "avatarURI":"/images/Response-F10.png"
    },
    {
        "name":"Liam Clayton", 
        "answer":"#A",
        "avatarURI":"/images/Response-M01.png"
    },
    {
        "name":"Oliver Adams", 
        "answer":"#B",
        "avatarURI":"/images/Response-M02.png"
    },
    {
        "name":"Lucas Mars", 
        "answer":"#B",
        "avatarURI":"/images/Response-M06.png"
    },
    {
        "name":"Ben Faston", 
        "answer":"#A",
        "avatarURI":"/images/Response-M07.png"
    },
    {
        "name":"Chase Dalton", 
        "answer":"#D",
        "avatarURI":"/images/Response-M08.png"
    },
    {
        "name":"Cindy Willows", 
        "answer":"#A",
        "avatarURI":"/images/Response-F01.png"
    },
    {
        "name":"Betty Stein", 
        "answer":"#A",
        "avatarURI":"/images/Response-F02.png"
    },
    {
        "name":"Wyatt Presley", 
        "answer":"#C",
        "avatarURI":"/images/Response-M09.png"
    },
    {
        "name":"Finn Clairmont", 
        "answer":"#D",
        "avatarURI":"/images/Response-M10.png"
    }
    ]



// This is our own SLEEP function
function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));  // Uses setTimeout with a PROMISE to sleep for ms
}

async function genRandomAnswer() {
    let responseFields = ['A', 'B', 'C', 'D'];
    let answerIndex = Math.floor(Math.random()*4);
    let randomAnswer = "#" + responseFields[answerIndex];
    console.log("Random answer: ", randomAnswer);
    return randomAnswer;
}

async function UpdateSMTitle(vMix_URI, input, responseData)
{
    const FETCH_SETTINGS = { method: "Get" };
    let titleUpdateUrl = null;
    let avatarURI = process.cwd();
    
    avatarURI += responseData.avatarURI;
    console.log("Avatar URI: " + avatarURI);
    try
    {

        titleUpdateUrl = vMix_URI + '/?Function=SetText&Input=' + input + '&SelectedName=FromName.Text&value=' + encodeURIComponent(responseData?.name);
        console.log("SM - Name: ",titleUpdateUrl);
        await fetch(titleUpdateUrl, FETCH_SETTINGS);

        titleUpdateUrl = vMix_URI + '/?Function=SetImage&Input=' + input + '&SelectedName=FromPhoto.Fill.Bitmap&value=' + encodeURIComponent(avatarURI);
        console.log("SM - AvatarURI: ",titleUpdateUrl);
        await fetch(titleUpdateUrl, FETCH_SETTINGS);

        let randomAnswer = await genRandomAnswer();

        titleUpdateUrl = vMix_URI + '/?Function=SetText&Input=' + input + '&SelectedName=Message.Text&value=' + encodeURIComponent(randomAnswer);
        console.log("SM - AvatarURI: ",titleUpdateUrl);
        await fetch(titleUpdateUrl, FETCH_SETTINGS);

        
    } catch (ex)
    {
        console.error('Failed to UpdateSMTitle [Input:', input, '] [responseData:', JSON.stringify(responseData), '] [Error:', ex, ']');
    }
}

async function main() {
    let step1 = true;
        
    while(step1) {
        for (let i = 0; i < Responses.length; i++)
        {
            console.log("Name: "+ Responses[i].name + "  Answer: " + Responses[i].answer);
            UpdateSMTitle(VMIX_URL, 1, Responses[i])
            await sleep(1000);
        } 
    }
}

main();