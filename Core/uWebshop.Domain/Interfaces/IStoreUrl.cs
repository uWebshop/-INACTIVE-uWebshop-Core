namespace uWebshop.Domain.Interfaces
{
	interface IStoreUrl
	{
		Store Store { get; }
		string Url { get; }
	}
}