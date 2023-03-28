﻿using Minimax.Classes;
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

namespace Minimax.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region VARIABLES AND NESTED CLASSES

        public enum Player
        {
            Max,
            Min
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

            Message = "Set M and K and press 'start game' to play";
        }

        #endregion


        #region EVENTS

        public event PropertyChangedEventHandler PropertyChanged;

        private void Start(object sender, RoutedEventArgs e)
        {
            Message = "";

            try
            {
                StartGame();

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
            MinPlay(1);
        }

        private void Get2Cubes(object sender, RoutedEventArgs e)
        {
            MinPlay(2);
        }

        private void GetKCubes(object sender, RoutedEventArgs e)
        {
            MinPlay(K);
        }

        #endregion


        #region METHODS

        private void StartGame()
        {
            GameIsRunning = true;
            CubesOnTable = M;
            Moves.Clear();
            MaxPlay();
        }

        private void StopGame()
        {
            Message = "Set M and K and press 'start game' to play";

            GameIsRunning = false;
        }

        private void MaxPlay()
        {
            Moves.Add($"On the table there are {CubesOnTable} cubes");

            // run minimax algorith
            State state = new State();
            state.CubesOnTable = CubesOnTable;
            State.Minimax(state, k);

            if (state.BestNextState == null)
            {

            }


            Moves.Add($"Max is playing and gets {1} cubes from the table");
            CubesOnTable--;
            Moves.Add($"On the table there are {CubesOnTable} cubes");

            // check if max wins
            if (cubesOnTable <= 0)
            {
                Moves.Add($"Max won the game!");
                StopGame();
            }
            else
            {
                // ask player to make a choice
                Moves.Add($"Press a button to select how many cuber to get from the table...");
            }
        }
        
        private void MinPlay(int cubes)
        {

            Moves.Add($"Min gets {cubes} cubes from the table");

            CubesOnTable -= cubes;

            // check if min wins
            if (cubesOnTable <= 0)
            {
                Moves.Add($"Min won the game!");
                StopGame();
            }
            else
            {
                MaxPlay();
            }
        }


        #endregion

        

    }
}
