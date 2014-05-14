using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Helpers;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class PaymentProviderService : IPaymentProviderService
	{
		private readonly IPaymentProviderRepository _paymentProviderRepository;

		public PaymentProviderService(IPaymentProviderRepository paymentProviderRepository)
		{
			_paymentProviderRepository = paymentProviderRepository;
		}

		public IEnumerable<PaymentProvider> GetAll(ILocalization localization)
		{
			return _paymentProviderRepository.GetAll(localization);
		}

		public PaymentProvider GetPaymentProviderWithName(string paymentProviderName, ILocalization localization)
		{
			return _paymentProviderRepository.GetAll(localization).FirstOrDefault(x => x.Name.ToLower() == paymentProviderName.ToLower());
		}

		public PaymentProvider GetById(int id, ILocalization localization)
		{
			return _paymentProviderRepository.GetById(id, localization);
		}

		public void LoadData(PaymentProvider paymentProvider, ILocalization localization)
		{
			_paymentProviderRepository.ReloadData(paymentProvider, localization);
		}
	}
}