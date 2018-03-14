using System.Collections.Generic;

namespace SuperSimpleWebshop.Common
{
    public class MultipleContentPicker
    {
        private string _Xml;
        private List<int> _NodeIds;
        public List<int> NodeIds
        {

            get
            {
                if (this._NodeIds == null)
                {
                    this._NodeIds = new List<int>();

                    if (!string.IsNullOrEmpty(_Xml))
                    {
                        var nodeDoc = _Xml;

                        new List<string>(nodeDoc.Split(',')).ForEach(x => _NodeIds.Add(int.Parse(x)));
                    }
                }

                return this._NodeIds;
            }
        }

        public MultipleContentPicker(string xml)
        {
            _Xml = xml;
        }
    }
}