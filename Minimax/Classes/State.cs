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
        public int Evaluation = 0;
        public int CubesRemoved = 0;
        public List<State> Childrens = new List<State>();

        public static List<int> Choices = new List<int>() { 1, 2, 4 };

        /// <summary>
        /// Get a children with a specific number of cubes to remove
        /// </summary>
        /// <param name="cubesToRemove"></param>
        /// <returns></returns>
        public State GetChild(int cubesToRemove)
        {
            try
            {
                State childState = new State();
                childState.CubesOnTable = CubesOnTable - cubesToRemove;
                childState.CubesRemoved = cubesToRemove;
                childState.Player = (Player == Player.Max) ? Player.Min : Player.Max;

                return childState;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the childrens of a state
        /// </summary>
        /// <returns></returns>
        public List<State> GetChildrens()
        {
            List<State> childrens = new List<State>();

            foreach(var choice in State.Choices)
            {
                if (this.CubesOnTable >= choice)
                    childrens.Add(GetChild(choice));
            }

            return childrens;
        }

        /// <summary>
        /// Evaluate a state
        /// </summary>
        public void Evaluate()
        {
            if (CubesOnTable == 0)
            {
                Evaluation = (Player == Player.Max) ? -1 : 1;
            }
            else
            {
                var childrens = GetChildrens();

                this.Childrens = childrens;

                foreach(var child in childrens)
                    child.Evaluate();

                if (Player == Player.Max)
                    Evaluation = childrens.Max(p => p.Evaluation);
                else
                    Evaluation = childrens.Min(p => p.Evaluation);
            }
        }

        /// <summary>
        /// Evaluates a state and returns the best next move
        /// </summary>
        /// <param name="initialState"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static State? Minimax(State initialState)
        {
            State? nextBestState = null;

            try
            {
                // if there are no cubes on the table then there is no next move
                if (initialState.CubesOnTable <= 0)
                    return null;

                // get the childrens of the initial state
                List<State> childrens = initialState.GetChildrens();

                // evaluate childrens
                foreach (var child in childrens)
                    child.Evaluate();

                // choose as next move the one with the better evaluation
                if (initialState.Player == Player.Max)
                    nextBestState = childrens.OrderByDescending(p => p.Evaluation).FirstOrDefault();
                else
                    nextBestState = childrens.OrderBy(p => p.Evaluation).FirstOrDefault();

                return nextBestState;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    

}
