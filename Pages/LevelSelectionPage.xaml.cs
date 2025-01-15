using MobileApp.Models;

namespace MobileApp.Pages
{
    public partial class LevelSelectionPage : ContentPage
    {
        public LevelSelectionPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MainMenuPage.PlayMusic();
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

        private void OnBackPressed(object sender, EventArgs e)
        {
            Navigation.PopAsync(); // Cofanie do poprzedniej strony
        }
    }
}