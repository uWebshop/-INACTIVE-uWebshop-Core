using System.Linq;
using NUnit.Framework;

namespace uWebshop.Test.Domain.Domain_classes.OrderInfoTests
{
	[TestFixture]
	public class CustomValidationTest
	{
		[Ignore] // todo: needs some decoupling
		[Test]
		public void ThatCustomErrorIsRunAndReceivesCorrectArgument()
		{
			IOC.UnitTest();
			IOC.SettingsService.InclVat();
			var info = DefaultFactoriesAndSharedFunctionality.CreateIncompleteOrderInfo(DefaultFactoriesAndSharedFunctionality.CreateProductInfo(1000, 1));
			info.TermsAccepted = true;
			bool wasRun = false;
			info.RegisterCustomOrderValidation(order =>
				{
					wasRun = true;
					return true;
				}, order => "test");

			Assert.IsTrue(!IOC.OrderService.Actual().Resolve().ValidateOrder(info, true).Any());
			Assert.IsTrue(wasRun);
		}
	}
}