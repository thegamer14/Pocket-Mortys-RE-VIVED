using System;

namespace UnityEngine.PlaymodeTestsRunner
{
	[Serializable]
	internal class PlaymodeTestsControllerSettings
	{
		[SerializeField]
		public TestRunnerFilter filter;

		public bool sceneBased;

		public string originalScene;

		public string bootstrapScene;

		public string resultFilePath;

		public bool isBatchModeRun;

		public static PlaymodeTestsControllerSettings CreateRunnerSettings(TestRunnerFilter filter)
		{
			PlaymodeTestsControllerSettings playmodeTestsControllerSettings = new PlaymodeTestsControllerSettings();
			playmodeTestsControllerSettings.filter = filter;
			playmodeTestsControllerSettings.sceneBased = false;
			playmodeTestsControllerSettings.bootstrapScene = null;
			playmodeTestsControllerSettings.resultFilePath = null;
			playmodeTestsControllerSettings.isBatchModeRun = false;
			playmodeTestsControllerSettings.originalScene = null;
			return playmodeTestsControllerSettings;
		}
	}
}
