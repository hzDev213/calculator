using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SimpleCalculatorLibs.ViewModels;

namespace SimpleCalculatorLibs.Views
{
    public partial class MainWindow : Window
    {
        // Объявляем плееры для звуков, чтобы методы Play...Sound не выдавали ошибку компиляции
        private readonly MediaPlayer _regularSound = new MediaPlayer();
        private readonly MediaPlayer _equalsSound = new MediaPlayer();
        private bool _piPending = false;

        public MainWindow()
        {
            InitializeComponent();
            
            // 1. Сначала применяем тему по умолчанию (например, Темную или Светлую), 
            // чтобы XAML сразу подтянул динамические ресурсы
            ApplyTheme("Dark");
            ApplyCursor("white");

            // 2. Инициализируем звуки (укажите правильные пути к вашим wav/mp3 файлам)
            InitializeSounds();

            // 3. Подписываемся на клавиатуру
            this.KeyDown += MainWindow_KeyDown;
        }

        private void InitializeSounds()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                
                // Пропишите здесь реальные имена ваших звуковых файлов
                string regularSoundPath = Path.Combine(baseDir, "Resources", "Sounds", "click.wav");
                string equalsSoundPath = Path.Combine(baseDir, "Resources", "Sounds", "equals.wav");

                if (File.Exists(regularSoundPath)) _regularSound.Open(new Uri(regularSoundPath));
                if (File.Exists(equalsSoundPath)) _equalsSound.Open(new Uri(equalsSoundPath));
            }
            catch { /* Подавляем ошибки инициализации аудио, чтобы приложение не падало */ }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm == null) return;

            bool isCommandExecuted = true;

            switch (e.Key)
            {
                // Цифры
                case Key.D0: case Key.NumPad0: vm.DigitCommand.Execute("0"); break;
                case Key.D1: case Key.NumPad1: vm.DigitCommand.Execute("1"); break;
                case Key.D2: case Key.NumPad2: vm.DigitCommand.Execute("2"); break;
                case Key.D3: case Key.NumPad3: vm.DigitCommand.Execute("3"); break;
                case Key.D4: case Key.NumPad4: vm.DigitCommand.Execute("4"); break;
                case Key.D5: case Key.NumPad5: vm.DigitCommand.Execute("5"); break;
                case Key.D6: case Key.NumPad6: vm.DigitCommand.Execute("6"); break;
                case Key.D7: case Key.NumPad7: vm.DigitCommand.Execute("7"); break;
                case Key.D8: case Key.NumPad8: vm.DigitCommand.Execute("8"); break;
                case Key.D9: case Key.NumPad9: vm.DigitCommand.Execute("9"); break;

                // Операторы
                case Key.Add:      vm.OperatorCommand.Execute("+"); break;
                case Key.Subtract: vm.OperatorCommand.Execute("-"); break;
                case Key.Multiply: vm.OperatorCommand.Execute("*"); break;
                case Key.Divide:   vm.OperatorCommand.Execute("/"); break;

                // + и - с основной клавиатуры
                case Key.OemPlus:  vm.OperatorCommand.Execute("+"); break;
                case Key.OemMinus: vm.OperatorCommand.Execute("-"); break;

                // Специальные
                case Key.Enter:   
                    vm.EqualsCommand.Execute(null); 
                    PlayEqualsSound(); // Воспроизводим звук "Равно"
                    isCommandExecuted = false; 
                    break;
                case Key.Back:                    vm.BackspaceCommand.Execute(null); break;
                case Key.Delete: case Key.Escape:    vm.ClearCommand.Execute(null);     break;
                case Key.OemPeriod: case Key.Decimal: vm.DecimalCommand.Execute(null);  break;
                
                default:
                    isCommandExecuted = false;
                    break;
            }

            // Если сработала любая обычная кнопка (кроме Enter) — играем клик
            if (isCommandExecuted)
            {
                PlayRegularSound();
            }

            // π — последовательное нажатие P → I
            if (e.Key == Key.P)
            {
                _piPending = true;
            }
            else if (e.Key == Key.I && _piPending)
            {
                vm.PiCommand.Execute(null);
                PlayRegularSound();
                _piPending = false;
            }
            else
            {
                _piPending = false;
            }
        }

        private void ApplyCursor(string theme)
        {
            // Использование Pack URI гарантирует, что курсор загрузится из ресурсов сборки (.exe/.dll)
            // и не сломается при переносе программы на другой ПК
            try
            {
                string cursorUri = theme == "black"
                    ? "pack://application:,,,/Resources/Cursors/arrow_cursor_black.cur"
                    : "pack://application:,,,/Resources/Cursors/arrow_cursor_white.cur";

                var stream = Application.GetResourceStream(new Uri(cursorUri))?.Stream;
                if (stream != null)
                {
                    this.Cursor = new Cursor(stream);
                }
            }
            catch
            {
                // Если файла курсора физически нет в проекте, оставляем дефолтный курсор Windows
                this.Cursor = Cursors.Arrow;
            }
        }

        private void DarkThemeButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Dark");
            ApplyCursor("white");
            if (DarkThemeCheck != null) DarkThemeCheck.Visibility = Visibility.Visible;
            if (LightThemeCheck != null) LightThemeCheck.Visibility = Visibility.Collapsed;
        }

        private void LightThemeButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Light");
            ApplyCursor("black");
            if (DarkThemeCheck != null) DarkThemeCheck.Visibility = Visibility.Collapsed;
            if (LightThemeCheck != null) LightThemeCheck.Visibility = Visibility.Visible;
        }

        private void ApplyTheme(string theme)
        {
            var res = this.Resources;
            if (theme == "Light")
            {
                res["WindowBackgroundColor"]          = Color.FromRgb(245, 245, 250);
                res["DigitButtonBackgroundColor"]     = Color.FromRgb(255, 255, 255);
                res["DigitButtonForegroundColor"]     = Color.FromRgb(30,  30,  30);
                res["OperatorButtonBackgroundColor"]  = Color.FromRgb(235, 235, 245);
                res["OperatorButtonForegroundColor"]  = Color.FromRgb(99,  102, 241);
                res["EqualsButtonBackgroundColor"]    = Color.FromRgb(99,  102, 241);
                res["EqualsButtonForegroundColor"]    = Color.FromRgb(255, 255, 255);
                res["DisplayForegroundColor"]         = Color.FromRgb(30,  30,  30);
                res["HistoryForegroundColor"]         = Color.FromRgb(150, 150, 160);
                res["ButtonHoverColor"]               = Color.FromRgb(220, 220, 235);
                res["ButtonPressedColor"]             = Color.FromRgb(200, 200, 220);
                res["MenuBackgroundColor"]            = Color.FromRgb(255, 255, 255);
                res["MenuBorderColor"]                = Color.FromRgb(220, 220, 230);
                res["MenuItemForegroundColor"]        = Color.FromRgb(30,  30,  30);
                res["MenuHoverColor"]                 = Color.FromRgb(240, 240, 250);
                res["MenuPressedColor"]               = Color.FromRgb(225, 225, 245);
            }
            else
            {
                res["WindowBackgroundColor"]          = Color.FromRgb(30,  30,  30);
                res["DigitButtonBackgroundColor"]     = Color.FromRgb(45,  45,  45);
                res["DigitButtonForegroundColor"]     = Color.FromRgb(255, 255, 255);
                res["OperatorButtonBackgroundColor"]  = Color.FromRgb(50,  50,  50);
                res["OperatorButtonForegroundColor"]  = Color.FromRgb(76,  194, 255);
                res["EqualsButtonBackgroundColor"]    = Color.FromRgb(76,  194, 255);
                res["EqualsButtonForegroundColor"]    = Color.FromRgb(0,   0,   0);
                res["DisplayForegroundColor"]         = Color.FromRgb(255, 255, 255);
                res["HistoryForegroundColor"]         = Color.FromRgb(136, 136, 136);
                res["ButtonHoverColor"]               = Color.FromRgb(100, 100, 100);
                res["ButtonPressedColor"]             = Color.FromRgb(150, 150, 150);
                res["MenuBackgroundColor"]            = Color.FromRgb(42,  42,  42);
                res["MenuBorderColor"]                = Color.FromRgb(68,  68,  68);
                res["MenuItemForegroundColor"]        = Color.FromRgb(255, 255, 255);
                res["MenuHoverColor"]                 = Color.FromRgb(51,  51,  51);
                res["MenuPressedColor"]               = Color.FromRgb(68,  68,  68);
            }
        }

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "HTML", "about.html");
                if (File.Exists(htmlPath))
                {
                    Process.Start(new ProcessStartInfo(htmlPath) { UseShellExecute = true });
                }
            }
            catch { /* Подавление ошибок открытия браузера */ }
        }

        private void PlayRegularSound()
        {
            try
            {
                _regularSound.Stop();
                _regularSound.Position = TimeSpan.Zero;
                _regularSound.Play();
            }
            catch { }
        }

        private void PlayEqualsSound()
        {
            try
            {
                _equalsSound.Stop();
                _equalsSound.Position = TimeSpan.Zero;
                _equalsSound.Play();
            }
            catch { }
        }
    }
}