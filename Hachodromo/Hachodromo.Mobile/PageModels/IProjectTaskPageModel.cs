using CommunityToolkit.Mvvm.Input;
using Hachodromo.Mobile.Models;

namespace Hachodromo.Mobile.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}