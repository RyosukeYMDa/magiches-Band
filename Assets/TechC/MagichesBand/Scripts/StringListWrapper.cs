using System.Collections.Generic;

namespace TechC.MagichesBand
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
