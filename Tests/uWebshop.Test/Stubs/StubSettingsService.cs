using System;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Test.Stubs
{
	public class StubSettingsService : ISettingsService
	{
		public bool IncludingVat { get; set; }
		public int IncompleteOrderLifetime { get; set; }
		public bool UseLowercaseUrls { get; set; }

		public StubSettingsService()
		{
		}

		public StubSettingsService(bool includingVat)
		{
			IncludingVat = includingVat;
		}

		internal static StubSettingsService InclVat()
		{
			return new StubSettingsService {IncludingVat = true};
		}

		public static StubSettingsService ExclVat()
		{
			return new StubSettingsService {IncludingVat = false};
		}

		public void RegisterSettingsChangedEvent(Action<ISettings> e)
		{
			
		}

		public void TriggerSettingsChangedEvent(ISettings settings)
		{
		}
	}
}