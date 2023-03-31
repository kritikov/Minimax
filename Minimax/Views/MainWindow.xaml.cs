using Minimax.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Minimax.Views.MainWindow;

namespace Minimax.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region VARIABLES AND NESTED CLASSES

        public enum GameType
        {
            AIvsPlayer,
            AIvsAI
        }

        private string message = "";
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Message"));
            }
        }

        private int m = 10;
        public int M
        {
            get
            {
                return m;
            }
            set
            {
                m = value;
                State.FinalStateScore = M * 2;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(M)));
            }
        }

        private int k = 4;
        public int K
        {
            get
            {
                return k;
            }
            set
            {
                k = value;
                State.Choices[2] = k;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(K)));
            }
        }

        private int cubesOnTable = 0;
        public int CubesOnTable
        {
            get
            {
                return cubesOnTable;
            }
            set
            {
                cubesOnTable = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CubesOnTable)));
            }
        }

        private bool gameIsRunning = false;
        public bool GameIsRunning
        {
            get
            {
                return gameIsRunning;
            }
            set
            {
                gameIsRunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GameIsRunning)));
            }
        }

        private bool minIsPlaying = false;
        public bool MinIsPlaying
        {
            get
            {
                return minIsPlaying;
            }
            set
            {
                minIsPlaying = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinIsPlaying)));
            }
        }

        public GameType TypeOfGame { get; set; } = GameType.AIvsPlayer;

        public string MaxName
        {
            get
            {
                if (TypeOfGame == GameType.AIvsPlayer)
                    return "AI";
                else
                    return "AI 1";
            }
        }

        public string MinName
        {
            get
            {
                if (TypeOfGame == GameType.AIvsPlayer)
                    return "Player";
                else
                    return "AI 2";
            }
        }

        ObservableCollection<string> Moves { get; set; } = new ObservableCollection<string>();

        private CollectionViewSource movesSource = new CollectionViewSource();
        public ICollectionView MovesView
        {
            get
            {
                return this.movesSource.View;
            }
        }

        #endregion


        #region CONSTRUCTORS

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            movesSource.Source = Moves;

            Message = "Set M and K and a game type to play";
        }

        #endregion


        #region EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        private void AIvsPlayer(object sender, RoutedEventArgs e)
        {
            Message = "";

            try
            {
                AIvsPlayer();

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

        }

        private void AIvsAI(object sender, RoutedEventArgs e)
        {
            Message = "";

            try
            {
                AIvsAI();

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            Message = "";

            try
            {
                StopGame();

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        private void Get1Cube(object sender, RoutedEventArgs e)
        {
            Message = "";

            try
            {
                PlayerPlay(1);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        private void Get2Cubes(object sender, RoutedEventArgs e)
        {
            Message = "";

            try
            {
                PlayerPlay(2);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        private void GetKCubes(object sender, RoutedEventArgs e)
        {
            Message = "";

            try
            {
                PlayerPlay(K);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        private void CreateTree(object sender, RoutedEventArgs e)
        {
            Message = "";

            try
            {
                State state = new State();
                state.CubesOnTable = M;
                state.Player = Player.Max;

                CreateTree(state);

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            
        }
        #endregion


        #region METHODS

        /// <summary>
        /// Initialize the game using the given parameters
        /// </summary>
        private void InitializeGame()
        {
            GameIsRunning = true;
            CubesOnTable = M;
            Moves.Clear();
            Moves.Add($"On the table there are {CubesOnTable} cubes");
        }

        /// <summary>
        /// Create a new game AI vs Player
        /// </summary>
        private void AIvsPlayer()
        {
            InitializeGame();
            TypeOfGame = GameType.AIvsPlayer;
            MaxPlay();
        }

        /// <summary>
        /// Create a new game AI vs AI
        /// </summary>
        private void AIvsAI()
        {
            InitializeGame();
            TypeOfGame = GameType.AIvsAI;
            MaxPlay();
        }

        /// <summary>
        /// Stop the current game
        /// </summary>
        private void StopGame()
        {
            Message = "Set M and K and a game type to play";
            GameIsRunning = false;
        }

        /// <summary>
        /// Max decides and makes a move
        /// </summary>
        private void MaxPlay()
        {

            if (cubesOnTable > 0)
            {
                State state = new State();
                state.CubesOnTable = CubesOnTable;
                state.Player = Player.Max;
                State nextBextState = State.Minimax(state);

                Moves.Add($"{MaxName} gets {nextBextState.CubesRemoved} cubes from the table");
                CubesOnTable -= nextBextState.CubesRemoved;
                Moves.Add($"On the table there are {CubesOnTable} cubes");
            }

            // check if max wins
            if (cubesOnTable <= 0)
            {
                Moves.Add($"{MaxName} won the game!");
                StopGame();
            }
            else
            {
                if (TypeOfGame == GameType.AIvsPlayer)
                {
                    Moves.Add($"Press a button to select how many cuber to get from the table...");
                }
                else
                {
                    MinPlay();
                }
            }
        }
        
        /// <summary>
        /// Min decides and makes a move
        /// </summary>
        private void MinPlay()
        {
            if (cubesOnTable > 0)
            {
                State state = new State();
                state.CubesOnTable = CubesOnTable;
                state.Player = Player.Min;
                State nextBextState = State.Minimax(state);

                Moves.Add($"{MinName} gets {nextBextState.CubesRemoved} cubes from the table");
                CubesOnTable -= nextBextState.CubesRemoved;
                Moves.Add($"On the table there are {CubesOnTable} cubes");
            }

            // check if min wins
            if (cubesOnTable <= 0)
            {
                Moves.Add($"{MinName} won the game!");
                StopGame();
            }
            else
            {
                MaxPlay();
            }
        }
        
        /// <summary>
        /// Min chooses and makes a move
        /// </summary>
        /// <param name="cubesToRemove"></param>
        private void PlayerPlay(int cubesToRemove)
        {
            if (CubesOnTable >= cubesToRemove)
            {
                CubesOnTable -= cubesToRemove;
                Moves.Add($"{MinName} gets {cubesToRemove} cubes from the table");

                // check if min wins
                if (cubesOnTable <= 0)
                {
                    Moves.Add($"{MinName} won the game!");
                    StopGame();
                }
                else
                {
                    MaxPlay();
                }
            }
            else
            {
                Moves.Add($"You cant get {cubesToRemove} cubes from the table, choose again...");
            }
        }

        private void CreateTree(State state)
        {
            try
            {
                if (state.CubesOnTable > 0)
                {
                    // get the childrens of the initial state
                    state.CreateChildrens();

                    // evaluate childrens
                    foreach (var child in state.Childrens)
                        child.Evaluate();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion




    }
}
