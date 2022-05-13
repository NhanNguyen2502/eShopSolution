using eShopSolution.Data.EF;
using eShopSolution.ViewModels.Result;
using eShopSolution.ViewModels.System.Languages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.Application.System.Languages
{
    public class LanguageService : ILanguageService
    {
        private readonly EShopDbContext _context;

        public LanguageService(EShopDbContext context)
        {
            _context = context;
        }

        public async Task<APIResultMessage<List<LanguageVM>>> GetAll()
        {
            var languages = await _context.Languages.Select(x => new LanguageVM()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();
            return new ApiSuccessResult<List<LanguageVM>>(languages);
        }
    }
}