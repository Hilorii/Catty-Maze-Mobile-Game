using MobileApp.Models;

namespace MobileApp.Pages
{
    public partial class LevelSelectionPage : ContentPage
    {
        public LevelSelectionPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public List<int> Levels => Enumerable.Range(1, LevelData.AllLevels.Count).ToList(); // Odwo³anie do LevelData
    }
}