using GooglePlayGames.BasicApi.SavedGame;

public class ManualConflictResolver
{
	public IConflictResolver resolver;

	public ISavedGameMetadata originalMeta;

	public object originalObj;

	public ISavedGameMetadata unmergedMeta;

	public object unmergedObj;

	public ManualConflictResolver(IConflictResolver resolver, ISavedGameMetadata originalMeta, object originalObj, ISavedGameMetadata unmergedMeta, object unmergedObj)
	{
		this.resolver = resolver;
		this.originalMeta = originalMeta;
		this.originalObj = originalObj;
		this.unmergedMeta = unmergedMeta;
		this.unmergedObj = unmergedObj;
	}
}
