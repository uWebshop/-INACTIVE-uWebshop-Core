using NUnit.Framework;
using uWebshop.Domain.Upgrading;

namespace uWebshop.Test.Domain.Updating
{
	[TestFixture]
	public class OrderUpdaterParseOrderNumberTest
	{
		private OrderTableUpdater orderTableUpdater;

		[SetUp]
		public void Setup()
		{
			orderTableUpdater = new OrderTableUpdater();
		}

		[TestCase("0001", 1)]
		[TestCase("2", 2)]
		[TestCase("ToyStore3", 3)]
		[TestCase("ToyStore0004", 4)]
		[TestCase("Toy1Store5", 5)]
		[TestCase("OID0025", 25)]
		[TestCase("13", 13)]
		[TestCase("0052", 52)]
		[TestCase(null, null)]
		public void ParsingStringShouldGiveLastContainingNumber(string orderNumber, int? expectedNumber)
		{
			var foundNumber = orderTableUpdater.TryParseOrderNumber(orderNumber);

			Assert.AreEqual(expectedNumber, foundNumber);
		}
	}
}