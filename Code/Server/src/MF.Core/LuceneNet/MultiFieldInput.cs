using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace MF.LuceneNet
{
    public class MultiFieldInput
    {
        public Occur Occur { get; set; } = Occur.MUST;

        public string Field { get; set; }

        public string Keyword { get; set; } = "";
    }
}
