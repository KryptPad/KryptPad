using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KryptPadCSApp.Models
{
    class GeneratePasswordDialogViewModel : BasePageModel
    {

        #region Properties

        private bool _useUpperLowerCase;

        public bool UseUpperLowerCase
        {
            get { return _useUpperLowerCase; }
            set
            {
                _useUpperLowerCase = value;
                // Notify of change
                OnPropertyChanged(nameof(UseUpperLowerCase));
            }
        }

        private bool _useSymbols;

        public bool UseSymbols
        {
            get { return _useSymbols; }
            set
            {
                _useSymbols = value;
                // Notify of change
                OnPropertyChanged(nameof(UseSymbols));
            }
        }


        private bool _useNumbers;

        public bool UseNumbers
        {
            get { return _useNumbers; }
            set { _useNumbers = value;
                // Notify of change
                OnPropertyChanged(nameof(UseNumbers));
            }
        }


        private int _length;
        public int Length
        {
            get { return _length; }
            protected set
            {
                _length = value;
                // Notify of change
                OnPropertyChanged(nameof(Length));
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                // Notify of change
                OnPropertyChanged(nameof(Password));

            }
        }


        public ICommand GenerateCommand { get; protected set; }

        #endregion

        public GeneratePasswordDialogViewModel()
        {
            // Register commands
            RegisterCommands();

            Length = 12;
        }

        /// <summary>
        /// Registers the commands for the view
        /// </summary>
        private void RegisterCommands()
        {
            GenerateCommand = new Command((p) =>
            {
                // Character buffets from which to choose
                var lower = "abcdefghijklmnopqrstuvwxyz";
                var upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var numbers = "0123456789";
                var symbols = "`~!@#$%^&*()-_=+[]{}\\|;':\",./?";

                // Build a string of our character buckets
                var buffet = lower
                    + (UseUpperLowerCase ? upper : "")
                    + (UseNumbers ? numbers : "")
                    + (UseSymbols ? symbols : "");

                // Build the regex
                var lowerReg = @"(?=.*[a-z])";
                var upperReg = @"(?=.*[A-Z])";
                var numberReg = @"(?=.*\d)";
                var symbolReg = @"(?=.*\W)";

                var rexParts = lowerReg
                    + (UseUpperLowerCase ? upperReg : "")
                    + (UseNumbers ? numberReg : "")
                    + (UseSymbols ? symbolReg : "");

                var reg = new Regex($"^{rexParts}(?!.*\\s).{{8,32}}$");


                var accepted = false;

                while (!accepted)
                {
                    // Shuffle so the characters are mixed around real nice
                    buffet = Shuffle(buffet);

                    var rnd = new Random();
                    var result = "";
                    var resultLength = 0;

                    // The default group is lower, that will always be there
                    while (resultLength < Length)
                    {
                        // Get next random character from buffet
                        var index = rnd.Next(0, buffet.Length);
                        // Character
                        var c = buffet[index];

                        // Add character to result
                        result += c;
                        resultLength++;
                    }

                    // Check against the regex
                    accepted = reg.IsMatch(result);

                    if (accepted)
                    {
                        // Finally!
                        Password = result;
                    }
                }



            });
        }

        /// <summary>
        /// Shuffle a string using the Fisher Yates algorithm
        /// </summary>
        /// <param name="array"></param>
        static string Shuffle(string s)
        {
            char[] array = s.ToCharArray();
            var rnd = new Random();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }

            return new string(array);

        }

    }
}
