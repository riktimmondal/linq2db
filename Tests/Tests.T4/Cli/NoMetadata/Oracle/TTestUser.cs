// ---------------------------------------------------------------------------------------------------
// <auto-generated>
// This code was generated by LinqToDB scaffolding tool (https://github.com/linq2db/linq2db).
// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
// ---------------------------------------------------------------------------------------------------

using System.Collections.Generic;

#pragma warning disable 1573, 1591
#nullable enable

namespace Cli.NoMetadata.Oracle
{
	public class TTestUser
	{
		public decimal UserId { get; set; } // NUMBER
		public string  Name   { get; set; } = null!; // VARCHAR2(255)

		#region Associations
		/// <summary>
		/// SYS_C007123 backreference
		/// </summary>
		public IEnumerable<TTestUserContract> TTestUserContracts { get; set; } = null!;
		#endregion
	}
}
