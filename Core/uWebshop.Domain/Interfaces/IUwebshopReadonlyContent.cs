using System;
using System.Collections.Generic;

namespace uWebshop.Domain.Interfaces
{
	/// <summary>
	/// 
	/// </summary>
	public interface IUwebshopReadonlyContent
	{
#pragma warning disable 1591
		string Path { get; }
		DateTime CreateDate { get; }
		DateTime UpdateDate { get; }
		int SortOrder { get; }
		string UrlName { get; }
		IUwebshopReadonlyContent Parent { get; }
		int Id { get; }
		string NodeTypeAlias { get; }
		string Name { get; }
		int template { get; }
		ICMSProperty GetProperty(string propertyAlias);
		ICMSProperty GetMultiStoreItem(string propertyAlias);
		string Url { get; }
		List<IUwebshopReadonlyContent> ChildrenAsList { get; }
	}

	public interface IUwebshopContent : IUwebshopReadonlyContent
	{
		new IUwebshopContent Parent { get; }
		T1 GetProperty<T1>(string propertyAlias);
		void SetProperty(string propertyAlias, object value);
		void Publish(int userId = 0);
		ICMSProperty getProperty(string propertyAlias);
	}

	public interface ICMSProperty
	{
		string Value { get; }
	}
}