using UnityEngine.PlaymodeTestsRunner;

namespace UnityEngine.PlaymodeTests
{
	public class PlaymodeTest
	{
		public static void Pass()
		{
			PlaymodeTestsController controller = PlaymodeTestsController.GetController();
			controller.PassPlaymodeTest();
		}

		public static void Fail()
		{
			PlaymodeTestsController controller = PlaymodeTestsController.GetController();
			controller.FailPlaymodeTest();
		}
	}
}
