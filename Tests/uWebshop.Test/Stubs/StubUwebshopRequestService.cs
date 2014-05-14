using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Stubs
{
	internal class StubUwebshopRequestService : IUwebshopRequestService
	{
		public StubUwebshopRequestService()
		{
			Current = new UwebshopRequest();
		}

		public UwebshopRequest Current { get; set; }
	}
}