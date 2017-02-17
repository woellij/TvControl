using System.ComponentModel;

using ReactiveUI;

namespace TvControl.Player.App.ViewModels
{
    public class ViewModelBase : ReactiveObject, INotifyPropertyChanged
    {

        public void OnPropertyChanged(string propertyName)
        {
            this.RaisePropertyChanged(propertyName);
        }

    }
}