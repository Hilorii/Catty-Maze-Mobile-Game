namespace MobileApp
{
    public partial class MainPage : ContentPage
    {
        private int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            CounterBtn.Text = $"Clicked {count} time{(count == 1 ? "" : "s")}";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }


}
