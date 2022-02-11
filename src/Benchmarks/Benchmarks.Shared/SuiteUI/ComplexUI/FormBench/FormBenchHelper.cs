using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Benchmarks.Shared.SuiteUI.ComplexUI.FormBench
{
    internal static class FormBenchHelper
    {
        internal static Grid MakeGrid()
        {
            var nameLabel = new TextBlock() { Text = "Name: " };
            Grid.SetRow(nameLabel, 0);
            Grid.SetColumn(nameLabel, 0);

            var emailLabel = new TextBlock() { Text = "Email: " };
            Grid.SetRow(emailLabel, 1);
            Grid.SetColumn(emailLabel, 0);

            var ageLabel = new TextBlock() { Text = "Age: " };
            Grid.SetRow(ageLabel, 2);
            Grid.SetColumn(ageLabel, 0);

            var occupationLabel = new TextBlock() { Text = "Occupation: " };
            Grid.SetRow(occupationLabel, 3);
            Grid.SetColumn(occupationLabel, 0);

            var notificationsLabel = new TextBlock() { Text = "Notifications: " };
            Grid.SetRow(notificationsLabel, 4);
            Grid.SetColumn(notificationsLabel, 0);

            var name = new TextBox();
            Grid.SetRow(name, 0);
            Grid.SetColumn(name, 1);

            var email = new TextBox();
            Grid.SetRow(email, 1);
            Grid.SetColumn(email, 1);

            var age = new NumberBox();
            Grid.SetRow(age, 2);
            Grid.SetColumn(age, 1);

            var occupation = new ComboBox()
            {
                ItemsSource = new[] { "Developer", "IT", "QA" },
                SelectedIndex = 0
            };
            Grid.SetRow(occupation, 3);
            Grid.SetColumn(occupation, 1);

            var notifications = new CheckBox();
            Grid.SetRow(notifications, 4);
            Grid.SetColumn(notifications, 1);

            var save = new Button() { Content = "Save changes" };
            Grid.SetRow(save, 5);
            Grid.SetColumn(save, 1);

            return new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = new GridLength(100d, GridUnitType.Pixel) },
                    new ColumnDefinition() { Width = new GridLength(150d, GridUnitType.Pixel) }
                },
                RowDefinitions =
                {
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto },
                    new RowDefinition() { Height = GridLength.Auto }
                },
                Children =
                {
                    nameLabel,
                    emailLabel,
                    ageLabel,
                    occupationLabel,
                    notificationsLabel,
                    name,
                    email,
                    age,
                    occupation,
                    notifications,
                    save
                }
            };
        }
    }
}