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

        public List<int> Levels => Enumerable.Range(1, LevelData.AllLevels.Count).ToList();

        private async void OnLevelSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is int selectedLevel)
            {
                var labyrinthGamePage = new LabyrinthGamePage();
                labyrinthGamePage.SetLevel(selectedLevel - 1); // Ustawienie wybranego poziomu
                await Navigation.PushAsync(labyrinthGamePage);
            }
        }
    }
}