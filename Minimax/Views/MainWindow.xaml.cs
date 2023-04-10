using Minimax.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading;
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

        private int m = 12;
        public int M
        {
            get
            {
                return m;
            }
            set
            {
                m = value;
                State.ScoreFactor = m * 2;
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

        public EvaluationType EvaluationType
        {
            get
            {
                return State.EvaluationType;
            }
            set
            {
                State.EvaluationType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EvaluationType)));
            }
        }

        public int EvaluationDepth
        {
            get
            {
                return State.EvaluationDepth;
            }
            set
            {
                State.EvaluationDepth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EvaluationDepth)));
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

        private bool aiIsThinking = false;
        public bool AIIsThinking
        {
            get
            {
                return aiIsThinking;
            }
            set
            {
				aiIsThinking = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AIIsThinking)));
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

        public bool AIFirstEvaluationSpoken = false;
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
                    return "You";
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

        private bool analysisIsRunning = false;
        public bool AnalysisIsRunning
        {
            get
            {
                return analysisIsRunning;
            }
            set
            {
                analysisIsRunning = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnalysisIsRunning)));
            }
        }

        public ObservableCollection<State> Analysis { get; set; } = new ObservableCollection<State>();
        private CollectionViewSource analysisSource = new CollectionViewSource();
        public ICollectionView AnalysisView
        {
            get
            {
                return this.analysisSource.View;
            }
        }

        private CancellationTokenSource analysisCancellToken = new CancellationTokenSource();
        private CancellationTokenSource gameCancellToken = new CancellationTokenSource();

        #endregion


        #region CONSTRUCTORS

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            movesSource.Source = Moves;
            M = 12;
            EvaluationType = EvaluationType.Limited;
            EvaluationDepth = 5;
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
                AIIsThinking = true;
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

                AnalysisIsRunning = true;

                CreateTree(state);

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        private void StopCreatingTree(object sender, RoutedEventArgs e)
        {
            Message = "";

            try
            {
                analysisCancellToken.Cancel();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

		private void DisplayAboutWindow(object sender, RoutedEventArgs e)
		{
			try
			{
				AboutWindow window = new AboutWindow();
				window.ShowDialog();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		#endregion


		#region METHODS

		/// <summary>
		/// do an action in the application main thread
		/// </summary>
		/// <param name="action"></param>
		public void Do(Action action)
		{
			if (Application.Current != null)
			{
				Application.Current.Dispatcher.Invoke(action);
			}
		}

		/// <summary>
		/// Add a message in the moves list
		/// </summary>
		/// <param name="message"></param>
		public void AddLog(string message)
		{
			Do(() => {
				Moves.Add(message);
			});
		}

		/// <summary>
		/// Initialize the game using the given parameters
		/// </summary>
		private void InitializeGame()
        {
            GameIsRunning = true;
            CubesOnTable = M;
            AIFirstEvaluationSpoken = false;
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
			AIIsThinking = false;
		}

        /// <summary>
        /// Max decides and makes a move
        /// </summary>
        private async Task MaxPlay()
        {
			Do(() => {
				AIIsThinking = true;
			});

			try
            {
				MainWindow window = this;

                await Task.Run(() =>
                {
                    if (cubesOnTable > 0)
                    {
						window.AddLog($"{MaxName} is thinking...");

						State state = new State();
                        state.CubesOnTable = CubesOnTable;
                        state.Player = Player.Max;
                        State nextBextState = State.Minimax(state);

                        if (nextBextState?.Score < 0 && EvaluationType == EvaluationType.Full && AIFirstEvaluationSpoken == false)
                        {
                            if(this.TypeOfGame == GameType.AIvsPlayer)
                                window.AddLog($"{MaxName} says: 'If you play good, there is no way i could win this.'");
                            else
                                window.AddLog($"{MaxName} says: 'Ok, i will loose, but i will delay hoping for a unexpected shutdown...'");
                            
                            AIFirstEvaluationSpoken = true;
                        }

                        window.AddLog($"{MaxName} gets {nextBextState.CubesRemoved} cubes from the table");
						CubesOnTable -= nextBextState.CubesRemoved;
						window.AddLog($"On the table there are {CubesOnTable} cubes");
					}

                    // check if max wins
                    if (cubesOnTable <= 0)
                    {
						window.AddLog($"{MaxName} won the game!");
						StopGame();
                    }
                    else
                    {
                        if (TypeOfGame == GameType.AIvsPlayer)
                        {
							window.AddLog($"Press a button to select how many cubes to get from the table...");

							Do(() => {
								AIIsThinking = false;
							});
                        }
                        else
                        {
							Do(() => {
								AIIsThinking = false;
							});
                            
							MinPlay();
                        }
                    }
                });
            }
            catch(Exception ex)
            {

            }
		}
        
        /// <summary>
        /// Min decides and makes a move
        /// </summary>
        private async void MinPlay()
        {
			Do(() => {
				AIIsThinking = true;
			});

            try
            {
				MainWindow window = this;

				await Task.Run(() =>
                {
                    if (cubesOnTable > 0)
                    {
						window.AddLog($"{MinName} is thinking...");

						State state = new State();
                        state.CubesOnTable = CubesOnTable;
                        state.Player = Player.Min;
                        State nextBextState = State.Minimax(state);

						window.AddLog($"{MinName} gets {nextBextState.CubesRemoved} cubes from the table");
						CubesOnTable -= nextBextState.CubesRemoved;
						window.AddLog($"On the table there are {CubesOnTable} cubes");
					}

                    // check if min wins
                    if (cubesOnTable <= 0)
                    {
						window.AddLog($"{MinName} won the game!");
						StopGame();
                    }
                    else
                    {
						Do(() => {
							AIIsThinking = false;
						});
						
                        MaxPlay();
                    }
                });
            }
			catch (Exception ex)
			{

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
				Moves.Add($"On the table there are {CubesOnTable} cubes");

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

        /// <summary>
        /// Create the search tree to view its nodes
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private async Task CreateTree(State state)
        {
            try
            {
                Analysis.Clear();

                if (state.CubesOnTable > 0)
                {
                    analysisCancellToken = new CancellationTokenSource();

                    await Task.Run(() =>
                    {
                        // get the childrens of the initial state
                        state.CreateChildrens();

                        // evaluate childrens
                        foreach (var child in state.Childrens)
                            child.Evaluate(analysisCancellToken.Token);
                    });
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Analysis.Add(state);
                analysisSource.Source = Analysis;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AnalysisView)));

                AnalysisIsRunning = false;

                Message = "Creating tree stopped";
            }
        }


		#endregion

		
	}
}
