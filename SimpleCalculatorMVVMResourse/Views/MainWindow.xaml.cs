using SimpleCalculatorMVVMIniFile.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SimpleCalculatorMVVMIniFile
{
    public partial class MainWindow : Window
    {
        private bool _pKeyPrevious = false;
        private bool _isDarkTheme = true;
        private MainWindowViewModel _viewModel;
        private readonly Dictionary<Key, (ICommand Command, object? Parameter)> _keyMappings;

        private readonly MediaPlayer _regularSound = new MediaPlayer();
        private readonly MediaPlayer _equalsSound = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainWindowViewModel)DataContext;

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
            SetColor("WindowBackgroundColor", "#1E1E1E");
            SetColor("DigitButtonBackgroundColor", "#2D2D2D");
            SetColor("DigitButtonForegroundColor", "#FFFFFF");
            SetColor("OperatorButtonBackgroundColor", "#323232");
            SetColor("OperatorButtonForegroundColor", "#4CC2FF");
            SetColor("EqualsButtonBackgroundColor", "#4CC2FF");
            SetColor("EqualsButtonForegroundColor", "#000000");
            SetColor("DisplayForegroundColor", "#FFFFFF");
            SetColor("HistoryForegroundColor", "#888888");
            SetColor("MenuBackgroundColor", "#2A2A2A");
            SetColor("MenuBorderColor", "#444444");
            SetColor("MenuItemForegroundColor", "#FFFFFF");
            SetColor("ButtonHoverColor", "#646464");
            SetColor("ButtonPressedColor", "#969696");
            SetColor("MenuHoverColor", "#333333");
            SetColor("MenuPressedColor", "#444444");
            DarkThemeCheck.Visibility = Visibility.Visible;
            LightThemeCheck.Visibility = Visibility.Collapsed;
        }

        private void ApplyLightTheme()
        {
            _isDarkTheme = false;
            SetColor("WindowBackgroundColor", "#F5F5F5");
            SetColor("DigitButtonBackgroundColor", "#FFFFFF");
            SetColor("DigitButtonForegroundColor", "#1E1E1E");
            SetColor("OperatorButtonBackgroundColor", "#E0E0E0");
            SetColor("OperatorButtonForegroundColor", "#0078D4");
            SetColor("EqualsButtonBackgroundColor", "#0078D4");
            SetColor("EqualsButtonForegroundColor", "#FFFFFF");
            SetColor("DisplayForegroundColor", "#1E1E1E");
            SetColor("HistoryForegroundColor", "#666666");
            SetColor("MenuBackgroundColor", "#FFFFFF");
            SetColor("MenuBorderColor", "#CCCCCC");
            SetColor("MenuItemForegroundColor", "#1E1E1E");
            SetColor("ButtonHoverColor", "#D0D0D0");
            SetColor("ButtonPressedColor", "#B8B8B8");
            SetColor("MenuHoverColor", "#EEEEEE");
            SetColor("MenuPressedColor", "#DDDDDD");
            DarkThemeCheck.Visibility = Visibility.Collapsed;
            LightThemeCheck.Visibility = Visibility.Visible;
        }

        private void SetColor(string key, string hex)
        {
            Resources[key] = (Color)ColorConverter.ConvertFromString(hex);
        }

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            string helpPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Resources/HTML/about.html");
            if (System.IO.File.Exists(helpPath))
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(helpPath) { UseShellExecute = true });
            else
                MessageBox.Show("Файл справки не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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