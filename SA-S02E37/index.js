// STREAMING ALCHEMY - Season 02 Episode 37
//
// Sponsored by Gnural Net, Inc.
//      www.gnuralnet.com
//         609-223-2434
//
//

// ********************************************************************
// *****  Use NODE.JS to sync SuperJoy to camera in vMix PREVIEW  *****
// ********************************************************************



// Initialize the required external libraries
const Axios = require('axios');     // AXIOS is used to make HTTP requests
const ParseXml = require('xml2js').parseStringPromise;  // XML2JS converts XML into JSON
const XPath = require('xml2js-xpath');      // XML2JS-XPATH provides access to elements via x-path

// This is our own SLEEP function
function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));  // Uses setTimeout with a PROMISE to sleep for ms
}

// Core functionality replacing script
async function main()
{
    let oldPreviewInput = '0';  // Start with NO INPUT being set as active in PREVIEW
    
    // Keep looping in the background
    while (true)
    {
        let result = null;  // To start, set the Web Response Object for vMix XML to NULL
        
        try {
            // Execute a GET Command on vMix XML using AXIOS Library    
            result = await Axios.get('http://localhost:8088/api');
            // Result contains the Web Response Object with the 'Data' Object,
            // inside the Web Response Object, containing the vMix XML as a String
        } catch (ex) {
            // There was no result - likely because vMix hasn't started yet
            console.log('Failed to Get vMix API: ' + ex);
            // Let's wait 10 Seconds and try again
            await sleep(10000);
            continue;
        }
    
        // Pull the vMix XML into a JSON Object
        let parsedJsonAPI = await ParseXml(result.data);
        
        // Using XPath, get the number of the INPUT that is in PREVIEW
        let previewInput = XPath.find(parsedJsonAPI, '/vmix/preview').at();

        // If the INPUT in PREVIEW hasn't changed, wait a second then check it all again
        if (oldPreviewInput === previewInput)
        {
            await sleep(1000);
            continue;
        }

        // If we have a new INPUT in PREVIEW save it
        oldPreviewInput = previewInput;

        try {
            // Now tell the SuperJoy to switch to the Camera in that INPUT
            await Axios.get('http://10.0.0.123/cgi-bin/joyctrl.cgi?f=camselect&group=1&camid=' + previewInput, {timeout: 500});
        } catch (ex) {
            // If there was a problem display an error
            console.log('Failed to Make GET Request to SuperJoy');
            // Clear the saved INPUT number
            oldPreviewInput = '0';
            // Let the vMix operator know that the connection with the SuperJoy is down
            await Axios.get('http://localhost:8088/API/?Function=PreviewInput&Input=4');
            // Wait 10 Seconds (for SuperJoy to reconnect) and check it all again
            await sleep(10000);
            continue;
        }
        // Wait 1 Second then check it all again
        await sleep(1000);
    }``
}

// Launch the function 'main' to kick this off
main();