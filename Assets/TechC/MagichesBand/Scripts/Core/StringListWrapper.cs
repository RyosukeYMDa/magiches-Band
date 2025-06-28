using System.Collections.Generic;

namespace TechC.MagichesBand.Core
{
    [System.Serializable]
    public class StringListWrapper
    {
        public List<string> idList;

        public StringListWrapper(List<string> idList)
        {
            this.idList = idList;
        }
    }
}
