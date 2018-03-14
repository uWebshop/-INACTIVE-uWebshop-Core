using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Services
{
	internal class UrlFormatService : IUrlFormatService
	{
		private readonly ISettingsService _settings;
		private readonly ICMSApplication _cmsApplication;

		public UrlFormatService(ISettingsService settings, ICMSApplication cmsApplication)
		{
			_settings = settings; // todo: this is wrong in the current setup if resolved before settings is replaced by actual
			_cmsApplication = cmsApplication;
		}

		public string FormatUrl(string resultUrl)
		{
			if (_cmsApplication.AddTrailingSlash && !resultUrl.EndsWith("/"))
			{
				resultUrl += "/";
			}
			if (resultUrl.StartsWith("//"))
			{
				resultUrl = resultUrl.Substring(1);
			}
			if (!resultUrl.StartsWith("/") && !resultUrl.StartsWith("http"))
			{
				resultUrl = resultUrl.Insert(0, "/");
			}
            //return _settings.UseLowercaseUrls ? resultUrl.ToLower() : resultUrl;
            return resultUrl.ToLower();
        }
	}
}