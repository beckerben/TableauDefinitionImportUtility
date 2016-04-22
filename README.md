# Tableau Definition Import Utility
This is a C# utility to import definition metadata into a Tableau workbook. This metadata shows up as a tooltip on the dimensions and measures.

I noticed that when Tableau data sources are connected to databases, the column definition which may be stored in the database is not brought in automatically as a tooltip.  I was thinking that it would be nice if there was a way to import the metadata which may be stored in the database data dictionary into the Tableau data source built upon the database tables and to have this metadata show up as a tooltip when hovering over the respective dimension and measure.

The Tableau workbooks are simply XML documents so this utility will import the definitions stored in a simple CSV file into the Tableau workbook where column matches are found. 

## Requirements
1. .NET 4.5 Framework
2. A Tableau workbook file (.twb, not packaged .twbx)
3. A definition flat file containing two columns for the database column name and the related definition. 

See the WIKI for more help using the utility

![alt tag](https://raw.githubusercontent.com/beckerben/TableauDefinitionImportUtility/master/Misc/Images/Application.png)


