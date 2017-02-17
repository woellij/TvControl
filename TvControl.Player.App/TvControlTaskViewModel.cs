using PropertyChanged;

namespace TvControl.Player.App
{
    [ImplementPropertyChanged]
    public class TvControlTaskViewModel
    {
        
        public string Id { get; set; }

        public string Description { get; set; }


    }
}