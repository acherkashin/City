namespace CyberCity.Models.HouseModels
{
    public class Tarifs
    {
        ///// <summary>
        ///// Тарифный коэффициент, обозначающий сколько стоит 1 куб м газа в рублях
        ///// </summary>
        public float Gas { get; set; }

        ///// <summary>
        ///// Тарифный коэффициент, обозначающий сколько стоит 1 кВт в рублях
        ///// </summary>
        public float Electric { get; set; }

        ///// <summary>
        ///// Тарифный коэффициент, обозначающий сколько стоит 1 куб м воды в рублях
        ///// </summary>
        public float Water { get; set; }
    }
}
