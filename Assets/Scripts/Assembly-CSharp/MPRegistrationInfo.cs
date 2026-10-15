using System.Collections.Generic;

public class MPRegistrationInfo : MPStateInfo
{
	public string avatarID;

	public List<string> mortyIDs;

	public string username;

	public eRegisterMortiesSortyBy sortMortiesBy;

	public bool sortMortiesByAscending;

	public MPRegistrationInfo(string avatarID = null, List<string> mortyIDs = null, string username = null, eRegisterMortiesSortyBy sortMortiesBy = eRegisterMortiesSortyBy.TYPE, bool sortMortiesByAscending = true)
		: base(eMPStateOrigin.NONE)
	{
		this.avatarID = avatarID;
		this.mortyIDs = mortyIDs;
		this.username = username;
		this.sortMortiesBy = sortMortiesBy;
		this.sortMortiesByAscending = sortMortiesByAscending;
	}
}
