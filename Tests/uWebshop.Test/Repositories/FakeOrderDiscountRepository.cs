using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;
using IOrderDiscount = uWebshop.Domain.Interfaces.IOrderDiscount;

namespace uWebshop.Test.Repositories
{
	internal class FakeOrderDiscountRepository : IOrderDiscountRepository
	{
		public List<IOrderDiscount> Entities = new List<IOrderDiscount>();

		public IOrderDiscount GetById(int id, ILocalization localization)
		{
			return Entities.FirstOrDefault(d => d.OriginalId == id);
		}

		public List<IOrderDiscount> GetAll(ILocalization localization)
		{
			return Entities;
		}

		public void ReloadData(IOrderDiscount entity, ILocalization localization)
		{
		}
	}
}