using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using Mapster;

namespace KASHOP.BLL.Mapping
{
    public static class MapsterConfig
    {
        public static void MapsterConfigRegister()
        {
            TypeAdapterConfig<Category, CategoryResponse>.NewConfig()
            .Map(dist => dist.Name, src => src.CategoryTranslations.Where(t => t.Language == CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
            .Select(t => t.Name).FirstOrDefault());
        }
    }
}
