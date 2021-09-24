// This function will sort the sheet based the 'SCORE' column

function sortSheet() 
{
  // Get the Spreadsheet with all of the scores
  let LeaderBoardSheet = SpreadsheetApp.getActiveSpreadsheet().getSheetByName("LEADERBOARD");

  // If we can't find the LEADERBOARD Sheet, then exit the script
  if (!LeaderBoardSheet)
  {
    Logger.error("Spreadsheet not Found");
    return;
  }
  
  // Get ALL The Rows and Columns in the LEADERBOARD Sheet
  let scoreDataRange = LeaderBoardSheet.getRange(2,1,9, 12)
   

  // If there is No Data, (Null, No Rows, No Columns) then exit the script
  if (!scoreDataRange) {
    Logger.error("No Data Found in LEADERBOARD Spreadsheet");
    return;
  }

// Sort the data rows in the sheet based on 'SCORE' (Column 3)

  scoreDataRange.sort({column:3, ascending:true});

}

// Automatically trigger the sorting of the sheet when data is edited

function onEdit(changedItem){
  sortSheet()
}

