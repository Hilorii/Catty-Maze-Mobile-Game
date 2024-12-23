namespace MobileApp.Pages;

public partial class MainMenuPage : ContentPage
{
    public MainMenuPage()
    {
        InitializeComponent();
    }

    private async void OnStartGame(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LabyrinthGamePage());
    }

    private async void OnSelectLevel(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LevelSelectionPage());
    }
}