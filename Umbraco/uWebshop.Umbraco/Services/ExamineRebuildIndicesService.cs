using System;
using Examine;
using uWebshop.Domain;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Umbraco.Services
{
	internal class ExamineRebuildIndicesService : IRebuildIndicesService
	{
		private static DateTime _lastExamineIndexRebuild = DateTime.Now.AddHours(-3);

		public void Rebuild()
		{
			//if (UwebshopConfiguration.Current.RebuildExamineIndex && DateTime.Now.AddHours(-1) > _lastExamineIndexRebuild)
			//{
			//	ExamineManager.Instance.IndexProviderCollection["ExternalIndexer"].RebuildIndex();
			//	_lastExamineIndexRebuild = DateTime.Now;
			//}
			//Log.Instance.LogWarning("UrlRewrite: Examine indexes are rebuilding"); [2:27:52 PM] Arnold Visser: gewoon niet loggen, gewoon doen
		}
	}
}