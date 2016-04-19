<Query Kind="Program" />

void Main()
{
	
	ParseTableau();
}

// Define other methods and classes here
void ParseTableau()
{

	bool blnOverwrite = false;
	string wb = @"n:\\temp\\workbooksnippet.xml";
	string def = @"n:\\temp\\definitions.csv";
	
  	//Load xml
	XDocument xdoc = XDocument.Load(wb);
	
	//Run query to get the columns in the Tableau workbook
	var cols = (from lv1 in xdoc.Descendants("column")
				select lv1).ToList();
	
	//Loop through columns
	foreach (var lv1 in cols)
	{
			//get the column name and normalize it (i.e. for duplicated columns that are aliased			
			string strOriginalCol = lv1.Attribute("name").Value;
			string strNormalizedCol = NormalizeColumn(lv1.Attribute("name").Value).Dump();
			
			//loop through the definitions for this column name and try to find a match
			string [] defs = File.ReadAllLines(def);
			var querydef = from line in defs	
				let data = line.Split(',')
				where data[0] == strNormalizedCol
				select new { Def = data[1] };
			string definition = null;
			foreach (var d in querydef)
			{
				definition = d.Def;
			}
		    
			//if we found a definition lets use it now
			if (definition != null) definition.Dump();
			
			//todo: search definition, see if we find a match for the normalized column
			//if match found then see if definition already exists
			//if a definition exists and the ovewrite is true, then update
			//else if a definition does not exist, then create
			
			bool definition_exists = false;		
			foreach(var lv2 in lv1.Descendants("run"))
			{
				("     " + lv2.Value).Dump();
				lv2.Value = "this is a test 2";
			}
	}
	
	xdoc.Save("n:\\temp\\workbooksnippet2.xml");
}


private string NormalizeColumn(string col)
{
	string strNormal = col;	
	if (col.Contains("(") && col.Contains(")"))
	{
		strNormal = col.Substring(0,col.IndexOf("(")-1) + "]";
	}
	return strNormal;
}