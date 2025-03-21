using MO_kursasch_25.Models;
using MO_kursasch_25.ViewModels.TaskViewModels.Interface;

namespace MO_kursasch_25.ViewModels.TaskViewModels
{
    public class TaskV26 : ITask
    {
        private readonly string formula = " С  =  α * (Т2– Т1)^А1 + β * 1 /  V1 * (Т1+Т2 -  γ *V2)^A2";

        private readonly double _alphaP;
        private readonly double _betaP;
        private readonly double _gammaP;
        private readonly double _a1P;
        private readonly double _a2P;
        private readonly double _v1P;
        private readonly double _v2P;
        private readonly double _limitT;
        private readonly double _minT1;
        private readonly double _maxT1;
        private readonly double _minT2;
        private readonly double _maxT2;

        public double AlphaP => _alphaP;
        public double BetaP => _betaP;
        public double GammaP => _gammaP;
        public double A1P => _a1P;
        public double A2P => _a2P;
        public double V1P => _v1P;
        public double V2P => _v2P;
        public double LimitT => _limitT;
        public double MinT1 => _minT1;
        public double MaxT1 => _maxT1;
        public double MinT2 => _minT2;
        public double MaxT2 => _maxT2;

        public TaskV26(double alphaP = 1, double betaP = 1, double gammaP = 1, double a1P = 2, double a2P = 2, double v1P = 9, double v2P = 10, double limitT = 10, double minT1 = -3, double maxT1 = 14, double minT2 = -3, double maxT2 = 14)
        {
            _alphaP = alphaP;
            _betaP = betaP;
            _gammaP = gammaP;
            _a1P = a1P;
            _a2P = a2P;
            _v1P = v1P;
            _v2P = v2P;
            _limitT = limitT;
            _minT1 = minT1;
            _maxT1 = maxT1;
            _minT2 = minT2;
            _maxT2 = maxT2;
        }

        public double CalculateObjectiveFunction(FuncPoint point)
        {
            return _alphaP * Math.Pow((point.First - point.Second), _a1P) + _betaP * (1.0 / _v1P) * Math.Pow((point.First + point.Second - _gammaP * _v2P), _a2P);
        }

        public bool CheckFirstOrderConstraints(FuncPoint point)
        {
            return point.First >= _minT1 && point.First <= _maxT1 &&
                   point.Second >= _minT2 && point.Second <= _maxT2;
        }

        public bool CheckSecondOrderConstraints(FuncPoint point)
        {
            return point.Second + point.First <= _limitT;
        }

        public string GetFormula()
        {
            return formula;
        }

        public (double FirstLower, double SecondLower) GetLowerBounds()
        {
            return new(_minT1, _minT2);
        }

        public (double FirstUpper, double SecondUpper) GetUpperBounds()
        {
            return new(_maxT1, _maxT2);
        }
    }
}