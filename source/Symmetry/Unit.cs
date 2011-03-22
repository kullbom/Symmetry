// Copyright 2011 Johan Kullbom (see the file license.txt)

namespace Symmetry
{
	using System;
	
	/// <summary>
	/// The unit type is a type that indicates the absence of a specific value;  
	/// 
	/// For futher information see "Unit Type (F#)": http://msdn.microsoft.com/en-us/library/dd483472.aspx
	/// </summary>
	public sealed class Unit : IEquatable<Unit>
	{
		/// <summary>
		/// The unit type has only a single value, which acts as a placeholder when no other value exists or is needed.
		/// </summary>
		public static Unit The = new Unit();
		
		private Unit () { }

		public bool Equals (Unit other) { return true; }
	}
}