namespace uWebshop.Umbraco.Interfaces
{
	internal interface ICMSContent
	{
		void SetValue(string propertyAlias, string value);
		void SaveAndPublish();
		string ContentTypeAlias { get; }
		bool HasProperty(string key);
	}
}