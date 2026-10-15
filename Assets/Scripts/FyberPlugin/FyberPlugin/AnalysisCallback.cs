namespace FyberPlugin
{
	public interface AnalysisCallback
	{
		void OnAnalysisSucceeded(IntegrationReport integrationReport);

		void OnAnalysisFailed(AnalysisError? analysisError);
	}
}
