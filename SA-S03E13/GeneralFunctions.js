const fs = require('fs').promises;
//******************************************************
// FUNCTION: sleep(ms)
//------------------------------------------------------
//          This is our own SLEEP function
//******************************************************

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));  // Uses setTimeout with a PROMISE to sleep for ms
}

async function LoadJSONFile(filePath)
{
    if (!filePath) {
        console.error('Invalid filePath in LoadJSONFile');
        return null;
    }

    try {
        const fileContents = await fs.readFile(filePath);
        let fileContentsAsJSON = JSON.parse(fileContents);
        return fileContentsAsJSON;
    } catch (ex)
    {
        console.error('Failed to Read File:', filePath,'In LoadJSONFile:',ex);
        return null;
    }
}

module.exports.sleep = sleep;
module.exports.LoadJSONFile = LoadJSONFile;