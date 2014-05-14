using System;
using uWebshop.Domain.ContentTypes;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.BaseClasses
{
	/// <summary>
	///     Class based on the Umbraco Document class to inherit from
	/// </summary>
	public class DocumentBase
	{
		private IUwebshopContent _document;

		/// <summary>
		///     Gets the document
		/// </summary>
		public IUwebshopContent Document
		{
			get { return _document ?? (_document = IO.Container.Resolve<ICMSContentService>().GetById(Id)); }
			internal set { _document = value; }
		}

		/// <summary>
		///     Gets the id of the document
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		///     Gets a System.DateTime object that is set to the date and time when the document is updated
		/// </summary>
		public DateTime UpdateDate
		{
			get { return Document.UpdateDate; }
		}

		/// <summary>
		///     Initializes a new instance of the uWebshop.Domain.DocumentBase class
		/// </summary>
		public DocumentBase()
		{
		}

		/// <summary>
		///     Initializes a new instance of the uWebshop.Domain.DocumentBase class
		/// </summary>
		/// <param name="id">NodeId of the document</param>
		public DocumentBase(int id)
		{
			Id = id;
		}
	}
}