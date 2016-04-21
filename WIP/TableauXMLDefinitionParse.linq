<Query Kind="Program" />

void Main()
{
	
	ParseTableau();
}

void ParseTableau()
{

	bool blnOverwrite = true;
	string strSeparator =  ",";
	string strEnclosureBegin = "[";
	string strEnclosureEnd = "]";
	string wb = @"n:\\temp\\workbooksnippet.xml";
	string def = @"n:\\temp\\definitions.csv";
	
  	//Load the tableau workbook into an xmldocument
	XDocument xdoc = XDocument.Load(wb);
	
	//Run query to get the columns in the Tableau workbook
	var cols = (from lv1 in xdoc.Descendants("column")
				select lv1).ToList();
	
	//Loop through columns in the tableau workbook
	foreach (var lv1 in cols)
	{
			//get the column name and normalize it (i.e. for duplicated columns that are aliased			
			string strOriginalCol = lv1.Attribute("name").Value;
			string strNormalizedCol = NormalizeColumn(lv1.Attribute("name").Value).Dump();
			
			//loop through the definitions for this column name and try to find a match
			string [] defs = File.ReadAllLines(def);
			var querydef = from line in defs	
				let data = line.Split(Convert.ToChar(strSeparator)) 
				where data[0].ToUpper() == strNormalizedCol.ToUpper().Replace(strEnclosureBegin,"[").Replace(strEnclosureEnd,"]")
				select new { Def = data[1] };
			string definition = null;
			foreach (var d in querydef)
			{
				definition = d.Def;
			}
		    
			//if we found a definition lets use it now
			if (definition != null) 
			{
				//if match found then see if definition already exists
				//if a definition exists and the ovewrite is true, then update
				//else if a definition does not exist, then create
				definition = definition.Replace(strEnclosureBegin,"").Replace(strEnclosureEnd,"");
				
				bool definition_exists = false;		
				foreach(var lv2 in lv1.Descendants("run"))
				{
					definition_exists = true;
					//if a definition exists and overwrite is true then update
					if(blnOverwrite) 
					{
						lv2.Value = definition;
					}
				}
				
				if (!definition_exists)
				{
					//we have a definition but one does not exist, create the elements for it
					lv1.Add(new XElement("desc",new XElement("formatted-text", new XElement("run",definition))));
				}
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