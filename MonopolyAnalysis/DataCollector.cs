using Monopoly;
using System;
using System.Collections.Generic;

namespace MonopolyAnalysis
{
    public class DataCollector
    {
        public delegate void simulationCompleteHandler(object source, List<Move> moves, int playerAmount);

        public event simulationCompleteHandler SimulationComplete;

        Monopoly.Program _program;

        List<Move> _moves = new List<Move>();

        private int _playerAmount;

        public DataCollector(int playerAmount)
        {
            _playerAmount = playerAmount;
            _program = new Monopoly.Program();
        }

        public void Start()
        {
            Board board = _program.StartGame(_playerAmount, 90, this);
            onSimulationComplete();
        }

        public void registerMove(Move move)
        {
            _moves.Add(move);
        }

        protected virtual void onSimulationComplete()
        {
            if (SimulationComplete != null)
                SimulationComplete(this, _moves, _playerAmount);
            
        }
       
    }
}
