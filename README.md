# Minimax
A simple game for Windows using the Minimax algorithm. It is created with C# and WPF.

On a table there are M cubes and two players are playing. Each player can draw 1, 2 or K cubes from the table. The winner is whoever takes the last cube from the table.

The application tries to solve the problem by using the MinMax algorithm and searching the state tree to find the next move. There is an option to search the entire tree for the best move, which is very time-consuming, or to search until a user-defined depth.

To evaluate a final node, in full search it gets 2 points if it wins. In depth-specified search, if the node at the specified depth is final then it also gets 2 points. If it is not final and there is a move that leaves 3 cubes remaining on the table then he gets 1 point because that way he will definitely win the next round. In all other cases it gets 0.

To evaluate the nodes, apart from whether it wins or loses, the algorithm also takes into account how many moves it takes to reach it. In this way, he always chooses the shortest path to win and, if he is going to lose, he chooses the longest.

For this reason, a factor large enough (defined as 2 * M) is used to evaluate the final node. From this factor,  the length of the path and the node's score are subtracted.

The application contains two kinds of games, one of computer against human (AI vs Player) and one between computer and computer (AI vs AI). It also has the possibility to construct the state tree of the MinMax algorithm for reviewing purposes.
