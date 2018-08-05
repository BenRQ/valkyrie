
function StatsGenerator() {

  this._data = {
    "FileGenerationDate":"",
    "scenarios_stats": []
   };
  
  this.scenario_score_sum = {};
  this.scenario_duration_sum = {};
  this.scenario_victory_sum = {};
  this.scenario_number_play = {};
  this.scenario_names = [];

}

//     0            1            2       3         4           5            6              7                 8                  9
// time	  |  Scenario name | Victory |  score | Comments | duration	| NB of players | investigators | Events activated | Language selected				

// Columns order in channel list sheet
var _col_scenario_name       = 1;
var _col_Victory             = 2;
var _col_score               = 3;
var _col_duration            = 5;


StatsGenerator.prototype.generate = function generate()
{
  this._parseSheet();
  
  this._prepareStats();
  
  // write JSON file
  this._writeFile();
}


StatsGenerator.prototype._parseSheet = function _parseSheet()
{
  var sheets = SpreadsheetApp.getActiveSpreadsheet().getSheets();
  var stats_sheet = sheets[0];
  var stats_data = stats_sheet.getDataRange().getValues();

  // Parsing sheet
  for (var i = 1; i < stats_data.length; i++) {

    var current_scenario_name = stats_data[i][_col_scenario_name].toLowerCase();
    
    if (this.scenario_names.indexOf( current_scenario_name ) == -1)
    {
      this.scenario_names.push( current_scenario_name );
      
      this.scenario_score_sum[current_scenario_name] = 0;
      this.scenario_duration_sum[current_scenario_name] = 0;
      this.scenario_victory_sum[current_scenario_name] = 0;
      this.scenario_number_play[current_scenario_name] = 0;
    }
    
    this.scenario_number_play[current_scenario_name] += 1;
    
    this.scenario_score_sum[current_scenario_name]     += stats_data[i][_col_score];
    this.scenario_duration_sum[current_scenario_name]  += stats_data[i][_col_duration];
    this.scenario_victory_sum[current_scenario_name]   += stats_data[i][_col_Victory];
    
  }
}

StatsGenerator.prototype._prepareStats = function _prepareStats()
{
  // creating object structure to generate JSON automatically
  for (var i = 0; i < this.scenario_names.length; i++) {

    var current_scenario_name = this.scenario_names[i];
    var current_scenario_number_play = this.scenario_number_play [current_scenario_name];
    
    if (current_scenario_number_play > 0) // should not be possible ... but just in case
    {
      this._data.scenarios_stats.push({
        "scenario_name":         current_scenario_name,
        "scenario_number_play":  current_scenario_number_play,
        "scenario_avg_score":    (this.scenario_score_sum   [current_scenario_name] / current_scenario_number_play),
        "scenario_avg_duration": (this.scenario_duration_sum[current_scenario_name] / current_scenario_number_play),
        "scenario_avg_victory":  (this.scenario_victory_sum [current_scenario_name] / current_scenario_number_play),
      });
    }
   
  }
  
  var date = (new Date()).toISOString();
  date = date.slice(0, this._data.FileGenerationDate.length-5); // get a clean date
  
  this._data.FileGenerationDate= date+'UTC';

}

//***********************************************************
// Miscellaneous
//***********************************************************

StatsGenerator.prototype._writeFile = function _writeFile() 
{
   var filename="ValkyrieStats.json";
   
   // get the JSON file id, if it exists
   var thisFile = DriveApp.getFileById( SpreadsheetApp.getActive().getId() );
   var sheetFolder = thisFile.getParents().next();
   var JSONfile = sheetFolder.getFilesByName(filename);
   
   if ( JSONfile.hasNext() )
   { 
      JSONfile.next().setContent( JSON.stringify(this._data) );
   } 
   else
   {
     sheetFolder.createFile(filename, JSON.stringify(this._data), "application/json");
   }

}
