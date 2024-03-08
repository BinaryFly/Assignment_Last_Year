/*
jeroen visser 0952491
*/
namespace Support
{
    class MenuItem
    {
        public string name;
        public Action action;

        public MenuItem(string name, Action action)
        {
            this.name = name;
            this.action = action;
        }
    }


    class Menu
    {
        private static Stack<int> DemoListChoices = new Stack<int>(new int[] { 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1 });
        private List<MenuItem> options;

        protected List<MenuItem> Options { get => options; set => options = value; }

        public Menu(List<MenuItem> options)
        {
            this.options = options;
        }

        public void Prompt(string prompt)
        {
            if (Options.Count == 0)
            {
                Console.WriteLine("No options to choose from");
                return;
            }

            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine(prompt);
            Show();
            Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++");

            string chosenOption;
            if (Constants.DEBUG && DemoListChoices.Count > 0)
            {
                Console.WriteLine(DemoListChoices.Peek());
                chosenOption = DemoListChoices.Pop().ToString();
            }
            else
            {
                chosenOption = ReadOption();
            }

            while (!ChosenOptionIsValid(chosenOption))
            {
                Console.Clear();
                Console.WriteLine($"{chosenOption} is not a valid option, please try again\n");
                Show();
                Console.WriteLine("Reading option...");
                chosenOption = ReadOption();
            }
            ExecuteMenuItem(chosenOption);
        }

        private void Show()
        {
            var menu = this.Options.Select((item, index) => $"[{index + 1}]: {item.name}").Aggregate((currentMenu, nextItem) => currentMenu + "\n" + nextItem);
            Console.WriteLine(menu);
        }

        private string ReadOption()
        {
            Console.Write("Please input your choice: ");
            var chosenOption = Console.ReadLine();
            return chosenOption == null ? "" : chosenOption;
        }

        private bool ChosenOptionIsValid(string option)
        {
            int parsedOption;
            var optionIsInt = int.TryParse(option, out parsedOption);
            return optionIsInt && parsedOption <= Options.Count;
        }

        private void ExecuteMenuItem(string chosenOption)
        {
            MenuItem chosenItem = GetMenuItem(chosenOption);
            chosenItem.action();
        }

        private MenuItem GetMenuItem(string index)
        {
            return this.Options[int.Parse(index) - 1];
        }

        public void AddOption(MenuItem option)
        {
            this.Options.Add(option);
        }
    }

    class CardMenu<CardType> : Menu where CardType : Card
    {
        public CardMenu(IEnumerable<CardType> cards, Action<CardType> action) : base(
                cards.Select((card) => new MenuItem(card.ToString(), () => { action(card); })).ToList())
        { }
    }

    class TargetMenu : Menu
    {
        public TargetMenu(List<Target> targets, Action<Target> action) : base(
                targets.Select((target) => new MenuItem(target.GetName(), () => { action(target); })).ToList())
        { }
    }

    class YesNoMenu : Menu
    {
        public YesNoMenu(Action yesAction, Action noAction) : base(
                new List<MenuItem>() { new MenuItem("Yes", yesAction), new MenuItem("No", noAction) })
        { }
    }
}
