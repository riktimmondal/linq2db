// ---------------------------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by LinqToDB scaffolding tool (https://github.com/linq2db/linq2db).
// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
// ---------------------------------------------------------------------------------------------------


#pragma warning disable 1573, 1591
#nullable enable

namespace Cli.NoMetadata.SqlServerNorthwind
{
	public class EmployeeTerritory
	{
		public int    EmployeeId  { get; set; } // int
		public string TerritoryId { get; set; } = null!; // nvarchar(20)

		#region Associations
		/// <summary>
		/// FK_EmployeeTerritories_Employees
		/// </summary>
		public Employee Employees { get; set; } = null!;

		/// <summary>
		/// FK_EmployeeTerritories_Territories
		/// </summary>
		public Territory Territories { get; set; } = null!;
		#endregion
	}
}