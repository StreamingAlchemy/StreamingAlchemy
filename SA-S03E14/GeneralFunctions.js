const fs = require('fs').promises;
//******************************************************
// FUNCTION: sleep(ms)
//------------------------------------------------------
//          This is our own SLEEP function
//******************************************************

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));  // Uses setTimeout with a PROMISE to sleep for ms
}


//**********************************************************
// FUNCTION: LoadJSONFile(filePath)
//----------------------------------------------------------
//   This loads a JSON file and returns it as a JSON Object
//**********************************************************

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


//**********************************************************
// FUNCTION: WriteFile(filePath)
//----------------------------------------------------------
//   This saves a data object out to a file
//**********************************************************

async function WriteFile(filePath, dataObject)
{
    if (!filePath) {
        console.error('Invalid filePath in WriteFile');
        return null;
    }


    // Try to write the file
    try {
        await fs.writeFile(filePath, dataObject);
    } catch (ex)
    {
        console.log('Unable to write the file:', filePath,' in WriteJSONFile');
        return null;
    }    

}


module.exports.sleep = sleep;
module.exports.WriteFile = WriteFile;
module.exports.LoadJSONFile = LoadJSONFile;