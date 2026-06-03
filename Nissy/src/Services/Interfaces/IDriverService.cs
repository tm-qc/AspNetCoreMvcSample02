using Nissy.ViewModels.Driver;

namespace Nissy.Services.Interfaces
{
    public interface IDriverService
    {
        Task<IndexViewModel> GetListAsync(int page);

        Task<bool> CreateAsync(CreateEditViewModel viewModel);

        Task<CreateEditViewModel?> GetEditViewModelAsync(int id);

        Task<bool> UpdateAsync(int id, CreateEditViewModel viewModel);

        Task<bool> DeleteAsync(int id);
    }
}