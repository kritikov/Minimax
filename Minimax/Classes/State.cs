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
        public int CubesOnTable = 0;
        public Player Player;
        public int evaluation = 0;
        public int CubesRemoved = 0;
        public State? BestNextState;

        public State GetChild(int cubesToRemove)
        {
            try
            {
                State childState = new State();
                childState.CubesOnTable = CubesOnTable - cubesToRemove;
                childState.CubesRemoved = cubesToRemove;

                return childState;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Evaluate(Player player, int k)
        {
            if (CubesOnTable == 1)
            {
                evaluation = 1;
                BestNextState = null;
                CubesRemoved = 1;
            }
            else if (CubesOnTable == 2)
            {
                evaluation = 1;
                BestNextState = null;
                CubesRemoved = 2;
            }
            if (CubesOnTable == k)
            {
                evaluation = 1;
                BestNextState = null;
                CubesRemoved = k;
            }
        }

        /// <summary>
        /// Evaluates a state and returns the best next move
        /// </summary>
        /// <param name="initialState"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static void Minimax(State initialState, int k)
        {
            try
            {
                // check if the algorithm in ended
                if (initialState.CubesOnTable == 1)
                {
                    initialState.evaluation = 1;
                    initialState.BestNextState = null;
                    initialState.CubesRemoved = 1;
                }
                else if (initialState.CubesOnTable == 2)
                {
                    initialState.evaluation = 1;
                    initialState.BestNextState = null;
                    initialState.CubesRemoved = 2;
                }
                if (initialState.CubesOnTable == k)
                {
                    initialState.evaluation = 1;
                    initialState.BestNextState = null;
                    initialState.CubesRemoved = k;
                }
                else
                {
                    // get the childrens of the initial state
                    List<State> childrens = new List<State>();
                    childrens.Add(initialState.GetChild(1));
                    childrens.Add(initialState.GetChild(2));
                    if (initialState.CubesOnTable > k)
                        childrens.Add(initialState.GetChild(k));

                    // evaluate the childrens
                    foreach(var children in childrens)
                    {
                        Minimax(children, k);
                    }

                    // choose the best children as the next move
                    initialState.BestNextState = childrens.OrderByDescending(p => p.evaluation).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    

}
