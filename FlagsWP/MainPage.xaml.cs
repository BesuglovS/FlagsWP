using FlagsWP.Core;
using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace FlagsWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private FlagsInfo Flags;
        private int flagIndex;
        private int lettersOpened;
        private bool justStarted = true;
        private bool juststartedGame;

        private GameMode gameMode;
        private String countryCode;
        private int FlagsSolved;

        private int lives;
        private int initialLivesCount = 5;

        Dictionary<int, String> buttonNames = new Dictionary<int, string>()
            {
                { 0, "b11" }, { 1, "b12" }, { 2, "b13" }, { 3, "b14" }, { 4, "b15" }, { 5, "b16" }, { 6, "b17" }, { 7, "b18" },
                { 8, "b21" }, { 9, "b22" }, { 10, "b23" }, { 11, "b24" }, { 12, "b25" }, { 13, "b26" }, { 14, "b27" }, { 15, "b28" }
            };

        Dictionary<String, int> buttonNamesReverse = new Dictionary<String, int>()
            {
                { "b11", 0 }, { "b12", 1 }, { "b13", 2 }, { "b14", 3 }, { "b15", 4 }, { "b16", 5 }, { "b17", 6 }, { "b18", 7 },
                { "b21", 8 }, { "b22", 9 }, { "b23", 10 }, { "b24", 11 }, { "b25", 12 }, { "b26", 13 }, { "b27", 14 }, { "b28", 15 }
            };

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }



        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            gameMode = GameMode.CountryNames;
            countryCode = "RU";
            
            await LoadFlags();

            ShowQuestion();

            lives = initialLivesCount;
            FlagsSolved = 0;
            juststartedGame = true;
            SetHealth(); 
        }

        private void SetHealth()
        {
            //<Image Margin="5" Source="pack://application:,,,/Icons/Health.png"></Image>
            healthStack.Children.Clear();

            for (int i = 0; i < lives; i++)
            {
                var healthImage = new Image();
                healthImage.Source = new BitmapImage { UriSource = new Uri("ms-appx:///Icons/Health.png", UriKind.Absolute) };
                healthImage.Margin = new Thickness(5);

                healthStack.Children.Add(healthImage);
            }
        }

        private void ShowQuestion()
        {
            if (!justStarted)
            {
                var nextButton = new Button();
                nextButton.Name = "NextBtn";
                nextButton.Content = "Далее";
                nextButton.Width = 100;
                nextButton.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
                next.Children.Add(nextButton);                
                nextButton.Click += nextClicked;

                return;
            }

            justStarted = false;

            nextClicked(this, null);
        }

        private void nextClicked(object sender, RoutedEventArgs e)
        {
            next.Children.Clear();            

            var r = new Random(DateTime.Now.Millisecond);
            flagIndex = r.Next(Flags.list.Count());

            Flag.Source = new BitmapImage { UriSource = new Uri("ms-appx:///Flags/" + Flags.list[flagIndex].filename + ".png", UriKind.Absolute) };
            
            var localizedName = Flags.list[flagIndex].localizedNames[ComposeKey()];

            SetEmptyAnswer(localizedName);

            SetAnswerLetters(localizedName);

            lettersOpened = 0;
        }

        private String ComposeKey()
        {
            var result = countryCode;

            if (gameMode.ID == GameMode.CountryCapitals.ID)
            {
                result += "-Capitals";
            }

            return result;
        }

        private void SetAnswerLetters(string localizedName)
        {
            Random r = new Random();

            for (int i = 0; i < 16; i++)
            {
                Button b = (Button)this.FindName(buttonNames[i]);
                b.IsEnabled = true;

                if (i < localizedName.Length)
                {
                    b.Content = localizedName[i].ToString().ToUpper();
                }
                else
                {
                    switch (countryCode)
                    {
                        case "RU":
                            b.Content = Convert.ToChar(r.Next(1040, 1071)).ToString().ToUpper();
                            break;
                        case "EN":
                        case "DE":
                        case "FR":
                            b.Content = Convert.ToChar(r.Next(65, 91)).ToString().ToUpper();
                            break;
                        default:
                            break;
                    }

                }
            }

            for (int i = 0; i < 100; i++)
            {
                var firstIndex = r.Next(16);
                int secondIndex;
                do
                {
                    secondIndex = r.Next(16);
                } while (secondIndex == firstIndex);

                var swap = ((Button)this.FindName(buttonNames[firstIndex])).Content;
                ((Button)this.FindName(buttonNames[firstIndex])).Content = ((Button)this.FindName(buttonNames[secondIndex])).Content;
                ((Button)this.FindName(buttonNames[secondIndex])).Content = swap;
            }
        }

        private void SetEmptyAnswer(string localizedName)
        {
            answer.Children.Clear();
            int i = 0;
            foreach (var c in localizedName)
            {
                Button newText = new Button();
                newText.Name = "a" + i.ToString();
                newText.FontSize = 12;
                newText.Content = "*";
                newText.MinWidth = 18;
                newText.Width = 18;
                newText.MinHeight = 50;
                newText.Padding = new Thickness(2);
                newText.Margin = new Thickness(2);                
                answer.Children.Add(newText);
                i++;
            }
        }

        private async Task LoadFlags()
        {
            Flags = new FlagsInfo();

            string IndexFile = await getFileString("CountryNames\\Index.txt");
            List<string> filenames = IndexFile.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var filename in filenames)
            {
                Flags.list.Add(new FlagInfo() { filename = filename, localizedNames = new Dictionary<string, string>() });
            }

            LoadCountryNamesFromString("RU", await getFileString("CountryNames\\RU.txt"), filenames);
            LoadCountryNamesFromString("RU-Capitals", await getFileString("CountryNames\\RU-Capitals.txt"), filenames);

            LoadCountryNamesFromString("EN", await getFileString("CountryNames\\EN.txt"), filenames);
            LoadCountryNamesFromString("EN-Capitals", await getFileString("CountryNames\\EN-Capitals.txt"), filenames);

            LoadCountryNamesFromString("DE", await getFileString("CountryNames\\DE.txt"), filenames);
            LoadCountryNamesFromString("DE-Capitals", await getFileString("CountryNames\\DE-Capitals.txt"), filenames);

            LoadCountryNamesFromString("FR", await getFileString("CountryNames\\FR.txt"), filenames);
            LoadCountryNamesFromString("FR-Capitals", await getFileString("CountryNames\\FR-Capitals.txt"), filenames);
        }

        private async Task<String> getFileString(string filename)
        {
            string result;

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///" + filename));

            using (StreamReader sRead = new StreamReader(await file.OpenStreamForReadAsync()))
            {
                result = await sRead.ReadToEndAsync();
            }

            return result;
        }


        private void LoadCountryNamesFromString(string countryCode, string namesFile, List<string> filenames)
        {
            List<string> localizedNames = namesFile.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < filenames.Count; i++)
            {
                if (i < localizedNames.Count)
                {
                    Flags.list.FirstOrDefault(fn => fn.filename == filenames[i]).localizedNames.Add(countryCode, localizedNames[i]);
                }
            }
        }
                

        private void LetterButtonClicked(object sender, RoutedEventArgs e)
        {
            juststartedGame = false;

            var button = (sender as Button);

            var buttonLetter = button.Content.ToString();

            var answerString = Flags.list[flagIndex].localizedNames[ComposeKey()].ToUpper();

            var nextLetterIndex = 0;

            if ((answer.Children[nextLetterIndex] as Button).Content.ToString() != "*")
            {
                do
                {
                    nextLetterIndex++;
                } while (nextLetterIndex < answerString.Length && (answer.Children[nextLetterIndex] as Button).Content.ToString() != "*");
            }

            if (nextLetterIndex >= answerString.Length)
            {
                return;
            }

            var answerLetter = (Button)answer.Children[nextLetterIndex];

            if (buttonLetter == answerString[nextLetterIndex].ToString())
            {
                answerLetter.Content = buttonLetter;
                button.IsEnabled = false;
                lettersOpened++;
            }
            else
            {
                if (healthStack.Children.Count > 0)
                {
                    healthStack.Children.RemoveAt(lives - 1);
                    lives--;

                    if (lives == 0)
                    {
                        RestartGame(gameMode, initialLivesCount);
                    }
                }
                return;
            }


            if (lettersOpened == answerString.Length)
            {
                FlagsSolved++;
                ShowQuestion();
            }
        }

        private void Countries_Click(object sender, RoutedEventArgs e)
        {
            RestartGame(GameMode.CountryNames, initialLivesCount);
        }

        private void Capitals_Click(object sender, RoutedEventArgs e)
        {
            RestartGame(GameMode.CountryCapitals, initialLivesCount);
        }

        private async void RestartGame(GameMode mode, int livesCount)
        {
            if (!juststartedGame)
            {
                //Show Result
                MessageDialog msgbox = new MessageDialog("Ваш результат = " + FlagsSolved);
                await msgbox.ShowAsync();
            }

            gameMode = mode;
            justStarted = true;
            lives = livesCount;
            FlagsSolved = 0;
            
            SetHealth();
            ShowQuestion();
        }

        private void RU_Click(object sender, RoutedEventArgs e)
        {
            countryCode = "RU";
            RestartGame(gameMode, initialLivesCount);
        }

        private void EN_Click(object sender, RoutedEventArgs e)
        {
            countryCode = "EN";
            RestartGame(gameMode, initialLivesCount);
        }

        private void DE_Click(object sender, RoutedEventArgs e)
        {
            countryCode = "DE";
            RestartGame(gameMode, initialLivesCount);
        }

        private void FR_Click(object sender, RoutedEventArgs e)
        {
            countryCode = "FR";
            RestartGame(gameMode, initialLivesCount);
        }        
    }
}
