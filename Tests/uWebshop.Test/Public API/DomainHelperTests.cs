using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Public_API
{
	[TestFixture]
	public class DomainHelperTests
	{
		private Mock<ICMSEntityRepository> _cmsEntityRepository;

		[Test]
		public void ObjectsByAlias_ShouldCallCMSEntityRepository()
		{
			IOC.UnitTest();
			IOC.CMSEntityRepository.Mock(out _cmsEntityRepository);

			DomainHelper.GetObjectsByAlias<DomainHelperTests>(string.Empty);

			_cmsEntityRepository.Verify(m => m.GetObjectsByAlias<DomainHelperTests>(string.Empty, It.IsAny<ILocalization>(), 0));
		}
	}
}