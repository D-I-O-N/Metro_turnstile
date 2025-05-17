using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.Caching;
using System.Globalization;



namespace CourseOneWpfApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    class CardMetro
    {
        public string number;
        public string riders;
        public string date;
    }

    public class Analytics
    {
        public int an_inf;
        public int an_num;

        public int count_inf()
        {
            an_inf++;
            return an_num;
        }
        public int count_num()
        {
            an_num++;
            return an_num;
        }
    }

    public class CardAccessManager
    {
        private static readonly MemoryCache _cardCache = MemoryCache.Default;
        private static readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
        };

        // Метод для добавления номера карты в кэш
        public static void BlockCardTemporarily(string cardNumber)
        {
            _cardCache.Add(cardNumber, cardNumber, _cachePolicy);
        }

        // Метод для проверки, заблокирована ли карта
        public static bool IsCardBlocked(string cardNumber)
        {
            return _cardCache.Contains(cardNumber);
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Analytics analytics = new Analytics();
        CardMetro cardMetro_1 = new CardMetro();

        bool sensor = false;

        // Валидация ввода для номера карты
        private void CardNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        // Маска ввода для номера карты
        private void CardNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string text = textBox.Text.Replace("-", "");
            if (text.Length > 24) text = text.Substring(0, 24);

            string formattedText = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (i > 0 && i % 4 == 0)
                {
                    formattedText += "-";
                }
                formattedText += text[i];
            }

            textBox.Text = cardMetro_1.number = formattedText;
            textBox.CaretIndex = formattedText.Length;
        }

        // Валидация ввода для числа поездок
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        // Логика для чекбокса "Безлимит"
        private void UnlimitedRidesCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RidesTextBox.Text = cardMetro_1.riders = "∞";
            RidesTextBox.IsEnabled = false;
        }

        private void UnlimitedRidesCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            RidesTextBox.Text = cardMetro_1.riders = "";
            RidesTextBox.IsEnabled = true;
        }

        // Маска ввода для даты
        private void DateValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string text = textBox.Text.Replace("-", "");
            if (text.Length > 8) text = text.Substring(0, 8);

            string formattedText = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (i == 2 || i == 4)
                {
                    formattedText += "-";
                }
                formattedText += text[i];
            }

            textBox.Text = formattedText;
            textBox.CaretIndex = formattedText.Length;
        }

        // Проверка корректности данных перед нажатием кнопки
        private void ProcessCard_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;
            cardMetro_1.riders = RidesTextBox.Text.Trim();
            cardMetro_1.date = ExpiryDateTextBox.Text;

            // Проверка номера карты
            if (CardNumberTextBox.Text.Length != 29) // 24 цифры + 5 дефисов
            {
                MessageBox.Show("Номер карты должен быть в формате 0000-0000-0000-0000-0000-0000");
                isValid = false;
            }

            // Проверка числа поездок
            if (!UnlimitedRidesCheckBox.IsChecked.Value && (string.IsNullOrEmpty(RidesTextBox.Text) || RidesTextBox.Text == "∞"))
            {
                MessageBox.Show("Введите количество поездок или выберите безлимит");
                isValid = false;
            }

            // Проверка даты
            if (!DateTime.TryParseExact(ExpiryDateTextBox.Text, "dd-MM-yyyy",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime resultDate))
            {
                MessageBox.Show("Дата должна быть в формате дд-мм-гггг и быть действительной");
                isValid = false;
            }
            else if (resultDate.Year < 1000)
            {
                MessageBox.Show("Год должен быть не менее 1000");
                isValid = false;
            }


            if (isValid)
            {
                MessageBox.Show("Данные введены корректно. Ожидайте результат проверки карты после закрытия окна.");
                SensorTriggered(cardMetro_1.number, cardMetro_1.riders, cardMetro_1.date, sensor);
            }
        }

        // Обработчик события "Пройти"
        private async void SensorTriggered(String cardNumber, String riders, String dateStr, bool indicator)
        {
          bool check = checkingDateAndCache(cardNumber, dateStr);
            if (check)
            {
                workProcess(riders, cardNumber, dateStr);
                await Task.Delay(5000);
                StopIndicator.Fill = new SolidColorBrush(Colors.Red);
                PassIndicator.Fill = new SolidColorBrush(Colors.Gray);
            }
            else 
            {
                StopIndicator.Fill = new SolidColorBrush(Colors.Red);
                PassIndicator.Fill = new SolidColorBrush(Colors.Gray);
                MessageBox.Show("Вы уже прошли через турникет использовав карту с ∞ поездками");
            }
        }

        private void workProcess(String riders, String userCardNumber, String dateStr)
        {
            
            DateTime currentDate = DateTime.Today;
            DateTime dateToCompare = DateTime.ParseExact(dateStr, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

            if (dateToCompare >= currentDate)
            {

                if (riders == "∞")
                {
                    if ((dateToCompare >= currentDate)) { analytics.count_inf(); }

                    CardAccessManager.BlockCardTemporarily(userCardNumber);
                    StopIndicator.Fill = new SolidColorBrush(Colors.Gray);
                    PassIndicator.Fill = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    int number = Convert.ToInt32(riders);

                    if (number > 0)
                    {
                        if ((dateToCompare >= currentDate)) { analytics.count_num(); }

                        StopIndicator.Fill = new SolidColorBrush(Colors.Gray);
                        PassIndicator.Fill = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        MessageBox.Show("Error: Количество поездок не может быть <= 0");
                    }
                }
            }
            else 
            {
                MessageBox.Show("Error: Ваша карта просрочена");
            }

        }

        private bool checkingDateAndCache(String cardNumber, String dateStr)
        {
            bool value = false;
            bool isUserBlocked = CardAccessManager.IsCardBlocked(cardNumber);
            DateTime currentDate = DateTime.Now;
            DateTime dateToCompare = DateTime.ParseExact(dateStr, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);

                if ((dateToCompare >= currentDate) | !(isUserBlocked))
                {
                    value = true;
                }
           else
           {
                    StopIndicator.Fill = new SolidColorBrush(Colors.Red);
                    PassIndicator.Fill = new SolidColorBrush(Colors.Gray);
                    value = false;
            }
            return value;
        }

        private void AnalyticsDay(object sender, RoutedEventArgs e)
        {
            int total = analytics.an_inf + analytics.an_num;

            if (total == 0)
            {
                MessageBox.Show(
                    $"Собранная статистика динамики за день {DateTime.Today.ToShortDateString()}" +
                    $"{Environment.NewLine}Нет данных для анализа (обе категории равны 0)",
                    "Статистика турникетов метро"
                );
            }
            else
            {
                MessageBox.Show(
                    $"Собранная статистика динамики за день {DateTime.Today.ToShortDateString()}" +
                    $"{Environment.NewLine}Безлимитных карт: {analytics.an_inf}, что составляет {analytics.an_inf * 100 / total}% от общей динамики" +
                    $"{Environment.NewLine}Обычных: {analytics.an_num}, что составляет {analytics.an_num * 100 / total}% от общей динамики" +
                    $"{Environment.NewLine}Общая динамика: {total}",
                    "Статистика турникетов метро"
                );
            }
        }

    }
}
