// STREAMING ALCHEMY - Season 02 Episode 29
//
// Sponsored by Gnural Net, Inc.
//      www.gnuralnet.com
//         609-223-2434
//


// ****************************************************************
// *****  Google Apps Scripts - Working with Panelist Groups  *****
// ****************************************************************




// Takes a parameter of "group" which is the Group that you want to Show in the Destination Sheet

function PopulateGroupN(group) 
{
  // Get the Spreadsheet for the Source (All Possible Values to Pull From) and Destination (Location where vMix will be Looking)
  let sourceSheet = SpreadsheetApp.getActiveSpreadsheet().getSheetByName("Source");
  let destinationSheet = SpreadsheetApp.getActiveSpreadsheet().getSheetByName("Destination");

  // If we can't find the Source or Destination Sheets, then exit the script
  if (!sourceSheet || !destinationSheet)
  {
    Logger.error("Source or Destination Spreadsheet not Found");
    return;
  }
  
  // Get ALL The Rows and Columns in the Source Sheet
  let sourceData = sourceSheet.getDataRange().getValues();

  // If there is No Data, (Null, No Rows, No Columns) then exit the script
  if (!sourceData || !sourceData.length || !sourceData[0].length) {
    Logger.error("No Data Found in Source Spreadsheet");
    return;
  }

  // Clear out ALL Rows and Columns in the Destination Sheet (Stops old values from lingering)
  destinationSheet.getRange(1, 1, destinationSheet.getMaxRows(), destinationSheet.getMaxColumns()).clearContent();

  // Copy over the Header Values from the Source Sheet to the Destination Sheet
  destinationSheet.getRange(1,1, 1, sourceData[0].length).setValues([sourceData[0]]);

  // Runs the "Filter" Function on the Data from the Source Sheet
  // For Each Row, it runs the "Arrow Function" which Checks if the Row is Null and then if the Row is in the
  // Desired Group. To do this it returns "false" if it should NOT keep the Row, or "true" if it SHOULD keep the Row
  let destinationData = sourceData.filter((row) => {
    if (!row) { return false; } // If Null, Don't Include
    if (row[0] == group) {return true;} // If Matches Group, Include
    return false; // Otherwise Don't Include
  });
  
  // If there is no Data to Write, then exit the script
  if (destinationData.length == 0)
  {
    return;
  }

  // Starting after the first row (To Avoid the Header) until the Total length of the destination data to write (Row, Column)
  // Set the Filtered values from the Source Sheet into the Destination Sheet
  destinationSheet.getRange(2,1, destinationData.length, destinationData[0].length).setValues(destinationData);
}

// Since you can not Assign a Function with Parameters to a Button in Sheets
// These Functions statically map a Group Number to the "PopulateGroupN" function Call
function PopulateGroup1()
{
  PopulateGroupN(1);
}
function PopulateGroup2()
{
  PopulateGroupN(2);
}
function PopulateGroup3()
{
  PopulateGroupN(3);
}
function PopulateGroup4()
{
  PopulateGroupN(4);
}
function PopulateGroup5()
{
  PopulateGroupN(5);
}
function PopulateGroup6()
{
  PopulateGroupN(6);
}
function PopulateGroup7()
{
  PopulateGroupN(7);
}
function PopulateGroup8()
{
  PopulateGroupN(8);
}
function PopulateGroup9()
{
  PopulateGroupN(9);
}
function PopulateGroup10()
{
  PopulateGroupN(10);
}


function onOpen(){
  let ui = SpreadsheetApp.getUi();
  ui.createMenu("Set Active Group")
    .addSeparator()
    .addItem("Group 1", "PopulateGroup1")
    .addItem("Group 2", "PopulateGroup2")
    .addItem("Group 3", "PopulateGroup3")
    .addItem("Group 4", "PopulateGroup4")
    .addItem("Group 5", "PopulateGroup5")
    .addItem("Group 6", "PopulateGroup6")
    .addItem("Group 7", "PopulateGroup7")
    .addItem("Group 8", "PopulateGroup8")
    .addItem("Group 9", "PopulateGroup9")
    .addItem("Group 10", "PopulateGroup10")
    .addToUi();
}