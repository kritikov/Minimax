using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Minimax.Classes
{
    public enum Player
    {
        Min,
        Max
    }

    public enum EvaluationType
    {
        Full = 1,
        Limited = 2
    }

    public class State
    {
        public static List<int> Choices = new List<int>() { 1, 2, 4 };
        public static int ScoreFactor = 20;
        public static EvaluationType EvaluationType = EvaluationType.Full;
        public static int EvaluationDepth = 5;

        public int CubesOnTable { get; set; } = 0;
        public Player Player { get; set; }
        public int Score { get; set; } = 0;
        public int CubesRemoved { get; set; } = 0;
        public int Depth { get; set; } = 0;
        public List<State> Childrens { get; set; } = new List<State>();
        public State? Parent { get; set; }


        /// <summary>
        /// Create a child state with reference to its parent
        /// </summary>
        /// <param name="cubesToRemove"></param>
        /// <returns></returns>
        public void CreateChildren(int cubesToRemove)
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
                CreateChildren(choice);
        }

        /// <summary>
        /// Evaluate the score of a state searching the whole tree. A state is final when there are no cubes on the table.
        /// The score of a final state is twice the initial number of the cubes on the table minus its depth in the tree
        /// This way, the best move from the availables is the winning one with the sorter path.
        /// If the state is not final then its score is the best from its childrens.
        /// </summary>
        public void Evaluate(CancellationToken? cancellationToken = null)
        {
            try
            {
				// If the state is final state then evaluate it directly
				if (CubesOnTable == 0)
				{
                    Score = 2;
                    Score = State.ScoreFactor - Depth + Score;
					Score = (Player == Player.Max) ? -1 * Score : 1 * Score;
				}
                else if (State.EvaluationType == EvaluationType.Limited && this.Depth == State.EvaluationDepth)
                {
                    if (State.Choices.Contains(CubesOnTable))
                    {
						Score = State.ScoreFactor - Depth + 2;
						Score = (Player == Player.Max) ? 1 * Score : -1 * Score;
					}
					else if (CubesOnTable - 1 == 3 || CubesOnTable - 2 == 3 || CubesOnTable - State.Choices[2] == 3)
                    {
						Score = State.ScoreFactor - Depth + 1;
						Score = (Player == Player.Max) ? 1 * Score : -1 * Score;
					}
					else
                        Score = 0;

                }
                else
				{
					// stop the process if the user has cancel it
					cancellationToken?.ThrowIfCancellationRequested();

					// We create recursive all the childrens only when we evaluate a state. 
					CreateChildrens();

					// evaluate the score of the childrens
					foreach (var child in this.Childrens)
						child.Evaluate(cancellationToken);

					// evaluate the score of the current state by choosing its best children
					if (Player == Player.Max)
						Score = this.Childrens.Max(p => p.Score);
					else
						Score = this.Childrens.Min(p => p.Score);
                }
			}
			catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Evaluates a state and returns the best next move
        /// </summary>
        /// <param name="state"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static State? Minimax(State state, CancellationToken? cancellationToken = null)
        {
            State? nextBestState = null;

            try
            {
                // if there are no cubes on the table then there is no next move
                if (state.CubesOnTable <= 0)
                    return null;

                // get the childrens of the state
                state.CreateChildrens();

                // evaluate its childrens
                foreach (var child in state.Childrens)
                    child.Evaluate(cancellationToken);

                // choose as next move the one with the better evaluation
                if (state.Player == Player.Max)
                    nextBestState = state.Childrens.OrderByDescending(p => p.Score).FirstOrDefault();
                else
                    nextBestState = state.Childrens.OrderBy(p => p.Score).FirstOrDefault();

                return nextBestState;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
