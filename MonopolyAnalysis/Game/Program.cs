using MonopolyAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Monopoly
{
    public class Program
    {
        public Board StartGame(int playerAmount, int roundAmount, DataCollector dataCollector)
        {
            var board = new Board(new ConsolePlayerInteracter(), playerAmount, roundAmount, dataCollector);
            board.Setup();
            while (true)
            {
                int value = board.DoTurn();
                if (value == -1)
                {
                    break;
                }
            }
            return board;
        }
    }
}
