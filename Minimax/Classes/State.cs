using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Minimax.Classes
{
    public enum Player
    {
        Min,
        Max
    }

    public class State
    {
        public static List<int> Choices = new List<int>() { 1, 2, 4 };
        public static int FinalStateScore = 20;

        public int CubesOnTable = 0;
        public Player Player;
        public int Evaluation = 0;
        public int CubesRemoved = 0;
        public int Depth = 0;
        public List<State> Childrens = new List<State>();
        public State? Parent;


        /// <summary>
        /// Create a child state with reference to its parent
        /// </summary>
        /// <param name="cubesToRemove"></param>
        /// <returns></returns>
        public void CreateChild(int cubesToRemove)
        {
            try
            {
                if (this.CubesOnTable >= cubesToRemove)
                {
                    State childState = new State();
                    childState.CubesOnTable = CubesOnTable - cubesToRemove;
                    childState.CubesRemoved = cubesToRemove;
                    childState.Depth = Depth + 1;
                    childState.Player = (Player == Player.Max) ? Player.Min : Player.Max;
                    childState.Parent = this;
                    this.Childrens.Add(childState);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Create the childrens of the state
        /// </summary>
        public void CreateChildrens()
        {
            foreach(var choice in State.Choices)
                CreateChild(choice);
        }

        /// <summary>
        /// Evaluate a state. A state is final when there are no cubes on the table.
        /// The score of a final state is twice the initial number of the cubes on the table.
        /// From this score, we remove the number of moves to reach this state in the tree.
        /// This way, the best move from the availables is the winning one with the sorter path.
        /// If the state is not final then its score is the best from its childrens.
        /// </summary>
        public void Evaluate()
        {
            if (CubesOnTable == 0)
            {
                Evaluation = State.FinalStateScore - Depth;

                Evaluation = (Player == Player.Max) ? -1 * Evaluation : 1 * Evaluation;
            }
            else
            {
                CreateChildrens();

                foreach(var child in this.Childrens)
                    child.Evaluate();

                if (Player == Player.Max)
                    Evaluation = this.Childrens.Max(p => p.Evaluation);
                else
                    Evaluation = this.Childrens.Min(p => p.Evaluation);
            }
        }

        /// <summary>
        /// Evaluates a state and returns the best next move
        /// </summary>
        /// <param name="state"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static State? Minimax(State state)
        {
            State? nextBestState = null;

            try
            {
                // if there are no cubes on the table then there is no next move
                if (state.CubesOnTable <= 0)
                    return null;

                // get the childrens of the initial state
                state.CreateChildrens();

                // evaluate childrens
                foreach (var child in state.Childrens)
                    child.Evaluate();

                // choose as next move the one with the better evaluation
                if (state.Player == Player.Max)
                    nextBestState = state.Childrens.OrderByDescending(p => p.Evaluation).FirstOrDefault();
                else
                    nextBestState = state.Childrens.OrderBy(p => p.Evaluation).FirstOrDefault();

                return nextBestState;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
