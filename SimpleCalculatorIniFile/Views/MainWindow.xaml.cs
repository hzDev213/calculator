using SimpleCalculatorIniFile.ViewModels;
using SimpleCalculatorMVVMIniFile.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SimpleCalculatorIniFile
{
    public partial class MainWindow : Window
    {
        private bool _pKeyPrevious = false;
        private bool _isDarkTheme = true;
        private MainWindowViewModel _viewModel;
        private readonly Dictionary<Key, (ICommand Command, object? Parameter)> _keyMappings;

        private readonly MediaPlayer _regularSound = new MediaPlayer();
        private readonly MediaPlayer _equalsSound = new MediaPlayer();

        // Текущий загруженный конфиг (null = используются встроенные значения)
        private AppConfig? _config;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainWindowViewModel)DataContext;

            // загружаем конфиг
            _config = AppConfigLoader.Load();
            ApplyWindowConfig();
            ApplyFontConfig();

            // выбираем начальную тему из конфига (или dark по умолчанию)
            string startTheme = _config?.DefaultTheme ?? "dark";
            if (startTheme == "light")
                ApplyLightTheme();
            else
                ApplyDarkTheme();

            _keyMappings = new Dictionary<Key, (ICommand, object?)>
            {
                [Key.D0] = (_viewModel.DigitButtonClickCommand, "0"),
                [Key.D1] = (_viewModel.DigitButtonClickCommand, "1"),
                [Key.D2] = (_viewModel.DigitButtonClickCommand, "2"),
                [Key.D3] = (_viewModel.DigitButtonClickCommand, "3"),
                [Key.D4] = (_viewModel.DigitButtonClickCommand, "4"),
                [Key.D5] = (_viewModel.DigitButtonClickCommand, "5"),
                [Key.D6] = (_viewModel.DigitButtonClickCommand, "6"),
                [Key.D7] = (_viewModel.DigitButtonClickCommand, "7"),
                [Key.D8] = (_viewModel.DigitButtonClickCommand, "8"),
                [Key.D9] = (_viewModel.DigitButtonClickCommand, "9"),
                [Key.NumPad0] = (_viewModel.DigitButtonClickCommand, "0"),
                [Key.NumPad1] = (_viewModel.DigitButtonClickCommand, "1"),
                [Key.NumPad2] = (_viewModel.DigitButtonClickCommand, "2"),
                [Key.NumPad3] = (_viewModel.DigitButtonClickCommand, "3"),
                [Key.NumPad4] = (_viewModel.DigitButtonClickCommand, "4"),
                [Key.NumPad5] = (_viewModel.DigitButtonClickCommand, "5"),
                [Key.NumPad6] = (_viewModel.DigitButtonClickCommand, "6"),
                [Key.NumPad7] = (_viewModel.DigitButtonClickCommand, "7"),
                [Key.NumPad8] = (_viewModel.DigitButtonClickCommand, "8"),
                [Key.NumPad9] = (_viewModel.DigitButtonClickCommand, "9"),
                [Key.C] = (_viewModel.ClearButtonClickCommand, null),
                [Key.Delete] = (_viewModel.ClearButtonClickCommand, null),
                [Key.Escape] = (_viewModel.ClearButtonClickCommand, null),
                [Key.Add] = (_viewModel.OperatorButtonClickCommand, "+"),
                [Key.OemPlus] = (_viewModel.OperatorButtonClickCommand, "+"),
                [Key.Subtract] = (_viewModel.OperatorButtonClickCommand, "-"),
                [Key.OemMinus] = (_viewModel.OperatorButtonClickCommand, "-"),
                [Key.Multiply] = (_viewModel.OperatorButtonClickCommand, "*"),
                [Key.Divide] = (_viewModel.OperatorButtonClickCommand, "/"),
                [Key.OemQuestion] = (_viewModel.OperatorButtonClickCommand, "/"),
                [Key.Decimal] = (_viewModel.PointButtonClickCommand, "."),
                [Key.OemPeriod] = (_viewModel.PointButtonClickCommand, "."),
                [Key.OemComma] = (_viewModel.PointButtonClickCommand, "."),
                [Key.Enter] = (_viewModel.EqualsButtonClickCommand, null),
                [Key.Return] = (_viewModel.EqualsButtonClickCommand, null),
                [Key.Back] = (_viewModel.DeleteLastSymbolButtonClickCommand, null),
            };

            this.PreviewKeyDown += OnKeyDown;

            _regularSound.Open(new Uri("Resources/Sounds/regular_button_click.mp3", UriKind.Relative));
            _equalsSound.Open(new Uri("Resources/Sounds/equals_button_click.mp3", UriKind.Relative));

            this.PreviewMouseLeftButtonDown += OnMouseButtonDown;
        }

        private void ApplyWindowConfig()
        {
            if (_config?.Window is not { } wc) return;

            this.Width = wc.Width;
            this.Height = wc.Height;
            this.MinWidth = wc.MinWidth;
            this.MinHeight = wc.MinHeight;
            this.Title = wc.Title;
        }

        private void ApplyFontConfig()
        {
            if (_config?.Fonts is not { } fc) return;

            Resources["ButtonFontFamily"] = new FontFamily(fc.ButtonFontFamily);
            Resources["ButtonFontSize"] = fc.ButtonFontSize;
            Resources["DisplayFontSize"] = fc.DisplayFontSize;
            Resources["HistoryFontSize"] = fc.HistoryFontSize;
            Resources["MenuFontSize"] = fc.MenuFontSize;
        }

        private void PlayRegularSound()
        {
            _regularSound.Position = TimeSpan.Zero;
            _regularSound.Play();
        }

        private void PlayEqualsSound()
        {
            _equalsSound.Position = TimeSpan.Zero;
            _equalsSound.Play();
        }

        private void OnMouseButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var button = FindParentButton(e.OriginalSource as DependencyObject);
            if (button == null) return;

            if (button.Style == (Style)FindResource("EqualsButton"))
                PlayEqualsSound();
            else
                PlayRegularSound();
        }

        private static Button? FindParentButton(DependencyObject? element)
        {
            while (element != null)
            {
                if (element is Button btn) return btn;
                element = VisualTreeHelper.GetParent(element);
            }
            return null;
        }

        private void DarkThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isDarkTheme) return;
            ApplyDarkTheme();
            MenuToggleButton.IsChecked = false;
        }

        private void LightThemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isDarkTheme) return;
            ApplyLightTheme();
            MenuToggleButton.IsChecked = false;
        }

        private void ApplyDarkTheme()
        {
            _isDarkTheme = true;

            if (_config?.Themes?.TryGetValue("dark", out var theme) == true)
                ApplyThemeColors(theme.Colors);
            else
                ApplyBuiltinDarkColors();

            DarkThemeCheck.Visibility = Visibility.Visible;
            LightThemeCheck.Visibility = Visibility.Collapsed;
        }

        private void ApplyLightTheme()
        {
            _isDarkTheme = false;

            if (_config?.Themes?.TryGetValue("light", out var theme) == true)
                ApplyThemeColors(theme.Colors);
            else
                ApplyBuiltinLightColors();

            DarkThemeCheck.Visibility = Visibility.Collapsed;
            LightThemeCheck.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Применяет цвета из словаря конфига.
        /// Для отсутствующих ключей использует встроенные значения соответствующей темы.
        /// </summary>
        private void ApplyThemeColors(Dictionary<string, string> colors)
        {
            // встроенные значения как резервные
            var fallback = _isDarkTheme ? BuiltinDark() : BuiltinLight();

            foreach (var (key, fallbackHex) in fallback)
            {
                string hex = colors.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v)
                    ? v
                    : fallbackHex;
                TrySetColor(key, hex);
            }
        }

        private void ApplyBuiltinDarkColors()
        {
            foreach (var (k, v) in BuiltinDark()) SetColor(k, v);
        }

        private void ApplyBuiltinLightColors()
        {
            foreach (var (k, v) in BuiltinLight()) SetColor(k, v);
        }

        private static Dictionary<string, string> BuiltinDark() => new()
        {
            ["WindowBackgroundColor"] = "#1E1E1E",
            ["DigitButtonBackgroundColor"] = "#2D2D2D",
            ["DigitButtonForegroundColor"] = "#FFFFFF",
            ["OperatorButtonBackgroundColor"] = "#323232",
            ["OperatorButtonForegroundColor"] = "#4CC2FF",
            ["EqualsButtonBackgroundColor"] = "#4CC2FF",
            ["EqualsButtonForegroundColor"] = "#000000",
            ["DisplayForegroundColor"] = "#FFFFFF",
            ["HistoryForegroundColor"] = "#888888",
            ["MenuBackgroundColor"] = "#2A2A2A",
            ["MenuBorderColor"] = "#444444",
            ["MenuItemForegroundColor"] = "#FFFFFF",
            ["ButtonHoverColor"] = "#646464",
            ["ButtonPressedColor"] = "#969696",
            ["MenuHoverColor"] = "#333333",
            ["MenuPressedColor"] = "#444444",
        };

        private static Dictionary<string, string> BuiltinLight() => new()
        {
            ["WindowBackgroundColor"] = "#F5F5F5",
            ["DigitButtonBackgroundColor"] = "#FFFFFF",
            ["DigitButtonForegroundColor"] = "#1E1E1E",
            ["OperatorButtonBackgroundColor"] = "#E0E0E0",
            ["OperatorButtonForegroundColor"] = "#0078D4",
            ["EqualsButtonBackgroundColor"] = "#0078D4",
            ["EqualsButtonForegroundColor"] = "#FFFFFF",
            ["DisplayForegroundColor"] = "#1E1E1E",
            ["HistoryForegroundColor"] = "#666666",
            ["MenuBackgroundColor"] = "#FFFFFF",
            ["MenuBorderColor"] = "#CCCCCC",
            ["MenuItemForegroundColor"] = "#1E1E1E",
            ["ButtonHoverColor"] = "#D0D0D0",
            ["ButtonPressedColor"] = "#B8B8B8",
            ["MenuHoverColor"] = "#EEEEEE",
            ["MenuPressedColor"] = "#DDDDDD",
        };

        private void SetColor(string key, string hex) =>
            Resources[key] = (Color)ColorConverter.ConvertFromString(hex);

        /// <summary>SetColor с обработкой некорректного hex — показывает предупреждение.</summary>
        private void TrySetColor(string key, string hex)
        {
            try { SetColor(key, hex); }
            catch
            {
                MessageBox.Show($"Некорректное значение цвета «{hex}» для параметра «{key}».\n" +
                                    "Значение будет пропущено.",
                                    "Ошибка конфигурации",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            string helpPath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "../../../Resources/HTML/about.html");
            if (System.IO.File.Exists(helpPath))
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo(helpPath) { UseShellExecute = true });
            else
                MessageBox.Show("Файл справки не найден.", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            bool isCtrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            bool isAltPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);
            if (isCtrlPressed || isAltPressed) return;

            if (e.Key == Key.P)
            {
                _pKeyPrevious = true;
                return;
            }

            if (e.Key == Key.I && _pKeyPrevious)
            {
                _viewModel.ConstButtonClickCommand.Execute("π");
                PlayRegularSound();
                _pKeyPrevious = false;
                e.Handled = true;
                return;
            }

            _pKeyPrevious = false;

            if (_keyMappings.TryGetValue(e.Key, out var mapping))
            {
                mapping.Command.Execute(mapping.Parameter);

                if (e.Key == Key.Enter || e.Key == Key.Return)
                    PlayEqualsSound();
                else
                    PlayRegularSound();

                e.Handled = true;
            }
        }
    }

}