namespace Benchmarks.Maui.SuiteUI.ComplexUI.FormBench
{
    internal static class FormBenchHelper
    {
        internal static Grid MakeGrid()
        {
            var nameLabel = new Label() { Text = "Name: " };
            Grid.SetRow(nameLabel, 0);
            Grid.SetColumn(nameLabel, 0);

            var emailLabel = new Label() { Text = "Email: " };
            Grid.SetRow(emailLabel, 1);
            Grid.SetColumn(emailLabel, 0);

            var ageLabel = new Label() { Text = "Age: " };
            Grid.SetRow(ageLabel, 2);
            Grid.SetColumn(ageLabel, 0);

            var occupationLabel = new Label() { Text = "Occupation: " };
            Grid.SetRow(occupationLabel, 3);
            Grid.SetColumn(occupationLabel, 0);

            var notificationsLabel = new Label() { Text = "Notifications: " };
            Grid.SetRow(notificationsLabel, 4);
            Grid.SetColumn(notificationsLabel, 0);

            var name = new Entry();
            Grid.SetRow(name, 0);
            Grid.SetColumn(name, 1);

            var email = new Entry();
            Grid.SetRow(email, 1);
            Grid.SetColumn(email, 1);

            var age = new Entry();
            age.Keyboard = Keyboard.Numeric;
            Grid.SetRow(age, 2);
            Grid.SetColumn(age, 1);

            var occupation = new Picker()
            {
                ItemsSource = new[] { "Developer", "IT", "QA" },
                SelectedIndex = 0
            };
            Grid.SetRow(occupation, 3);
            Grid.SetColumn(occupation, 1);

            var notifications = new CheckBox();
            Grid.SetRow(notifications, 4);
            Grid.SetColumn(notifications, 1);

            var save = new Button() { Text = "Save changes" };
            Grid.SetRow(save, 5);
            Grid.SetColumn(save, 1);

            return new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = new GridLength(100d, GridUnitType.Absolute) },
                    new ColumnDefinition() { Width = new GridLength(150d, GridUnitType.Absolute) }
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