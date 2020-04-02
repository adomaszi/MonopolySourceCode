using Monopoly;
using System;
using System.Collections.Generic;

namespace MonopolyAnalysis
{
    public class DataCollector
    {
        public delegate void simulationCompleteHandler(object source, List<Move> moves, int playerAmount, Board board);

        public event simulationCompleteHandler SimulationComplete;

        private Monopoly.Program _program;
        private Board _board;
        private List<Move> _moves = new List<Move>();
        private int _playerAmount;
        private List<int> _roundAmounts = new List<int>();

        public DataCollector(int playerAmount)
        {
            _playerAmount = playerAmount;
            FillRoundAmounts();
            _program = new Monopoly.Program();
        }

        public void Start()
        {
            int roundAmount = _roundAmounts[_playerAmount];

            _board = _program.StartGame(_playerAmount, roundAmount, this);
            onSimulationComplete();
        }

        public void registerMove(Move move)
        {
            _moves.Add(move);
        }

        protected virtual void onSimulationComplete()
        {
            if (SimulationComplete != null)
            {
                SimulationComplete(this, _moves, _playerAmount, _board);
            }
        }

        private void FillRoundAmounts()
        {
            _roundAmounts.Add(0);
            _roundAmounts.Add(0);
            for (int i = 90; i >= 60; i -= 5)
            {
                _roundAmounts.Add(i);
            }
        }

    }
}
