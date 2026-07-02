using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Dto
{
    public class CategoryTranslationRequest
    {
        public string Name { get; set; }
        public string Language { get; set; }
    }
}
