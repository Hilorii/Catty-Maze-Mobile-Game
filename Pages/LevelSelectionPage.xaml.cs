using MobileApp.Models;

namespace MobileApp.Pages
{
    public partial class LevelSelectionPage : ContentPage
    {
        private List<int> _levels => Enumerable.Range(1, LevelData.AllLevels.Count).ToList();

        public List<KeyValuePair<int, string>> LevelButtons
        {
            get
            {
                var levelButtons = new List<KeyValuePair<int, string>>(); // nr poziomu, nazwa pliku obrazka
                foreach (var level in _levels)
                {
                    var imageFileName = $"level{level}{(GameState.CompletedLevels.Contains(level - 1) ? "down" : "up")}";
                    var levelButton = new KeyValuePair<int, string>(level, imageFileName);
                    levelButtons.Add(levelButton);
                }
                return levelButtons;
            }
        }

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

            LevelSelect.SelectedItem = null; // aby umo¿liwiæ wybór tego samego poziomu jeszcze raz
        }


        private async void OnLevelSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is KeyValuePair<int, string> selectedButton)
            {
                var selectedLevel = selectedButton.Key;
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