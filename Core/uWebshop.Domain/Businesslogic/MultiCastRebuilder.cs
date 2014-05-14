using System.Collections.Generic;
using System.Linq;
using uWebshop.Domain.Interfaces;

namespace uWebshop.Domain.Businesslogic
{
	internal class MultiCastRebuilder : ICacheRebuilder
	{
		private readonly IEnumerable<ICacheRebuilder> _rebuilders;

		public MultiCastRebuilder(IEnumerable<ICacheRebuilder> rebuilders)
		{
			_rebuilders = rebuilders.Where(r => r != null).ToList(); // toList() very much required
		}

		public void Lock()
		{
			foreach (var rebuilder in _rebuilders)
			{
				rebuilder.Lock();
			}
		}

		public void Rebuild()
		{
			foreach (var rebuilder in _rebuilders)
			{
				rebuilder.Rebuild();
			}
		}

		public void SwitchCache()
		{
			foreach (var rebuilder in _rebuilders)
			{
				rebuilder.SwitchCache();
			}
		}

		public void Unlock()
		{
			foreach (var rebuilder in _rebuilders)
			{
				rebuilder.Unlock();
			}
		}
	}
}